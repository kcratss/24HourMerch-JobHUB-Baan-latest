using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Models;
using KEN_DataAccess;

namespace KEN.Interfaces.Iservices
{
    public interface ILoginService:IServiceBase<tbluser>
    {
        tbluser GetByUsername(string username, string hashed_password);
   
    }
}