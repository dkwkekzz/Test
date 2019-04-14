//using System;

//namespace SpeakingLanguage.Core
//{
//    public abstract class EventEntity
//    {
//        public const string ALWAYS = "always";

//        public abstract Token Execute(string me);
        
//        public Token IF(string src, string dest)
//        {
//            return TokenFactory.Create(TokenFlag.Read | TokenFlag.Enter, src, dest);
//        }

//        public Token IF(string src, string dest, int value, int weight)
//        {
//            return TokenFactory.Create(TokenFlag.Read | TokenFlag.Enter, src, dest, value, weight);
//        }

//        public Token IF_ALWAYS()
//        {
//            return TokenFactory.Create(TokenFlag.None);
//        }
//    }
//}
