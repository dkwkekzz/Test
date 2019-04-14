using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component
{
    public static class Function
    {
        public static void BidirectLink(SLComponent lhs, SLComponent rhs)
        {
            lhs.LinkTo(rhs);
            rhs.LinkTo(lhs);
        }
    }
}
