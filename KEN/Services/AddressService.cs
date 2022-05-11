using AutoMapper;
using KEN.AppCode;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;
using KEN_DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace KEN.Services
{
    public class AddressService : IAddressService
    {
        private readonly IRepository<tblAddress> _tblAddressRepository;
        private readonly IRepository<tblUserAddressMapping> _tblUserAddressRepository;
        private readonly IRepository<tblState> _tblStateRepository;

        public AddressService(IRepository<tblAddress> tblAddressRepository, IRepository<tblUserAddressMapping> tblUserAddressRepository, IRepository<tblState> tblStateRepository)
        {
            _tblAddressRepository = tblAddressRepository;
            _tblUserAddressRepository = tblUserAddressRepository;
            _tblStateRepository = tblStateRepository;
        }

        public bool AddAddress(ClientAddressViewModel model)
        {
            try
            {
                var tblAddressEntity = Mapper.Map<tblAddress>(model);

                tblAddressEntity.CreatedBy = DataBaseCon.ActiveUser();// DataBaseCon.ActiveClient();
                tblAddressEntity.CreatedOn = DateTime.Now;
                _tblAddressRepository.Insert(tblAddressEntity);
                _tblAddressRepository.Save();

                var addressId = tblAddressEntity.AddressId;
                var userId = DataBaseCon.ActiveClientId();
                if (addressId != 0 && userId != 0)
                {
                    var tblUserAddressMapping = new tblUserAddressMapping();
                    tblUserAddressMapping.AddressId = addressId;
                    tblUserAddressMapping.UserId = userId;
                    tblUserAddressMapping.CreatedBy = DataBaseCon.ActiveUser();//DataBaseCon.ActiveClient();
                    tblUserAddressMapping.CreatedOn = DateTime.Now;
                    _tblUserAddressRepository.Insert(tblUserAddressMapping);
                    _tblUserAddressRepository.Save();
                }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool UpdateAddress(ClientAddressViewModel model)
        {
            try
            {
                var tblAddressEntity = _tblAddressRepository.Get(x => x.AddressId == model.AddressId).FirstOrDefault();
                if (tblAddressEntity != null)
                {
                    tblAddressEntity.Address1 = model.AddressLine1;
                    tblAddressEntity.Address2 = model.AddressLine2;
                    tblAddressEntity.State = model.State;
                    tblAddressEntity.Postcode = model.PostCode;
                    tblAddressEntity.TradingName = model.Name;
                    tblAddressEntity.Attention = model.Attention;
                    tblAddressEntity.AddNotes = model.AddressNote;
                    tblAddressEntity.UpdatedBy = DataBaseCon.ActiveUser();//DataBaseCon.ActiveClient();
                    tblAddressEntity.UpdatedOn = DateTime.Now;

                    _tblAddressRepository.Update(tblAddressEntity);
                    _tblAddressRepository.Save();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return false;

        }

        public bool DeleteAddress(int addressId)
        {
            var tblUserAddressMapping = _tblUserAddressRepository.Get(x => x.AddressId == addressId).FirstOrDefault();
            if (tblUserAddressMapping != null)
            {
                _tblUserAddressRepository.Delete(tblUserAddressMapping);
                _tblUserAddressRepository.Save();
            }
            else
            {
                return false;
            }
            var tblAddressEntity = _tblAddressRepository.Get(x => x.AddressId == addressId).FirstOrDefault();
            if (tblAddressEntity != null)
            {
                _tblAddressRepository.Delete(tblAddressEntity);
                _tblAddressRepository.Save();

            }
            else
            {
                return false;
            }
            return true;

        }

        public List<ClientAddressViewModel> GetAddressList(int userId)
        {
            List<ClientAddressViewModel> addressList = new List<ClientAddressViewModel>();

            string cnnString = ConfigurationManager.ConnectionStrings["KENNEWEntities"].ConnectionString;
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(cnnString);
            builder.Metadata = null;
            cnnString = builder.ProviderConnectionString;

            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "GetAllAddress";
            cmd.Parameters.AddWithValue("@userId", userId);

            cnn.Open();
            try
            {
                StringBuilder sb = new StringBuilder();

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sb.Append(reader.GetValue(0).ToString());
                        }
                    }
                    addressList = JsonConvert.DeserializeObject<List<ClientAddressViewModel>>(sb.ToString());
                    addressList = addressList.OrderByDescending(x => x.AddressId).ToList();

                }

            }
            catch (Exception e)
            {
                var msg = e.Message;

            }
            finally
            {
                cnn.Close();
            }
            return addressList;


        }

        public List<tblState> GetAllState()
        {
            var allState = _tblStateRepository.Get().ToList();

            return allState;
        }

        public ClientAddressViewModel GetAddressById(int addressId)
        {
            var tblAddressEntity = _tblAddressRepository.Get(x => x.AddressId == addressId).FirstOrDefault();
            var addressModel = Mapper.Map<ClientAddressViewModel>(tblAddressEntity);
            return addressModel;
        }
    }
}