using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ML4D.Compiler
{
    public class SymbolTable
    {
        private static Stack<SymbolTable> symbolTableStack = new Stack<SymbolTable>();
        
        protected SymbolTable? Parent { get; set; }
        protected List<SymbolTable> children = new List<SymbolTable>();
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
            children.Add(child);
            symbolTableStack.Push(child);
        }
        
        public void CloseScope()
        {
            if (symbolTableStack.Peek().Parent is null)
            {
                Console.WriteLine("You are at the bottom of the stack, popping to null value");
                symbolTableStack.Pop();
            }
            symbolTableStack.Pop();
        }

        public void Insert(string name, string type)
        {
            // Gets current table from top of stack, and adds symbol
            symbolTableStack.Peek().symbols.Add(name, new Symbol(name, type));            
        }

        public bool Retrieve(string name)
        {
            bool success = symbolTableStack.Peek().symbols.TryGetValue(name, out Symbol value);
            if (success)
                return true;

            // Should be a recursive call through all the parents
            if (symbolTableStack.Peek().Parent is not null)
                return symbolTableStack.Peek().Parent.Retrieve(name, symbolTableStack.Peek().Parent);
            
            // Variable not found in current or parent scope
            return false;
        }
        
        public bool Retrieve(string name, SymbolTable symbolTable)
        {
            bool success = symbolTable.symbols.TryGetValue(name, out Symbol value);
            if (success)
                return true;

            // Should be a recursive call through all the parents
            if (symbolTable.Parent is not null)
                return symbolTable.Parent.Retrieve(name, symbolTable.Parent);
            
            // Variable not found in current or parent scope
            return false;
        }
    }

    public class Symbol
    {
        private string Name { get; set; }
        private string Type { get; set; }
        //private SymbolTable Scope { get; set; } // Kan addes til constructor, så man har en ref til sit eget scope

        public Symbol(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}