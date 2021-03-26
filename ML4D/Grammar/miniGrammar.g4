gnrammar Grammar;

prog: lines EOF;

lines:
    (dcl SEMICOLON lines |
     stmt SEMICOLON lines)?;

dcl:
    type ID (ASSIGN expr_stmt)? |
    
expr_stmt:
    addition_expr |
    subtraction_expr;

addition_expr:
    (multiplication_expr | division_expr) PLUS (addition_expr | subtraction_expr) |
    multiplication_expr |
    division_expr;

subtraction_expr:
    (multiplication_expr | division_expr) MINUS (addition_expr | subtraction_expr) |
    multiplication_expr |
    division_expr;

multiplication_expr:
    power_expr MUL (multiplication_expr | division_expr) |
    power_expr;

division_expr:
    factor DIV (multiplication_expr | division_expr) |
    factor;

factor:
    ID |
    INUM |
    FNUM |
    BOOLVAL |
    LPAREN expr_stmt RPAREN |

not_expr:
    NOT ID;

type:
    INT |
    BOOL |
    DOUBLE;

GRAD:'grad';
NOT: 'not';
AND: 'and';
OR: 'or';  
VOID: 'void';
DOT: '.';
WHILE: 'while';
DOUBLE: 'double';
INT: 'int';
BOOL: 'bool';
BOOLVAL: ('true'|'false');
RETURN: 'return';
BACKWARDS: '<-';
ASSIGN: '=';
POWER: '**';
MUL: '*';
DIV: '/';
PLUS: '+';
MINUS: '-';
COMMA: ',';
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
SEMICOLON: ';';
WS: [ \t\r\n]+ -> skip;
INUM: [-]?[0-9]+; // Unary?
FNUM: [-]?[0-9]+ [.][0-9]+; // Unary?
ID: [A-z]([0-9A-z])*;