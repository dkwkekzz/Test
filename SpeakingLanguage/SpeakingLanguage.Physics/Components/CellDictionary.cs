using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Physics
{
    sealed class Location
    {
        private Dictionary<string, Cell> _cellDic = new Dictionary<string, Cell>();

        public void Insert(int x, int y, string key)
        {

        }
    }

    sealed class LocationDictionary
    {

    }

    sealed class CellDictionary
    {
        private Dictionary<string, Dictionary<string, Cell>> _cellDic = new Dictionary<string, Dictionary<string, Cell>>();


    }
}
