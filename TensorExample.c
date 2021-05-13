#include <stdio.h>
#include <stdlib.h>
#include <stdbool.h>
#include <math.h>

typedef struct Value {
  double data;
  double grad;
  struct Backward* backward;
  struct LinkedList* _prev;
  char op[];
} Value;

typedef struct Tensor{
    struct Value** values;
    double** gradients;
    int rows;
    int columns; 
} Tensor;


typedef struct Backward{
  struct Value* self;
  struct Value* other;
  struct Value* out;
  void (*invoke)(struct Value* self, Value* other, Value* out); 
} Backward;

typedef struct Node {
  struct Value* value;
  struct Node* prev;
  struct Node* next;
} Node;

/* Needed for backward (visited = set()) */
typedef struct LinkedList {
  struct Node* head;
  //  void (*append)(struct Node** head, Value* new_value);
} LinkedList;

struct LinkedList* newLinkedList();
struct Backward* newBackward();
struct Value* newValue(double data);
struct Tensor newTensor(Value* rows[], Value* columns[], int rowLength, int columnLength);
Value* add(Value* self, Value* other);
Value* mul(Value* self, Value* other);
Value* relu(Value* self);
void add_backward(Value* self, Value* other, Value* out);
void mul_backward(Value* self, Value* other, Value* out);
void power_backward(Value* self, Value* other, Value* out);
void relu_backward(Value* self, Value* other, Value* out);
Value* neg(Value* self);
Value* sub(Value* self, Value* other);
Value* truediv(Value* self, Value* other);

Tensor tmul(Tensor a, Tensor b);
void tensorBackwards(Tensor tensor);
double** readGradients(Tensor tensor);
void printTensor(Tensor t);

void freeLinkedList(struct LinkedList** head_ref);
void freeNodeRec(Node** node);
void freeNode(struct Node** node);
void freeValue(struct Value** Value);
void freeBackward(struct Backward** backward);

Node* wrapValue(Value* value){
    struct Node* node = (struct Node*)malloc(sizeof(struct Node));
    node->value = value;
    node->prev = NULL;
    node->next = NULL;

    return node;
}

