using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors;

public class TokenTypeEnumVisitor : IVisitor<string>
{
    public SourceText Text(ASTNode node)
    {
        return SourceText.From(node.Accept(this), Encoding.ASCII);
    }

    public string Visit(GrammarNode grammarNode)
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
        return tokenSymbolNode.Name;
    }

    public string Visit(Block block)
    {
        var sb = new StringBuilder();

        sb.AppendLine("namespace Parsing;");
        sb.AppendLine();

        sb.AppendLine("public enum TokenType {");

        sb.AppendLine("\tInvalid,");
        sb.AppendLine("\tEOF,");

        foreach (var node in block.Body)
        {
            var visited = node.Accept(this);

            if (visited != string.Empty)
            {
                sb.AppendLine("\t" + visited + ",");
            }
        }

        sb.AppendLine("}");

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

    public string Visit(RangeExpr charackterClassRange)
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
        return tokenSpecNode.Rule.Name.FirstCharToUpper();
    }
}