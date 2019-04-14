using System;
using System.Collections.Generic;

namespace SpeakingLanguage.DataManagement
{
    public class World
    {
        //private readonly int _worldIndex;
        //private Cell[] _cellSet;

        //public World(int index)
        //{
        //    _worldIndex = index;
        //}

        //public void Construct()
        //{
        //    var worldSet = BigDictionary.SearchTable<Table.WorldTable>();
        //    worldSet.OnCompleted = () =>
        //    {
        //        var world = worldSet.Body.Worlds[_worldIndex];
        //        var x_len = world.width / Config.SIZE_CELL + 1;
        //        var y_len = world.height / Config.SIZE_CELL + 1;

        //        _cellSet = new Cell[y_len * x_len];
        //        for (int y = 0; y != y_len; y++)
        //        {
        //            for (int x = 0; x != x_len; x++)
        //            {
        //                var cell = new Cell(_worldIndex, x, y);
        //                cell.Left = _cellSet[x_len * y + Math.Max(0, x - 1)];
        //                cell.Right = _cellSet[x_len * y + Math.Min(x_len, x + 1)];
        //                cell.Top = _cellSet[x_len * Math.Max(0, y - 1) + x];
        //                cell.Bottom = _cellSet[x_len * Math.Min(y_len, y + 1) + x];

        //                _cellSet[x_len * y + x] = cell;
        //            }
        //        }
        //    };
        //}
    }
}
