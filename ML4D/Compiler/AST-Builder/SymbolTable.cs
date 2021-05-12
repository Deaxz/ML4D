using System.Collections.Generic;
using ML4D.Compiler.Nodes;

namespace ML4D.Compiler
{
    public class SymbolTable
    {
        private static Stack<SymbolTable> symbolTableStack = new Stack<SymbolTable>();

        private SymbolTable? Parent { get; set; }
        private List<SymbolTable> children = new List<SymbolTable>(); // TODO overvej om vi skal beholde children og parent, bliver ikke brugt til noget. Men tænker Code generation maybe
        private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();

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
            symbolTableStack.Pop();
        }

        public void Insert(string ID, string type, bool isFunction)
        {
            symbolTableStack.Peek().symbols.Add(ID, new Symbol(ID, type, isFunction));            
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

        public void Clear()
        {
            symbolTableStack.Clear();
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
}