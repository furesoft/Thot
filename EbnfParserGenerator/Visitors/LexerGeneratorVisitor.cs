using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors
{
    public class LexerGeneratorVisitor : IVisitor<string>
    {
        public SourceText Text(ASTNode node)
        {
            var sb = GenerateClass(node);

            return SourceText.From(sb.ToString(), Encoding.ASCII);
        }

        public string Visit(RuleNode rule)
        {
            return string.Empty;
        }

        public string Visit(InvalidNode invalidNode)
        {
            return string.Empty;
        }

        public string Visit(LiteralNode literal)
        {
            return string.Empty;
        }

        public string Visit(CharacterClassExpression charackterClassExpression)
        {
            return string.Empty;
        }

        public string Visit(InvalidExpr invalidExpr)
        {
            return string.Empty;
        }

        public string Visit(GroupExpr groupExpr)
        {
            return string.Empty;
        }

        public string Visit(TokenSymbolNode tokenSymbolNode)
        {
            return string.Empty;
        }

        public string Visit(Block block)
        {
            var sb = new StringBuilder();

            foreach (var node in block.Body)
            {
                var visited = node.Accept(this);

                if (visited != string.Empty)
                {
                    sb.AppendLine(visited);
                }
            }

            return sb.ToString();
        }

        public string Visit(OptionalExpression optionalExpression)
        {
            return string.Empty;
        }

        public string Visit(NameExpression nameExpression)
        {
            return string.Empty;
        }

        public string Visit(ZeroOrMoreExpression zeroOrMoreExpression)
        {
            return string.Empty;
        }

        public string Visit(CharackterClassRange charackterClassRange)
        {
            return string.Empty;
        }

        public string Visit(OneOrMoreExpression oneOrMoreExpression)
        {
            return string.Empty;
        }

        public string Visit(AlternateNode alternateNode)
        {
            return string.Empty;
        }

        public string Visit(NotExpression notExpression)
        {
            return string.Empty;
        }

        public string Visit(TokenSpecNode tokenSpecNode)
        {
            return string.Empty;
        }

        public string Visit(TypeDeclaration typeDeclaration)
        {
            return string.Empty;
        }

        public string Visit(SubTypeDeclaration subTypeDeclaration)
        {
            return string.Empty;
        }

        private StringBuilder GenerateClass(ASTNode node)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"namespace Parsing.AST;");

            sb.AppendLine("public class Lexer {");

            sb.AppendLine(GenerateMethods());

            sb.AppendLine("\t private Token? NextToken() {");

            sb.AppendLine(node.Accept(this));

            sb.AppendLine("}");
            return sb;
        }

        private string GenerateMethods()
        {
            throw new NotImplementedException();
        }
    }
}