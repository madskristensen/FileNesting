using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.Web.Editor;

namespace MadsKristensen.FileNesting
{
    class FileNestingFactory
    {
        private static IEnumerable<Lazy<IFileNester>> _nesters;
        private static ProjectItemsEvents _events;
        public static bool Enabled { get; set; }

        public static void Enable(DTE2 dte)
        {
            if (_nesters == null)
            {
                _nesters = ComponentLocatorWithOrdering<IFileNester>.ImportMany();
                _events = ((Events2)dte.Events).ProjectItemsEvents;
                _events.ItemAdded += ItemAdded;
                _events.ItemRenamed += ItemRenamed;
            }

            Enabled = true;
        }

        private static void ItemRenamed(ProjectItem item, string OldName)
        {
            if (item.Properties != null && !(item.Collection.Parent is ProjectItem))
            {
                RunNesting(item);
            }
        }

        private static void ItemAdded(ProjectItem item)
        {
            if (item.Properties != null && !(item.Collection.Parent is ProjectItem))
            {
                RunNesting(item);
            }
        }

        public static void RunNesting(ProjectItem item)
        {
            if (!Enabled || !IsAutoNestEnabled(item.ContainingProject))
                return;

            string fileName = item.Properties.Item("FullPath").Value.ToString();

            foreach (var nester in _nesters)
            {
                NestingResult result = nester.Value.Nest(fileName);

                if (result == NestingResult.StopProcessing)
                    break;
            }
        }

        public static bool IsAutoNestEnabled(Project project)
        {
            string projectPath = project.FullName;
            Microsoft.Build.Evaluation.Project targetProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault((p) => p.FullPath == projectPath);

            if (targetProject != null)
            {
                var property = targetProject.GetProperty("AutoNestFiles");
                return property != null && property.EvaluatedValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        public static void SetAutoNesting(Project project, bool enabled)
        {
            string projectPath = project.FullName;
            Microsoft.Build.Evaluation.Project targetProject = Microsoft.Build.Evaluation.ProjectCollection.GlobalProjectCollection.LoadedProjects.FirstOrDefault((p) => p.FullPath == projectPath);

            if (targetProject != null)
            {
                targetProject.SetProperty("AutoNestFiles", enabled.ToString().ToLowerInvariant());
            }
        }
    }
}
