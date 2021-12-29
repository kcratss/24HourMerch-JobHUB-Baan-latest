using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    //public class IEventService
    //{

    //}

    public interface IEventService : IServiceBase<tblEvent>
    {
        IEnumerable<EventViewModel> GetEventByName(string Prefix);
        tblEvent  GetEventById(int EventId);
        List<EventViewModel> GetLeadsEventTypeList();
        ResponseViewModel EventBatchTransaction(tblEvent Entity, int OppID, BatchOperation operation, string PageSource);  //PageSource (N)
        List<EventViewModel> GetCustomEventList(string CustomText, string TableName);
        //17 Aug 2018 (N)
        List<opportunityViewModel> GetOpportunityByEventId(string Stage, int EventId);
        //17 Aug 2018 (N)
    }
}