void append(struct Node** head, Value* new_value) {
  Node* new_node = wrapValue(new_value);

  if ((*head) == NULL) {
    new_node->prev = NULL;
    *head = new_node;
    return;
  }
  
  Node* temp = *head;
  while (temp->next != NULL) {
    temp = temp->next;
  }
  temp->next = new_node;
  
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

struct Value* newValue(double data){
  struct Value* val = (struct Value*)malloc(sizeof(struct Value));

  val->data = data;
  val->_prev = newLinkedList();
  val->grad = 0;
  val->backward = newBackward();

  return val;
}

struct LinkedList* newLinkedList(){
  struct LinkedList* list = (struct LinkedList*)malloc(sizeof(struct LinkedList));
  list->head = NULL;

  return list;
}

struct Backward* newBackward(){
  struct Backward* backward = (struct Backward*)malloc(sizeof(struct Backward));
  backward->self = NULL;
  backward->other = NULL;
  backward->out = NULL;
  backward->invoke = NULL;
}

Value* add (Value* self, Value* other) {
  struct Value* out = newValue(self->data + other->data);

  append(&out->_prev->head, self);
  append(&out->_prev->head, other);

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
  struct Value* out = newValue(self->data * other->data);

  append(&out->_prev->head, self);
  append(&out->_prev->head, other);

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
  struct Value* out = newValue(pow(self->data, other->data));

  append(&out->_prev->head, self);

  out->backward->self = self;
  out->backward->other = other;
  out->backward->out = out;
  out->backward->invoke = &power_backward;
  
  return out;
}

void power_backward(Value* self, Value* other, Value* out){
    self->grad = self->grad + (other->data * pow(self->data, other->data -1.0)) * out->grad;
}

Value* relu(Value* self){
  Value* out = newValue(self->data < 0 ? 0 : self->data);
  append(&out->_prev->head, self);
  
  out->backward->self = self;
  out->backward->out = out;
  out->backward->invoke = &relu_backward;

  return out;
}

void relu_backward(Value* self, Value* other, Value* out){
  self->grad = self->grad + (out->data > 0) * out->grad;
}

void backward(Value* self) {
  // topological order all of the children in the graph
  struct LinkedList* topo = newLinkedList();
  struct LinkedList* visited = newLinkedList();

  void build_topo(Value* self){
      if(!search(visited->head, self)){
          append(&visited->head, self);
          
          Node* childrenPointer = (self->_prev)->head;
          
          while(childrenPointer != NULL){
              build_topo(childrenPointer->value);
              childrenPointer = childrenPointer->next;
          }
          append(&topo->head, self);
      }
  }
  build_topo(self);
  
  // go one variable at a time and apply the chain rule to get its gradient
  self->grad = 1.0;
  if(topo->head != NULL){
    Node* pointer = topo->head;
    while(pointer->next != NULL){
        pointer = pointer->next;
    }


    while(pointer->prev != NULL){
        if(pointer->value->backward->invoke != NULL){
          pointer->value->backward->invoke(
            pointer->value->backward->self,
            pointer->value->backward->other,
            pointer->value->backward->out
          ); // v._backward()
        }
        
        pointer = pointer->prev;
    }
  }
  //printf("topo pointer = %p\n", topo);
  freeLinkedList(&topo);
  //printf("topo pointer = %p\n", topo);
  freeLinkedList(&visited);
}

Value* neg(Value* self){
  return mul(self, newValue(-1.0));
}

Value* sub(Value* self, Value* other){
  return add(self, neg(other));
}

Value* truediv(Value* self, Value* other){
  return mul(self, power(other, newValue(-1.0)));
}


/* Function to delete linked list */
void freeLinkedList(struct LinkedList** linkedList){
  struct Node* current = (*linkedList)->head;
  struct Node* next;

  freeNodeRec(&(*linkedList)->head);
  // while (current != NULL)
  // {
  //   next = current->next;
  //   //printf("current data = %lf\n", current->value->data);
  //   free(current);
  //   //current = NULL;
  //   //printf("current data = %lf\n", current->value->data);
  //   current = next;
  // }
  free(*linkedList);
  *linkedList = NULL;
}

// 

// typedef struct Value {       <----- malloc
//   double data;
//   double grad;
//   struct Backward* backward; <----- malloc
//   struct LinkedList* _prev;  <----- malloc
//   char op[];
// } Value;


void freeNodeRec(Node** node){
  if((*node) == NULL){
    return;
  } 
  Node* next = (*node)->next;
  freeNodeRec(&next);
  //freeNode(node);
  free(node);
}

void freeNode(struct Node** node){
  freeValue(&((*node)->value));
  free(*node);
  *node = NULL;
}

void freeValue(struct Value** value){
  freeBackward(&((*value)->backward));
  freeLinkedList(&((*value)->_prev));
  free(*value);
  *value = NULL;
}

void freeBackward(struct Backward** backward){
  free(*backward);
  *backward = NULL;
}

Tensor newTensor(Value* rows[], Value* columns[], int rowLength, int columnLength){
    Tensor res;

    res.values = malloc(rowLength * columnLength * sizeof(Value));
    res.rows = rowLength;
    res.columns = columnLength;

    return res;
}


int main(){
    // Value* first = newValue(10);
    // Value* second = newValue(20);
    // Value* third = mul(first, second);
    // Value* forth = newValue(44);

    // Value* result = mul(third, forth);
    // //f()=((10*20)*44)^2
    // //f'()=2*(10*20*44)=17600

    // Value* final = power(result, newValue(2));
    // //final= ax

    // backward(final);
    // printf("result data: %lf, grad: %lf\n", result->data, result->grad);
    // printf("data: %lf, grad: %lf\n", final->data, final->grad);
    // printf("first: data: %lf, grad: %lf\n", first->data, first->grad);

    Value* r1 = newValue(0);
    Value* r2 = newValue(1);
    //Value* r3 = newValue(2);
    //Value* r4 = newValue(3);
    
    Value* c1 = newValue(20);
    Value* c2 = newValue(21);
    //Value* c3 = newValue(22);
    //Value* c4 = newValue(23);

    Value* p1 = mul(r1, c1);
    Value* p2 = mul(r2, c2);
    //Value* p3 = mul(r3, c3);
    //Value* p4 = mul(r4, c4);
    
    //Value* res = add(add(add(p1, p2), p3), p4);
    Value* res = add(p1, p2);

    backward(res);
    printf("res data: %lf\n", res->data);
    printf("res grad: %lf\n", res->grad);

    printf("r1 data: %lf\n", r1->data);
    printf("r1 grad: %lf\n", r1->grad);
}

void tensorBackwards(Tensor tensor){
  Value** values = tensor.values;

  for(int i=0; i < tensor.rows; i++){
    for(int j=0; j < tensor.columns; j++){
      backward(&values[i][j]);
    }
  }
}

void printTensor(Tensor t){
    for (int i = 0; i < t.rows; ++i) {
        for (int j = 0; j < t.columns; ++j) {
            printf("%lf  ", t.values[i][j]);
            if (j == t.columns - 1)
            printf("\n");
        }
    }
}

double** readGradients(Tensor tensor){
  double** gradients = (double**)malloc(tensor.rows * sizeof(double));
  for(int i=0; i < tensor.rows; i++) gradients[i] = (double *)malloc(tensor.columns * sizeof(double));

  for (int i = 0; i < tensor.rows; i++)
  {
    for (int j = 0; j < tensor.columns; j++)
    {
      gradients[i][j] = tensor.values[i][j].grad;
    }
  }

  return gradients;
}

Tensor tmul(Tensor a, Tensor b){
    Tensor* res = (Tensor*)malloc(sizeof(Tensor));
    res->rows = a.columns;
    res->columns = b.rows;

    Value** resvalues = (Value**)malloc(res->rows * sizeof(Value));
    for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value));

    for (int i = 0; i < a.rows; i++)
    {
        for (int j = 0; j < b.columns; j++)
        {
            for (int k = 0; k < a.columns; k++)
            {
                resvalues[i][j] = *add(&resvalues[i][j], mul(&a.values[i][k], &b.values[k][j]));
            }
        }
    }

    res->values = resvalues;
    return *res;    
}
