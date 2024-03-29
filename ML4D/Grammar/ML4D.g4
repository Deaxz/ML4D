grammar ML4D;

lines
    :   (dcl | stmt)+
    |   EOF
    ;

dcl
    :   type=types id=ID '=' init=bool_expr ';'                                                                 # varDecl
    |   type=TENSOR id=ID '[' rows=INUM ']' '[' coloumns=INUM ']' '=' (init=tensor_init | assignInit=bool_expr) ';' # tensorDecl
    |   type=types id=ID '(' (argtype+=types argid+=ID (',' argtype+=types argid+=ID)*)? ')' '{' body=lines '}' # funcDecl
    ;

tensor_init
    :   '{' '[' firstRow+=expr (',' firstRow+=expr)*']' (',' '[' elements+=expr (',' elements+=expr)*']')* '}'
    ;

stmt
    :   IF '(' pred+=bool_expr ')' '{' body+=lines '}' (ELSE IF '(' pred+=bool_expr ')' '{' body+=lines '}' )* (ELSE '{' body+=lines '}')?   # ifStmt
    |   FOR '(' init=dcl pred=bool_expr ';' final=assign_expr ')' '{' body=lines '}'                                                         # forStmt
    |   WHILE '(' pred=bool_expr ')' '{' body=lines '}'                                                                                 # whileStmt
    |   RETURN inner=bool_expr? ';'                                                                                                          # returnStmt
    |   id=ID '(' (argexpr+=bool_expr (',' argexpr+=bool_expr)*)? ')' ';'                                                                    # funcStmt
    |   GRADIENTS '(' tensor=ID ',' '(' gradvar+=ID '<<' gradtensor+=ID (',' gradvar+=ID '<<' gradtensor+=ID)* ')' ',' '{' body=lines '}' ')' ';' # gradientsStmt
    |   assign_expr ';'                                                                                                                      # assignStmt
    ;

assign_expr
    : id=ID op='=' right=bool_expr                              
    ;

bool_expr
    :   left=expr op=('<'|'<='|'>'|'>=') right=expr             # infixRelationalExpr
    |   left=expr op=('=='|'!=') right=expr                     # infixRelationalExpr
    |   left=bool_expr op='and' right=bool_expr                 # infixBoolExpr
    |   left=bool_expr op='or'  right=bool_expr                 # infixBoolExpr
    |   expr                                                    # exprExpr
    ;

expr
    :   '(' inner=bool_expr ')'                                         # parensExpr
    |   <assoc=right> op='-' right=expr                                 # unaryExpr  
    |   <assoc=right> op='not' inner=bool_expr                          # unaryExpr 
    |   <assoc=right> left=expr op='**' right=expr                      # infixValueExpr
    |   left=expr op=('*'|'/') right=expr                               # infixValueExpr
    |   left=expr op=('+'|'-') right=expr                               # infixValueExpr
    |   id=ID '(' (argexpr+=bool_expr (',' argexpr+=bool_expr)*)? ')'   # funcExpr 
    |   value=(INUM|FNUM|BOOLVAL|ID)                                    # typeExpr
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
TENSOR: 'tensor';

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
IF: 'if';
ELSE: 'else';
WHILE: 'while';
FOR: 'for';
RETURN: 'return';
GRADIENTS: 'gradients';
WS: [ \t\r\n]+ -> skip;

// Values
BOOLVAL: ('true'|'false');
INUM: [0]          | [1-9][0-9]*; 
FNUM: [0][.][0-9]+ | [1-9][0-9]*[.][0-9]+; 
ID: [A-Za-z][0-9_A-Za-z]*;

// Comments
BLOCKCOMMENT: '/*' .*? '*/' -> skip;
LINECOMMENT: '//' ~[\r\n]* -> skip;