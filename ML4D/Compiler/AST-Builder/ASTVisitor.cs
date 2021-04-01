using System;

namespace ML4D.Compiler
{
    public abstract class ASTVisitor<T>
    {
        public abstract T Visit(LinesNode node);
        
        public abstract T Visit(VariableDCLNode node);
        
        // Statement
        
        
        // Expression
        
            // Value
            public abstract T Visit(IDNode node);
            public abstract T Visit(VoidNode node);
            public abstract T Visit(DoubleNode node);
            public abstract T Visit(IntNode node);
            public abstract T Visit(BoolNode node);
            
            // Arithmetic
            public abstract T Visit(AdditionNode node);
            public abstract T Visit(SubtractionNode node);
            public abstract T Visit(MultiplicationNode node);
            public abstract T Visit(DivisionNode node);
            public abstract T Visit(PowerNode node);
            
             // Boolean
             public abstract T Visit(AndNode node);
             public abstract T Visit(OrNode node);
             public abstract T Visit(NotNode node);
            
             // Equality
             public abstract T Visit(EqualNode node);
             public abstract T Visit(NotEqualNode node);
            
             // Relational
             public abstract T Visit(GreaterThanNode node);
             public abstract T Visit(GreaterEqualThanNode node);
             public abstract T Visit(LessThanNode node);
             public abstract T Visit(LessEqualThanNode node);
        
        public T Visit(Node node)
        {
            return Visit((dynamic)node);
        }
    }
}