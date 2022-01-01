using EbnfParserGenerator.Ebnf.AST.Expressions;
using EbnfParserGenerator.Ebnf.AST;

namespace EbnfParserGenerator.Ebnf.AST
{
    public interface IVisitor<T>
    {
        T Visit(RuleNode rule);
        T Visit(InvalidNode invalidNode);
        T Visit(LiteralNode literal);
        T Visit(CharacterClassExpression charackterClassExpression);
        T Visit(InvalidExpr invalidExpr);
        T Visit(GroupExpr groupExpr);
        T Visit(OptionalExpression optionalExpression);
        T Visit(NameExpression nameExpression);
        T Visit(ZeroOrMoreExpression zeroOrMoreExpression);
        T Visit(CharackterClassRange charackterClassRange);
        T Visit(OneOrMoreExpression oneOrMoreExpression);
        T Visit(AlternateNode alternateNode);
        T Visit(NotExpression notExpression);
    }
}