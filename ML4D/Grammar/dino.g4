grammar dino;
  
lines
    :   (dcl | stmt) ';' ((dcl | stmt ) ';')*
    |   EOF
    ;

dcl
    :   type=types id=ID (op='=' right=bool_expr)?                                # declVar                          
    |   type=types func=ID '(' (types ID (',' types ID)*)? ')' '{' body=lines '}' # declFunc // TODO Undersøg blanke # methods og overvej no/one parameter i starten.
    ;

stmt
    :   id=ID op='=' right=bool_expr                            # assignStmt
    |   WHILE '(' predicate=bool_expr ')' '{' body=lines '}'    # whileStmt
    |   id=ID op='<-'                                           # backwardStmt
    |   RETURN right=bool_expr                                  # returnStmt
    |   bool_expr                                               # exprStmt                // Nødvendig for at kunne kalde void metoder, i.e. zero(); overvej at kopier kun method call herop.
    ;

bool_expr 
    :    left=bool_expr op='and' right=bool_expr     # infixBoolExpr
    |    left=bool_expr op='or'  right=bool_expr     # infixBoolExpr
    |    expr                                        # dingdongExpr // TODO change at some point
    ;
    
expr  // TODO introduce negation, can be done similarly to Math AST
    :   '(' bool_expr ')'                               # parensExpr // Ændret til bool_expr fra expr, fordi fx "q = (c or d) and e;"
    |   op='not' inner=bool_expr                        # unaryExpr  // Ændret fra "not ID", da vi skal skrive fx "!(c or d)", så vi må takle det til type checking.
    |   <assoc=right> left=expr op='**' right=expr      # infixExpr
    |   left=expr op=('*'|'/') right=expr               # infixExpr
    |   left=expr op=('+'|'-') right=expr               # infixExpr
    |   left=expr op=('=='|'!=') right=expr             # infixExpr
    |   func=ID '(' (bool_expr (COMMA bool_expr)*)? ')' # funcExpr  // Måske overvej no multiple parameter i starten
    |   left=expr op=('<'|'<='|'>'|'>=') left=expr      # infixExpr
    |   value=(INUM|FNUM|BOOLVAL|ID)                    # typeExpr
    ;
        
types
    :   type=INT 
    |   type=BOOL 
    |   type=DOUBLE
    |   type=VOID
    ;

// Types - TODO Nok add capitalisation mulighed, alstå DOUBLE, INT osv.
VOID: 'void';
DOUBLE: 'double'; 
INT: 'int';
BOOL: 'bool';

// Operators
NOT: 'not';
POW: '**';
MUL: '*';
DIV: '/';
PLUS: '+';
MINUS: '-';
EQUALS: '==';
NOTEQUALS: '!=';
GTHAN: '>';
LTHAN: '<';
GETHAN: '>=';
LETHAN: '<=';
AND: 'and';
OR: 'or'; 
BACKWARDS: '<-';
ASSIGN: '=';

// Delimiters
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACK: '[';
RBRACK: ']';
COMMA: ',';
SEMICOLON: ';';

// Control Structure
WHILE: 'while';
RETURN: 'return';
WS: [ \t\r\n]+ -> skip;

// Values
BOOLVAL: ('true'|'false');
INUM: [-]?[0-9]+; 
//FNUM: [-]?[0-9]+ [.][0-9]+;
FNUM: [-]?[0-9]+ ('.' [0-9]+)?; // Nakket fra AST eksempel. 
ID: [A-z]([0-9A-z])*;