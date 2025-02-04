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
using System.Diagnostics;
using System.IO;
using System.Text;
using Grasshopper.Kernel;
using Rhino.Geometry;
using PythonConnect;
using System.ComponentModel;
using System.Reflection;
using log4net.Core;

namespace MyGrasshopperPlugIn.PythonConnection.Components
{
    public class StartPythonComponent : GH_Component
    {

        public event GH_DocumentServer.DocumentRemovedEventHandler DocumentRemovedEvent; //does not seem to work ! 

        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public StartPythonComponent()
          : base("StartPythonComponent", "Python",
              "Initialize Python to be able to run any calculation",
              "MyGrasshopperPlugIn", "0.")
        {
            DocumentRemovedEvent += DocumentClose;
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("User mode", "user", "true for user mode, false for developer mode.", GH_ParamAccess.item, true);
            pManager[0].Optional = true;
            pManager.AddBooleanParameter("Start Python", "Start", "Connect here a toggle. If true, Python/Anaconda starts and can calculate.", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Level", "lvl", "Level of messages in the Log file for debbuging: choose the level from the provided list", GH_ParamAccess.item);
            pManager[2].Optional = true;
            pManager.AddIntegerParameter("TimeOut", "timeout", "This is the time (in milliseconds) a python script will be allowed to run before it is aborted.", GH_ParamAccess.item, 10000);
            pManager[3].Optional = true;

            var levels = pManager[2] as Grasshopper.Kernel.Parameters.Param_Integer;
            levels.AddNamedValue(Level.Debug.DisplayName, Level.Debug.Value);
            levels.AddNamedValue(Level.Info.DisplayName, Level.Info.Value);
            levels.AddNamedValue(Level.Warn.DisplayName, Level.Warn.Value);
            levels.AddNamedValue(Level.Error.DisplayName, Level.Error.Value);
            levels.AddNamedValue(Level.Fatal.DisplayName, Level.Fatal.Value);
            levels.AddNamedValue(Level.Off.DisplayName, Level.Off.Value);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //1) Collect Data
            bool user_mode = true;
            bool start = false;
            int logLvl = Level.Off.Value;

            int timeout = 10000; //10 seconds. This is the time a python script will be allowed to run before it is killed.
            
            if (!DA.GetData(0, ref user_mode)) return;
            if (!DA.GetData(1, ref start)) return;
            if (!DA.GetData(2, ref logLvl)) return;
            if (!DA.GetData(3, ref timeout)) return;

            //2) Process Data. Is it valid ?
            AccessToAll.user_mode = user_mode;
            List<int> listOfValidLevelValues = new List<int> { Level.Off.Value, Level.Fatal.Value, Level.Error.Value, Level.Warn.Value, Level.Info.Value, Level.Debug.Value };

            if (!listOfValidLevelValues.Contains(logLvl))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Please choose a valid LogLevel from the provided input list");
                logLvl = Level.Off.Value;
            }
            int index = listOfValidLevelValues.IndexOf(logLvl);
            List<Level> listOfValidLevel = new List<Level> { Level.Off, Level.Fatal, Level.Error, Level.Warn, Level.Info, Level.Debug };
            Level currentLevel = listOfValidLevel[index];

            if (timeout <= 0)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Timeout must be greater than 0");
                timeout = 10000;
            }


            //3) Initialize Python (and log4net for debugging)

            if (start && AccessToAll.pythonManager != null) //already started
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There is already one Anaconda ready to perform on the Canvas.");
                return;
            }

