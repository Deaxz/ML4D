#include <stdio.h>
#include <stdlib.h>

void build_topo(Value* v);

typedef struct Value Value;
typedef struct Node Node;
typedef struct Linked_list;

/* Node of double linked list */
struct Node {
  Value value;
  Node* prev;
  Node* next;
};


struct Linked_list {
  Node* head;
  void (*append)(Node** head, Value new_value);
};

void append(Node** head, Value new_value) {
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
  /* 5. Else traverse until last node */
  Node* temp = head;
  while (temp->next != NULL) {
    temp = temp->next;
  }
  /* 6. Change the next of last node */
    temp->next = new_node;
  /* 7. Make the last node as previous of new node */
    new_node->prev = temp;
}

/* Check whether p_value is in linked list */
bool search(Node** head, Value* p_value) {
  struct Node* current = head; // Make a node 'current' to search through linked list
  while (current != NULL){
    if (current->key == p_value)
        return true;
    current = current->next;
  }
  return false;
}


struct Value {
  float data;
  float grad;
  Value (*_backward)(Value* self, Value* other);
  Value _prev[];
  char op[];
} Value;


int main() 
{

}


void backward(Value* self) {
  Value topo[];
  Value visited[];

  build_topo(self);

}

void build_topo(Value* v, Value* visited[]) {
  if (v_in_visited(*v, *visited) == false) {
    
  }
}






backward(*parent) {

  *Value topo; // Linkedlist
  *Value visited; // Linkedlist
  
  build_topo(*parent)


}

build_topo(*Value current) {

  if v not in visited {
    visited.add(current)
    
    for (i = 0; i < 2; i++) {

      if (current.children[i] == 0)
        return;

      build_topo(current.children[i]);
    }

    topo.append(v);
  }    
}

































d.backward()

d._backward()



a = 2.0
b = 1.0

d = a * b + pow(a, 3)

a._backward = lambda
b._backward = lambda

(a*b)._backward = multbackward
(pow(a,3))._backward = powbackward
d._backward = plusbackward

d.backward









