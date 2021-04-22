using System;

namespace ML4D.Compiler.Exceptions
{
    public class VariableInitialisationException : Exception
    {
        public VariableDCLNode Node { get; }

        public VariableInitialisationException() : base() {}
        public VariableInitialisationException(string message) : base(message) {}
        public VariableInitialisationException(string message, Exception inner) : base(message, inner) {}

        public VariableInitialisationException(VariableDCLNode node, string message) : base(message)
        {
            Node = node;
        }           
    }
    
    public class VariableAssignmentException : Exception
    {
        public AssignNode Node { get; } // TODO change

        public VariableAssignmentException() : base() {}
        public VariableAssignmentException(string message) : base(message) {}
        public VariableAssignmentException(string message, Exception inner) : base(message, inner) {}

        public VariableAssignmentException(AssignNode node, string message) : base(message)
        {
            Node = node;
        }           
    }
    
    public class FunctionCallArgumentException : Exception
    {
        public VariableDCLNode Node { get; } // TODO change

        public FunctionCallArgumentException() : base() {}
        public FunctionCallArgumentException(string message) : base(message) {}
        public FunctionCallArgumentException(string message, Exception inner) : base(message, inner) {}

        public FunctionCallArgumentException(VariableDCLNode node, string message) : base(message)
        {
            Node = node;
        }           
    }
    
    public class InvalidOperandsException : Exception
    {
        public ExpressionNode Node { get; } // TODO change

        public InvalidOperandsException() : base() {}
        public InvalidOperandsException(string message) : base(message) {}
        public InvalidOperandsException(string message, Exception inner) : base(message, inner) {}

        public InvalidOperandsException(ExpressionNode node, string message) : base(message)
        {
            Node = node;
        }           
    }
}