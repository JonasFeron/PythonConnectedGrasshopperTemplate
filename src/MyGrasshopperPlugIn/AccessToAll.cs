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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonConnect;

namespace MyGrasshopperPlugIn
{

    /// <summary>
    /// the class AccessToAll contains the properties and methods accessible from all Grasshopper components.
    /// </summary>
    public static class AccessToAll
    {
        #region Properties

        /// <summary>
        /// Gets or sets the PythonManager for the current Canvas. 
        /// A PythonManager manages the communication between the Python scripts and the Grasshopper component.
        /// </summary>
        public static PythonManager pythonManager = null;

        /// <summary>
        /// Gets or sets a value indicating whether the plugin is in user mode.
        /// True for user mode, false for developer mode.
        /// </summary>
        public static bool user_mode = true;

        /// <summary>
        /// Gets or sets the solution directory path.
        /// </summary>
        public static string projectDirectory = null;

        #endregion Properties
    }
}

