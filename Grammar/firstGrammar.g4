grammar firstGrammar;

prog: lines;

lines:
    dcl SEMICOLON lines | 
    stmt ';' lines;

dcl:
    type ID |
    FUNDCL type? ID LPAREN (type ID)? RPAREN LBRACE (lines)* RBRACE |
    FUNDCL type? ID LPAREN (type ID)(type ID COMMA)+ RPAREN LBRACE (lines)* RBRACE |
    ID type LBRACK INT RBRACK;

stmt:
    ID ASSIGN equality_expr |
    ID ASSIGN LBRACK arraydata RBRACE |
    ID BACKWARDS |
    WHILE LPAREN equality_expr RPAREN LBRACE lines RBRACE;

arraydata:
    FNUM |
    FNUM COMMA arraydata;

equality_expr:
    relational_expr (EQUALS | NOTEQUALS) equality_expr |
    relational_expr;

relational_expr:
    plinus_expr (GTHAN | LTHAN | GETHAN | LTHAN) relational_expr |
    plinus_expr;

plinus_expr:
    muldiv_expr (PLUS | MINUS) plinus_expr |
    muldiv_expr;
    
muldiv_expr:
    factor (MUL | DIV) muldiv_expr |
    factor;

factor:
    ID |
    INUM |
    FNUM |
    LPAREN equality_expr RPAREN;

type:
    FLOAT |
    INT |
    TENSOR;


FLOAT: 'float';
INT: 'int';
TENSOR: 'tensor';
BACKWARDS: '<-';
FUNDCL: 'fun';
PRINT: 'p';
ASSIGN: '=';
PLUS: '+';
MINUS: '-';
MUL: '*';
DIV: '/';
POWER: '**';
INUM: [0-9]+;
FNUM: [0-9]+ [.][0-9]+;
ID: [b-e]+; //placeholder
COMMA: ',';
SEMICOLON: ';';
LPAREN: '(';
RPAREN: ')';
LBRACE: '{';
RBRACE: '}';
LBRACK: '[';
RBRACK: ']';
EQUALS: '==';
NOTEQUALS: '!=';
GTHAN: '>';
LTHAN: '<';
GETHAN: '>=';
LETHAN: '<=';
FOR: 'for';
WHILE: 'while';





