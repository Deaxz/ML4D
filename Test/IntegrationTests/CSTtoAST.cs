using Antlr4.Runtime;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ML4D.Compiler;
using ML4D.Compiler.Nodes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ML4DParser;

namespace Test
{
    [TestClass]
    public class CSTtoAST
    {
        [TestMethod]
        public void ScalarMulCSTtoAST()
        {
            string source = "double a = 2.5; tensor b[2][2] = { [80, 25.2], [19.6, 66] }; tensor c[2][2] = a * b;";

            AntlrInputStream inputStream = new AntlrInputStream(new StringReader(source));
            Lexer lexer = new ML4DLexer(inputStream);
            CommonTokenStream tokenStream = new CommonTokenStream(lexer);
            ML4DParser parser = new ML4DParser(tokenStream);
            LinesContext cst = parser.lines();
            LinesNode ast = new ASTBuilder().VisitLines(cst);

            Assert.IsTrue(ast.lines.Count == 3);
            Assert.IsTrue(ast.lines[0] is VariableDCLNode);
            List<Node> children0 = ast.lines[0].GetChildren();
            Assert.IsTrue(children0.Count == 1 && children0[0] is DoubleNode);

            Assert.IsTrue(ast.lines[1] is TensorDCLNode);
            List<Node> children1 = ast.lines[1].GetChildren();
            Assert.IsTrue(children1.Count == 1 && children1[0] is TensorInitNode);

            Assert.IsTrue(ast.lines[2] is TensorDCLNode);
            List<Node> children2 = ast.lines[2].GetChildren();
            Assert.IsTrue(children2.Count == 1 && children2[0] is MultiplicationNode);

            List<Node> mulChilldren = children2[0].GetChildren();
            Assert.IsTrue(mulChilldren[0] is IDNode);
            Assert.IsTrue(mulChilldren[1] is IDNode);
        }
        
    }
}
