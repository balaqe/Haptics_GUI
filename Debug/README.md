# Debug plotter

## Setup:

### Create Python virtual environment (optional)
```
python3 -m venv venv
```
### Activate virtual environment
```
source venv/bin/activate
```
### Install Matplotlib
```
python3 -m pip install matplotlib
```
### Install Watchdog
```
python3 -m pip install watchdog
```

## Use plotter

Plotter requires data in csv with the first column being the index and the second being the data.
### Example data formatting
```
0,0.00
1,0.01
2,0.03
3,0.04
4,0.06
5,0.07
6,0.09
7,0.10
8,0.11
9,0.13
10,0.14
```

### Run

Run the python program and provide the csv file of the data you'd like to visualize
```
python3 livePlotter.py mydata.csv
```