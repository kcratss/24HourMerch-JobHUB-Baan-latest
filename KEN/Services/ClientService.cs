using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;
using KEN_DataAccess;
using Newtonsoft.Json;
using AutoMapper;
using Intuit.Ipp.Data;
using KEN.AppCode;


namespace KEN.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<tblcontact> _tblContactRepository;
       
        public ClientService(IRepository<tblcontact> tblContactRepository)
        {
            _tblContactRepository = tblContactRepository;
           
        }
        public List<ClientOptionViewModel> GetOptionData(int id)
        {
            List<ClientOptionViewModel> dataList = new List<ClientOptionViewModel>();
            var contactId = _tblContactRepository.Get(x => x.acct_manager_id == id).Select(x => x.id).FirstOrDefault();
            string cnnString = @"data source=DESKTOP-2S775V1\MSSQL_SERVER;initial catalog=KenLocalBackup;MultipleActiveResultSets=True;App=EntityFramework;Integrated Security=true;";

            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "SelectAllOptions";
            cmd.Parameters.AddWithValue("@ContactId", 19875); //Give contactId instead 494  20144 20416

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
                    dataList = JsonConvert.DeserializeObject<List<ClientOptionViewModel>>(sb.ToString());
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
            foreach (var item in dataList)
            {
                var ImagepathFront = @"~\Content\uploads\Application\" + item.FrontDesign;
                item.ImageFilePath = ImagepathFront;
                var ImagepathBack = @"~\Content\uploads\Application\" + item.BackDesign;
                item.ImageFilePathBack = ImagepathBack;
            }

            return dataList;


        }

      
    }
}