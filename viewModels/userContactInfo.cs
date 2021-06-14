using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class userContactInfo
    {
        public int ID { get; set; }
        public int USERREF { get; set; }
        public string PHONE { get; set; }
        public string MAIL { get; set; }
        public string COUNTRY { get; set; }
        public string CITY { get; set; }
        public string DISTRICT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string OPEN_ADRESS { get; set; }
        public string PROFILE_PHOTO { get; set; }
    }
}