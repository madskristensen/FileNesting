using System.Collections.Generic;
using System.IO;
using EnvDTE;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.FileNesting
{
    [Name("Bundle Nester")]
    [Order(Before = "Added Extension Nester")]
    internal class BundleNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".js", new [] {".bundle"}},
            {".css", new [] {".bundle"}},
            {".html", new [] {".bundle"}},
        };

        public NestingResult Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_mapping.ContainsKey(extension))
                return NestingResult.Continue;

            foreach (string ext in _mapping[extension])
            {
                string parent = fileName + ext;
                ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(parent);

                if (item != null)
                {
                    item.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }
        
        public bool IsEnabled()
        {
            return FileNestingPackage.Options.EnableBundleRule;
        }
    }
}
