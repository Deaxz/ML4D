using System;

namespace ML4D.Compiler
{
    public abstract class ExpressionNode
    {
    }

    public abstract class InfixExpressionNode : ExpressionNode
    {
        public ExpressionNode Left { get; set; }
        public ExpressionNode Right { get; set; }
    }

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

    public class NegateNode : ExpressionNode
    {
        public ExpressionNode InnerNode { get; set; }
    }

    public class FunctionNode : ExpressionNode
    {
        public Func<double, double> Function { get; set; }
        public ExpressionNode Argument { get; set; }
    }

    public class NumberNode : ExpressionNode
    {
        public double Value { get; set; }
    }
}