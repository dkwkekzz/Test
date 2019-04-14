using System;

namespace SpeakingLanguage.Core
{
    public class Token
    {
        public TokenFlag _flag;
        private string _src;
        private string _dest;
        private int _value;
        private int _weight;
        private Token _head;
        private Token _next;
        private Token _enter;

        public string Src => _src;
        public string Dest => _dest;
        public int Value => _value;
        public int Weight => _weight;
        public Token Head => _head;
        public Token Next => _next;
        public Token Enter => _enter;

        public Token(TokenFlag flag, string src, string dest)
        {
            _flag = flag;
            _src = src;
            _dest = dest;
            _value = 0;
            _weight = 0;
            _head = this;
            _next = null;
        }

        public Token(TokenFlag flag, string src, string dest, int value, int weight)
        {
            _flag = flag;
            _src = src;
            _dest = dest;
            _value = value;
            _weight = weight;
            _head = this;
            _next = null;
        }

        public Token Append(Token token)
        {
            _next = token;
            token._head = this._head;
            return _next;
        }

        public Token Push(Token token)
        {
            _enter = token;
            return token;
        }

        public static Token operator+ (Token lhs, Token rhs)
        {
            lhs.Push(rhs);
            return rhs;
        }
    }
}
