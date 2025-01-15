using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGrasshopperPlugIn.PythonConnection.TwinObjects
{
    /// <summary>
    /// A Twin result object is used to transfer result between the (parallel) python thread and the main Grasshopper/C# thread.\n
    /// To share a complex object between the C# and Python codes, write the same class in C# and in Python, with identical properties.\n\n
    /// 
    /// For instance, this TwinResult object is created in Python after processing/reshaping the TwinData\n
    /// The python TwinResult object will be automatically converted into a C# object with the same properties.\n
    /// </summary>
    public class TwinResult
    {
        #region Properties

        public string TypeName { get { return "TwinResult"; } }


        public List<List<double>> Matrix { get; set; } //A matrix 


        #endregion Properties

        #region Constructors


        /// <summary>
        /// Initialize all Properties
        /// </summary>
        private void Init()
        {
            Matrix = new List<List<double>>();
        }


        /// <summary>
        /// Default constructor
        /// </summary>
        public TwinResult()
        {
            Init();
        }


        /// <summary>
        /// Converts a list of lists of doubles to a Grasshopper structure of GH_Numbers.
        /// </summary>
        /// <param name="datalistlist">The list of lists of doubles to convert.</param>
        /// <returns>A GH_Structure containing the converted GH_Numbers.</returns>
        public static GH_Structure<GH_Number> ListListToGH_Struct(List<List<double>> datalistlist)
        {
            GH_Path path;
            int i = 0;
            GH_Structure<GH_Number> res = new GH_Structure<GH_Number>();
            foreach (List<double> datalist in datalistlist)
            {
                path = new GH_Path(i);
                res.AppendRange(datalist.Select(data => new GH_Number(data)), path);
                i++;
            }
            return res;
        }

        #endregion Methods

    }
}
