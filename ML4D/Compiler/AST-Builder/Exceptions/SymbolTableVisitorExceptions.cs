﻿using System;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler.Exceptions
{
    public class VariableNotDeclaredException : Exception
    {
        public IDNode Node { get; }

        public VariableNotDeclaredException() : base() {}
        public VariableNotDeclaredException(string message) : base(message) {}
        public VariableNotDeclaredException(string message, Exception inner) : base(message, inner) {}
        public VariableNotDeclaredException(IDNode node, string message) : base(message)
        {
            Node = node;
        }           
    }

    public class VariableAlreadyDeclaredException : Exception
    {
        public VariableDCLNode Node { get; }

        public VariableAlreadyDeclaredException() : base() {}
        public VariableAlreadyDeclaredException(string message) : base(message) {}
        public VariableAlreadyDeclaredException(string message, Exception inner) : base(message, inner) {}
        public VariableAlreadyDeclaredException(VariableDCLNode node, string message) : base(message)
        {
            Node = node;
        }
    }
    
    public class FunctionNotDeclaredException : Exception
    {
        public FunctionNode Node { get; }

        public FunctionNotDeclaredException() : base() {}
        public FunctionNotDeclaredException(string message) : base(message) {}
        public FunctionNotDeclaredException(string message, Exception inner) : base(message, inner) {}
        public FunctionNotDeclaredException(FunctionNode node, string message) : base(message)
        {
            Node = node;
        }
    }
    
    public class FunctionAlreadyDeclaredException : Exception
    {
        public FunctionDCLNode Node { get; }

        public FunctionAlreadyDeclaredException() : base() {}
        public FunctionAlreadyDeclaredException(string message) : base(message) {}
        public FunctionAlreadyDeclaredException(string message, Exception inner) : base(message, inner) {}
        public FunctionAlreadyDeclaredException(FunctionDCLNode node, string message) : base(message)
        {
            Node = node;
        }
    }
    
    public class ReturnOutsideFunctionException : Exception
    {
        public ReturnNode Node { get; }

        public ReturnOutsideFunctionException() : base() {}
        public ReturnOutsideFunctionException(string message) : base(message) {}
        public ReturnOutsideFunctionException(string message, Exception inner) : base(message, inner) {}
        public ReturnOutsideFunctionException(ReturnNode node, string message) : base(message)
        {
            Node = node;
        }
    }
}