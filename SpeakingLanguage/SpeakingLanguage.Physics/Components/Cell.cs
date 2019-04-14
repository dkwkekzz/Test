using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Physics
{
    sealed class Cell : IEquatable<Cell>
    {
        private static int indexer;
        public static ObjectPool<Cell> Factory { get; } =
            new ObjectPool<Cell>(() => { return new Cell { _handle = indexer++ }; }, Config.COUNT_MAX_CELL, cell => cell.Clear());

        private int _handle;
        private int _w;
        private int _x;
        private int _y;

        public int Handle => _handle;
        public int World => _w;
        public int X => _x;
        public int Y => _y;

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

}
