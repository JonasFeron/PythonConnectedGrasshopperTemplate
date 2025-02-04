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
using Grasshopper.Kernel;
using PythonConnect;
using Rhino.Geometry;

namespace MyGrasshopperPlugIn.PythonConnection.Components
{
    public class TestPythonComponent : GH_Component
    {
        private static readonly log4net.ILog log = LogHelper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Initializes a new instance of the TestPythonComponent class.
        /// </summary>
        public TestPythonComponent()
          : base("TestPython", "test",
              "Test to see if python is well working.",
              "MyGrasshopperPlugIn", "0.")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        /// <param name="pManager">The input parameter manager.</param>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("TextToLower", "ToLower", "a test script", GH_ParamAccess.item);
            pManager.AddTextParameter("TextToUpper", "ToUpper", "a test script", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        /// <param name="pManager">The output parameter manager.</param>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Result text", "result", "string0.ToLower + string1.ToUpper", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The data access object for retrieving input and setting output.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            if (AccessToAll.pythonManager == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Please restart the \"StartPythonComponent\" .");
                return;
            }

            string str0 = "";
            string str1 = "";

            if (!DA.GetData(0, ref str0)) { return; }
            if (!DA.GetData(1, ref str1)) { return; }

            string result = null;

            // Set the paths to the files that will contain the data/results.
            string pathToDataFile = Path.Combine(AccessToAll.rootDirectory, ".io", "Data4TestPythonComponent.txt"); // The main C# thread will write the data to the file, and the python thread will read it.
            string pathToResultFile = Path.Combine(AccessToAll.rootDirectory, ".io", "Result4TestPythonComponent.txt"); // The python thread will write the results to the file, and the main C# thread will read it.
            if (AccessToAll.pythonManager != null)
            {
                log.Debug("TestPythonComponent.SolveInstance(): pythonManager exists");

                result = AccessToAll.pythonManager.ExecuteCommand("main_TestPythonComponent.py", pathToDataFile, pathToResultFile, str0, str1);
            }

            DA.SetData(0, result);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("22620f84-d35f-44fb-9562-4e394e068088"); }
        }
    }
}
