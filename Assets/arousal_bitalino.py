import time
from bitalino import BITalino
import numpy as np
import matplotlib.pyplot as plt
from scipy.signal import find_peaks
import cmd
from threading import Thread
import os


class HeartRate(cmd.Cmd):
    # create the device
    device = BITalino("00:21:06:BE:15:D9")

    # TO SET
    # collect for n secs
    running_time = 5
    # look for peak greater than n
    height = 600

    # DEFAULT
    # number of sampling
    sampling = 1000
    # if the capture is currently running
    running = False
    # peak per minute
    bpm = 60

    # COMPUTED
    # group capture by group of size n
    nSamples = samplig * running_time
    # nb sample per seconds (min 10 otherwise parameter error)
    samplingRate = sampling
    peaks_in_n_sec = (bpm / 60) * running_time
    intro = "Hello"
    prompt = "|> "

    def __init__(self):
        self.device.battery(30)
        super().__init__()

    def do_start(self, inp):
        self.running = True
        self.device.start(self.samplingRate, [1])
        Thread(target=self.continuous_capture).start()

    def do_stop(self, inp):
        self.running = False
        time.sleep(5)
        self.device.stop()
        print("Sampling stopped")

    def continuous_capture(self):
        while (self.running):
            self.capture()

    def capture(self):
        start = time.time()
        end = time.time()
        while (end-start) < self.running_time and self.running:
            x = self.device.read(self.nSamples)[:, 5]
            end = time.time()

        peaks, _ = find_peaks(x, height=self.height)
        new_capture = len(peaks)
        speed_weight = new_capture / self.peaks_in_n_sec
        file_path = "value.txt"
        try:
            with open(file_path, "w") as f:
                f.write(str(speed_weight))
                print(speed_weight)
        except IOError:
            pass

    def do_callibrate(self, inp):
        self.device.start(self.samplingRate, [1])
        # set the callibration time
        if inp == "":
            seconds = 60
        else:
            seconds = int(inp)

        # start the data collection
        start = time.time()
        end = time.time()
        print("Start callibration \n please wait a few seconds")
        while (end - start) < seconds:
            x = self.device.read(self.sampling * seconds)[:, 5]
            end = time.time()
        # stop the device without closing the connection
        self.device.stop()

        peaks, _ = find_peaks(x, height=self.height)
        self.peaks_in_n_sec = (len(peaks) / seconds) * self.running_time
        self.bpm = (len(peaks) / seconds) * 60
        self.do_status("")
        plt.plot(x)
        plt.plot(peaks, x[peaks], x)
        plt.plot(np.zeros_like(x), "--", color="gray")
        plt.axhline(x=self.height, color="r", linestyle="--")
        plt.show()

    def do_height(self, inp):
        self.height = int(inp)

    def do_running_time(self, inp):
        self.running_time = int(inp)
        self.nSamples = self.sampling * self.running_time
        self.peaks_in_n_sec = (self.bpm / 60) * self.running_time

    def do_cls(self, inp):
        os.system("cls")

    def do_status(self, inp):
        print("height: ", self.height)
        print("bpm: ", self.bpm)
        print("Peaks (in {self.running_time} seconds):", self.peaks_in_n_sec)


if __name__ == "__main__":
    HeartRate().cmdloop()
