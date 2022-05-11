using KEN.Models;
using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KEN.Interfaces.Iservices
{
    public interface IAddressService
    {
        bool AddAddress(ClientAddressViewModel model);
        bool UpdateAddress(ClientAddressViewModel model);
        bool DeleteAddress(int id);
        List<ClientAddressViewModel> GetAddressList(int id);

        List<tblState> GetAllState();

        ClientAddressViewModel GetAddressById(int addresId);
    }
}
