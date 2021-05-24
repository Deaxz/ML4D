import torch

a = torch.tensor([[-1.0, 412., -55.2],
                  [77.1, 981., -48.]], requires_grad=True)

b = torch.tensor([[-144., 412.],
                  [-1., 5.],
                  [55., 13.]], requires_grad=True)

c = ((torch.matmul(a / 0.5, b) / 20)**8) + 4

d = torch.tensor([[1.], [-1.]], requires_grad=True)

e = (torch.matmul(c, d) * 45) / 14

f = torch.tensor([11., -19], requires_grad=True)

result = (torch.matmul(f, e)**0.5) - 30

result.backward()

print(a.grad)
