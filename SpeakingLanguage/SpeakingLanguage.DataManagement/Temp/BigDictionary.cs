using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SpeakingLanguage.DataManagement
{
    public class BigDictionary
    {
        private static readonly Dictionary<Type, IDataSet3> _tableDic = new Dictionary<Type, IDataSet3>();
        private static readonly Dictionary<Type, IGroup> _groupDic = new Dictionary<Type, IGroup>();
        private static readonly TableBuilder _tableBuilder = new TableBuilder();
        
        public static DataSet<TDataTable> SearchTable<TDataTable>() 
            where TDataTable : class, DataManagement.Table.IDataTable
        {
            var type = typeof(TDataTable);
            if (!_tableDic.TryGetValue(type, out IDataSet3 set))
            {
                set = new DataSet<TDataTable>();
                set.LoadAsync();
                _tableDic.Add(type, set);
            }

            return set as DataSet<TDataTable>;
        }

        public static IGroup SearchGroup(Type type)
        {
            return (!_groupDic.TryGetValue(type, out IGroup group)) ? null : group; 
        }

        public static Group<TProperty> SearchGroup<TProperty>()
            where TProperty : struct
        {
            var type = typeof(TProperty);
            if (!_groupDic.TryGetValue(type, out IGroup group))
            {
                group = new Group<TProperty>(Config.COUNT_MAX_PROPERTY);
                _groupDic.Add(type, group);
            }

            return group as Group<TProperty>;
        }

        public static Agent SearchAgent(string idStr, string pwd)
        {
            return new Agent();
        }
    }
}
