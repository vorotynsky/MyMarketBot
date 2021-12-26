import matplotlib.pyplot as plt
import sys

data_w = []
data_m = []
data_0 = []

(x, yw) = zip(*data_w)
(_, ym) = zip(*data_m)
(_, y0) = zip(*data_0)
x = list(map(str, x))

plt.grid(axis='y', color='whitesmoke', linestyle='dotted')

plt.scatter(x, yw, marker='.', c='silver')
plt.scatter(x, ym, marker='.', c='gainsboro')
plt.scatter(x, y0, marker=".", c='red')

plt.legend(["week ago", "month ago", "now"])

fig = plt.gcf()
fig.tight_layout()

plt.savefig("zcyc.png", facecolor='w', dpi=240)
