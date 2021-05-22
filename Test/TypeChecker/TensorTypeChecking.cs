using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ML4D.Compiler.Nodes;
using ML4D.Compiler.ASTVisitors;
using ML4D.Compiler;
using ML4D.Compiler.Exceptions;

namespace Test
{
    [TestClass]
    public class TensorTypeChecking
    {
        [TestMethod]
        public void IllegalMatrixMultiplicationTest()
        {
            // tensor a[2][1] = { [2.4], [5.1] }
            // tensor b[5][2] = { [1, 1],
            //                    [1, 1],
            //                    [1, 1],
            //                    [1, 1],
            //                    [1, 1] }
            // tencor c[2][2] = a*b;

            TensorInitNode aInit = new TensorInitNode();
            aInit.FirstRowElements = new List<ExpressionNode>()
            {
                new DoubleNode(2.4)
            };
            aInit.Elements = new List<ExpressionNode>()
            {
                new DoubleNode(5.1)
            };
            TensorDCLNode a = new TensorDCLNode("tensor", "a", 2, 1, aInit);

            TensorInitNode bInit = new TensorInitNode();
            bInit.FirstRowElements = new List<ExpressionNode>()
            {
                new DoubleNode(1), new DoubleNode(1)
            };
            bInit.Elements = new List<ExpressionNode>()
            {
                new DoubleNode(1), new DoubleNode(1),
                new DoubleNode(1), new DoubleNode(1),
                new DoubleNode(1), new DoubleNode(1),
                new DoubleNode(1), new DoubleNode(1)
            };
            TensorDCLNode b = new TensorDCLNode("tensor", "b", 5, 2, bInit);

            MultiplicationNode mulNode = new MultiplicationNode("*");
            mulNode.Left = new IDNode("a");
            mulNode.Right = new IDNode("b");

            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);

            Assert.ThrowsException<InvalidOperandsException>(() =>
            {
                visitor.Visit(a);
                visitor.Visit(b);
                visitor.Visit(mulNode);
            });
        }

        [TestMethod]
        public void IllegalTensorAddition()
        {
            // tensor a[2][1] = { [2.4], [5.1] }
            // tensor b[2][2] = { [1, 1],
            //                    [1, 1] }
            // tencor c[2][2] = a*b;

            TensorInitNode aInit = new TensorInitNode();
            aInit.FirstRowElements = new List<ExpressionNode>()
            {
                new DoubleNode(2.4)
            };
            aInit.Elements = new List<ExpressionNode>()
            {
                new DoubleNode(5.1)
            };
            TensorDCLNode a = new TensorDCLNode("tensor", "a", 2, 1, aInit);

            TensorInitNode bInit = new TensorInitNode();
            bInit.FirstRowElements = new List<ExpressionNode>()
            {
                new DoubleNode(1), new DoubleNode(1)
            };
            bInit.Elements = new List<ExpressionNode>()
            {
                new DoubleNode(1), new DoubleNode(1),
            };
            TensorDCLNode b = new TensorDCLNode("tensor", "b", 2, 2, bInit);

            AdditionNode mulNode = new AdditionNode("+");
            mulNode.Left = new IDNode("a");
            mulNode.Right = new IDNode("b");

            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);

            Assert.ThrowsException<InvalidOperandsException>(() =>
            {
                visitor.Visit(a);
                visitor.Visit(b);
                visitor.Visit(mulNode);
            });
        }
    }
}
