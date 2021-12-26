import matplotlib.pyplot as plt
import matplotlib.dates as mdate
import sys

data = []

date, on, w1, w2, m1, m2, m3, m6 = zip(*data)

plt.plot(date, on, color='#e1b80a')
plt.plot(date, w1, color='#e3af00')
plt.plot(date, w2, color='#e29500')
plt.plot(date, m1, color='#e07b00')
plt.plot(date, m2, color='#db6600')
plt.plot(date, m3, color='#d44000')
plt.plot(date, m6, color='#ca1010')

fig = plt.gcf()
fig.tight_layout()

ldg = plt.legend(['on', '1w', '2w', '1m', '2m', '3m', '6m'], loc=0,)

locator = mdate.AutoDateLocator()
plt.gca().xaxis.set_major_locator(locator)

plt.grid(color='whitesmoke', linestyle='dotted')

plt.savefig(sys.argv[1], facecolor='w', dpi=240)
