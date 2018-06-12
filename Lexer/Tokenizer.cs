using System;
using System.Collections.Generic;
using Lexer.Entity;

namespace Lexer
{
    public class Tokenizer
    {
        private string _str;
        private int _curIndex;
        private int _curRow;
        private int _curLine;
        private int _curTokenPos;
        private char _curChar;
        private List<Token> _tokens;
        private List<string> _ids;
        private HashSet<string> _keyWords;
        private HashSet<char> _unaryOperations;
        private HashSet<string> _binaryOperations;
        private HashSet<char> _separators;
        private HashSet<char> _endToken;
        private HashSet<char> _emptySymbols;

        public class Builder
        {
            private HashSet<string> _keyWords;
            private HashSet<char> _unaryOperations;
            private HashSet<string> _binaryOperations;
            private HashSet<char> _separators;
            private HashSet<char> _endTokens;
            private HashSet<char> _emptySymbols;


            public Builder WithKeyWords(HashSet<string> keyWords)
            {
                _keyWords = keyWords;
                return this;
            }

            public Builder WithUnaryOperations(HashSet<char> unaryOperations)
            {
                _unaryOperations = unaryOperations;
                return this;
            }

            public Builder WithBinaryOperations(HashSet<string> binaryOperations)
            {
                _binaryOperations = binaryOperations;
                return this;
            }

            public Builder WithSeparators(HashSet<char> separators)
            {
                _separators = separators;
                return this;
            }

            public Builder WithEndTokens(HashSet<char> endTokens)
            {
                _endTokens = endTokens;
                return this;
            }

            public Builder WithEmptySymbols(HashSet<char> emptySymbols)
            {
                _emptySymbols = emptySymbols;
                return this;
            }

            public Tokenizer Build()
            {
                var tokenizer = new Tokenizer
                {
                    _keyWords = _keyWords,
                    _unaryOperations = _unaryOperations,
                    _binaryOperations = _binaryOperations,
                    _separators = _separators,
                    _endToken = _endTokens,
                    _emptySymbols = _emptySymbols
                };
                return tokenizer;
            }
        }

        public Result tokenize(string str)
        {
            Init(str.Trim());

            var token = NextToken();
            while (!Types.EndToken.Equals(token.Type))
            {
                token = NextToken();
            }

            var result = new Result(_ids, _tokens);
            return result;
        }

        private Tokenizer()
        {
        }

        private void Init(string str)
        {
            this._str = str;
            _curIndex = 0;
            _curRow = 0;
            _curLine = 0;
            _curTokenPos = 0;
            _tokens = new List<Token>();
            _ids = new List<string>();
        }

        private Token NextToken()
        {

            if (_curIndex >= _str.Length)
                return null;

            _curChar = _str[_curIndex];
            _curTokenPos = _curRow;

            if (char.IsLetter(_curChar))
            {
                return ObtainLetterToken();
            }

            if (char.IsDigit(_curChar))
            {
                return ObtainConstToken();
            }

            return ObtainOtherTokens();
        }

        // Token processors

        private Token ObtainLetterToken()
        {
            var token = "";
            var i = _curIndex;
            var ch = _str[i];
            while (char.IsLetter(ch))
            {
                token += ch;
                i++;
                ch = _str[i];
            }

            if (IsKeyWord(token))
            {
                return AddToken(token, Types.KeyWord, GetPosition());
            }

            if (_ids.IndexOf(token) < 0)
            {
                _ids.Add(token);
            }

            return AddToken(token, Types.Id, GetPosition());
        }

        private Token ObtainConstToken()
        {
            var token = "";
            var i = _curIndex;
            while (char.IsDigit(_str[i]))
            {
                token += _curChar;
                i++;
            }

            return AddToken(token, Types.Const, GetPosition());
        }


        private Token ObtainOtherTokens()
        {

            var token = ObtainAmbiguousToken();
            if (token != null)
            {
                return token;
            }

            if (IsLineBreaker(_curChar))
            {
                IncLine();
                return NextToken();
            }

            if (_emptySymbols.Contains(_curChar))
            {
                IncIndex(1);
                return NextToken();
            }

            if (_unaryOperations.Contains(_curChar))
            {
                return AddToken(_curChar, Types.UnaryOperator, GetPosition());
            }

            if (_separators.Contains(_curChar))
            {
                return AddToken(_curChar, Types.Separator, GetPosition());
            }

            if (_binaryOperations.Contains(_curChar.ToString()))
            {
                return AddToken(_curChar, Types.BinaryOperator, GetPosition());
            }

            if (IsEndToken(_curChar))
            {
                return AddToken(_curChar, Types.EndToken, GetPosition());
            }

            return AddToken(_curChar, Types.Undefined, GetPosition());
        }

        private Token ObtainAmbiguousToken()
        {
            switch (_curChar)
            {
                case ':':
                {
                    var token = _curChar.ToString();
                    token += _str[_curIndex + 1];
                    return ":=" == token
                        ? AddToken(token, Types.BinaryOperator, GetPosition())
                        : AddToken(_curChar, Types.Separator, GetPosition());
                }
                case '=':
                {
                    var token = _curChar.ToString();
                    token += _str[_curIndex + 1];
                    if ("==" == token)
                    {
                        return AddToken(token, Types.BinaryOperator, GetPosition());
                    }

                    break;
                }
                default:
                    return null;
            }

            return null;
        }

        // Сhecks

        private bool IsKeyWord(string str)
        {
            return _keyWords.Contains(str);
        }

        private bool IsEndToken(char ch)
        {
            return _endToken.Contains(ch);
        }

        private bool IsLineBreaker(char ch)
        {
            return ch == '\n';
        }

        // Utils

        private void IncIndex(int i)
        {
            _curIndex += i;
            _curRow += i;
        }

        private void IncLine()
        {
            _curIndex++;
            _curLine++;
            _curRow = 0;
            _curChar = _str[_curIndex];
        }

        private Position GetPosition()
        {
            return new Position(_curLine, _curTokenPos);
        }

        private Token AddToken<T>(T val, Types type, Position pos)
        {
            var value = val.ToString();
            IncIndex(value.Length);
            if (Types.Id.Equals(type))
            {
                value = _ids.IndexOf(value).ToString();
            }

            var token = new Token(value, type, pos);
            _tokens.Add(token);
            return token;
        }
    }
}