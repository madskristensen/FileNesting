using System;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class VsDocNester : IFileNester
    {
        public NestingResult Nest(string fileName)
        {
            if (!fileName.EndsWith("-vsdoc.js", StringComparison.OrdinalIgnoreCase))
                return NestingResult.Continue;

            string parent = fileName.Replace("-vsdoc.js", ".js");
            ProjectItem item = VSPackage.DTE.Solution.FindProjectItem(parent);

            if (item != null)
            {
                item.ProjectItems.AddFromFile(fileName);
                return NestingResult.StopProcessing;
            }

            return NestingResult.Continue;
        }


        public bool IsEnabled()
        {
            return VSPackage.Options.EnableVsDocRule;
        }
    }
}
