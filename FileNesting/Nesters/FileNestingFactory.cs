using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;

namespace MadsKristensen.FileNesting
{
    static class FileNestingFactory
    {
        private static IEnumerable<Lazy<IFileNester>> _nesters;
        private static ProjectItemsEvents _events;
        public static bool Enabled { get; set; }

        public static void Enable(DTE2 dte)
        {
            if (_events == null)
            {
                _events = ((Events2)dte.Events).ProjectItemsEvents;
                _events.ItemAdded += ItemAdded;
                _events.ItemRenamed += ItemRenamed;
            }
        }

        private static void ItemRenamed(ProjectItem item, string OldName)
        {
            ItemAdded(item);
        }

        private static void ItemAdded(ProjectItem item)
        {
            if (FileNestingPackage.Options.EnableAutoNesting && item.Properties != null)
            {
                ProjectItem parent = item.Collection.Parent as ProjectItem;

                if (parent == null || parent.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFile_string, StringComparison.OrdinalIgnoreCase))
                    RunNesting(item);
            }
        }

        public static void RunNesting(ProjectItem item)
        {
            if (!Enabled)
                return;

            EnsureImports();

            foreach (var nester in _nesters.Where(n => n.Value.IsEnabled()))
            {
                NestingResult result = nester.Value.Nest(item.FileNames[0]);

                if (result == NestingResult.StopProcessing)
                    break;
            }
        }

        private static void EnsureImports()
        {
            if (_nesters == null)
                _nesters = Microsoft.Web.Editor.ComponentLocatorWithOrdering<IFileNester>.ImportMany();
        }
    }
}
