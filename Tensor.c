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

Tensor* tmul(Tensor* a, Tensor* b);
Tensor* tadd(Tensor* a, Tensor* b);
Tensor* tsub(Tensor* a, Tensor* b);

void scalarmul(double scalar, Tensor* tensor);
void tensorBackwards(Tensor* tensor);
double** readGradients(Tensor* tensor);
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

// int main(){
//   printf("hello world2\n");
//   int rowcount = 1;
//   int colcount = 1;

//   Value*** values1 = (Value***)malloc(rowcount * sizeof(Value*));
//     for(int i=0; i < rowcount; i++) values1[i] = (Value*)malloc(colcount * sizeof(Value*));

//   //Value *values1[1][1];

//   values1[0][0] = newValue(0.0);

//   Tensor r1 = {
//       .values = values1,
//       .rows = rowcount,
//       .columns = colcount        
//   };

//   Value*** values2 = (Value***)malloc(rowcount * sizeof(Value*));
//     for(int i=0; i < rowcount; i++) values2[i] = (Value *)malloc(colcount * sizeof(Value*));
//   // Value *values2[1][1];

//   values2[0][0] = newValue(1.0);

//   Tensor r2 = {
//       .values = values2,
//       .rows = rowcount,
//       .columns = colcount        
//   };

//   Value*** values3 = (Value***)malloc(rowcount * sizeof(Value*));
//     for(int i=0; i < rowcount; i++) values3[i] = (Value *)malloc(colcount * sizeof(Value*));
//   // Value *values3[1][1];

//   values3[0][0] = newValue(20.0);

//   Tensor c1 = {
//       .values = values3,
//       .rows = rowcount,
//       .columns = colcount        
//   };

//   Value*** values4 = (Value***)malloc(rowcount * sizeof(Value*));
//     for(int i=0; i < rowcount; i++) values4[i] = (Value *)malloc(colcount * sizeof(Value*));
//   // Value *values4[1][1];

//   values4[0][0] = newValue(21.0);

//   Tensor c2 = {
//       .values = values4,
//       .rows = rowcount,
//       .columns = colcount        
//   };

//   Tensor* p1 = tmul(&r1, &c1);
//   Tensor* p2 = tmul(&r2, &c2);
//   Tensor* res = tadd(p1, p2);
//   printf("before backwards\n");
//   printf("res data: %lf\n", res->values[0][0]->data);
//   printf("res grad: %lf\n", res->values[0][0]->grad);
//   tensorBackwards(res);
//   printf("res data: %lf\n", res->values[0][0]->data);
//   printf("res grad: %lf\n", res->values[0][0]->grad);

//   printf("r1 data: %lf\n", r1.values[0][0]->data);
//   printf("r1 grad: %lf\n", r1.values[0][0]->grad);
//   printf("r2 data: %lf\n", r2.values[0][0]->data);
//   printf("r2 grad: %lf\n", r2.values[0][0]->grad);
  
// }

int main(){
    printf("hello world\n");

    int rowcount = 2;
    int colcount = 2;

    Value*** values1 = (Value***)malloc(rowcount * sizeof(Value*));
    for(int i=0; i < rowcount; i++) values1[i] = (Value*)malloc(colcount * sizeof(Value*));

    for(int i=0; i < rowcount; i++){
        for (int j = 0; j < colcount; j++)
        {
            values1[i][j] = newValue(i+j+1);
        }
    }

    Tensor t1 = {
        .values = values1,
        .rows = rowcount,
        .columns = colcount        
    };

    printf("Tensor t1: \n");
    printTensor(&t1);

    Value*** values2 = (Value***)malloc(rowcount * sizeof(Value*));
    for(int i=0; i < rowcount; i++) values2[i] = (Value*)malloc(colcount * sizeof(Value*));

    for(int i=0; i < rowcount; i++){
        for (int j = 0; j < colcount; j++)
        {
            values2[i][j] = newValue(i+j+j+j+j+20);
        }
    }

    Tensor t2 = {
        .values = values2,
        .rows = rowcount,
        .columns = colcount        
    };

    printf("Tensor t2: \n");
    printTensor(&t2);
    // scalarmul(10, &t2);
    // printf("after scalar mul with 10\n");
    // printTensor(&t2);

    Tensor* c = tmul(&t1, &t2);

    printf("rows of tensor c: %d\n", c->rows);

    printf("\nOutput Matrix:\n");
    printTensor(c);

    tensorBackwards(c);   
    double** gradients = readGradients(c);

    printf("\ngradients Matrix C:\n");
    for (int i = 0; i < c->rows; i++) {
        for (int j = 0; j < c->columns; j++) {
            printf("%lf  ", gradients[i][j]);
            if (j == c->columns - 1)
            printf("\n");
        }
    }

    double** gradients1 = readGradients(&t1);
    printf("\ngradients Matrix t1:\n");
    for (int i = 0; i < t1.rows; i++) {
        for (int j = 0; j < t1.columns; j++) {
            printf("%lf  ", gradients1[i][j]);
            if (j == t1.columns - 1)
            printf("\n");
        }
    }

    double** gradients2 = readGradients(&t2);
    printf("\ngradients Matrix t2:\n");
    for (int i = 0; i < t2.rows; i++) {
        for (int j = 0; j < t2.columns; j++) {
            printf("%lf  ", gradients2[i][j]);
            if (j == t1.columns - 1)
            printf("\n");
        }
    }
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

double** readGradients(Tensor* tensor){
  double** gradients = (double**)malloc(tensor->rows * sizeof(double));
  for(int i=0; i < tensor->rows; i++) gradients[i] = (double *)malloc(tensor->columns * sizeof(double));

  for (int i = 0; i < tensor->rows; i++)
  {
    for (int j = 0; j < tensor->columns; j++)
    {
      gradients[i][j] = tensor->values[i][j]->grad;
    }
  }

  return gradients;
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

void scalarmul(double scalar, Tensor* tensor){
  Value* valueScalar = newValue(scalar);

  for(int i=0; i < tensor->rows; i++){
    for (int j = 0; j < tensor->columns; j++)
    {
        tensor->values[i][j] = mul(valueScalar, tensor->values[i][j]);
    }    
  }
}
