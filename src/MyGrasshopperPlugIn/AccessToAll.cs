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
//------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel;
using PythonConnect;

namespace MyGrasshopperPlugIn
{

    /// <summary>
    /// the class AccessToAll contains the properties and methods accessible from all Grasshopper components.
    /// </summary>
    public static class AccessToAll
    {

        private static MyGrasshopperPlugInInfo info = new MyGrasshopperPlugInInfo();

        public static string GHAssemblyName
        {
            get
            {
                return info.Name;
            }
        }
        public static string GHComponentsFolder0 { get { return "0. Initialize Python"; } }
        public static string GHComponentsFolder1 { get { return "1. Main Components"; } } 


        #region Properties

        /// <summary>
        /// Gets or sets the PythonManager for the current Canvas. 
        /// A PythonManager manages the communication between the Python scripts and the Grasshopper component.
        /// </summary>
        public static PythonManager pythonManager = null;


        private static string _anacondaPath = null;

        /// <summary>
        /// Gets the path to the Anaconda installation directory.
        /// </summary>
        /// <remarks>
        /// This property checks for the existence of Anaconda in two possible locations:
        /// 1. The user's profile directory (e.g., "C:\Users\Me\Anaconda3")
        /// 2. The ProgramData directory (e.g., "C:\ProgramData\Anaconda3")
        /// If Anaconda is found in either location, the path is returned. Otherwise, null is returned.
        /// </remarks>
        public static string anacondaPath
        {
            get
            {
                if (_anacondaPath != null)
                {
                    return _anacondaPath;
                }
                else
                {
                    string[] possiblePaths = {
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Anaconda3"), // "C:\Users\Me\Anaconda3"
                    @"C:\ProgramData\Anaconda3"
                    };
                    foreach (var path in possiblePaths)
                    {
                        if (Directory.Exists(path))
                        {
                            return path;
                        }
                    }
                    return null;
                }
            }
            set
            {
                if (File.Exists(Path.Combine(value, "python.exe")))
                {
                    _anacondaPath = value;
                }
                else
                {
                    throw new ArgumentException("The specified path does not contain a valid Anaconda3 installation. ");
                }
            }
        }

        private static string _condaActivateScript = null;
        /// <summary>
        /// Gets or sets the path to the conda activate script.
        /// </summary>
        /// <remarks>
        /// This property constructs the path to the conda activate script if it is not already set.
        /// It first checks if the Anaconda path is available. If it is, it constructs the path to the activate.bat script.
        /// If the Anaconda path is not available, it returns null.
        /// </remarks>
        public static string condaActivateScript
        {
            get
            {
                if (anacondaPath is null)
                {
                    return null;
                }
                else
                {
                    _condaActivateScript = Path.Combine(anacondaPath, "Scripts", "activate.bat"); // "C:\Users\Me\Anaconda3\Scripts\activate.bat"
                    return _condaActivateScript;
                }
            }
        }


        private static string _condaEnvName = "base";

        /// <summary>
        /// Gets or sets the name of the conda environment.
        /// </summary>
        public static string condaEnvName
        {
            get
            {
                return _condaEnvName;
            }
            set
            {
                //check if value is a valid environment
                if (value == "base")
                {
                    _condaEnvName = value;
                }
                else if (File.Exists(Path.Combine(anacondaPath, "envs", value, "python.exe")))               
                {
                    _condaEnvName = value;
                }
                else
                {
                    throw new ArgumentException("The specified anaconda environment does not exist.");
                }
            }
        }


        /// <summary>
        /// Gets or sets a value indicating whether the plugin is in user mode.
        /// True for user mode, false for developer mode.
        /// </summary>
        public static bool user_mode = true;

        /// <summary>
        /// Gets the Special Folder with path : "C:\\Users\\Me\\AppData\\Roaming\\Grasshopper\\Libraries\\"
        /// </summary>
        public static string specialFolder
        {
            get 
            { 
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); // AppData = "C:\Users\Me\AppData\Roaming"
                return Path.Combine(AppData,"Grasshopper", "Libraries");
            }
        }

        /// <summary>
        /// Gets the root directory (containing the solution, the python and C# projects, the temporary folder,...).
        /// </summary>
        public static string rootDirectory
        {
            get
            {
                if (user_mode)
                {
                    return Path.Combine(specialFolder, GHAssemblyName);
                }
                else
                {
                    var currentDirectory = Directory.GetCurrentDirectory(); //rootDirectory/MyGrasshopperPlugIn/bin/Debug/net48/
                    for (int i = 0; i < 4; i++) //rootDirectory is 4 levels above the current directory
                    {
                        currentDirectory = Directory.GetParent(currentDirectory).FullName;
                    }
                    return currentDirectory;
                }
            }
        }



        /// <summary>
        /// Gets the python project directory (containing the python scripts,...).
        /// </summary>
        public static string pythonProjectDirectory
        {
            get { return Path.Combine(rootDirectory, "MyPythonScripts"); }
        }

        /// <summary>
        /// Gets the path to activateCondaEnv.bat file. Users must ensure this file is located in pythonProjectDirectory.
        /// </summary>
        /// <remarks>
        /// If activateCondaEnv.bat exists in pythonProjectDirectory, the path is returned. Otherwise, null is returned.
        /// </remarks>
        public static string activateCondaEnvScript
        {
            get
            {
                string path = Path.Combine(pythonProjectDirectory, "activateCondaEnv.bat");
                if (File.Exists(path))
                {
                    return path;
                }
                else return null;
            }
        }


        public static string csharpProjectDirectory
        {
            get { return Path.Combine(rootDirectory, GHAssemblyName); }
        }
        public static string tempDirectory
        {
            get { return Path.Combine(rootDirectory, ".temp"); }
        }

        #endregion Properties




    }
}

