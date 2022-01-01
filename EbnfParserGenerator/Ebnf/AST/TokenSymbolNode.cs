namespace EbnfParserGenerator.Ebnf.AST
{
    public class TokenSymbolNode : ASTNode
    {
        public TokenSymbolNode(string symbol)
        {
            Symbol = symbol;
            Name = symbol.FirstCharToUpper();
        }

        public string Name { get; }
        public string Symbol { get; }

        public override T Accept<T>(IVisitor<T> visitor)
        {
            return visitor.Visit(this);
        }

        public override string ToString()
        {
            return $"@token '{Symbol}';";
        }
    }
}