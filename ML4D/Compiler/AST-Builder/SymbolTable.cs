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
        protected List<SymbolTable> children = new List<SymbolTable>(); // TODO overvej om vi skal beholde children, bliver ikke brugt til noget.
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

        // New Retrieve for typechecking
        public Symbol Retrieve(string name)
        {
            foreach (SymbolTable symTab in symbolTableStack)
            {
                bool success = symTab.symbols.TryGetValue(name, out Symbol value);
                if (success)
                    return value;
            }
            return null;
        }
        // end
        
        // Old, delete when sure New works as intended.
        
        // public bool Retrieve(string name)
        // {
        //     bool success = symbolTableStack.Peek().symbols.TryGetValue(name, out Symbol value);
        //     if (success)
        //         return true;
        //     else
        //     {
        //         while (symbolTableStack.Peek())
        //     }
        //
        //         // Should be a recursive call through all the parents
        //     if (symbolTableStack.Peek().Parent is not null)
        //         return symbolTableStack.Peek().Parent.Retrieve(name, symbolTableStack.Peek().Parent);
        //     
        //     // Variable not found in current or parent scope
        //     return false;
        // }
        //
        // public bool Retrieve(string name, SymbolTable symbolTable)
        // {
        //     bool success = symbolTable.symbols.TryGetValue(name, out Symbol value);
        //     if (success)
        //         return true;
        //
        //     // Should be a recursive call through all the parents
        //     if (symbolTable.Parent is not null)
        //         return symbolTable.Parent.Retrieve(name, symbolTable.Parent);
        //     
        //     // Variable not found in current or parent scope
        //     return false;
        // }
    }

    public class Symbol
    {
        public string Name { get; set; }
        public string Type { get; set; }
        
        public Symbol(string name, string type)
        {
            Name = name;
            Type = type;
        }
    }
}