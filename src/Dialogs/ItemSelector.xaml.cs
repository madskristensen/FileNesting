using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio;

namespace MadsKristensen.FileNesting
{
    public partial class ItemSelector : Window
    {
        public string SelectedFile { get; private set; }
        private IDictionary<string, string> _files;

        public ItemSelector(IEnumerable<EnvDTE.ProjectItem> items)
        {
            InitializeComponent();

            ddlFiles.Focus();

            EnvDTE.ProjectItem currentItem = items.ElementAt(0);

            var siblings = ItemSelector.GetSiblings(currentItem);
            _files = this.GetSource(siblings, items, new Dictionary<string, string>(), string.Empty);
            ddlFiles.ItemsSource = _files.Keys;
            var index = GetMatchParentIndex(currentItem.Name, _files.Keys.ToArray());
            ddlFiles.SelectedIndex = index;
        }

        private int GetMatchParentIndex(string currentFileName, string[] allFiles)
        {
            int maxFileIndex = 0;
            int maxFileEqualCharCount = 0;

            for (int i = 0; i < allFiles.Length; i++)
            {
                var fileNameInList = allFiles[i];
                var count = GetEqualCharCount(currentFileName , fileNameInList);
                if (count <= maxFileEqualCharCount) continue;
                maxFileEqualCharCount = count;
                maxFileIndex = i;
            }
            return maxFileIndex;
        }

        private int GetEqualCharCount(string currentFileName, string fileNameInList)
        {
            return fileNameInList.TakeWhile((t, i) => i < currentFileName.Length).TakeWhile((t, i) => currentFileName[i] == t).Count();
        }

        private IDictionary<string, string> GetSource(IEnumerable<EnvDTE.ProjectItem> parents, IEnumerable<EnvDTE.ProjectItem> selected, Dictionary<string, string> paths, string indentation)
        {
            foreach (EnvDTE.ProjectItem item in parents)
            {
                if (!selected.Contains(item))
                {
                    string path = indentation + item.Name;

                    if (!paths.ContainsKey(path))
                        paths.Add(path, item.FileNames[0]);
                }

                GetSource(item.ProjectItems.Cast<EnvDTE.ProjectItem>(), selected, paths, indentation + "    ");
            }

            return paths;
        }

        private static IEnumerable<EnvDTE.ProjectItem> GetSiblings(EnvDTE.ProjectItem item)
        {
            EnvDTE.ProjectItem folder = item.Collection.Parent as EnvDTE.ProjectItem;
            List<EnvDTE.ProjectItem> items = new List<EnvDTE.ProjectItem>();

            while (folder != null)
            {
              if (!folder.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFolder_string, System.StringComparison.OrdinalIgnoreCase))
              {
                folder = folder.Collection.Parent as EnvDTE.ProjectItem;
              }
              else
              {
                break;
              }
            }

            if (folder != null)
            {
              items.AddRange(folder.ProjectItems.Cast<EnvDTE.ProjectItem>());
            }
            else if (item.ContainingProject != null && item.ContainingProject.ProjectItems != null)
            {
              items.AddRange(item.ContainingProject.ProjectItems.Cast<EnvDTE.ProjectItem>());
            }

            return items.Where(i => i.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFile_string, System.StringComparison.OrdinalIgnoreCase));
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile = _files[ddlFiles.SelectedItem.ToString()];
            DialogResult = true;
            this.Close();
        }
    }
}
