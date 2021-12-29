using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IQuickBooksService : IServiceBase<tblCommonData>
    {
        List<tblCommonData> AuthdataListByDesc();
       bool  UpdateTOken(string Token, string Type);
    }
}