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
        public static string solutionDirectory = null;

        #endregion Properties
    }
}

