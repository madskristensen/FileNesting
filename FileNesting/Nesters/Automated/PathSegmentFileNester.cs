using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Utilities;

namespace MadsKristensen.FileNesting.Nesters
{
    [Export(typeof(IFileNester))]
    [Name("Path Segment Nester")]
    [Order(Before = "Added Extension Nester")]
    internal class PathSegmentNester : IFileNester
    {
        public NestingResult Nest(string fileName)
        {
            if (!IsSupported(fileName))
                return NestingResult.Continue;

            string name = Path.GetFileNameWithoutExtension(fileName);
            ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(fileName);

            int index = name.LastIndexOf('.');
            if (index > -1)
            {                
                string directory = Path.GetDirectoryName(fileName);
                string extension = Path.GetExtension(fileName);
                string firstName = name.Substring(0, index);
                string parentFileName = Path.Combine(directory, firstName + extension);

                ProjectItem parent = FileNestingPackage.DTE.Solution.FindProjectItem(parentFileName);
                if (parent != null)
                {
                    parent.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }

        private bool IsSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string[] allowed = new[] { ".js", ".css", ".html", ".htm", ".less", ".scss", ".coffee", ".iced", ".config" };

            return allowed.Contains(extension);
        }
    }
}
