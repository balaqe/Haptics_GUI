import matplotlib.pyplot as plt
import csv
import sys
import threading
from watchdog.observers import Observer
from watchdog.events import FileSystemEventHandler

# Global figure and axis
fig, ax = plt.subplots()
path = sys.argv[1]


def update_plot():
    """Read CSV file and update the plot."""
    x, y = [], []
    try:
        with open(path, 'r') as csvfile:
            plots = csv.reader(csvfile, delimiter=',')
            for row in plots:
                if len(row) > 1:
                    x.append(row[0])
                    y.append(float(row[1]))
    except Exception as e:
        print("Error reading file:", e)
        return

    # Clear and replot the updated data.
    ax.clear()
    ax.plot(x, y, color='g', label="Value")
    ax.set_xlabel('Sample')
    ax.set_ylabel('Value')
    ax.set_title(path)
    ax.legend()

    # Set x-ticks (example: every 1000th)
    tick_step = 1000
    ax.set_xticks(range(0, len(x), tick_step))
    ax.set_xticklabels(x[::tick_step], rotation=45)

    # Redraw the canvas
    fig.canvas.draw_idle()

class CSVFileEventHandler(FileSystemEventHandler):
    """Event handler that triggers an update when the file changes."""
    def on_modified(self, event):
        if event.src_path.endswith(path):
            print("File updated. Refreshing plot...")
            update_plot()

# Set up the watchdog observer.
observer = Observer()
event_handler = CSVFileEventHandler()
# Monitor the current directory (change if needed)
observer.schedule(event_handler, path=".", recursive=False)

# Running the observer in a background thread.
observer_thread = threading.Thread(target=observer.start)
observer_thread.daemon = True
observer_thread.start()

# Initial plot update.
update_plot()

# Show the plot (this call blocks until the plot window is closed)
plt.show()

# Cleanly stop the observer after closing the plot.
observer.stop()
observer.join()

