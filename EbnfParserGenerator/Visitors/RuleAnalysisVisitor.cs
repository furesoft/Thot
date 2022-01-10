using EbnfParserGenerator.Ebnf;
using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;

namespace EbnfParserGenerator.Visitors;

public class RuleAnalysisVisitor : IVisitor<bool>
{
    private readonly List<string> _ruleDefintionNames = new();
    public bool HasNoGrammarBlock { get; private set; }
    public List<Message> Messages { get; set; } = new();

    public bool Visit(RuleNode rule)
    {
        return rule.Body.Accept(this);
    }

    public bool Visit(InvalidNode invalidNode)
    {
        return false;
    }

    public bool Visit(LiteralNode literal)
    {
        return false;
    }

    public bool Visit(TypeDeclaration typeDeclaration)
    {
        return false;
    }

    public bool Visit(SubTypeDeclaration subTypeDeclaration)
    {
        return false;
    }

    public bool Visit(InvalidExpr invalidExpr)
    {
        return false;
    }

    public bool Visit(GroupExpr groupExpr)
    {
        return groupExpr.Expression.Accept(this);
    }

    public bool Visit(TokenSymbolNode tokenSymbolNode)
    {
        return false;
    }

    public bool Visit(Block block)
    {
        var grammarNode = block.Body.OfType<GrammarNode>().FirstOrDefault();
        if (block.Parent is null)
        {
            if (grammarNode == null)
            {
                HasNoGrammarBlock = true;

                return false;
            }
            else
            {
                var rules = grammarNode?.Body.Body.OfType<RuleNode>();

                if (!rules.Any() || rules.FirstOrDefault(_ => _.Name == "start") == null)
                {
                    Messages.Add(Message.Error("Grammar has to define a 'start' rule", 1, 1));
                    return true;
                }
            }
        }

        return block.Body.Select(_ => _.Accept(this)).Aggregate((f, s) => f || s);
    }

    public bool Visit(OptionalExpression optionalExpression)
    {
        return optionalExpression.Expression.Accept(this);
    }

    public bool Visit(GrammarNode grammarNode)
    {
        var collectResult = CollectRules(grammarNode.Body);

        if (!collectResult)
        {
            return grammarNode.Body.Accept(this);
        }

        return false;
    }

    public bool Visit(NameExpression nameExpression)
    {
        var result = !_ruleDefintionNames.Contains(nameExpression.Name);

        if (result)
        {
            Messages.Add(Message.Error($"'{nameExpression.Name}' rule is not defined", nameExpression.NameToken.Line, nameExpression.NameToken.Column));
        }

        return result;
    }

    public bool Visit(ZeroOrMoreExpression zeroOrMoreExpression)
    {
        return zeroOrMoreExpression.Expression.Accept(this);
    }

    public bool Visit(RangeExpr charackterClassRange)
    {
        return false;
    }

    public bool Visit(OneOrMoreExpression oneOrMoreExpression)
    {
        return oneOrMoreExpression.Expression.Accept(this);
    }

    public bool Visit(AlternateNode alternateNode)
    {
        var left = alternateNode.Left.Accept(this);
        var right = alternateNode.Right.Accept(this);

        return left || right;
    }

    public bool Visit(NotExpression notExpression)
    {
        return notExpression.Expression.Accept(this);
    }

    public bool Visit(TokenSpecNode tokenSpecNode)
    {
        return false;
    }

    private bool CollectRules(Block body)
    {
        bool hasError = false;
        foreach (var rule in body.Body.OfType<RuleNode>())
        {
            if (_ruleDefintionNames.Contains(rule.Name))
            {
                hasError = true;

                Messages.Add(Message.Error($"Rule '{rule.Name}' already defined", rule.NameToken.Line, rule.NameToken.Column));
            }
            else
            {
                _ruleDefintionNames.Add(rule.Name);
            }
        }

        return hasError;
    }
}