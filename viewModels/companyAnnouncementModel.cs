using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class companyAnnouncementModel
    {
        public int ANNID { get; set; }
        public int OWNER_ID { get; set; }
        public int COMPANY_ID { get; set; }
        public System.DateTime CREATED_AT { get; set; }
        public System.DateTime DEADLINE { get; set; }
        public string TITLE { get; set; }
        public string CONTENT { get; set; }
    }
}