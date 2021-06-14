using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class departmentModel
    {
        public int DEPID { get; set; }
        public int COMPREF { get; set; }
        public string DEPARTMENT_NAME { get; set; }
        public string DEPARTMENT_DEF { get; set; }
        public List<teamModel> TEAMS { get; set; }


    }
}