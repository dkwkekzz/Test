using System;

namespace SpeakingLanguage.Core
{
    public class Config
    {
        public const int MS_PER_UPDATE = 1000;
        public const int MS_CACHE_DURATION = 1 << 20;

        public const int COUNT_MAX_SOURCE = 1 << 16;
        public const int COUNT_MAX_CELL = 1 << 8;
        public const int COUNT_MAX_VALUE_NODE = 1 << 8;
        public const int COUNT_MAX_CHUNK = 1 << 6;
        public const int COUNT_DEFAULT_WORKER = 4;
        public const int COUNT_DEFAULT_PROP = 1 << 8;
        public const int COUNT_MAX_OVERLAPS = 1 << 4;
        public const int LENGTH_MAX_KEY = 1 << 8;
        public const int LENGTH_PAGE = 1 << 16;
        public const int LENGTH_MIN_CHUNK = 1 << 4;
        public const int LENGTH_CELL_WIDTH = 1 << 8;
        public const int LENGTH_CELL_HEIGHT = 1 << 8;

        public const string NAME_ASSEMBLY_EVENT = "SpeakingLanguage.Command.Event";
        public const string NAME_FILE_PAGE_SOURCE = "test.csv";
    }
}
