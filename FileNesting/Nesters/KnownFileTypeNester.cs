using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    [Export(typeof(IFileNester))]
    internal class KnownFileTypeNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".js", new [] {".coffee", ".iced", ".ts"}},
            {".css", new [] {".less", ".scss", ".sass"}},
        };

        public bool Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_mapping.ContainsKey(extension))
                return false;

            foreach (string ext in _mapping[extension])
            {
                string parent = Path.ChangeExtension(fileName, ext);
                ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(parent);

                if (item != null)
                {
                    return item.ProjectItems.AddFromFile(fileName) != null;
                }
            }

            return true;
        }
    }
}
