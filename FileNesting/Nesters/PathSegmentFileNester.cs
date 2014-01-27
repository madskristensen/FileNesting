using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using EnvDTE;

namespace MadsKristensen.FileNesting.Nesters
{
    [Export(typeof(IFileNester))]
    internal class PathSegmentNester : IFileNester
    {
        public bool Nest(string fileName)
        {
            if (!IsSupported(fileName))
                return false;

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
                    return true;
                }
            }

            return false;
        }

        private bool IsSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string[] allowed = new[] { ".js", ".css", ".html", ".htm", ".less", ".scss", ".coffee", ".iced" };

            return allowed.Contains(extension);
        }
    }
}
