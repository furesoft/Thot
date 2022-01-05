using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors;

public class PrintVisitorGeneratorVisitor : IVisitor<string>
{
    public SourceText Text(ASTNode node)
    {
        return SourceText.From(node.Accept(this), Encoding.ASCII);
    }

    public string Visit(RuleNode rule)
    {
        return string.Empty;
    }

    public string Visit(GrammarNode grammarNode)
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

        sb.AppendLine("using Parsing.AST;");
        sb.AppendLine("using System.Text;");
        sb.AppendLine("");
        sb.AppendLine("namespace Parsing;");
        sb.AppendLine("");

        sb.AppendLine("public class PrintVisitor : IVisitor<string> {");
        foreach (var node in block.Body)
        {
            var visited = node.Accept(this);

            if (visited != string.Empty)
            {
                sb.Append(visited);
            }
        }

        sb.AppendLine("\tpublic string Visit(Block block) {");
        sb.AppendLine("\t\tvar sb = new StringBuilder();");

        sb.AppendLine("\t\tsb.AppendLine(\"(Block \");");
        sb.AppendLine("\t\tforeach(var node in block.Body) {");
        sb.AppendLine("\t\t\tsb.AppendLine(\"\t\" + node.Accept(this));");
        sb.AppendLine("\t\t}");
        sb.AppendLine("\t\tsb.AppendLine(\")\");");

        sb.AppendLine("\t\treturn sb.ToString();");
        sb.AppendLine("\t}");

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

        foreach (var node in typeDeclaration.Block.Body)
        {
            var visited = node.Accept(this);

            if (visited != string.Empty)
            {
                sb.Append(visited);
            }
        }

        return sb.ToString();
    }

    public string Visit(SubTypeDeclaration subTypeDeclaration)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"\tpublic string Visit({subTypeDeclaration.Name} {subTypeDeclaration.Name.FirstCharToLower()}) {{");

        sb.AppendLine($"\t\treturn $\"({subTypeDeclaration.Name} " + string.Join(" ", subTypeDeclaration.Properties.Select(_ => "{" + subTypeDeclaration.Name.FirstCharToLower() + "." + _.name + "}")) + ")\";");

        sb.AppendLine("\t}");

        return sb.ToString();
    }
}