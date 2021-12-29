using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;

namespace KEN.Interfaces.Iservices
{
    public interface IDepartmentService:IServiceBase<tbldepartment>
    {
        IEnumerable<tbldepartment> GetAllDepartmentList();
       
    }
}