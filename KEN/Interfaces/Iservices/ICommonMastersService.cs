using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface ICommonMastersService: IServiceBase<tblband>
    {
        List<DepartmentViewModel> DepartmentListForMasters(string ddlstatusvalue);
        List<BrandViewModel> BrandListForMasters(string ddlstatusvalue);
        List<ItemViewModel> ItemsListForMasters(string ddlstatusvalue);
        List<OptionCodeBrandItemViewModel> OptionCodeListMasters();
        ResponseViewModel MasterBatchTransaction(int id, string name, string status, string table, BatchOperation operation);
        ResponseViewModel OptionCodeTransaction(tblOptionCode model, BatchOperation operation);
    }
}
