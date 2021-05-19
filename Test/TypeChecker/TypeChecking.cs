using Microsoft.VisualStudio.TestTools.UnitTesting;
using ML4D.Compiler.Nodes;
using ML4D.Compiler.ASTVisitors;
using ML4D.Compiler;
using ML4D.Compiler.Exceptions;

namespace Test
{
    [TestClass]
    public class TypeChecking
    {
        [TestMethod]
        public void MixedExpressionDoubleAndIntBecomesDoubleNode()
        {
            //Arrange
            InfixExpressionNode node = new AdditionNode("+");
            node.Left = new DoubleNode(25.5);
            node.Right = new IntNode(1);

            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);

            //Act
            visitor.Visit(node);

            //Assert
            Assert.AreEqual(node.Type, "double");            
        }

        [TestMethod]
        public void AssigningDoubleToIntVariable()
        {
            //Arrange
            ExpressionNode exprNode = new DoubleNode(25.5);
            VariableDCLNode node = new VariableDCLNode("int", "a", exprNode);

            //Act
            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);

            //Assert
            Assert.ThrowsException<VariableInitialisationException>(() =>
            {
                visitor.Visit(node);
            });
        }

        [TestMethod]
        public void InvalidOperandstoOrOperation()
        {
            NotEqualNode node = new NotEqualNode("!=");
            node.Left = new BoolNode(true);
            node.Right = new IntNode(2);

            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);
          
            Assert.ThrowsException<InvalidOperandsException>(() =>
            {
                visitor.Visit(node);
            });
        }

        [TestMethod]
        public void ValidOperandsToOrOperation()
        {
            OrNode node = new OrNode("Or");

            EqualNode fourplus6equal10 = new EqualNode("=");

            InfixExpressionNode addNode = new AdditionNode("+");
            addNode.Left = new IntNode(4);
            addNode.Right = new IntNode(6);
            fourplus6equal10.Left = addNode;
            fourplus6equal10.Right = new IntNode(10);

            node.Left = fourplus6equal10;
            node.Right = new BoolNode(false);

            SymbolTable symbolTable = new SymbolTable();
            TypeCheckSymbolTableVisitor visitor = new TypeCheckSymbolTableVisitor(symbolTable);

            visitor.Visit(node);

            Assert.AreEqual(node.Type, "bool");
        }




    }
}
