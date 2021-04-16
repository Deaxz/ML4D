grammar dino;
  
lines
    :   (dcl | stmt) ';' ((dcl | stmt ) ';')*
    |   EOF
    ;

dcl
    :   type=types id=ID (op='=' right=bool_expr)?                                              # varDecl                          
    |   type=types id=ID '(' (argtype+=types argid+=ID (',' types ID)*)? ')' '{' body=lines '}' # funcDecl
    ;

stmt
    :   id=ID op='=' right=bool_expr                            # assignStmt
    |   WHILE '(' predicate=bool_expr ')' '{' body=lines '}'    # whileStmt
    |   id=ID op='<-'                                           # backwardStmt
    |   RETURN inner=bool_expr                                  # returnStmt // TODO return inner=bool_expr*, så det er optional da vi har void funktioner.
    |   id=ID '(' (argexpr+=bool_expr (',' bool_expr)*)? ')'    # funcStmt
    ;

bool_expr 
    :   left2=expr op=('<'|'<='|'>'|'>=') right2=expr           # infixBoolExpr // TODO mega wack med left2, men ellers ikke muligt.    
//  |   left2=expr op=('=='|'!=') right2=expr                   # infixBoolExpr // TODO skal implementeres
    |   left=bool_expr op='and' right=bool_expr                 # infixBoolExpr
    |   left=bool_expr op='or'  right=bool_expr                 # infixBoolExpr
    |   expr                                                    # dingdongExpr // TODO change name at some point
    ;
    
expr  // TODO introduce unary minus, can be done similarly to Math AST
    :   '(' bool_expr ')'                                       # parensExpr 
    |   op='not' inner=bool_expr                                # unaryExpr  // TODO Man kan skrive "b = not not not not a;", men den kan ikke flyttes pga precendence, men tror heller ikke det er et problem.
    |   <assoc=right> left=expr op='**' right=expr              # infixExpr
    |   left=expr op=('*'|'/') right=expr                       # infixExpr
    |   left=expr op=('+'|'-') right=expr                       # infixExpr
    |   left=expr op=('=='|'!=') right=expr                     # infixExpr // TODO har samme problem som relational havde. Man kan skrive "c = 2 == 3 == 4;"
    |   id=ID '(' (argexpr+=bool_expr (',' bool_expr)*)? ')'    # funcExpr  
    |   value=(INUM|FNUM|BOOLVAL|ID)                            # typeExpr
    ;
        
types
    :   type=INT 
    |   type=BOOL 
    |   type=DOUBLE
    |   type=VOID
    ;

// Types
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
ID: [A-Za-z]([0-9_A-Za-z])*;