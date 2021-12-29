using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN.Interfaces.Repository;
using KEN.Models;
using AutoMapper;
using KEN.AppCode;

namespace KEN.Services
{
    public class EventService : IEventService
    {
        private readonly IRepository<tblEvent> _tblEventRepository;
        private readonly IRepository<tblOpportunity> _tblOpportunityRepository;
        //18 Aug 2018 (N)
        private readonly IRepository<tbloption> _tbloptionRepository;
        private readonly IRepository<Vw_tblOpportunity> _Vw_tblOpportunityRepository;
        //18 Aug 2018 (N)
        ResponseViewModel response = new ResponseViewModel();
        KENNEWEntities DbContext = new KENNEWEntities();

        public EventService(IRepository<tblEvent> tblEventRepository, IRepository<tblOpportunity> tblOpportunityRepository, IRepository<tbloption> tbloptionRepository, IRepository<Vw_tblOpportunity> Vw_tblOpportunityRepository)
        {
            _tblEventRepository = tblEventRepository;
            _tblOpportunityRepository = tblOpportunityRepository;
            //18 Aug 2018 (N)
            _tbloptionRepository = tbloptionRepository;
            _Vw_tblOpportunityRepository = Vw_tblOpportunityRepository;
            //18 Aug 2018 (N)
        }
        public bool Add(tblEvent entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblEvent Entity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }
            public ResponseViewModel EventBatchTransaction(tblEvent Entity, int OppID, BatchOperation operation, string PageSource)  //PageSource (N)
        {
            string tblName = "";
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {

                            tblName = TableNames.tblEvent;
                            Entity.CreatedBy = DataBaseCon.ActiveUser();
                            Entity.CreatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                            _tblEventRepository.Insert(Entity);
                           
                                _tblEventRepository.Save();

                            var Opportunity = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppID).FirstOrDefault();
                            if(Opportunity!=null)
                            {
                                tblName = TableNames.tblOpportunity;
                                Opportunity.EventId = Entity.EventId;
                                Opportunity.UpdatedBy= DataBaseCon.ActiveUser();
                                Opportunity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                _tblOpportunityRepository.Update(Opportunity);
                                _tblOpportunityRepository.Save();
                            }


                            response.Message = ResponseMessage.SuccessMessage;
                            response.ID = Entity.EventId;
                            response.Result = ResponseType.Success;
                            response.tblName = TableNames.tblEvent;

                            break;
                        }
                    case BatchOperation.Delete:
                        {

                            //var entity = _tblEventRepository.Get(_ => _.email == Entity.email).FirstOrDefault();
                            //if (entity != null)
                            //{
                            //    _st_contactRepository.Delete(entity);
                            //    _st_contactRepository.Save();
                            //}

                            break;
                        }
                    default:
                        {
                            // 23 Aug 2018 (N)
                            if (PageSource == "Opportunity")
                            {

                                var Opportunity = _tblOpportunityRepository.Get(_ => _.OpportunityId == OppID).FirstOrDefault();
                                if (Opportunity != null)
                                {
                                    tblName = TableNames.tblOpportunity;
                                    Opportunity.EventId = Entity.EventId;
                                    Opportunity.UpdatedBy = DataBaseCon.ActiveUser();
                                    Opportunity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblOpportunityRepository.Update(Opportunity);
                                    _tblOpportunityRepository.Save();
                                }
                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = Entity.EventId;
                                response.Result = ResponseType.Success;
                                tblName = TableNames.tblEvent;
                            }
                            if (PageSource == "Event")
                            {
                                tblName = TableNames.tblEvent;
                                var entity = _tblEventRepository.Get(_ => _.EventId == Entity.EventId).FirstOrDefault();
                                if (entity != null)
                                {
                                    entity.EventName = Entity.EventName;
                                    entity.EventDate = Entity.EventDate;
                                    entity.EventCycle = Entity.EventCycle;
                                    entity.NextDate = Entity.NextDate;
                                    entity.EventLocation = Entity.EventLocation;
                                    entity.EventWebsite = Entity.EventWebsite;
                                    entity.EventNotes = Entity.EventNotes;
                                    entity.UpdatedBy = DataBaseCon.ActiveUser();
                                    entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                    _tblEventRepository.Update(entity);
                                    _tblEventRepository.Save();
                                }
                                response.Message = ResponseMessage.SuccessMessage;
                                response.ID = Entity.EventId;
                                response.Result = ResponseType.Success;
                                response.tblName = TableNames.tblEvent;

                            }
                            break;
                            // 23 Aug 2018 (N)
                        }
                }
            }
            catch(Exception ex)
                {

                response.Message = ex.Message;
                response.Result = ResponseType.Error;
                response.tblName = tblName;
                response.ErrorCode = ex.HResult;
               
            }

            return response;
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblEvent> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<EventViewModel> GetEventByName(string Prefix)
        {
            var data = _tblEventRepository.Get(_ => _.EventName.Contains(Prefix));
            var newdata = data.Select(item => new EventViewModel
            {
                EventName = item.EventName,
                EventId = item.EventId,
                EventDate1 = Convert.ToDateTime(item.EventDate).ToShortDateString(),
                NextDate1 = Convert.ToDateTime(item.NextDate).ToShortDateString(),
                EventDate = item.EventDate,
                NextDate=item.NextDate,
                EventCycle=item.EventCycle,
                EventNotes=item.EventNotes,
                EventWebsite=item.EventWebsite,
                EventLocation = item.EventLocation,

                
            }
           ).ToList();
            return newdata;
        }

        public tblEvent GetById(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(tblEvent entity)
        {
            throw new NotImplementedException();
        }

        public tblEvent GetEventById(int EventId)
        {
           return _tblEventRepository.Get(_ => _.EventId == EventId).FirstOrDefault();
        }
        public List<EventViewModel> GetLeadsEventTypeList()
        {
            List<EventViewModel> Event = new List<EventViewModel>();
            Event = Mapper.Map<List<EventViewModel>>(_tblEventRepository.Get().ToList().Take(6500)).ToList();
            return Event;
        }

        public List<EventViewModel> GetCustomEventList(string CustomText, string TableName)
        {
            //  var ddd = DbContext.Pro_Search(TableName, CustomText);
            var CustomEventData = Mapper.Map<List<EventViewModel>>(DbContext.Database.SqlQuery<tblEvent>("exec Pro_Search '" + TableName + "','" + CustomText + "'").ToList()).ToList();
            //  DbContext.Pro_Search
            return CustomEventData;
        }

        //17 Aug 2018 (N)
        public List<opportunityViewModel> GetOpportunityByEventId(string Stage, int EventId)
        {
            List<opportunityViewModel> griddata = new List<opportunityViewModel>();

            if (Stage == "All")
            {
                var Opportunitydata = _Vw_tblOpportunityRepository.Get(_ => _.EventId == EventId).ToList();
                griddata = new List<opportunityViewModel>();
                griddata = Mapper.Map<List<opportunityViewModel>>(Opportunitydata).ToList();
            }
            else
            {
                if (Stage == "Lost")
                {
                    var data = _Vw_tblOpportunityRepository.Get(_ => _.EventId == EventId && _.Lost == "Yes");
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                }
                else if (Stage == "Declined")
                {
                    var data = _Vw_tblOpportunityRepository.Get(_ => _.EventId == EventId && _.Declined == "Yes");
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                }
                else if (Stage == "Cancelled")
                {
                    var data = _Vw_tblOpportunityRepository.Get(_ => _.EventId == EventId && _.Cancelled == "Yes");
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                }
                else
                {

                    var data = _Vw_tblOpportunityRepository.Get(_ => _.EventId == EventId && _.Stage == Stage);
                    griddata = new List<opportunityViewModel>();
                    griddata = Mapper.Map<List<opportunityViewModel>>(data).ToList();
                }
            }
            return griddata;
        }
        //17 Aug 2018 (N)
    }
}