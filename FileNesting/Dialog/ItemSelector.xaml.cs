using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace MadsKristensen.FileNesting.Dialog
{
    /// <summary>
    /// Interaction logic for ItemSelector.xaml
    /// </summary>
    public partial class ItemSelector : Window
    {
        public string SelectedFile { get; private set; }

        public ItemSelector(IEnumerable<EnvDTE.ProjectItem> items)
        {
            InitializeComponent();

            EnvDTE.ProjectItem folder = FindFolder(items.ElementAt(0));
            
            if (folder == null)
                return;
            
            ddlFiles.ItemsSource = GetSource(folder, items, new List<string>(), string.Empty);
            ddlFiles.SelectedIndex = 0;
        }

        private IEnumerable<string> GetSource(EnvDTE.ProjectItem parent, IEnumerable<EnvDTE.ProjectItem> selected, List<string> paths, string indentation)
        {
            foreach (EnvDTE.ProjectItem item in parent.ProjectItems)
            {
                GetSource(item, selected, paths, indentation + "    ");

                if (!selected.Contains(item))
                    paths.Add(indentation + item.Name);
            }

            return paths;
        }

        private EnvDTE.ProjectItem FindFolder(EnvDTE.ProjectItem item)
        {
            while (item.Collection.Parent != null)
            {
                var parent = item.Collection.Parent as EnvDTE.ProjectItem;
                if (parent != null)
                {
                    if (Directory.Exists(parent.Properties.Item("FullPath").Value.ToString()))
                        return parent;
                }
                else
                {
                    break;
                }
            }

            return null;
        }

        private void ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedFile = ddlFiles.SelectedItem.ToString();
            this.Close();
        }
    }
}
