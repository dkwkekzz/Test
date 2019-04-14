using SpeakingLanguage.Library;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeakingLanguage.Core
{
    public sealed class Cell : IEquatable<Cell>
    {
        private static int indexer;
        public static ObjectPool<Cell> Factory { get; } = 
            new ObjectPool<Cell>(() => { return new Cell { _handle = indexer++ }; }, Config.COUNT_MAX_CELL, cell => cell.Clear());

        private int     _handle;
        private int     _w;
        private int     _x;
        private int     _y;
        private Cell    _next;
        
        public int      Handle => _handle;
        public int      World => _w;
        public int      X => _x;
        public int      Y => _y;
        public Cell     Next => _next;

        public bool Equals(Cell other)
        {
            return _w == other._w && _x == other._x && _y == other._y;
        }

        public override int GetHashCode()
        {
            return _w ^ ~_x ^ _y;
        }
        
        public void Clear()
        {
            _next = null;
        }
    }

    public sealed class CellDictionary : IStateComponent
    {
        class CellStock
        {
            public static ObjectPool<CellStock> Factory { get; } =
                new ObjectPool<CellStock>(() => { return new CellStock(); }, Config.COUNT_MAX_CELL, stock => stock.Clear());

            private Dictionary<string, Null> _dicOverlaps = new Dictionary<string, Null>(Config.COUNT_MAX_OVERLAPS, new StringRefComparer());
            private Vector _force;

            public Vector Force => _force;
            public bool IsPressed => (_force.x | _force.y | _force.z) != 0;
            public void Simulate(ref Vector value) => _force = value;
            public void Release() => _force = Vector.One;
            public IEnumerable<string> GetEntities => from pair in _dicOverlaps select pair.Key;
            public void Add(string entity) => _dicOverlaps[entity] = new Null { };
            public void Remove(string entity) => _dicOverlaps.Remove(entity);
            public void Clear() => _dicOverlaps.Clear();
        }

        private Dictionary<int, Cell> _cellDic = new Dictionary<int, Cell>(); 
        private ConcurrentDictionary<Cell, CellStock> _cellStockDic = new ConcurrentDictionary<Cell, CellStock>();

        public void Awake()
        {
        }

        public void Dispose()
        {
        }
        
        public bool Fill(string key, Cell root, DynamicGraph graph)
        {
            var cell = root;
            while (null != cell)
            {
                CellStock stock;
                if (!_cellStockDic.TryGetValue(cell, out stock))
                {
                    stock = CellStock.Factory.GetObject();
                    _cellStockDic.TryAdd(cell, stock);
                }

                if (stock.IsPressed)
                {   // can't trust client.
                    return false;
                }

                foreach (var other in stock.GetEntities)
                {
                    graph.AddLink(key, other, 2);
                    graph.AddLink(other, key, 1);
                }

                stock.Add(key);

                cell = cell.Next;
            }

            return true;
        }

        public void Refresh()
        {   // cell의 존재는 남아있고 cell의 중첩정보만 지운다.
            foreach (var cellPair in _cellStockDic)
            {
                cellPair.Value.Clear();
            }
        }

        public void Clear()
        {   
            _cellStockDic.Clear();
        }
    }
}
