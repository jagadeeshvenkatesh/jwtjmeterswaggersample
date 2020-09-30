using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JmeterTestBackend.Models
{
    public class TestModel
    {
        public Guid ResponseId { get; set; }
        public DateTime ResponseTime { get; set; }
        public string CallingUser { get; set; }
        public List<KeyValuePair<string, string>> CallingUserClaims { get; set; }
    }
}
