using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class companyModel
    {
        public int COMPID { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string DISTRICT { get; set; }
        public string OPEN_ADRESS { get; set; }
        public string POSTAL_CODE { get; set; }
        public string MAIL { get; set; }
        public string PHONE { get; set; }
        public string TITLE { get; set; }
        public string COMMERCIAL_TITLE { get; set; }

        public List<departmentModel> DEPARTMENTS { get; set; }
    }
}