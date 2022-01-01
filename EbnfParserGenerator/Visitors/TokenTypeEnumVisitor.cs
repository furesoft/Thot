using EbnfParserGenerator.Ebnf.AST;
using EbnfParserGenerator.Ebnf.AST.Expressions;

namespace EbnfParserGenerator.Visitors
{
    public class TokenTypeEnumVisitor : IVisitor<ASTNode>
    {
        public TokenTypeEnumVisitor()
        {
        }

        public ASTNode Visit(RuleNode rule)
        {
            return null;
        }

        public ASTNode Visit(InvalidNode invalidNode)
        {
            return null;
        }

        public ASTNode Visit(LiteralNode literal)
        {
            return null;
        }

        public ASTNode Visit(CharacterClassExpression charackterClassExpression)
        {
            return null;
        }

        public ASTNode Visit(InvalidExpr invalidExpr)
        {
            return null;
        }

        public ASTNode Visit(GroupExpr groupExpr)
        {
            return null;
        }

        public ASTNode Visit(TokenSymbolNode tokenSymbolNode)
        {
            return null;
        }

        public ASTNode Visit(Block block)
        {
            throw new NotImplementedException();
        }

        public ASTNode Visit(OptionalExpression optionalExpression)
        {
            return null;
        }

        public ASTNode Visit(NameExpression nameExpression)
        {
            return null;
        }

        public ASTNode Visit(ZeroOrMoreExpression zeroOrMoreExpression)
        {
            return null;
        }

        public ASTNode Visit(CharackterClassRange charackterClassRange)
        {
            return null;
        }

        public ASTNode Visit(OneOrMoreExpression oneOrMoreExpression)
        {
            return null;
        }

        public ASTNode Visit(AlternateNode alternateNode)
        {
            return null;
        }

        public ASTNode Visit(NotExpression notExpression)
        {
            return null;
        }

        public ASTNode Visit(TokenSpecNode tokenSpecNode)
        {
            return null;
        }
    }
}