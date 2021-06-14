using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class teamModel
    {
        public int TEAMID { get; set; }
        public int DEPREF { get; set; }
        public string DEP_NAME { get; set; }
        public string TEAM_DEF { get; set; }
        public string TEAM_NAME { get; set; }
        public int MEMBER_COUNT { get; set; }
        public List<teamMemberModel> MEMBERS { get; set; }
    }
}