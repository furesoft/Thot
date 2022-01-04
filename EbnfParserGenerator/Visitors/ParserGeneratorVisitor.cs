using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors;

public class ParserGeneratorVisitor : IVisitor<string>
{
    public bool HasStartRule { get; set; }

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

    public string Visit(GrammarNode grammarNode)
    {
        HasStartRule = grammarNode.Body.Body.OfType<RuleNode>().FirstOrDefault(_ => _.Name.Equals("start")) != null;

        if (HasStartRule)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"using System.Collections.Generic;");
            sb.AppendLine($"using Parsing.AST;");

            sb.AppendLine();
            sb.AppendLine($"namespace Parsing;");

            sb.AppendLine($"public class {grammarNode.Name.FirstCharToUpper()}Parser : BaseParser<{grammarNode.Type.FirstCharToUpper()}, Lexer, {grammarNode.Name}Parser> {{");

            sb.AppendLine(GenerateCtor(grammarNode.Name.FirstCharToUpper()));

            sb.AppendLine($"\tprotected override {grammarNode.Type} Start() {{");

            //sb.AppendLine(node.Accept(this));

            sb.AppendLine("\t\treturn default;");
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            return sb.ToString();
        }

        return string.Empty;
    }

    private string GenerateCtor(string name)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"\t{name}Parser(List<Token> tokens, List<Message> messages) : base(tokens, messages) {{}}");

        return sb.ToString();
    }
}