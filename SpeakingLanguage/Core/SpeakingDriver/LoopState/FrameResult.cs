using System;

namespace SpeakingLanguage.Core
{
    public struct FrameResult
    {
        public static readonly FrameResult None = new FrameResult { };
        public static readonly FrameResult Success = new FrameResult { res = Result.Success, skip = 1 };
        public static readonly FrameResult Fail = new FrameResult { res = Result.Fail };

        public enum Result
        {
            None,
            Success,
            Fail,
        }

        public Result res;
        public string msg;
        public int skip;
    }
}
