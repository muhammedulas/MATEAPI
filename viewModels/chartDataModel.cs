using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MATE.viewModels
{
    public class chartDataModel
    {
        public string dataDefinition { get; set; }
        public List<string> labels { get; set; }
        public List<int> series { get; set; }
    }
}