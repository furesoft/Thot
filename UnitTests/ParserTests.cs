using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using Thot.Ebnf;
using Thot.Ebnf.AST;

namespace UnitTests;

[TestClass]
public class ParserTests
{
    [TestMethod]
    public void Expression()
    {
        var src = "grammar Main for Expr { start -> main; main -> 'hello'? | 'world'+ | 'some'; };";
        var ast = Parser.Parse(new SourceDocument("test.grammar", src));
        Assert.IsNotNull(ast);
    }

    [TestMethod]
    public void SimpleToken_Should_Pass()
    {
        var src = "token 'hello';";
        var p = Parser.Parse(new SourceDocument("test.grammar", src));

        AssertNoError(p);

        Assert.IsTrue(p.Tree is Block b && b.Body.OfType<TokenSymbolNode>().Any());

        var body = ((Block)p.Tree).Body;

        var node = body[0];

        Assert.IsTrue(node is TokenSymbolNode t && t.Symbol == "hello" && t.Name == "Hello");
    }

    [TestMethod]
    public void Type_Should_Pass()
    {
        var src = "type Expr -> | Literal(Value : Object);";
        var p = Parser.Parse(new SourceDocument("test.grammar", src));

        AssertNoError(p);

        Assert.IsTrue(p.Tree is Block b && b.Body.OfType<TypeDeclaration>().Any());

        var body = ((Block)p.Tree).Body;

        var node = body[0];

        Assert.IsTrue(node is TypeDeclaration t && t.Name == "Expr" && t.Block.Body.Count == 1);
    }

    [TestMethod]
    public void Type_Without_Property_Should_Pass()
    {
        var src = "type Expr -> | Literal();";
        var p = Parser.Parse(new SourceDocument("test.grammar", src));

        AssertNoError(p);

        Assert.IsTrue(p.Tree is Block b && b.Body.OfType<TypeDeclaration>().Any());

        var body = ((Block)p.Tree).Body;

        var node = body[0];

        Assert.IsTrue(node is TypeDeclaration t && t.Name == "Expr" && t.Block.Body.Count == 1);
    }

    private static void AssertNoError((ASTNode Tree, List<Message> Messages) p)
    {
        Assert.IsTrue(p.Messages.Count == 0);
        Assert.IsNotNull(p.Tree);
        Assert.IsTrue(p.Tree is Block b && b.Body.Count > 0);
    }
}