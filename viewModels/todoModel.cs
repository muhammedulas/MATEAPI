using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class todoModel
    {
        public int TASKID { get; set; }
        public int USERREF { get; set; }
        public string TASK_TITLE { get; set; }
        public string TASK_DESCRIPTION { get; set; }
        public short STATUS { get; set; }
    }
}