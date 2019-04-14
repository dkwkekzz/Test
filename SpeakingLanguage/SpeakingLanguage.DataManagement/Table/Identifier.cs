using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.DataManagement.Table
{
    [Serializable]
    public class Identifier
    {
        public int handle;
        public string id;
        public string pwd;
        public DateTime lastConnectionTime;
    }
}
