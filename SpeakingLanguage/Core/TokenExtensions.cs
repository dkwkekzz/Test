using System;

namespace SpeakingLanguage.Core
{
    public static class TokenExtensions
    {
        public static Token AND(this Token token, string src, string dest)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Read | TokenFlag.And, src, dest));
        }

        public static Token AND(this Token token, string src, string dest, int value, int weight)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Read | TokenFlag.And, src, dest, value, weight));
        }

        public static Token AND(this Token lhstoken, Token rhstoken)
        {
            return lhstoken.Append(rhstoken);
        }

        public static Token OR(this Token token, string src, string dest)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Read | TokenFlag.Or, src, dest));
        }

        public static Token OR(this Token token, string src, string dest, int value, int weight)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Read | TokenFlag.Or, src, dest, value, weight));
        }

        public static Token OR(this Token lhstoken, Token rhstoken)
        {
            return lhstoken.Append(rhstoken);
        }

        public static Token CREATE(this Token token, string src, string dest)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Write | TokenFlag.Add, src, dest));
        }

        public static Token DESTROY(this Token token, string src, string dest)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Write | TokenFlag.Remove, src, dest));
        }

        public static Token ACT(this Token token, string src, string dest)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Write, src, dest));
        }

        public static Token ACT(this Token token, string src, string dest, int value, int weight)
        {
            return token.Append(TokenFactory.Create(TokenFlag.Write, src, dest, value, weight));
        }

        public static Token ACT(this Token lhstoken, Token rhstoken)
        {
            return lhstoken.Append(rhstoken);
        }

        public static Token PERIOD(this Token token)
        {
            return token.Head;
        }
    }
}
