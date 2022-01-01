using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors
{
    public class IVisitorGeneratorVisitor : IVisitor<string>
    {
        public SourceText Text(ASTNode node)
        {
            return SourceText.From(node.Accept(this), Encoding.ASCII);
        }

        public string Visit(RuleNode rule)
        {
            return null;
        }

        public string Visit(InvalidNode invalidNode)
        {
            return null;
        }

        public string Visit(LiteralNode literal)
        {
            return null;
        }

        public string Visit(CharacterClassExpression charackterClassExpression)
        {
            return null;
        }

        public string Visit(InvalidExpr invalidExpr)
        {
            return null;
        }

        public string Visit(GroupExpr groupExpr)
        {
            return null;
        }

        public string Visit(TokenSymbolNode tokenSymbolNode)
        {
            return null;
        }

        public string Visit(Block block)
        {
            var sb = new StringBuilder();

            sb.AppendLine("public interface IVisitor<T> {");
            foreach (var node in block.Body)
            {
                var visited = node.Accept(this);

                if (visited != null)
                {
                    // sb.AppendLine("\t" + visited + ";");
                }
            }

            sb.AppendLine("}");

            return sb.ToString();
        }

        public string Visit(OptionalExpression optionalExpression)
        {
            return null;
        }

        public string Visit(NameExpression nameExpression)
        {
            return null;
        }

        public string Visit(ZeroOrMoreExpression zeroOrMoreExpression)
        {
            return null;
        }

        public string Visit(CharackterClassRange charackterClassRange)
        {
            return null;
        }

        public string Visit(OneOrMoreExpression oneOrMoreExpression)
        {
            return null;
        }

        public string Visit(AlternateNode alternateNode)
        {
            return null;
        }

        public string Visit(NotExpression notExpression)
        {
            return null;
        }

        public string Visit(TokenSpecNode tokenSpecNode)
        {
            return null;
        }
    }
}