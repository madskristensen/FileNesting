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
        private static List<IFileNester> _nesters = new List<IFileNester>()
        {
            new KnownFileTypeNester(),
            new VsDocNester(),
            new BundleNester(),
            new InterfaceImplementationNester(),
            new PathSegmentNester(),
            new SpriteNester(),
            new AddedExtensionNester(),

        };

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
            // Node.js project system doesn't support 'item.Collection'
            if (item.ContainingProject.Kind.Equals("{9092aa53-fb77-4645-b42d-1ccca6bd08bd}", StringComparison.OrdinalIgnoreCase))
                return;

            if (VSPackage.Options != null && VSPackage.Options.EnableAutoNesting && item != null && item.Properties != null)
            {
                try
                {
                    ProjectItem parent = item.Collection.Parent as ProjectItem;

                    if (parent == null || parent.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFile_string, StringComparison.OrdinalIgnoreCase))
                        RunNesting(item);
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
        }

        public static void RunNesting(ProjectItem item)
        {
            if (!Enabled)
                return;

            foreach (var nester in _nesters.Where(n => n.IsEnabled()))
            {
                NestingResult result = nester.Nest(item.FileNames[0]);

                if (result == NestingResult.StopProcessing)
                    break;
            }
        }
    }
}
