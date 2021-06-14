using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MATE.viewModels;

namespace MATE.viewModels
{
    public class userModel
    {
        public int USERID { get; set; }
        public string USERNAME { get; set; }
        public string PW_KEY { get; set; }
        public string NAME { get; set; }
        public string SURNAME { get; set; }
        public bool IS_ADMIN { get; set; }
        public short STATUS { get; set; }
        public userContactInfo CONTACT_INFO { get; set; }
        
    }
}