using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using KEN.Models;
using AutoMapper;
using System.Text;
using KEN.AppCode;

namespace KEN.Services
{
    public class KanBanService : IKanBanService
    {
        private readonly IRepository<tblkanban> _tblKanBanRepository;
        private readonly IRepository<tblOpportunity> _tblOpportunityRepository;
        private readonly IRepository<tbldepartment> _st_departmentRepository;
        private readonly IRepository<vw_tblKanban> _VW_tblkanban;
        ResponseViewModel response = new ResponseViewModel();

        public KanBanService(IRepository<tblkanban> tblKanBanRepository,IRepository<tblOpportunity> tblOpportunityRepository,IRepository<vw_tblKanban> VW_tblkanban,IRepository<tbldepartment> st_departmentRepository)
        {
            _tblKanBanRepository = tblKanBanRepository;
            _tblOpportunityRepository = tblOpportunityRepository;
            _VW_tblkanban = VW_tblkanban;
            _st_departmentRepository = st_departmentRepository;
        }

        public ResponseViewModel BatchTransaction(tblkanban entitity, BatchOperation operation)
        {
            try
            {
                switch (operation)
                {
                    case BatchOperation.Insert:
                        {
                            entitity.CreatedBy = "1";
                            entitity.CreatedOn = DateTime.Now;

                            _tblKanBanRepository.Insert(entitity);
                            _tblKanBanRepository.Save();

                            response.ID = entitity.KanbanId;
                            response.Message = "Data saved successfully";
                            response.Result = "Success";
                            break;
                        }
                    case BatchOperation.Delete:
                        {
                            var entity = _tblKanBanRepository.Get(_ => _.KanbanId == entitity.KanbanId).FirstOrDefault();

                            if (entity != null)
                            {
                                _tblKanBanRepository.Delete(entity);
                                _tblKanBanRepository.Save();
                            }
                            response.Message = "Data Deleted Successfully";
                            response.Result = "Success";
                            break;
                        }

                    default:
                        {
                            var entity = _tblKanBanRepository.Get(_ => _.KanbanId == entitity.KanbanId).FirstOrDefault();

                            if (entity != null)
                            {
                                entity.UpdatedBy = DataBaseCon.ActiveUser();
                                entity.UpdatedOn = Convert.ToDateTime(DataBaseCon.ToTimeZoneTime(DateTime.Now.ToUniversalTime()));
                                entity.MachineNo = entitity.MachineNo;
                                entity.Priority = entitity.Priority;
                                entity.ProductionDate = entitity.ProductionDate;
                                
                                _tblKanBanRepository.Update(entity);
                                _tblKanBanRepository.Save();
                            }

                            response.ID = entitity.KanbanId;
                            response.Message = "Data saved Successfully";
                            response.Result = "Success";
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Result = "Error";
            }
            return response;
        }

     

        public bool Add(vw_tblKanban entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(vw_tblKanban entity)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(vw_tblKanban entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }



        public ResponseViewModel KanbanCheck(tblkanban Entity)
        {
            if (Entity.KanbanId > 0)
            {
                response = BatchTransaction(Entity, BatchOperation.Update);
            }
            else
            {
                //var data = _tblOpportunityRepository.Get(_ => _.OpportunityId == Entity.OppId).FirstOrDefault();
                //if (data != null)
                //{
                //    string[] DeptId = data.job_department.Split(',');

                //    for (int i = 0; i < DeptId.Length; i++)
                //    {
                //        var DepartId = Convert.ToInt32(DeptId[i]);
                //        var entity = _tblKanBanRepository.Get(_ => _.DeptId == DepartId && _.OppId == Entity.OppId).FirstOrDefault();

                //        if (entity != null)
                //        {
                //            Entity.DeptId = DepartId;
                //            response = BatchTransaction(Entity, BatchOperation.Update);
                //        }
                //        else
                //        {
                //            Entity.DeptId = Convert.ToInt32(DeptId[i]);
                //            response = BatchTransaction(Entity, BatchOperation.Insert);
                //        }
                //    }
                //}
            }

            return response;
        }

        public List<KanBanViewModel> GetSideBarUncompleteKanBan(string DeptId)
        {
            List<vw_tblKanban> Uncompleteddata = new List<vw_tblKanban>();
            DateTime NewDate = DateTime.Now;
            //var GetcurrentDate = Convert.ToDateTime(NewDate.ToShortDateString());
            var GetcurrentDate = NewDate.Date;

            if (DeptId == "All")
            {
                Uncompleteddata = _VW_tblkanban.Get(_ => _.KanbanId > 0 && _.ProductionDate != null && _.ConfirmedDate != null && _.DecoratedDate == null && _.ProductionDate < GetcurrentDate).OrderBy(_ => _.ConfirmedDate).ToList();
            }
            else
            {
                int GetDept = Convert.ToInt32(DeptId);
                Uncompleteddata = _VW_tblkanban.Get(_ => _.KanbanId > 0 && _.DeptId == GetDept && _.ConfirmedDate != null && _.ProductionDate != null && _.DecoratedDate == null && _.ProductionDate < GetcurrentDate).OrderBy(_ => _.ConfirmedDate).ToList();
            }

            var newUncompleteddata = Mapper.Map<List<KanBanViewModel>>(Uncompleteddata).ToList();

            return newUncompleteddata;
        }
        
        public List<KanBanViewModel> GetSidebarunassignKanban(string DeptId)
        {
            List<vw_tblKanban> Unassigneddata = new List<vw_tblKanban>();
            
            if (DeptId == "All")
            {
                Unassigneddata = _VW_tblkanban.Get(_ => _.KanbanId > 0 && _.ProductionDate == null && _.DecoratedDate == null && _.ConfirmedDate != null).OrderBy(_ => _.ConfirmedDate).ToList();
            }
            else {
                int GetDept = Convert.ToInt32(DeptId);
                Unassigneddata = _VW_tblkanban.Get(_ => _.KanbanId > 0 && _.DeptId == GetDept && _.ProductionDate == null && _.DecoratedDate == null && _.ConfirmedDate != null).OrderBy(_ => _.ConfirmedDate).ToList();
            }
            
            var newUnassigneddata = Mapper.Map<List<KanBanViewModel>>(Unassigneddata).ToList();

            return newUnassigneddata;
        }
        
        public List<KanBanViewModel> GetAllKanbanJobs(string DeptId)
        {
            List<vw_tblKanban> data = new List<vw_tblKanban>();
            DateTime NewDate = DateTime.Now;
            var GetcurrentDate = NewDate.Date;

            if (DeptId == "All")
            {
                data = _VW_tblkanban.Get(_=> _.DecoratedDate == null && _.ProductionDate >= GetcurrentDate && _.ConfirmedDate != null).ToList();
            }
            else
            {
                int GetDept = Convert.ToInt32(DeptId);
                data = _VW_tblkanban.Get(_ => _.DeptId == GetDept && _.DecoratedDate == null && _.ProductionDate >= GetcurrentDate && _.ConfirmedDate != null).ToList();
            }

            var Oppdata = Mapper.Map<List<KanBanViewModel>>(data).ToList();

            return Oppdata;

        }

        public IEnumerable<tbldepartment> GetAllDepartmentList()
        {
            return _st_departmentRepository.Get(_=>_.KanbanOrder != null).OrderBy(_=>_.KanbanOrder).ToList();
        }

        public tblkanban GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblkanban> GetAll()
        {
            throw new NotImplementedException();
        }

        public bool Add(tblkanban entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel Update(tblkanban entity)
        {
            throw new NotImplementedException();
        }
    }
}