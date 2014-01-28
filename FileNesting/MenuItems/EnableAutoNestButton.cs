using System;
using System.ComponentModel.Design;
using System.Linq;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    class EnableAutoNestButton
    {
        private static Project _project;

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
            _project = Helpers.GetSelectedProject();

            if (_project == null)
            {
                _project = Helpers.GetSelectedItems().First().ContainingProject;

                if (_project != null)
                    button.Text = "Enable automatic nesting for " + _project.Name;
            }

            button.Checked = _project != null && FileNestingFactory.IsAutoNestEnabled(_project);
        }

        private static void AutoNest(object sender, EventArgs e)
        {
            var isEnabled = FileNestingFactory.IsAutoNestEnabled(_project);

            FileNestingFactory.ToggleAutoNesting(_project, !isEnabled);
        }
    }
}