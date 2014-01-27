using System.ComponentModel.Composition;
using System.IO;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    [Export(typeof(IFileNester))]
    internal class AddedExtensionNester : IFileNester
    {
        public bool Nest(string fileName)
        {
            string trimmed = Path.GetFileNameWithoutExtension(fileName);
            ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(trimmed);

            if (item != null)
            {
                return item.ProjectItems.AddFromFile(fileName) != null;
            }

            return true;
        }
    }
}
