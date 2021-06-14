using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class taskModel
    {
            public int TASKID { get; set; }
            public bool TEAM_TASK { get; set; }
            public Nullable<int> ASSIGNED_TEAM { get; set; }
            public string ASSIGNED_TEAM_NAME { get; set; }
            public Nullable<int> ASSIGNED_USER { get; set; }
            public string ASSIGNED_USER_USERNAME { get; set; }
            public string CREATED_BY { get; set; }
            public System.DateTime CREATED_AT { get; set; }
            public System.DateTime DEADLINE { get; set; }
            public string MODIFIED_BY { get; set; }
            public Nullable<System.DateTime> MODIFIED_AT { get; set; }
            public string TASK_TITLE { get; set; }
            public string TASK_DESCRIPTION { get; set; }
            public short STATUS { get; set; }
            public string STATUS_COMMENT { get; set; }
            public Nullable<short> RESULT { get; set; }
            public Nullable<System.DateTime> CLOSED_AT { get; set; }
        }
}