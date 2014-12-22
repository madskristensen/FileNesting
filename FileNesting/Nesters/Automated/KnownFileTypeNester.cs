using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class KnownFileTypeNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".js", new [] {".coffee", ".iced", ".ts", ".bundle", ".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".css", new [] {".less", ".scss", ".sass", ".bundle", ".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".ts", new [] {".html", ".cshtml", ".vbhtml", ".aspx", ".master", ".ascx"}},
            {".html", new [] {".bundle"}},
            {".map", new [] {".js", ".css"}},
        };

        public NestingResult Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_mapping.ContainsKey(extension))
                return NestingResult.Continue;

            foreach (string ext in _mapping[extension])
            {
                string parent = Path.ChangeExtension(fileName, ext);
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
            return FileNestingPackage.Options.EnableKnownFileTypeRule;
        }
    }
}
