using System;

namespace SpeakingLanguage.Core
{
    public class TokenFactory
    {
        public static Token Create(TokenFlag flag)
        {
            return new Token(flag, string.Empty, string.Empty);
        }

        public static Token Create(TokenFlag flag, string src, string dest)
        {
            return new Token(flag, src, dest);
        }

        public static Token Create(TokenFlag flag, string src, string dest, int value, int weight)
        {
            return new Token(flag, src, dest, value, weight);
        }
    }
}
