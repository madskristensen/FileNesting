using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    static class UnNestButton
    {
        private static IEnumerable<ProjectItem> _nested;

        public static void Register(MenuCommandService mcs)
        {
            CommandID unNestId = new CommandID(PackageGuids.guidFileNestingCmdSet, PackageIds.cmdUnNest);
            OleMenuCommand menuUnNest = new OleMenuCommand(UnNest, unNestId);
            mcs.AddCommand(menuUnNest);
            menuUnNest.BeforeQueryStatus += BeforeUnNest;
        }

        private static void BeforeUnNest(object sender, EventArgs e)
        {
            _nested = from p in Helpers.GetSelectedItems()
                      let parent = p.Collection.Parent as ProjectItem
                      where parent != null && !parent.Kind.Equals(VSConstants.ItemTypeGuid.PhysicalFolder_string, StringComparison.OrdinalIgnoreCase)
                      select p;

            var button = (OleMenuCommand)sender;
            button.Enabled = _nested.Any();
        }

        private static void UnNest(object sender, EventArgs e)
        {
            FileNestingFactory.Enabled = false;

            foreach (ProjectItem item in _nested)
            {
                ManualNester.UnNest(item);
            }

            FileNestingFactory.Enabled = true;
            Telemetry.TrackEvent("Unnest item");
        }
    }
}