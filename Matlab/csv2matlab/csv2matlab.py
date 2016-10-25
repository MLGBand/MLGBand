import scipy.io
import numpy as np
import csv
import sys

if len(sys.argv) == 3:
    source_path = sys.argv[1]
    destination_path = sys.argv[2]
else:
    print "Invalid number of arguments.\nUsage: python csv2mat.py [source csv file] [destination matlab file]"
    exit()

csvFile = open(source_path, 'rb')
reader = csv.reader(csvFile)

data_list = list(reader)

numpy_list = np.zeros((len(data_list),len(data_list[1])), dtype=np.object)

i = 0
j = 0
# Do remainder of items
for item in data_list:
    j = 0
    for index in item:
        try:
            numpy_list[i, j] = float(index)
        except ValueError:
            numpy_list[i, j] = index
        j += 1
    i += 1

scipy.io.savemat(destination_path, mdict={"data": numpy_list})

#print data_list
