using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using ML4D.Compiler.Exceptions;

namespace ML4D.Compiler
{
    public class SymbolTable
    {
        private static Stack<SymbolTable> symbolTableStack = new Stack<SymbolTable>();
        
        protected SymbolTable? Parent { get; set; }
        protected List<SymbolTable> children = new List<SymbolTable>(); // TODO overvej om vi skal beholde children og parent, bliver ikke brugt til noget. Men tænker Code generation maybe
        protected Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

        // Init constructor
        public SymbolTable()
        {
            Parent = null;
            symbolTableStack.Push(this);
        }
        
        public SymbolTable(SymbolTable parent)
        {
            Parent = parent;
        }

        public void OpenScope()
        {
            SymbolTable child = new SymbolTable(symbolTableStack.Peek());
            symbolTableStack.Peek().children.Add(child);
            symbolTableStack.Push(child);
        }
        
        public void CloseScope()
        {
            if (symbolTableStack.Peek().Parent is null) // TODO, remove check, det er umuligt at lukke det sidste scope. Da closescope() altid bliver kaldt efter openscope()
            {
                Console.WriteLine("You are at the bottom of the stack, popping to null value");
                symbolTableStack.Pop();
            }
            symbolTableStack.Pop();
        }

        public void Insert(string ID, string type)
        {
            symbolTableStack.Peek().symbols.Add(ID, new Symbol(ID, type));            
        }

        public Symbol Retrieve(string ID)
        {
            foreach (SymbolTable symTab in symbolTableStack)
            {
                bool success = symTab.symbols.TryGetValue(ID, out Symbol value);
                if (success)
                    return value;
            }
            return null;
        }

        public void Clear()
        {
            symbolTableStack.Clear();
        }
    }

    public class Symbol
    {
        public string Name { get; set; }
        public string Type { get; set; }
        
        // TODO add attributes info
        
        public Symbol(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}