using SpeakingLanguage.Core;
using SpeakingLanguage.Library;
using System;
using System.Collections.Generic;
using System.Text;

namespace SpeakingLanguage.Compile
{
    class Interpreter
    {
        private readonly Committer _commit = new Committer();
        private readonly IExecutor[] _executor = new IExecutor[6]
        {
            null,
            new Body(),
            new Source(),
            new Destination(),
            new Value(),
            new Weight(),
        };

        public Token Result => _commit.Result;

        public void Execute(byte c, ref CompileState state)
        {
            state = _executor[(int)state].Execute((char)c);
        }

        public void Commit()
        {
            _commit.Execute(_executor);
        }

        public void Clear()
        {
            for (int i = 0; i != _executor.Length; i++)
                _executor[i].Clear();
            _commit.Clear();
        }

        interface IExecutor
        {
            int ResultToInt { get; }
            string ResultToStr { get; }
            CompileState Execute(char c);
            void Clear();
        }

        sealed class Body : IExecutor
        {
            private StringBuilder _builder = new StringBuilder(Config.LENGTH_MAX_KEY);

            public int ResultToInt => throw new NotImplementedException("can't using integer result in executor:body.");
            public string ResultToStr
            {
                get
                {
                    if (_builder[0] == 'i' && _builder[1] == 'f') return Constants.IF;
                    if (_builder[0] == 'a' && _builder[1] == 'n' && _builder[1] == 'd') return Constants.AND;
                    if (_builder[0] == 'o' && _builder[1] == 'r') return Constants.OR;
                    if (_builder[0] == 'a' && _builder[1] == 'c' && _builder[1] == 't') return Constants.ACT;
                    return string.Empty;
                }
            }

            public CompileState Execute(char c)
            {
                switch (c)
                {
                    case ' ':
                        return CompileState.Body;
                    case '(':
                        return CompileState.Src;
                    case ')':
                        throw new InvalidOperationException("wrong operation in source: )");
                }

                _builder.Append(Char.ToLower(c));
                return CompileState.Body;
            }

            public void Clear() => _builder.Clear();
        }

        sealed class Source : IExecutor
        {
            private StringBuilder _builder = new StringBuilder(Config.LENGTH_MAX_KEY);

            public int ResultToInt => throw new NotImplementedException("can't using integer result in executor:source.");
            public string ResultToStr => _builder.ToString();

            public CompileState Execute(char c)
            {
                switch (c)
                {
                    case ' ':
                        return CompileState.Src;
                    case ',':
                        return CompileState.Dest;
                    case ')':
                        throw new InvalidOperationException("wrong operation in source: )");
                }

                _builder.Append(c);
                return CompileState.Src;
            }

            public void Clear()
            {
                _builder.Clear();
            }
        }

        sealed class Destination : IExecutor
        {
            private StringBuilder _builder = new StringBuilder(Config.LENGTH_MAX_KEY);

            public int ResultToInt => throw new NotImplementedException("can't using integer result in executor:destination.");
            public string ResultToStr => _builder.ToString();

            public CompileState Execute(char c)
            {
                switch (c)
                {
                    case ' ':
                        return CompileState.Dest;
                    case ',':
                        return CompileState.Value;
                    case ')':
                        return CompileState.Finish;
                }

                _builder.Append(c);
                return CompileState.Dest;
            }

            public void Clear()
            {
                _builder.Clear();
            }
        }

        sealed class Value : IExecutor
        {
            private int _builder;
            public int ResultToInt => _builder;
            public string ResultToStr => throw new NotImplementedException("can't using string result in executor:value.");
            
            public CompileState Execute(char c)
            {
                switch (c)
                {
                    case ' ':
                        return CompileState.Value;
                    case ',':
                        return CompileState.Weight;
                    case ')':
                        return CompileState.Finish;
                }

                _builder *= 10;
                _builder += (c - '0');
                return CompileState.Value;
            }

            public void Clear()
            {
                _builder = 0;
            }
        }

        sealed class Weight : IExecutor
        {
            private int _builder;
            public int ResultToInt => _builder;
            public string ResultToStr => throw new NotImplementedException("can't using string result in executor:weight.");

            public CompileState Execute(char c)
            {
                switch (c)
                {
                    case ' ':
                        return CompileState.Weight;
                    case ')':
                        return CompileState.Finish;
                    case ',':
                        throw new InvalidOperationException("wrong operation in Weight: ,");
                }

                _builder *= 10;
                _builder += (c - '0');
                return CompileState.Weight;
            }

            public void Clear()
            {
                _builder = 0;
            }
        }

        class Committer
        {
            private Token token = null;
            private string tokenCommand = string.Empty;
            private string tokenSrc = string.Empty;
            private string tokenDest = string.Empty;
            private int tokenValue = 1;
            private int tokenWeight = 1;

            public Token Result => token.Head;

            public void Clear()
            {
                token = null;
                tokenCommand = string.Empty;
                tokenSrc = string.Empty;
                tokenDest = string.Empty;
                tokenValue = 1;
                tokenWeight = 1;
            }

            public void Execute(IExecutor[] executors)
            {
                tokenCommand = executors[1].ResultToStr;
                tokenSrc = executors[2].ResultToStr;
                tokenDest = executors[3].ResultToStr;
                tokenValue = executors[4].ResultToInt;
                tokenWeight = executors[5].ResultToInt;

                if (string.IsNullOrEmpty(tokenCommand)
                        || string.IsNullOrEmpty(tokenSrc)
                        || string.IsNullOrEmpty(tokenDest))
                    throw new InvalidOperationException("invaild finish result: token is null");

                if (Object.ReferenceEquals(Constants.IF, tokenCommand))
                {
                    token = TokenFactory.Create(TokenFlag.Read, tokenSrc, tokenDest, tokenValue, tokenWeight);
                }
                else if (Object.ReferenceEquals(Constants.AND, tokenCommand))
                {
                    token = token.AND(tokenSrc, tokenDest, tokenValue, tokenWeight);
                }
                else if (Object.ReferenceEquals(Constants.OR, tokenCommand))
                {
                    token = token.OR(tokenSrc, tokenDest, tokenValue, tokenWeight);
                }
                else if (Object.ReferenceEquals(Constants.ACT, tokenCommand))
                {
                    token = token.ACT(tokenSrc, tokenDest, tokenValue, tokenWeight);
                }
            }
        }
    }
}
