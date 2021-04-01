using System;

namespace ML4D.Compiler
{
    // Expression  
    public abstract class ExpressionNode : Node
    {
    }
    public abstract class InfixExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
    }
    public class UnaryExpressionNode : ExpressionNode
    {
        public ExpressionNode Inner { get; set; }
    }
    
    // Value
    public class IDNode : ExpressionNode
    {
        public string Name { get; set; }
        public IDNode(string name)
        {
            Name = name;
        }
    }
    public class VoidNode : ExpressionNode
    {
    }
    public class DoubleNode : ExpressionNode
    {
        public double Value { get; set; }
        public DoubleNode(double value)
        {
            Value = value;
        }
    }
    public class IntNode : ExpressionNode
    {
        public int Value { get; set; }
        public IntNode(int value)
        {
            Value = value;
        }
    }
    public class BoolNode : ExpressionNode
    {
        public bool Value { get; set; }
        public BoolNode(bool value)
        {
            Value = value;
        }
    }
    
    // Arithmetic
    public class AdditionNode : InfixExpressionNode
    {
    }
    public class SubtractionNode : InfixExpressionNode
    {
    }
    public class MultiplicationNode : InfixExpressionNode
    {
    }
    public class DivisionNode : InfixExpressionNode
    {
    }
    public class PowerNode : InfixExpressionNode
    {
    }

    // Boolean
    public class AndNode : InfixExpressionNode
    {
    }
    public class OrNode : InfixExpressionNode
    {
    }
    public class NotNode : UnaryExpressionNode
    {
    }
    
    // Equality
    public class EqualNode : InfixExpressionNode
    {
    }
    public class NotEqualNode : InfixExpressionNode
    {
    }
    
    // Relational
    public class LessThanNode : InfixExpressionNode
    {
    }
    public class LessEqualThanNode : InfixExpressionNode
    {
    }
    public class GreaterThanNode : InfixExpressionNode
    {
    }
    public class GreaterEqualThanNode : InfixExpressionNode
    {
    }
}