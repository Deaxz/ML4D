using System;
using System.Collections.Generic;

namespace ML4D.Compiler.Nodes
{
    // Value nodes
    public class IDNode : ExpressionNode
    {
        public string ID { get; set; }
        public IDNode(string id)
        {
            ID = id;
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        { return new List<Node>(); }
    }
    
    public class DoubleNode : ExpressionNode
    {
        public double Value { get; set; }
        public DoubleNode(double value)
        {
            Value = value;
            Type = "double";
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        { return new List<Node>(); }
    }
    
    public class IntNode : ExpressionNode
    {
        public int Value { get; set; }
        public IntNode(int value)
        {
            Value = value;
            Type = "int";
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        { return new List<Node>(); }
    }
    
    public class BoolNode : ExpressionNode
    {
        public bool Value { get; set; }
        public BoolNode(bool value)
        {
            Value = value;
            Type = "bool";
        }
        
        public override List<Node> GetChildren() // Necessary because of class organisation, don't know how to not have it.
        { return new List<Node>(); }
    }

    // Function node - Used for both funcExpr and funcStmt
    public class FunctionExprNode : ExpressionNode 
    {
        public string ID { get; set; }
        public List<Node> Arguments = new List<Node>();

        public FunctionExprNode(string id)
        {
            ID = id;
        }

        public override List<Node> GetChildren() // Expects list of Node, but can't cast list. TODO consider changing list to nodes, and casting to Node in AST builder
        {
            return Arguments;
        }
    }
    
    // Arithmetic nodes
    public class AdditionNode : InfixExpressionNode {}
    public class SubtractionNode : InfixExpressionNode {}
    public class MultiplicationNode : InfixExpressionNode {}
    public class DivisionNode : InfixExpressionNode {}
    public class PowerNode : InfixExpressionNode {}

    // Boolean nodes
    public class AndNode : InfixExpressionNode {}
    public class OrNode : InfixExpressionNode {}
    public class NotNode : UnaryExpressionNode {}
    
    // Equality nodes
    public class EqualNode : InfixExpressionNode {}
    public class NotEqualNode : InfixExpressionNode {}
    
    // Relational nodes 
    public class LessThanNode : InfixExpressionNode {}
    public class LessEqualThanNode : InfixExpressionNode {}
    public class GreaterThanNode : InfixExpressionNode {}
    public class GreaterEqualThanNode : InfixExpressionNode {}
}