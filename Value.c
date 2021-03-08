#include <stdio.h>
#include <stdlib.h>

void build_topo(Value* v);

typedef struct Value Value;
typedef struct Node Node;
typedef struct Linked_list;

/* Node of double linked list */
struct Node{
  Value value;
  Node* prev;
  Node* next;
};


struct Linked_list{
  Node* head;
  void (*append)(Node** head, Value new_value);
};

void append(Node** head, Value new_value){
  /* 1. Allocade node*/
  struct Node* new_node = (Node*)malloc(1, sizeof(Node));
  /* 2. Put data in node*/
  new_node->value = new_value;
  /* 3. As this is the end node it goes to NULL */
  new_node->next = NULL;
  /* 4. If Linked List is empty, then make this node new head */
  if (**head == NULL) {
    new_node->prev = NULL;
    *head = new_node;
    return;
  }
  /* 5. Else traverse untill last node */
  Node* temp = head;
  while (temp->next != NULL){
    temp = temp->next;
  }
  /* 6. Change the next of last node */
    temp->next = new_node;
  /* 7. Make the last node as previous of new node */
    new_node->prev = temp;
}


struct Value{
  float data;
  float grad;
  Value (*_backward)(Value* self, Value* other);
  Value _prev[];
  char op[];
}Value;


int main() 
{

}


void backward(Value* self){
  Value topo[];
  Value visited[];

  build_topo(self);

}

void build_topo(Value* v, Value* visited[]){
  if(v_in_visited(*v, *visited) == false){
    
  }
}


a = Value(2.0);

b = Value(1.0);

c = a + b; // Value(3.0)

d = a * b + a**3 // Value(9.0) = Value(8.0) + Value(1.0)

e = c * d // Value(27.0) = Value(3.0) * Value(9.0)




e.backward()

topo[a, b, c, d, e]
visited = {e, c, a, b, d}

e
for {c, d}

c
for {a, b}

a topo append

c
for {a, b}

b topo append

c topo append

e
for {c, d}

d 
for (a, b)

a already visited

d 
for (a, b)

b already visited

d topo append

e topo append


reversed_topo[e, d, c, b, a]

e._backward() = lambda

d._backward() = out(a*b) + out(a**3)

d._backward() = out(out(a*b) + out(a**3)) 



c._backward() = { 
  a.grad += c.grad 
  b.grad += c.grad
}





d._backward() = out(out(a*b) + out(a**3)) 


Value(a*b) = 2.0
Value(a*b).grad = 0


out(a*b) = {
  a.grad += b.data * out.grad(???)
}

out(a**3) = {
  a.grad += (3 * a.data**(3-1)) * out.grad(???)
}

a = value(1.0)
b = value(2.0)

a + b = value(a.data + b.data, (a,b), '+')
  
c = value(a.data + b.data, (a,b), '+')
























a = Value(2.0);

b = Value(1.0);

d = a * b + a**3 // Value(10.0) = Value(8.0) + Value(1.0)


d._backward();

n + m

n = out(a*b)
m = out(a**3)




















n.grad += out.grad
m.grad += out.grad



out(a*b) = {
  a.grad += b.data * out.grad(???)
}

out(a**3) = {
  a.grad += (3 * a.data**(3-1)) * out.grad(???)
}
































f(g(x))

f'(g(x)) * g'(x)

(2 * x)^2


g(x) = 2 * x

f(x) = x^2


g'(x) = 2
f'(x) = 2x

x = 4


(2 * 2 * x) * (2) 
f' (g(x)) * g'(x)








