//Copyright <2021-2025> <Université catholique de Louvain (UCLouvain)>

//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at

//    http://www.apache.org/licenses/LICENSE-2.0

//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.

//List of the contributors to the development of PythonConnectedGrasshopperTemplate: see NOTICE file.
//Description and complete License: see NOTICE file.

//this file was imported from the official Grasshopper Template:
//https://github.com/mcneel/RhinoVisualStudioExtensions/releases
//and is used quasi without modification.
//------------------------------------------------------------------------------------------------------------

using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;

namespace MyGrasshopperPlugIn
{
    public class MyGrasshopperPlugInInfo : GH_AssemblyInfo
    {
        public override string Name => "MyGrasshopperPlugIn";

        public override Bitmap Icon => null;

        public override string Description => "";


        // Here provide a new Guid for your plugin !
        public override Guid Id => new Guid("739c1aea-d42a-4109-8a30-8bfbd975ec13");



        public override string AuthorName => "";

        public override string AuthorContact => "";

        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}