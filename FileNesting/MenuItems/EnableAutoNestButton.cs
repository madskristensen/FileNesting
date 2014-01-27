using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    class EnableAutoNestButton
    {
        public static void Register(DTE2 dte, OleMenuCommandService mcs)
        {
            CommandID autoId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.cmdAutoNesting);
            OleMenuCommand menuAuto = new OleMenuCommand(AutoNest, autoId);
            mcs.AddCommand(menuAuto);
            menuAuto.BeforeQueryStatus += BeforeAutoNest;
        }

        private static void BeforeAutoNest(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            var project = Helpers.GetSelectedProject();

            if (project == null)
            {
                project = Helpers.GetSelectedItems().First(i => i.ContainingProject != null).ContainingProject;
                button.Text = "Enable automatic nesting for " + project.Name;
            }

            button.Checked = project != null && FileNestingFactory.IsAutoNestEnabled(project);
        }

        private static void AutoNest(object sender, EventArgs e)
        {
            var project = Helpers.GetSelectedProject();
            var isEnabled = FileNestingFactory.IsAutoNestEnabled(project);

            FileNestingFactory.SetAutoNesting(project, !isEnabled);
        }
    }
}