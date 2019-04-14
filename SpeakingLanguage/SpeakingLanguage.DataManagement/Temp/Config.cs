using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.DataManagement
{
    public class Config
    {
        public const int SIZE_SEGMENT = 1 << 10;
        public const int COUNT_MAX_PROPERTY = 1 << 10;
        public const int COUNT_MAX_CELL = 1 << 6;
        public const int COUNT_MAX_USER = 1 << 10;
        public const int SIZE_CELL = 1 << 10;
    }
}
