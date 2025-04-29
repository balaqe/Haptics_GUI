syms i

samp_rate = 10
f0 = 1
f1 = 2
n = 21 % s
phi0 = 0

arg0 = ( (f0/samp_rate) + (f1 - f0)/(2*samp_rate*n)*i ) * 2*pi*i + phi0
y0 = sin(arg0)

phase(i) = mod(arg0, 2*pi)
% fplot (phase, [0, 20])
phi1 = phase(n)
% double(phi1)

arg1 = ( (f0/samp_rate) + (f1 - f0)/(2*samp_rate*n)*(i-n) ) * 2*pi*(i-n) + phi1
y1 = sin(arg1)

fplot (y0, [0, n])
hold on
fplot (y1, [n, 50])
hold off