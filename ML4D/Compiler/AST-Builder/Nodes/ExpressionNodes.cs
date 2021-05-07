using System.Collections.Generic;
using System.Runtime.CompilerServices;

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
        
        public override List<Node> GetChildren()
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
        
        public override List<Node> GetChildren()
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
        
        public override List<Node> GetChildren()
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
        
        public override List<Node> GetChildren() 
        { return new List<Node>(); }
    }

    // Function node - Used for both funcExpr and funcStmt
    public class FunctionExprNode : FunctionNode
    {
        public FunctionExprNode(string id) : base(id) {}
    }
    
    // Arithmetic nodes
    public class AdditionNode : InfixExpressionNode
    {
        public AdditionNode(string symbol) : base(symbol) {}
    }
    public class SubtractionNode : InfixExpressionNode
    {
        public SubtractionNode(string symbol) : base(symbol) {}
    }
    public class MultiplicationNode : InfixExpressionNode
    {
        public MultiplicationNode(string symbol) : base(symbol) {}
    }
    public class DivisionNode : InfixExpressionNode
    {
        public DivisionNode(string symbol) : base(symbol) {}
    }
    public class PowerNode : InfixExpressionNode
    {
        public PowerNode(string symbol) : base(symbol) {}
    }

    // Boolean nodes
    public class AndNode : InfixExpressionNode
    {
        public AndNode(string symbol) : base(symbol) {}
    }
    public class OrNode : InfixExpressionNode
    {
        public OrNode(string symbol) : base(symbol) {}
    }
    public class NotNode : UnaryExpressionNode
    {
        public NotNode(string symbol) : base(symbol) {}
    }
    
    // Equality nodes
    public class EqualNode : InfixExpressionNode
    {
        public EqualNode(string symbol) : base(symbol) {}
    }
    public class NotEqualNode : InfixExpressionNode
    {
        public NotEqualNode(string symbol) : base(symbol) {}
    }
    
    // Relational nodes 
    public class LessThanNode : InfixExpressionNode
    {
        public LessThanNode(string symbol) : base(symbol) {}
    }
    public class LessEqualThanNode : InfixExpressionNode {
        public LessEqualThanNode(string symbol) : base(symbol) {}
    }
    public class GreaterThanNode : InfixExpressionNode
    {
        public GreaterThanNode(string symbol) : base(symbol) {}
    }
    public class GreaterEqualThanNode : InfixExpressionNode
    {
        public GreaterEqualThanNode(string symbol) : base(symbol) {}
    }
}