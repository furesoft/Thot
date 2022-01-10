using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace EbnfParserGenerator.Visitors;

public class NodeGeneratorVisitor : IVisitor<string>
{
    public SourceText Text(ASTNode node)
    {
        return SourceText.From(node.Accept(this), Encoding.ASCII);
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
        var sb = new StringBuilder();

        sb.AppendLine($"using System.Collections.Generic;");
        sb.AppendLine();
        sb.AppendLine($"namespace Parsing.AST;");

        sb.AppendLine($"public abstract class {typeDeclaration.Name} {{");

        sb.AppendLine($"\tpublic {typeDeclaration.Name}? Parent {{ get; set; }}");

        sb.AppendLine("\tpublic abstract T Accept<T>(IVisitor<T> visitor);");
        sb.AppendLine("}");

        foreach (SubTypeDeclaration subType in typeDeclaration.Block.Body.OfType<SubTypeDeclaration>())
        {
            sb.AppendLine(GenerateSubType(subType, typeDeclaration.Name));
        }

        var blockType = new SubTypeDeclaration("Block", new() { ("Body", $"List<{typeDeclaration.Name}>") });
        sb.AppendLine(GenerateSubType(blockType, typeDeclaration.Name));

        return sb.ToString();
    }

    public string Visit(SubTypeDeclaration subTypeDeclaration)
    {
        return string.Empty;
    }

    private string GenerateCtors(SubTypeDeclaration typeDeclaration, string name)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"\tpublic {typeDeclaration.Name}() {{}}");
        sb.AppendLine();

        sb.AppendLine($"\tpublic {typeDeclaration.Name}({name} parent) {{ this.Parent = parent; }}");
        sb.AppendLine();

        if (typeDeclaration.Properties.Any())
        {
            sb.AppendLine($"\tpublic {typeDeclaration.Name}({string.Join(",", typeDeclaration.Properties.Select(_ => $"{_.type} _{_.name.ToLower()}"))}, {name}? parent = null) {{");

            foreach (var prop in typeDeclaration.Properties)
            {
                sb.AppendLine($"\t\tthis.{prop.name} = _{prop.name.ToLower()};");
            }
            sb.AppendLine("\t\tthis.Parent = parent;");

            sb.AppendLine("\t}");
        }

        return sb.ToString();
    }

    private string GenerateSubType(SubTypeDeclaration subType, string name)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"public class {subType.Name} : {name} {{");

        sb.AppendLine();

        foreach (var prop in subType.Properties)
        {
            sb.AppendLine($"\tpublic {prop.type} {prop.name} {{ get; set; }}");
        }

        sb.AppendLine();

        sb.AppendLine(GenerateCtors(subType, name));

        sb.AppendLine("\tpublic override T Accept<T>(IVisitor<T> visitor) {");
        sb.AppendLine($"\t\treturn visitor.Visit(this);");
        sb.AppendLine("\t}");
        sb.AppendLine("}");

        return sb.ToString();
    }
}