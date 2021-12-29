using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KEN_DataAccess;

using KEN.Models;

namespace KEN.Interfaces.Iservices
{
   public interface IDecorationCostService : IServiceBase<tblDecorationCost>
    {
        //List<tblDecorationCost>  GetDecorationCostList();      // 03 Oct 2018 (N)

        ResponseViewModel DecorationBatchTransaction(tblDecorationCost Entity, BatchOperation operation);

        List<DecorationCostMasterViewModel> GetDecorationList(string Status,string Type);   // 03 Oct 2018 (N)

        List<tblCommonData> GetCommonDataList();          //Common Data View Tarun 15/09/2018
        //List<DecorationCostExportViewModel> ExportData();
    }
}
