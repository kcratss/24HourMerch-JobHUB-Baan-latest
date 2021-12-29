using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using KEN_DataAccess;
using KEN.Models;

namespace KEN.Interfaces.Iservices
{
    public interface IKanBanService:IServiceBase<tblkanban>
    {
        ResponseViewModel KanbanCheck(tblkanban Entity);
        //List<KanBanViewModel> GetSidebarKanbanOpportunity();

        //string GetSidebarKanbanOpportunity(string DeptId);
        List<KanBanViewModel> GetSideBarUncompleteKanBan(string DeptId);
        List<KanBanViewModel> GetSidebarunassignKanban(string DeptId);

        //string GetAllKanbanJobs(List<String> Week, List<string> WeekDays, string ActiveTabs,string DeptId);
        //List<KanBanViewModel> GetAllKanbanJobs(List<String> Week, List<string> WeekDays, string ActiveTabs, string DeptId);

        List<KanBanViewModel> GetAllKanbanJobs(string DeptId);

        IEnumerable<tbldepartment> GetAllDepartmentList();
    }
}