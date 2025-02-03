import sys
import json
import Util.InputOutput as IO
import TwinObjects.Twindata as d
import TwinObjects.TwinResult as r

def main():
    """
    This main function is basically the same than in main_TestPythonComponent.py.
    The only difference is: the strings written in the DataFile.txt and ResultFile.txt are formatted in JSON.

    This allows to automatically convert a TwinData object from C# into Python.
    And reversely, a TwinResult object from Python is automatically converted into C#.

    This approach enables effortless integration of Python's powerful features into your C# applications, extending the capabilities of both languages.

    :return: a signal to C# that the python script has been executed.
    The results are stored in a ResultFile.txt of given path "PathTo_ResultFile"
    The ResultFile.txt stores a string that contains all the properties of the TwinResult object using the JSON format.
    """
    key = sys.argv[1] 
    PathTo_DataFile = sys.argv[2]  
    PathTo_ResultFile = sys.argv[3] 
    
    (key_FromDataFile, data_FromDataFile) = IO.ReadDataFile(PathTo_DataFile)  
    output_string = " "
    if key == key_FromDataFile: 
        try:
            output_string = MyFunction(data_FromDataFile[0])

        except Exception as e:
            output_string = str(e)
    else:
        output_string = "Keys from command prompt and Data text file did not match"

    IO.WriteResultFile(PathTo_ResultFile, key, output_string)

    print(key + ":" + PathTo_ResultFile)    



def MyFunction(DataString):
    """
    :param DataString: a string formated in json containing a dictionnary of data coming from C#
    :return: print key:output
    """
    result = r.TwinResult() #initialize empty results

    # the Data read from the data file.txt are stored in TwinData object (identical in python an C#)
    data = json.loads(DataString, object_hook = d.ToTwinDataObject)

    if isinstance(data, d.TwinData):#check that data is a TwinData object !
        
        # Process the data
        # for instance: reshape the array
        array = data.AList.reshape(data.rowNumber, data.colNumber)
        
        #register the results
        result.populateWith(array)

    # Results are saved as dictionnary JSON.
    output_dct = json.dumps(result, cls=r.TwinResultEncoder, ensure_ascii=False)  # , indent="\t"
    return output_dct

if __name__ == '__main__': #what happens if called from C#
    main()