            if (start && AccessToAll.pythonManager == null) //start Anaconda
            {
                string solutionDirectory;
                string pythonProjectDirectory;
                string pathToActivateConda;

                if (user_mode)
                {
                    DirectoryInfo specialFolder = SpecialFolder();
                    solutionDirectory = Path.Combine(specialFolder.FullName, "MyGrasshopperPlugIn");
                    //retrieve path to anaconda from the PathToAnaconda.txt file
                    pathToActivateConda = PathToAnaconda(solutionDirectory);
                }
                else // in case of working in debug/developer mode
                {
                    pathToActivateConda = @"C:\Users\Jonas\anaconda3\Scripts\activate.bat"; //overwrite your own path to anaconda
                    solutionDirectory = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.Parent.FullName; // new DirectoryInfo(Directory.GetCurrentDirectory()) return @"...\Rhino.Template.CSharpPython\Rhino.Template.CSharpPython\bin\Debug\net48" where PythonConnect.TestConsole.exe is located. 
                }
                //define path to the python project
                pythonProjectDirectory = Path.Combine(solutionDirectory, "MyPythonScripts");

                if (string.IsNullOrEmpty(pythonProjectDirectory))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to retrieve python scripts\n Check your path \\AppData\\Roaming\\Grasshopper\\Libraries\\MyGrasshopperPlugIn\\MyPythonScripts");
                    return;
                }
                if (string.IsNullOrEmpty(pathToActivateConda))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to retrieve Anaconda script: activate.bat\n Check that PathToAnaconda is well configured in \\AppData\\Roaming\\Grasshopper\\Libraries\\MyGrasshopperPlugIn\\PathToAnaconda.txt");
                    return;
                }

                AccessToAll.projectDirectory = solutionDirectory;

                // Set up the logger with a desired level
                LogHelper.Setup(currentLevel.DisplayName, AccessToAll.projectDirectory);
                var log = LogHelper.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

                // Start Anaconda
                PythonManager.Setup(timeout, pathToActivateConda, pythonProjectDirectory);
                AccessToAll.pythonManager = PythonManager.Instance;

                if (AccessToAll.pythonManager != null)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Remark, "Anaconda is ready to perform.");
                }
                else
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to start Anaconda.");
                }
                return;
            }

            if (!start && AccessToAll.pythonManager != null) //stop Anaconda
            {
                AccessToAll.pythonManager.Dispose();
                AccessToAll.pythonManager = null;
            }

            if (AccessToAll.pythonManager == null) 
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Anaconda stopped. Please relaunch it.");
                return;
            }
        }

        /// <summary>
        /// When Grasshopper is closed, stop the pythonManager
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="doc"></param>
        private void DocumentClose(GH_DocumentServer sender, GH_Document doc)
        {
            if (AccessToAll.pythonManager != null)
            {
                AccessToAll.pythonManager.Dispose();
                AccessToAll.pythonManager = null;
            }
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
            get { return new Guid("ad3f8b77-9942-471e-aa12-aebc7c2b075a"); }
        }

        /// <summary>
        /// return the directory with path : "C:\\Users\\Me\\AppData\\Roaming\\Grasshopper\\Libraries\\"
        /// </summary>
        /// <returns></returns>
        private DirectoryInfo SpecialFolder()
        {
            foreach (GH_AssemblyFolderInfo dir in Grasshopper.Folders.AssemblyFolders)
            {
                if (dir.Folder.Contains("\\AppData\\Roaming\\Grasshopper\\Libraries"))
                {
                    return new DirectoryInfo(dir.Folder);
                }
            }
            return null;
        }

        private string PathToAnaconda(string solutionFolder)
        {
            string file = "PathToAnaconda.txt";
            try
            {
                string fullPathToFile = Path.Combine(solutionFolder, file);
                foreach (var line in File.ReadAllLines(fullPathToFile))
                {
                    if (line.Contains("Users\\Jonas"))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Please configure the path to anaconda in your special folder: \\AppData\\Roaming\\Grasshopper\\Libraries\\MyGrasshopperPlugIn\\PathToAnaconda.txt");
                    }
                    if (line.Contains("activate.bat") || line.Contains("activate"))
                    {
                        return line;
                    }
                }
                return null;
            }
            catch
            {
                return null;
            }
        }

    }

}
