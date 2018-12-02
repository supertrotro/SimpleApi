import matplotlib.pyplot as plt
import random

x = [random.randint(0,20) for i in range(60)]
y = [random.randint(0,20) for i in range(60)]

plt.scatter(x,y)
plt.show()
