import json
import numpy as np

class TwinResult():
    def __init__(self):
        """
        initialise empty TwinResult
        """
        self.TypeName = "TwinResult"
        self.Matrix = []

    def populate_with(self, array):
        if isinstance(array, np.ndarray):
            self.Matrix = array.tolist()



class TwinResultEncoder(json.JSONEncoder):
    """
    The TwinResultEncoder class is used to record all the properties of a TwinResult object in a dictionary and send them to C#.
    """
    def default(self, obj):
        if isinstance(obj, TwinResult):
            return obj.__dict__ # obj.__dct__ = {'property': value, ...}
        else : # Let the base class default method raise the TypeError
            return json.JSONEncoder.default(self, obj)
