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
  //  void (*append)(struct Node** head, Value* new_value);
} Linked_list;

struct Linked_list* newLinkedList();
struct Value* newValue(float data, Linked_list* children);

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
  struct Linked_list* topo = newLinkedList();
  struct Linked_list* visited = newLinkedList();
 
  void build_topo(Value* self){
      if(!search(visited->head, self)){
          append(visited->head, self);
          printf("Visited head appended\n");
          
          Node* childrenPointer = (self->_prev)->head;
          printf("childrenPointer initialised\n");
          
          while(childrenPointer != NULL){
              build_topo(childrenPointer->value);
              printf("mid while loop\n");
              childrenPointer = childrenPointer->next;
          }
          append(topo->head, self);
      }
  }
  build_topo(self);
  printf("Topo built\n\n");

  // go one variable at a time and apply the chain rule to get its gradient
  self->grad = 1.0;
  printf("address of self %p", self);
  printf("Grad set\n");
  printf("pointer grad %p\n", self->grad);
  printf("value grad %d\n", self->grad);
  if(topo->head != NULL){
    Node* pointer = topo->head;

    while(pointer->next != NULL){
        pointer = pointer->next;
    }
    printf("Linked List traversed\n");
    //should maybe be a null check of pointer.
    while(pointer->prev != NULL){
        pointer->value->backward();
        pointer = pointer->prev;
    }
    printf("Backwards done\n");
  }
}

//def __radd__(self, other): # other + self
//    return self + other

int main() 
{
    printf("Hello World\n");

    
    Value* a = newValue(10, newLinkedList());
    Value* b = newValue(8, newLinkedList());

    printf("address of a %p\n", a);
    printf("address of b %p\n", b);
    printf("Values initialized\n");
    a->data = 99;
    printf("a value before %d\n", a->data);
    
    add(a, b);
    printf("a + b\n");

    printf("a value before %d\n", a->data);
    printf("a grad before %d\n", a->grad);
    printf("b grad before %d\n", b->grad);
    backward(a);
    backward(b);
    printf("a grad after %d\n", a->grad);
    printf("b grad after %d\n", b->grad);
}

struct Linked_list* newLinkedList(){
  struct Linked_list* list = (struct Linked_list*)malloc(sizeof(struct Linked_list));
  list->head = NULL;

  return list;
}

struct Value* newValue(float data, Linked_list* children){
  struct Value* val = (struct Value*)malloc(sizeof(struct Value));

  val->data = data;
  val->_prev = children;
  val->grad = 0;
  printf("initialised grad %p\n", val->grad);
  val->backward = NULL; 

  return val;
}

/*
typedef struct Value {
  float data;
  float grad;
  void (*backward)();
  struct Linked_list* _prev;
  char op[];
} Value;
*/
