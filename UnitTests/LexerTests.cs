using EbnfParserGenerator.Ebnf;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void Identifiers_Should_Pass()
        {
            AssertToken("hello123", TokenType.Identifier);
            AssertToken("aBc", TokenType.Identifier);
        }

        [TestMethod]
        public void Keywords_Should_Pass()
        {
            AssertToken("token", TokenType.TokenKeyword);
            AssertToken("type", TokenType.TypeKeyword);
            AssertToken("grammar", TokenType.GrammarKeyword);
            AssertToken("for", TokenType.For);
        }

        [TestMethod]
        public void Number_Should_Pass()
        {
            AssertToken("123", TokenType.Number);
        }

        [TestMethod]
        public void String_Should_Pass()
        {
            AssertToken("'hello world 123'", TokenType.StringLiteral);
            AssertToken("\"hello world 123\"", TokenType.StringLiteral);
        }

        [TestMethod]
        public void Symbol_Should_Pass()
        {
            AssertToken("->", TokenType.GoesTo);
            AssertToken(";", TokenType.Semicolon);
            AssertToken("|", TokenType.Pipe);
            AssertToken("+", TokenType.Plus);
            AssertToken("*", TokenType.Star);
            AssertToken("?", TokenType.Question);
            AssertToken("!", TokenType.Exclamation);
            AssertToken("(", TokenType.OpenParen);
            AssertToken(")", TokenType.CloseParen);
            AssertToken("{", TokenType.OpenCurly);
            AssertToken("}", TokenType.CloseCurly);
        }

        private static void AssertToken(string src, TokenType tokenType)
        {
            var lexer = new Lexer();

            var tokens = lexer.Tokenize(src);

            Assert.IsTrue(lexer.Messages.Count == 0);
            Assert.AreEqual(2, tokens.Count);

            Assert.AreEqual(tokens[0].Type, tokenType);
            Assert.AreEqual(tokens[1].Type, TokenType.EOF);
        }
    }
}