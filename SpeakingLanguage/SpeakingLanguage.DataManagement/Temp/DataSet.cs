using System;
using System.Collections.Generic;
using System.Linq;

namespace SpeakingLanguage.DataManagement
{
    public interface IDataSet3
    {
        bool IsLocked { get; }
        bool IsCompleted { get; }
        void LoadAsync();
    }

    public sealed class DataSet<TTable> : IDataSet3 
        where TTable : class, Table.IDataTable
    {
        public TTable Body { get; private set; }
        public string Name { get; private set; } = $"{typeof(TTable).Name}.bin";

        public bool IsLocked { get; set; }
        public bool IsCompleted => Body != null;
        public Action OnCompleted { get; set; }
        
        public void Load()
        {
            Body = BinaryConverter.ConvertBinaryToObject<TTable>(Name) as TTable;
        }

        public async void LoadAsync()
        {
            IsLocked = true;

            var name = typeof(TTable).Name;
            Body = await BinaryConverter.ConvertBinaryToObjectAsync<TTable>(Name) as TTable;

            IsLocked = false;
            OnCompleted?.Invoke();
        }

        public async void UpdateAsync()
        {
            IsLocked = true;

            var name = typeof(TTable).Name;
            await BinaryConverter.ConvertObjectToBinaryAsync(Body, Name);
            Body = await BinaryConverter.ConvertBinaryToObjectAsync<TTable>(Name) as TTable;

            IsLocked = false;
        }
    }
}
