using EbnfParserGenerator.Ebnf.AST.Expressions;
using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf.AST
{
    public interface IVisitor<T>
    {
        T Visit(RuleNode rule);

        T Visit(LiteralNode literal);
        T Visit(GroupExpr groupExpr);
        T Visit(OptionalExpression optionalExpression);
        T Visit(NameExpression nameExpression);
        T Visit(ZeroOrMoreExpression zeroOrMoreExpression);
        T Visit(OneOrMoreExpression oneOrMoreExpression);
        T Visit(AlternateNode alternateNode);
        T Visit(NotExpression notExpression);
    }
}