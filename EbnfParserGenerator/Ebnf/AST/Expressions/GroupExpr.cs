namespace EbnfParserGenerator.Ebnf.AST.Expressions
{
    public class GroupExpr : Expr
    {
        public GroupExpr(Expr expression)
        {
            Expression = expression;
        }

        public Expr Expression { get; set; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"({Expression})";
        }
    }
}