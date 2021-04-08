#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>

/* Fra def __init__ */
typedef struct Value {
  float data;
  float grad;
  void (*backward)();
  struct Linked_list* _prev;
  char op[];
} Value;

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

/* Wraps a node around a value */
Node* wrapValue(Value* value){
    /* 1. Allocade node*/
    struct Node* node = (struct Node*)malloc(sizeof(struct Node));
    /* 2. Put data in node*/
    node->value = value;
    /* 3. As this is a new node it's prev and next initialises to NULL */
    node->prev = NULL;
    node->next = NULL;

    return node;
}

void append(struct Node* head, Value* new_value) {
  /* 1. Allocade node*//* 2. Put data in node*/
  Node* new_node = wrapValue(new_value);
  /* 4. If Linked List is empty, then make this node new head */
  if (head == NULL) {
    new_node->prev = NULL;
    head = new_node;
    return;
  }
  printf("Before while\n");
  /* 5. Else traverse until last node */
  Node* temp = head;
  while (temp->next != NULL) {
      printf("in the while\n");
    temp = temp->next;
    printf("%s", temp);
        printf("did the assign");
  }
   printf("after\n");
  /* 6. Change the next of last node */
    temp->next = new_node;
  /* 7. Make the last node as previous of new node */
    new_node->prev = temp;
}

/* Check whether p_value is in linked list */
bool search(Node* head, Value* p_value) {
  struct Node* current = head; // Make a node 'current' to search through linked list
  while (current != NULL){
    if (current->value == p_value)
        return true;
    current = current->next;
  }
  return false;
}

// __add__
Value* add (Value* self, Value* other) {
  struct Value* out = (struct Value*)malloc(sizeof(struct Value));
  
  out->data = self->data + other->data;

  struct Linked_list* children = (struct Linked_list*)malloc(sizeof(struct Linked_list));
  children->head = NULL;

  printf("going to append");

  append(children->head, self);
  append(children->head, other);

  printf("appended the children");

  out->_prev = children;

    void backward(){
        self->grad = self->grad + other->data * out->grad;
        other->grad = other->grad + self->data * out->grad;
    }
    out->backward = backward;

  return out;
}

void backward(Value* self) {

  // topological order all of the children in the graph
  struct Linked_list* topo = (struct Linked_list*)malloc(sizeof(struct Linked_list));
  struct Linked_list* visited = (struct Linked_list*)malloc(sizeof(struct Linked_list));
  topo->head = NULL;
  visited->head = NULL;
 
  void build_topo(Value* self){
      if(!search(visited->head, self)){
          append(visited->head, self);

          Node* childrenPointer = (self->_prev)->head;
          
          while(childrenPointer != NULL){
              build_topo(childrenPointer->value);
              
              childrenPointer = childrenPointer->next;
          }
          append(topo->head, self);
      }
  }
  build_topo(self);

  // go one variable at a time and apply the chain rule to get its gradient
    self->grad = 1;
    
    Node* pointer = topo->head;

    while(pointer->next != NULL){
        pointer = pointer->next;
    }

    //should maybe be a null check of pointer.
    while(pointer->prev != NULL){
        pointer->value->backward();
        pointer = pointer->prev;
    }
    
}

//def __radd__(self, other): # other + self
//    return self + other

int main() 
{
    printf("Hello World\n");

    struct Value* a = (struct Value*)malloc(sizeof(struct Value));
    struct Value* b = (struct Value*)malloc(sizeof(struct Value));

    printf("Values initialized\n");
    
    add(a, b);
    printf("a + b\n");


    printf("a grad before %d\n", a->grad);
    backward(a);
    printf("a grad after %d\n", a->grad);

}