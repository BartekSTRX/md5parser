using System;
using System.Collections.Generic;
using System.Globalization;

namespace Parser.Old
{
    public abstract class Md5Parser
    {
        protected Queue<Token> TokenQueue;

        private void ParseSimpleToken(TokenType expectedTokenType, string errorMessage)
        {
            if (TokenQueue.Dequeue().Type != expectedTokenType)
            {
                throw new Exception(errorMessage);
            }
        }

        protected void ParseLeftParenthesis()
        {
            ParseSimpleToken(TokenType.LParen, "left parenthesis '(' expected");
        }

        protected void ParseRightParenthesis()
        {
            ParseSimpleToken(TokenType.RParen, "right parenthesis ')' expected");
        }

        protected void ParseLeftBrace()
        {
            ParseSimpleToken(TokenType.LBrace, "left brace '{' expected");
        }

        protected void ParseRightBrace()
        {
            ParseSimpleToken(TokenType.RBrace, "right brace '}' expected");
        }

        protected int ParseInt(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.Integer && token.Type != TokenType.Float)
            {
                throw new Exception(name + " expected (integer number)");
            }
            return int.Parse(token.Value);
        }

        protected float ParseFloat(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.Float && token.Type != TokenType.Integer)
            {
                throw new Exception(name + " expected (floating point number)");
            }
            return float.Parse(token.Value, CultureInfo.InvariantCulture);
        }

        protected string ParseQuotedString(string name)
        {
            var token = TokenQueue.Dequeue();
            if (token.Type != TokenType.QuotedString)
            {
                throw new Exception(name + " expected (quoted string)");
            }
            return token.Value;
        }

        protected void ParseKeyword(string name)
        {
            var token = TokenQueue.Dequeue();
            if (!(token.Type == TokenType.Word && token.Value == name))
            {
                throw new Exception(name + " keyword expected");
            }
        }

        protected void AssertCorrectListSize<T>(string name, IList<T> list, int expectedSize)
        {
            if (list.Count != expectedSize)
            {
                throw new Exception(string.Format(
                    "number of parsed {0} ({1}) is different than expected ({2})", name, list.Count, expectedSize));
            }
        }

        protected IList<T> ParseList<T>(string blockName, Func<T> parseItem)
        {
            ParseKeyword(blockName);

            ParseLeftBrace();

            var items = new List<T>();
            while (TokenQueue.Peek().Type != TokenType.RBrace)
            {
                items.Add(parseItem());
            }
            ParseRightBrace();

            return items;
        }
    }
}