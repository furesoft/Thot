using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors;

public class ParserGeneratorVisitor : IVisitor<string>
{
    public SourceText Text(ASTNode node)
    {
        var source = node.Accept(this);
        return SourceText.From(source, Encoding.ASCII);
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
        var sb = new StringBuilder();

        sb.AppendLine($"using Parsing.AST;");

        sb.AppendLine();
        sb.AppendLine($"namespace Parsing;");

        sb.AppendLine($"public class Parser : BaseParser<{typeDeclaration.Name}, Lexer, Parser> {{");

        sb.AppendLine(GenerateCtor());

        sb.AppendLine($"\t protected override {typeDeclaration.Name} Start() {{");

        //sb.AppendLine(node.Accept(this));

        sb.AppendLine("\t\treturn default; \n");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        return sb.ToString();
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

    private string GenerateCtor()
    {
        var sb = new StringBuilder();

        sb.AppendLine("\tParser(List<Token> tokens, List<Message> messages) : base(tokens, messages) {}");

        return sb.ToString();
    }
}