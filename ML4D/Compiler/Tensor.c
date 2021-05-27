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
    struct Value*** values;
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
} LinkedList;

struct LinkedList* newLinkedList();
struct Backward* newBackward();
struct Value* newValue(double data);
struct Tensor* newTensor(double* inputValues, int rowLength, int columnLength);

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

Tensor* tmul(Tensor* a, Tensor* b);
Tensor* tadd(Tensor* a, Tensor* b);
Tensor* tsub(Tensor* a, Tensor* b);
Tensor* tdiv(Tensor* a, double b);
Tensor* scalarmul(double scalar, Tensor* tensor);
Tensor* convertToTensor(double val, int rows, int cols);
Tensor* tpow(Tensor* base, double exponent);

void zeroValue(Node* head);
void zeroGradients(Tensor* tensor);

void tensorBackwards(Tensor* tensor);
Tensor* readGradients(Tensor* tensor);
void printTensor(Tensor* t);

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

Value* add(Value* self, Value* other) {
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
          );
        }
        
        pointer = pointer->prev;
    }
  }
  freeLinkedList(&topo);
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
  free(*linkedList);
  *linkedList = NULL;
}

void freeNodeRec(Node** node){
  if((*node) == NULL){
    return;
  } 
  Node* next = (*node)->next;
  freeNodeRec(&next);
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

Tensor* newTensor(double* inputValues, int rowLength, int columnLength){
    Value*** tensorValues = (Value***)malloc(rowLength * sizeof(Value));
    for(int i=0; i < rowLength; i++) tensorValues[i] = (Value*)malloc(columnLength * sizeof(Value*));

    int length = rowLength * columnLength;
    int i = 0;
    for(int j=0; i < length; j++){
      Value* val = newValue(inputValues[i]);
      tensorValues[i][j] = val;
      if(j % columnLength == 0) i++;
    }

    Tensor* res = malloc(sizeof(Tensor));
    res->values = tensorValues;
    res->rows = rowLength;
    res->columns = columnLength;

    free(inputValues);
    return res;
}

void tensorBackwards(Tensor* tensor){
  backward(tensor->values[0][0]);
}

void printTensor(Tensor* t){
  for (int i = 0; i < t->rows; ++i) {
      for (int j = 0; j < t->columns; ++j) {
          printf("%lf  ", t->values[i][j]->data);
          if (j == t->columns - 1)
          printf("\n");
      }
  }
}

Tensor* readGradients(Tensor* tensor){
  int length = tensor->rows + tensor->columns;
  double* gradients = (double*)malloc(length * sizeof(double));

  for (int i = 0; i < tensor->rows; i++)
  {
    for (int j = 0; j < tensor->columns; j++)
    {
      gradients[i+j] = tensor->values[i][j]->grad;
    }
  }

  return newTensor(gradients, tensor->rows, tensor->columns);
}

Tensor* tmul(Tensor* a, Tensor* b){
    Tensor* res = (Tensor*)malloc(sizeof(Tensor));
    res->rows = a->rows;
    res->columns = b->columns;

    Value*** resvalues = (Value***)malloc(res->rows * sizeof(Value*));
    for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value*));
    res->values = resvalues;

    for (int i = 0; i < a->rows; i++)
    {
        for (int j = 0; j < b->columns; j++)
        {
            int k=0;
            resvalues[i][j] = mul(a->values[i][k], b->values[k][j]);
            k++;
            for (; k < a->columns; k++)
            {
                resvalues[i][j] = add(resvalues[i][j], mul(a->values[i][k], b->values[k][j]));
            }
        }
    }

    return res;    
}

Tensor* tadd(Tensor* a, Tensor* b){
  Tensor* res = (Tensor*)malloc(sizeof(Tensor));
  res->rows = a->rows;
  res->columns = a->columns;

  Value*** resvalues = (Value***)malloc(res->rows * sizeof(Value*));
  for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value*));
  res->values = resvalues;

  for (int i = 0; i < a->rows; i++)
  {
      for (int j = 0; j < b->columns; j++)
      {
          resvalues[i][j] = add(a->values[i][j], b->values[i][j]);
      }
  }

  return res;  
}

Tensor* tsub(Tensor* a, Tensor* b){
  Tensor* res = (Tensor*)malloc(sizeof(Tensor));
  res->rows = a->rows;
  res->columns = a->columns;

  Value*** resvalues = (Value***)malloc(res->rows * sizeof(Value*));
  for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value*));
  res->values = resvalues;

  for (int i = 0; i < a->rows; i++)
  {
      for (int j = 0; j < b->columns; j++)
      {
          resvalues[i][j] = sub(a->values[i][j], b->values[i][j]);
      }
  }

  return res;  
}

Tensor* tpow(Tensor* base, double exponent){
  Tensor* res = (Tensor*)malloc(sizeof(Tensor));
  res->rows = base->rows;
  res->columns = base->columns;

  Value*** resvalues = (Value***)malloc(res->rows * sizeof(Value*));
  for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value*));
  res->values = resvalues;

  for(int i=0; i < base->rows; i++){
    for (int j = 0; j < base->columns; j++)
    {
      res->values[i][j] = power(base->values[i][j], newValue(exponent));
    }
  }
  
  return res;
}


Tensor* scalarmul(double scalar, Tensor* tensor){
  Value* valueScalar = newValue(scalar);
  
  Tensor* res = (Tensor*)malloc(sizeof(Tensor));
  res->rows = tensor->rows;
  res->columns = tensor->columns;

  Value*** resvalues = (Value***)malloc(res->rows * sizeof(Value*));
  for(int i=0; i < res->rows; i++) resvalues[i] = (Value *)malloc(res->columns * sizeof(Value*));
  res->values = resvalues;

  for(int i=0; i < tensor->rows; i++){
    for (int j = 0; j < tensor->columns; j++)
    {
        res->values[i][j] = mul(valueScalar, tensor->values[i][j]);
    }    
  }

  return res;
}

Tensor* convertToTensor(double val, int rows, int cols){
  int length = rows * cols;
  double* values = (double*)malloc(length * sizeof(double));

  for (int i = 0; i < length; i++)
  {
    values[i] = val;
  }

  return newTensor(values, rows, cols);
}

Tensor* tdiv(Tensor* a, double b){
  return scalarmul(1/b, a);
}

void zeroGradients(Tensor* tensor){
  zeroValue(tensor->values[0][0]->_prev->head);
}

void zeroValue(Node* head){
  Node* curr = head;
  while(curr != NULL){
    curr->value->grad = 0; 
    zeroValue(curr->next);
    curr = curr->next;
  }
}