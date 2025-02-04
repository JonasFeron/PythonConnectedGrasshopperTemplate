//This file is imported without modification from the official Grasshopper Template:
//https://github.com/mcneel/RhinoVisualStudioExtensions/releases

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