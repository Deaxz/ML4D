#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <math.h>

//prototypes

/* Fra def __init__ */
typedef struct Value {
  float data;
  float grad;
  struct Backward* backward;
  struct Linked_list* _prev;
  char op[];
} Value;

typedef struct Backward{
  struct Value* self;
  struct Value* other;
  struct Value* out;
  void (*invoke)(struct Value* self, Value* other, Value* out); 
} Backward;


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
void add_backward(Value* self, Value* other, Value* out);
void mul_backward(Value* self, Value* other, Value* out);
void power_backward(Value* self, Value* other, Value* out);

//Functions

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

void append(struct Node** head, Value* new_value) {
  /* 1. Allocade node *//* 2. Put data in node */
  Node* new_node = wrapValue(new_value);
  /* 4. If Linked List is empty, then make this node new head */
  if ((*head) == NULL) {
    new_node->prev = NULL;
    *head = new_node;
    return;
  }
  /* 5. Else traverse until last node */
  Node* temp = *head;
  while (temp->next != NULL) {
    temp = temp->next;
    //printf("%s\n", temp);
  }
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

Value* add (Value* self, Value* other) {
  struct Linked_list* children = newLinkedList();  
  struct Value* out = newValue(self->data + other->data, children);

  append(&children->head, self);
  append(&children->head, other);

  out->backward->self = self;
  out->backward->other = other;
  out->backward->out = out;
  out->backward->invoke = &add_backward;

  return out;
}

void add_backward(Value* self, Value* other, Value* out){
    self->grad = self->grad + out->grad;
    other->grad = other->grad + out->grad;
}

Value* mul (Value* self, Value* other) {
  struct Linked_list* children = newLinkedList();
  struct Value* out = newValue(self->data * other->data, children);

  append(&children->head, self);
  append(&children->head, other);

/*  void backward(){
      self->grad = self->grad + other->data * out->grad;
      other->grad = other->grad + self->data * out->grad;
  }*/
  out->backward->self = self;
  out->backward->other = other;
  out->backward->out = out;
  out->backward->invoke = &mul_backward;

  return out;
}

void mul_backward(Value* self, Value* other, Value* out){
    self->grad = self->grad + other->data * out->grad;
    other->grad = other->grad + self->data * out->grad;
}

Value* power(Value* self, Value* other){
  struct Linked_list* children = newLinkedList();
  struct Value* out = newValue(pow(self->data, other->data), children);

  out->backward->self = self;
  out->backward->other = other;
  out->backward->out = out;
  out->backward->invoke = &power_backward;
  printf("Power successful\n");
  return out;
}

void power_backward(Value* self, Value* other, Value* out){
    //self.grad += (other * self.data**(other-1)) * out.grad
    self->grad = (pow(other->data * self->data, other->data - 1)) * out->grad;
}



void backward(Value* self) {

  // topological order all of the children in the graph
  struct Linked_list* topo = newLinkedList();
  struct Linked_list* visited = newLinkedList();

  void build_topo(Value* self){
      printf("counting\n");
      if(!search(visited->head, self)){
          append(&visited->head, self);
          
          Node* childrenPointer = (self->_prev)->head;
          
          printf("Before children while!\n\n");
          while(childrenPointer != NULL){
              build_topo(childrenPointer->value);
              childrenPointer = childrenPointer->next;
          }
          append(&topo->head, self);
      }
  }
  build_topo(self);
  
  printf("I've built the topo!!\n\n");
  // go one variable at a time and apply the chain rule to get its gradient
  self->grad = 1.0;
  if(topo->head != NULL){
    Node* pointer = topo->head;
    while(pointer->next != NULL){
        pointer = pointer->next;
    }


    while(pointer->prev != NULL){
        printf("pointer curr: %p\n", pointer->value);
        printf("curr backward: %p\n", pointer->value->backward);
        printf("curr backward invoke: %p\n", pointer->value->backward->invoke);
        printf("pointer prev: %p\n", pointer->prev);
        


        printf("Start second while\n");

        printf("backward function pointer: %p\n", pointer->value->backward);
        printf("value pointer: %p\n", pointer->value);
        printf("value pointer's backward: %p\n", pointer->value->backward);

        if(pointer->value->backward->invoke != NULL){
          printf("calling backward\n");
          printf("Backward->invoke = %p\n", pointer->value->backward->invoke);
          printf("Backward->self = %p\n", pointer->value->backward->self);
          printf("Backward->other = %p\n", pointer->value->backward->other);
          printf("Backward->out = %p\n\n", pointer->value->backward->out);
          pointer->value->backward->invoke(
            pointer->value->backward->self,
            pointer->value->backward->other,
            pointer->value->backward->out
          ); // v._backward()
        }
        
        printf("After backward()\n");
        pointer = pointer->prev;
    }
    printf("Build_topo end\n");
  }
}

// int main() 
// {
//     printf("Hello World\n");

    
//     Value* a = newValue(99, newLinkedList());
//     Value* b = newValue(8, newLinkedList());
//     printf("A pointer: %p\n", a);
//     printf("B pointer: %p\n", b);
//     Value* c;

//     //printf("address of a %p\n", a);
//     //printf("address of b %p\n", b);
//     //printf("Values initialized\n");
//     //a->data = 99;
    
//     //add(&a, &b);
//     c = power(a, b);
//     printf("after power\n");
//     //printf("%p\n", c->_prev->head->value);

//     printf("c's children");
//     //printf("%p\n", c->_prev->head->value);
//     //pow(&c);
//     printf("a + b\n");

//     printf("a grad before %f\n", a->grad);
//     printf("b grad before %f\n", b->grad);
//     printf("c grad before %f\n", c->grad);
//     //backward(a);
//     //backward(b);
//     backward(c);
//     printf("a data after: %f a grad after %f\n", a->data, a->grad);
//     printf("b data after: %f b grad after %f\n", b->data, b->grad);
//     printf("c data after: %f c grad after %f\n", c->data, c->grad);

//     return 0;
// }

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
  val->backward = (struct Backward*)malloc(sizeof(struct Backward));

  return val;
}

