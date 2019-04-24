using System;
using System.Collections.Generic;

namespace SpeakingLanguage.Component.Ext
{
    public static class Extensions
    {
        public static bool TryFind(this SLComponent iCom, ComponentType type, out SLComponent oCom)
        {
            if (!iCom.TryFind(type, out SLWrapper wrap))
            {
                oCom = null;
                return false;
            }

            oCom = wrap.First();
            return true;
        }

        public static bool TryFind(this SLComponent iCom, int handle, out SLComponent oCom)
        {
            if (!iCom.TryFind(handle, out SLWrapper wrap))
            {
                oCom = null;
                return false;
            }

            oCom = wrap.First();
            return true;
        }

        public static bool TryFind(this SLComponent iCom, string key, out SLComponent oCom)
        {
            if (!iCom.TryFind(key, out SLWrapper wrap))
            {
                oCom = null;
                return false;
            }

            oCom = wrap.First();
            return true;
        }

        public static bool TryFind(this SLComponent iCom, object key, out SLComponent oCom)
        {
            if (!iCom.TryFind(key, out SLWrapper wrap))
            {
                oCom = null;
                return false;
            }

            oCom = wrap.First();
            return true;
        }
    }
}
