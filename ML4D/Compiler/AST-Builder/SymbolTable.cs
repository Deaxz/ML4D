using System.Collections.Generic;

namespace ML4D.Compiler
{
    public class SymbolTable
    {
        private static Stack<SymbolTable> symbolTableStack = new Stack<SymbolTable>();
        private Dictionary<string, Symbol> symbols = new Dictionary<string, Symbol>();
        private string _scopeName { get; set; }

        // Init constructor
        public SymbolTable()
        {
            _scopeName = "global";
            symbolTableStack.Push(this);
        }
        
        public SymbolTable(string scopeName)
        {
            _scopeName = scopeName;
        }

        public void OpenScope(string scopeName)
        {
            symbolTableStack.Push(new SymbolTable(scopeName));
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

        public List<string> ScopeList()
        {
            List<string> scopes = new List<string>();
            foreach (SymbolTable symTab in symbolTableStack)
                scopes.Add(symTab._scopeName);
            return scopes;
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