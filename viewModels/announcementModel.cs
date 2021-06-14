using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class announcementModel
    {
        public List<teamAnnouncementModel> TeamAnnouncements { get; set; }
        public List<companyAnnouncementModel> CompanyAnnouncements { get; set; }
        public List<departmentAnnouncementModel> DepartmentAnnouncements { get; set; }
    }
}