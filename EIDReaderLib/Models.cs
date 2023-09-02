using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EIDReaderLib
{
    public class EIDData
    {
        public EIDData()
        {
            Status = false;
            Message = "";
            EIDDataXMLString = "";
        }
        /// <summary>
        /// EID Reading status, returns true if success, false if failure
        /// </summary>
        public bool Status { get; set; }

        /// <summary>
        /// Response message data.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Response data
        /// </summary>
        public string EIDDataXMLString { get; set; }

        public bool InitStatus { get; set; }
        public bool ConnectStatus { get; set; }

        public dynamic Readers { get; set; }
        //public dynamic SelectedReader { get; set; }
    }
}
