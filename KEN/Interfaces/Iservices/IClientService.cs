using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IClientService
    {
        List<ClientOptionViewModel> GetOptionData(int id);


        
    }
}
