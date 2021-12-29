using KEN_DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEN.Models;


namespace KEN.Interfaces.Iservices
{
    public interface IOrderService:IServiceBase<tblOpportunity>
    {
        List<OrderViewModal> GetOrdersDetails(string type, string Department, string UserProfile, string StartDate, string EndDate);
    }
}