int main(){
  //Value* b = newValue(8, newLinkedList())
  Value* x1 = newValue(0, newLinkedList());
  Value* y1 = newValue(0, newLinkedList());

  Value* x2 = newValue(1, newLinkedList());
  Value* y2 = newValue(1, newLinkedList());

  // Value* x3 = newValue(2, newLinkedList());
  // Value* y3 = newValue(2, newLinkedList());

  // Value* x4 = newValue(3, newLinkedList());
  // Value* y4 = newValue(3, newLinkedList());

  // Value* x5 = newValue(4, newLinkedList());
  // Value* y5 = newValue(4, newLinkedList());

  Value* a = newValue(-1, newLinkedList());
  Value* b = newValue(2, newLinkedList());

  Value* step_size = newValue(0.1, newLinkedList());

  printf("Before loop\n");

  for(int i = 0; i < 1; i++){

          // d1 = a * x1 + b - y1
      Value* d1 = add(add(mul(a, x1), b), mul(y1, newValue(-1.0, newLinkedList())));
      Value* d2 = add(add(mul(a, x2), b), mul(y2, newValue(-1.0, newLinkedList())));
      // Value* d3 = add(add(mul(a, x3), b), mul(y3, newValue(-1.0, newLinkedList())));
      // Value* d4 = add(add(mul(a, x4), b), mul(y4, newValue(-1.0, newLinkedList())));
      // Value* d5 = add(add(mul(a, x5), b), mul(y5, newValue(-1.0, newLinkedList())));

      printf("Before loss\n");

      //loss = (d1**2 + d2**2 + d3**2 + d4**2 + d5**2)/5
      //Value* loss = mul(add(add(add(add(power(d1, newValue(2, newLinkedList())), power(d2, newValue(2, newLinkedList()))), power(d3, newValue(2, newLinkedList()))), power(d4, newValue(2, newLinkedList()))), power(d5, newValue(2, newLinkedList()))), newValue(0.2, newLinkedList()));
      Value* loss = mul(add(mul(d1, newValue(d1->data, newLinkedList())),
                            mul(d2, newValue(d2->data, newLinkedList()))
                                                                        ), newValue(0.5, newLinkedList()));
      
      printf("After backward(loss)\n");
      backward(loss);
      printf("After loss\n");

      printf("A data: %f, A grad: %f\n", a->data, a->grad);
      printf("B data: %f, B grad: %f\n", b->data, b->grad);

      //a = add(mul(a, step_size), mul(a, newValue(-1.0, newLinkedList())));
      a = add(mul(a, newValue(-1.0, newLinkedList())), mul(a, step_size));
      b = add(mul(b, newValue(-1.0, newLinkedList())), mul(b, step_size));

      a->grad = 0;
      b->grad = 0;

  }

  printf("a data after: %f a grad after %f\n", a->data, a->grad);
  printf("b data after: %f b grad after %f\n", b->data, b->grad);

  return 0;
}