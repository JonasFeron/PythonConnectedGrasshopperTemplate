import numpy as np


class TwinData():
    def __init__(self, TypeName,
                 AList,
                 rowNumber,
                 colNumber): #the Names of the __init__(arguments) must be identical to the Names of the TwinData properties in C#
        """
        Initialize all the properties of a TwinData Object.
        A TwinData Object is an object that contains the same data in C# than in Python in order to communicate between the two languages via a file.txt encrypted in json format.
        """
        self.TypeName = TypeName
        self.AList = np.array(AList, dtype=int)  #input arguments from C# are lists which are converted in numpy.array
        self.rowNumber = rowNumber
        self.colNumber = colNumber

def ToTwinDataObject(dct):
    """
    Function that takes in a dictionary and returns a TwinData object associated to the dict.
    """
    if 'TwinData' in dct.values(): # if TypeName == 'TwinData':
        return TwinData(**dct) #call the constructor of TwinData with all the values of the dictionnary.
    return dct

