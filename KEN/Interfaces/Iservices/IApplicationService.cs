using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IApplicationService:IServiceBase<TblApplication>
    {
        List<tblApplicationStatu> GetApplicationStatus();
        List<tblApplicationArtSuppplier> GetApplicationArtSupplier();
        List<tblApplicationType> GetApplicationType();
        List<tblApplicationArt> GetApplicationArt();
        List<tblApplicationDesigner> GetApplicationDesigner();
        ResponseViewModel SaveArtSupplier(string ArtSupplierName);
        ResponseViewModel ApplicationBatchTransaction(TblApplication Entity, BatchOperation operation);
        ApplicationViewModel GetApplicationById(int ApplicationId);
        ResponseViewModel ApplicationColoursBatchTransaction(TblApplicationColour Entity, int ApplicationId, BatchOperation operation);
        List<ApplicationColourViewModel> GetApplicationColoursGrid(int ApplicationId);
        List<ApplicationViewModel> GetApplicationGridData(string ApplicationType);
        List<ApplicationViewModel> GetApplicationCustomData(string CustomText, string TableName);
        List<ApplicationJobListViewModel> ApplicationJobsList(int ApplicationId);
        IEnumerable<PantoneMasterViewModel> GetPantone(string Prefix);
        int SavePantone(tblPantoneMaster Model);
        ResponseViewModel SaveCustomInfo(TblApplicationCustomInfo Entity, int ApplicationId, BatchOperation operation);
        List<ApplicationCustomInfoViewModel> GetApplicationCustomGrid(int ApplicationId);
    }
}