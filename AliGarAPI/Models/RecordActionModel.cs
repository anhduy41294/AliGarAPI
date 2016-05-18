using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AliGarAPI.Models
{
    public class RecordActionModel
    {
        public decimal IdRecordAction { get; set; }
        public decimal IdAction { get; set; }
        public double Duration { get; set; }
        public bool Status { get; set; }
    
    }
}