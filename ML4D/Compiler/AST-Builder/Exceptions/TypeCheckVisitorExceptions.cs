using System;
using ML4D.Compiler.Nodes;

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
        public AssignNode Node { get; }

        public VariableAssignmentException() : base() {}
        public VariableAssignmentException(string message) : base(message) {}
        public VariableAssignmentException(string message, Exception inner) : base(message, inner) {}

        public VariableAssignmentException(AssignNode node, string message) : base(message)
        {
            Node = node;
        }           
    }

    public class InvalidOperandsException : Exception
    {
        public ExpressionNode Node { get; } 

        public InvalidOperandsException() : base() {}
        public InvalidOperandsException(string message) : base(message) {}
        public InvalidOperandsException(string message, Exception inner) : base(message, inner) {}

        public InvalidOperandsException(ExpressionNode node, string message) : base(message)
        {
            Node = node;
        }           
    }

    public class InvalidCallToVariable : Exception
    {
        public Node Node { get; }
        public InvalidCallToVariable() : base() {}
        public InvalidCallToVariable(string message) : base(message) {}
        public InvalidCallToVariable(string message, Exception inner) : base(message, inner) {}
        
        public InvalidCallToVariable(Node node, string message) : base(message)
        {
            Node = node;
        }   
    }
}