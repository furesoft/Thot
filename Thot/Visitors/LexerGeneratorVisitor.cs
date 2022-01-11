using Microsoft.CodeAnalysis.Text;
using System.Text;
using Thot.Ebnf.AST.Expressions;
using Thot.Ebnf.AST;

namespace Thot.Visitors;

public class LexerGeneratorVisitor : IVisitor<string>
{
    public SourceText Text(ASTNode node)
    {
        var sb = GenerateClass(node);

        return SourceText.From(sb.ToString(), Encoding.ASCII);
    }

    public string Visit(CompilationUnit compilationUnit)
    {
        return compilationUnit.Body.Accept(this);
    }

    public string Visit(GrammarNode grammarNode)
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
        return string.Empty;
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

    private StringBuilder GenerateClass(ASTNode node)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"namespace Parsing;");

        sb.AppendLine("public class Lexer : BaseLexer {");

        sb.AppendLine("\t protected override Token NextToken() {");

        sb.AppendLine(node.Accept(this));

        sb.AppendLine("return Token.Invalid; \n}");
        sb.AppendLine("}");
        return sb;
    }
}
