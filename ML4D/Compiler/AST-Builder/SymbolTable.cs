using System.Collections.Generic;
using System.Linq;

namespace ML4D.Compiler
{
    public class SymbolTable
    {
        private static Stack<SymbolTable> symbolTableStack = new Stack<SymbolTable>();
        private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
        private string? FuncType { get; set; }

        public void OpenScope()
        {
            symbolTableStack.Push(new SymbolTable());
        }
        
        public void CloseScope()
        {
            symbolTableStack.Pop();
        }

        public void Insert(string ID, string type, bool isFunction)
        {
            symbolTableStack.Peek().symbols.Add(ID, new Symbol(ID, type, isFunction)); 
            if (isFunction)
                FuncType = type;
        }

        public void Insert(string ID, string type, bool isFunction, int rows, int columns)
        {
            symbolTableStack.Peek().symbols.Add(ID, new TensorSymbol(ID, type, isFunction, rows, columns));            
        }
        
        public Symbol Retrieve(string ID)
        {
            foreach (SymbolTable symTab in symbolTableStack)
            {
                bool success = symTab.symbols.TryGetValue(ID, out Symbol symbol);
                if (success)
                    return symbol;
            }
            return null;
        }

        public string GetCurrentFunctionType()
        {
            if (FuncType is not null)
                return FuncType;

            foreach (SymbolTable symTab in symbolTableStack)
                if(symTab.FuncType is not null)
                    return FuncType;
            return null;
        }
        
        public static void Clear()
        {
            symbolTableStack.Clear();
        }
        
        public void OnlyGlobalScope()
        {
            SymbolTable global = symbolTableStack.Last(); 
            symbolTableStack.Clear();
            symbolTableStack.Push(global);
        }
    }

    public class Symbol
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsFunction { get; set; }

        public Symbol(string name, string type, bool isfunction)
        {
            Name = name;
            Type = type;
            IsFunction = isfunction;
        }
    }

    public class TensorSymbol : Symbol
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        
        public TensorSymbol(string name, string type, bool isfunction, int rows, int columns) : base(name, type, isfunction)
        {
            Rows = rows;
            Columns = columns;
        }
    }
}