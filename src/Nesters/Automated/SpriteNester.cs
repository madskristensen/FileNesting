using System.Collections.Generic;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class SpriteNester : IFileNester
    {
        private static Dictionary<string, string[]> _mapping = new Dictionary<string, string[]>(){
            {".png", new [] {".sprite"}},
            {".jpg", new [] {".sprite"}},
            {".gif", new [] {".sprite"}},
        };

        public NestingResult Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();

            if (!_mapping.ContainsKey(extension))
                return NestingResult.Continue;

            foreach (string ext in _mapping[extension])
            {
                string parent = Path.ChangeExtension(fileName, ext);
                ProjectItem item = VSPackage.DTE.Solution.FindProjectItem(parent);

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
            return VSPackage.Options.EnableSpriteRule;
        }
    }
}
