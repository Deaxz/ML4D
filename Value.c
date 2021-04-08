#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>

void build_topo(Value* v);

/* Fra def __init__ */
typedef struct Value {
  float data;
  float grad;
  struct Backwards backwards;
  //struct Value* (*_backward)(Value* self, Value* other);
  struct Linked_List* _prev;
  char op[];
} Value;

/* Backwards as struct since we can't have closures in C */
typedef struct Backwards{
  Value* self;
  Value* other;
  Value* out;
  void (*invoke)(Value, Value);
} Backwards;

/* Node of double linked list */
typedef struct Node {
  struct Value* value;
  struct Node* prev;
  struct Node* next;
} Node;

/* Needed for backward (visited = set()) */
typedef struct Linked_list {
  struct Node* head;
  void (*append)(struct Node** head, Value* new_value);
} Linked_list;

void append(struct Node* head, Value* new_value) {
  /* 1. Allocade node*/
  struct Node* new_node = (struct Node*)malloc(1, sizeof(struct Node));
  /* 2. Put data in node*/
  new_node->value = new_value;
  /* 3. As this is the end node it goes to NULL */
  new_node->next = NULL;
  /* 4. If Linked List is empty, then make this node new head */
  if (head == NULL) {
    new_node->prev = NULL;
    head = new_node;
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
    if (current->value == p_value)
        return true;
    current = current->next;
  }
  return false;
}


int main() 
{

}


// __add__
Value* add (Value* self, Value* other) {
  struct Value* out = (struct Value*)malloc(1, sizeof(struct Value));
  
  out->data = self->data + other->data;

  struct Linked_list* children = (struct Value*)malloc(1, sizeof(struct Linked_list));
  children->head = self;
  children->append(children->head, other);
  out->_prev = children;

  // Can't do nested functions in C, so made struct add_backwards simulating closures
  out->backwards.self = self;
  out->backwards.other = other;
  out->backwards.out = out;
  out->backwards.invoke = add_backwards;

  return out;
}

void add_backwards(Value* self, Value* other, Value* out){
    self->grad = self->grad + other->grad * out->grad;
    other->grad = other->grad + self->grad * out->grad;
}

void backward(Value* self) {
  struct Linked_list* topo = (struct Value*)malloc(1, sizeof(struct Linked_list));
  struct Linked_list* visited = (struct Value*)malloc(1, sizeof(struct Linked_list));
 
  build_topo(self);

}

void build_topo(Value* v, Value* visited[]) {
  if (v_in_visited(*v, *visited) == false) {
    
  }

  if(!search(topo->head, v)){

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





