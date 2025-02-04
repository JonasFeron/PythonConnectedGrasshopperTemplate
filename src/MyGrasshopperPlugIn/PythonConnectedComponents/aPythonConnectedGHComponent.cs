//Copyright < 2021 - 2025 > < Université catholique de Louvain (UCLouvain)>

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

//List of the contributors to the development of PythonConnectedGrasshopperTemplate: see NOTICE file.
//Description and complete License: see NOTICE file.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using MyGrasshopperPlugIn;
using MyGrasshopperPlugIn.PythonConnectedComponents.TwinObjects;
using Newtonsoft.Json;
using PythonConnect;
using Rhino.Commands;
using Rhino.Geometry;

namespace MyGrasshopperPlugIn.PythonConnectedComponents
{
    public class aPythonConnectedGHComponent : GH_Component
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public aPythonConnectedGHComponent()
          : base("aPythonConnectedGrasshopperComponent", "DoStuffInPython",
                "This is a component that shows how to transfer complex data between the main Grasshopper/C# thread and the (parallel) python thread.\n" +
                "For instance:\n" +
                "this script takes as input in Grasshopper/C#: a list, a number of columns and a number of rows \n" +
                "These input are sent to python\n" +
                "Python turns the list into a Numpy array of shape (rowNumber,colNumber)\n" +
                "then the Numpy array is returned in C#/Grasshopper as a Tree",
              "MyGrasshopperPlugIn", "1. GHComponents")
        {
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("7e35c61f-3bd0-4e8d-8e37-de5f7e7d363b"); }
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddNumberParameter("A List of Numbers", "list", "A list of numbers to be converted in python into a Numpy array with rowNumber rows and colNumber columns", GH_ParamAccess.list);
            pManager.AddIntegerParameter("Row Number", "rowNumber", "The Number of Rows of the returned Numpy array", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Columns Number", "colNumber", "The Number of Columns of the returned Numpy array", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter("Grasshopper tree", "tree", "A python numpy array converted back into a Grasshopper tree", GH_ParamAccess.tree);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            log.Info("aPythonConnectedGrasshopperComponent.SolveInstance(): NEW CALL To SolveInstance()");

            //1) Collect Data

            List<GH_Number> list = new List<GH_Number>();
            int row = 0;
            int col = 0;

            if (!DA.GetDataList(0, list)) { return; }
            if (!DA.GetData(1, ref row)) { return; }
            if (!DA.GetData(2, ref col)) { return; }


            //2) Solve in python

            if (AccessToAll.pythonManager == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please restart the \"StartPythonComponent\" .");
                DA.SetData(0, null);
                return;
            }

            TwinData twinData = new TwinData(list, row, col); 
            TwinResult twinResult = new TwinResult();
            string pathToDataFile = Path.Combine(AccessToAll.rootDirectory, ".io", "TwinData.txt");
            string pathToResultFile = Path.Combine(AccessToAll.rootDirectory, ".io", "TwinResult.txt");

            if (AccessToAll.pythonManager != null) // run calculation in python by transfering the data base as a string. 
            {
                //convert the TwinData object into a string (with JSON format)
                string dataString = JsonConvert.SerializeObject(twinData, Formatting.None);

                //execute the python script
                log.Debug("aPythonConnectedGrasshopperComponent.SolveInstance(): try executing the python Script");
                string resultString = null;
                resultString = AccessToAll.pythonManager.ExecuteCommand("main_aPythonConnectedGrasshopperComponent.py", pathToDataFile, pathToResultFile, dataString);
                log.Debug("aPythonConnectedGrasshopperComponent.SolveInstance(): the python Script returned: " + resultString);


                //convert the result (contained in a string) into a TwinResult object
                try
                {
                    JsonConvert.PopulateObject(resultString, twinResult); 
                }
                catch
                {

                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Something went wrong while solving: " + resultString);
                    twinResult = null;
                }
            }

            //3) return the result
            List<List<double>> matrix = twinResult.Matrix;
            GH_Structure<GH_Number> gh_structure = TwinResult.ListListToGH_Struct(matrix); //convert the result into a Grasshopper tree

            log.Debug("aPythonConnectedGrasshopperComponent.SolveInstance(): return the results to the Grasshopper Component");
            DA.SetDataTree(0, gh_structure);
            log.Info("aPythonConnectedGrasshopperComponent.SolveInstance(): END SOLVE INSTANCE");
        }

    }
}