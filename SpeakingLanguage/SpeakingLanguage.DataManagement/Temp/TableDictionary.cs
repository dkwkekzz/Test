using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.DataManagement
{
    public sealed class TableDictionary
    {
        private Dictionary<Type, IDataSet3> _tableDic = new Dictionary<Type, IDataSet3>();
        
        public DataSet<T> Search<T>() where T : class, Table.IDataTable
        {
            var type = typeof(T);
            if (!_tableDic.TryGetValue(type, out IDataSet3 set))
            {
                set = new DataSet<T>();
                set.LoadAsync();
                _tableDic.Add(type, set);
            }

            return set as DataSet<T>;
        }

        public void Remove<T>()
        {
            var type = typeof(T);
            _tableDic.Remove(type);
        }
    }
}
