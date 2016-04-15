using System.IO;
using System.Linq;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class PathSegmentNester : IFileNester
    {
        public NestingResult Nest(string fileName)
        {
            if (!IsSupported(fileName))
                return NestingResult.Continue;

            string name = Path.GetFileNameWithoutExtension(fileName);

            int index = name.LastIndexOf('.');
            if (index > -1)
            {
                string directory = Path.GetDirectoryName(fileName);
                string extension = Path.GetExtension(fileName);
                string firstName = name.Substring(0, index);
                string parentFileName = Path.Combine(directory, firstName + extension);

                ProjectItem parent = VSPackage.DTE.Solution.FindProjectItem(parentFileName);
                if (parent != null)
                {
                    parent.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }

        private static bool IsSupported(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string[] allowed = new[] { ".js", ".css", ".html", ".htm", ".less", ".scss", ".coffee", ".iced", ".config", ".cs", "vb", ".sql" };

            return allowed.Contains(extension);
        }

        public bool IsEnabled()
        {
            return VSPackage.Options.EnablePathSegmentRule;
        }
    }
}
