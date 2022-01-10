using EbnfParserGenerator.Ebnf.AST.Expressions;

namespace EbnfParserGenerator.Ebnf.AST;

public interface IVisitor<T>
{
    T Visit(RuleNode rule);

    T Visit(InvalidNode invalidNode);

    T Visit(LiteralNode literal);

    T Visit(TypeDeclaration typeDeclaration);

    T Visit(SubTypeDeclaration subTypeDeclaration);
    T Visit(CompilationUnit compilationUnit);
    T Visit(InvalidExpr invalidExpr);

    T Visit(GroupExpr groupExpr);

    T Visit(TokenSymbolNode tokenSymbolNode);

    T Visit(Block block);

    T Visit(OptionalExpression optionalExpression);

    T Visit(GrammarNode grammarNode);

    T Visit(NameExpression nameExpression);

    T Visit(ZeroOrMoreExpression zeroOrMoreExpression);

    T Visit(RangeExpr charackterClassRange);

    T Visit(OneOrMoreExpression oneOrMoreExpression);

    T Visit(AlternateNode alternateNode);

    T Visit(NotExpression notExpression);

    T Visit(TokenSpecNode tokenSpecNode);
}