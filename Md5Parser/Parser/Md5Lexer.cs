using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Parser
{
    public enum TokenType
    {
        Float, Integer, QuotedString, Word, LParen, RParen, LBrace, RBrace, Comment, Whitespace, EndOfFile
    }

    public class TokenDefinition
    {
        public TokenDefinition(Regex regex, TokenType type, bool isIgnored)
        {
            Regex = regex;
            Type = type;
            IsIgnored = isIgnored;
        }

        public TokenDefinition(Regex regex, TokenType type)
            : this(regex, type, false){}

        public bool IsIgnored { get; private set; }
        public Regex Regex { get; private set; }
        public TokenType Type { get; private set; }
    }

    public class Token
    {
        public Token(TokenType type, string value, TokenPosition tokenPosition)
        {
            Type = type;
            Value = value;
            Position = tokenPosition;
        }

        public TokenPosition Position { get; private set; }
        public TokenType Type { get; private set; }
        public string Value { get; private set; }

        public override string ToString()
        {
            return string.Format("Type: {0}, Value: {1}", Type, Value);
        }
    }

    public class TokenPosition
    {
        public TokenPosition(int currentIndex, int currentLine, int currentColumn)
        {
            Index = currentIndex;
            Line = currentLine;
            Column = currentColumn;
        }

        public int Column { get; private set; }
        public int Index { get; private set; }
        public int Line { get; private set; }

        public override string ToString()
        {
            return string.Format("Column: {0}, Index: {1}, Line: {2}", Column, Index, Line);
        }
    }

    public class Md5Lexer
    {
        private readonly IEnumerable<TokenDefinition> _tokenDefinitions;

        private readonly Regex _endOfLineRegex;

        public Md5Lexer()
        {
            _endOfLineRegex = new Regex(@"$");

            _tokenDefinitions = new List<TokenDefinition>
            {
                new TokenDefinition(new Regex(@"[-+]?\d+\.\d+"),  TokenType.Float),
                new TokenDefinition(new Regex(@"[-+]?\d+"),  TokenType.Integer),
                new TokenDefinition(new Regex(@"""(.*?)"""), TokenType.QuotedString),
                new TokenDefinition(new Regex(@"\w+"), TokenType.Word),
                new TokenDefinition(new Regex(@"\("),  TokenType.LParen),
                new TokenDefinition(new Regex(@"\)"), TokenType.RParen),
                new TokenDefinition(new Regex(@"{"),  TokenType.LBrace),
                new TokenDefinition(new Regex(@"}"), TokenType.RBrace),
                new TokenDefinition(new Regex(@"//.*\n"), TokenType.Comment, true),
                new TokenDefinition(new Regex(@"\s+"), TokenType.Whitespace, true)
            };
        }

        public IEnumerable<Token> Tokenize(string source)
        {
            int currentIndex = 0;
            int currentLine = 1;
            int currentColumn = 0;

            while (currentIndex < source.Length)
            {
                TokenDefinition matchedDefinition = null;
                int matchLength = 0;

                foreach (var rule in _tokenDefinitions)
                {
                    var match = rule.Regex.Match(source, currentIndex);

                    if (match.Success && match.Index - currentIndex == 0)
                    {
                        matchedDefinition = rule;
                        matchLength = match.Length;
                        break;
                    }
                }

                if (matchedDefinition == null)
                {
                    throw new Exception(
                        string.Format("Unrecognized symbol '{0}' at index {1} (line {2}, column {3}).", 
                            source[currentIndex], currentIndex, currentLine, currentColumn));
                }
                else
                {
                    var value = source.Substring(currentIndex, matchLength);

                    if (!matchedDefinition.IsIgnored)
                        yield return new Token(matchedDefinition.Type, value,
                            new TokenPosition(currentIndex, currentLine, currentColumn));

                    var endOfLineMatch = _endOfLineRegex.Match(value);
                    if (endOfLineMatch.Success)
                    {
                        currentLine += 1;
                        currentColumn = value.Length - (endOfLineMatch.Index + endOfLineMatch.Length);
                    }
                    else
                    {
                        currentColumn += matchLength;
                    }

                    currentIndex += matchLength;
                }
            }

            yield return new Token(TokenType.EndOfFile, "", new TokenPosition(currentIndex, currentLine, currentColumn));
        }
    }
}
