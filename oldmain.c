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