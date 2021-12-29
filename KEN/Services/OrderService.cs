using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN.Interfaces;
using KEN.Interfaces.Iservices;
using KEN_DataAccess;
using KEN.Interfaces.Repository;
using AutoMapper;
using KEN.AppCode;

using KEN.Models;

namespace KEN.Services
{
    public class OrderService:IOrderService
    {
        private readonly IRepository<tblOpportunity> _tblOpportunityList;
        private readonly IRepository<Vw_tblOpportunity> _VW_tblOpportunityRepository;
        KENNEWEntities DbContext = new KENNEWEntities();
        public OrderService(IRepository<tblOpportunity> tblOpportunityList, IRepository<Vw_tblOpportunity> VW_tblOpportunityRepository)
        {
            _tblOpportunityList = tblOpportunityList;
            _VW_tblOpportunityRepository = VW_tblOpportunityRepository;
        }

        public bool Add(tblOpportunity entity)
        {
            throw new NotImplementedException();
        }

        public ResponseViewModel BatchTransaction(tblOpportunity entitity, BatchOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<tblOpportunity> GetAll()
        {
            throw new NotImplementedException();
        }

        public tblOpportunity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public List<OrderViewModal> GetOrdersDetails(string type, string Department, string UserProfile, string StartDate, string EndDate)
        {
            var CurrentProfile = 0;
            var FromDate = Convert.ToDateTime(StartDate);
            var ToDate = Convert.ToDateTime(EndDate).AddDays(1);

            var IsAdmin = false;
            var ActiveUser = DataBaseCon.ActiveUser();
            var Data = DbContext.tblusers.Where(_ => _.email == ActiveUser).FirstOrDefault();
            if (UserProfile == "All")
            //if (Data.admin == true && UserProfile == Data.id)
            {
                IsAdmin = true;
            }
            else
            {
                CurrentProfile = Convert.ToInt32(UserProfile);
            }
            List<OrderViewModal> listData = new List<OrderViewModal>();
            try
            {
                if (type == "All" && Department == "All")
                {
                    if (!IsAdmin)
                    {
                        listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                    }
                    else
                    {
                        listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                    }
                }

                else if (type == "All" && Department != "All")
                {
                    if (!IsAdmin)
                    {
                        listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.DepartmentName.Contains(Department) && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                    }
                    else
                    {
                        listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.DepartmentName.Contains(Department) && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                    }
                }
                else if (type != "All" && Department == "All")
                {
                    switch (type)
                    {
                        case "Lost":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Declined":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Packing":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Stock Decorated" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Stock Decorated" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Invoiced":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Job":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderBy(_ => _.ConfirmedDate).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderBy(_ => _.ConfirmedDate).ToList();
                                }
                                break;
                            }
                        case "Quote":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Quote" && _.Lost == "NO" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Quote" && _.Lost == "NO" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Order":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Order" && _.Cancelled == "No" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Order" && _.Cancelled == "No" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Opportunity":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Opportunity" && _.Declined == "No" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Opportunity" && _.Declined == "No" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Complete":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Complete" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Complete" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Cancelled":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Shipping":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        default:
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == type && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == type && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                    }
                  
                }

                else
                {

                    switch (type)
                    {
                        case "Lost":
                         {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.DepartmentName.Contains(Department) && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Lost == "Yes" && _.DepartmentName.Contains(Department) && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;

                         }
                        case "Declined":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.AcctManagerId == CurrentProfile && _.DepartmentName.Contains(Department) && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Declined == "Yes" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Cancelled":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Cancelled == "Yes" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Packing":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Stock Decorated" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Stock Decorated" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Invoiced":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Packed" || _.Stage == "Order Invoiced") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Job":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderBy(_ => _.ConfirmedDate).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Confirmed" || _.Stage == "Job") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderBy(_ => _.ConfirmedDate).ToList();
                                }
                                break;
                            }
                        case "Quote":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Quote" && _.Lost == "NO" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Quote" && _.Lost == "NO" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Order":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Order" && _.Cancelled == "No" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Order" && _.Cancelled == "No" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Opportunity":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Opportunity" && _.Declined == "No" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Opportunity" && _.Declined == "No" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Complete":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Complete" && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == "Complete" && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        case "Shipping":
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => (_.Stage == "Order Shipped" || _.Stage == "Paid") && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate && _.DepartmentName.Contains(Department))).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                        default:
                            {
                                if (!IsAdmin)
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == type && _.DepartmentName.Contains(Department) && _.AcctManagerId == CurrentProfile && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                else
                                {
                                    listData = Mapper.Map<List<OrderViewModal>>(_VW_tblOpportunityRepository.Get(_ => _.Stage == type && _.DepartmentName.Contains(Department) && _.StageWiseDate >= FromDate && _.StageWiseDate < ToDate)).OrderByDescending(_ => _.OpportunityId).ToList();
                                }
                                break;
                            }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return listData;
        }

        public ResponseViewModel Update(tblOpportunity entity)
        {
            throw new NotImplementedException();
        }
    }
}