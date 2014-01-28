using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.VisualStudio;

namespace MadsKristensen.FileNesting.Dialog
{
    public partial class ItemSelector : Window
    {
        public string SelectedFile { get; private set; }

        public ItemSelector(IEnumerable<EnvDTE.ProjectItem> items)
        {
            InitializeComponent();

            var siblings = GetSiblings(items.ElementAt(0));
            
            ddlFiles.ItemsSource = GetSource(siblings, items, new List<string>(), string.Empty);
            ddlFiles.SelectedIndex = 0;
        }

        private IEnumerable<string> GetSource(IEnumerable<EnvDTE.ProjectItem> parents, IEnumerable<EnvDTE.ProjectItem> selected, List<string> paths, string indentation)
        {
            foreach (EnvDTE.ProjectItem item in parents)
            {
                if (!selected.Contains(item))
                    paths.Add(indentation + item.Name);

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
                if (folder.Kind != VSConstants.ItemTypeGuid.PhysicalFolder_string)
                    folder = folder.Collection.Parent as EnvDTE.ProjectItem;
                else
                    break;
            }

            if (folder != null)
                items.AddRange(folder.ProjectItems.Cast<EnvDTE.ProjectItem>());
            else
                items.AddRange(item.ContainingProject.ProjectItems.Cast<EnvDTE.ProjectItem>());

            return items.Where(i => i.Kind == VSConstants.ItemTypeGuid.PhysicalFile_string);
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile = ddlFiles.SelectedItem.ToString();
            DialogResult = true;
            this.Close();
        }
    }
}
