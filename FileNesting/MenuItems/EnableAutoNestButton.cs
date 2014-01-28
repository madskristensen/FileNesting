using System;
using System.ComponentModel.Design;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    static class EnableAutoNestButton
    {
        private static DTE2 _dte;
        
        public static void Register(DTE2 dte, MenuCommandService mcs)
        {
            _dte = dte;
            CommandID autoId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.cmdAutoNesting);
            OleMenuCommand menuAuto = new OleMenuCommand(AutoNest, autoId);
            mcs.AddCommand(menuAuto);
            menuAuto.BeforeQueryStatus += BeforeAutoNest;
        }

        private static void BeforeAutoNest(object sender, EventArgs e)
        {
            var button = (OleMenuCommand)sender;
            button.Checked = FileNestingPackage.Options.EnableAutoNesting;
        }

        private static void AutoNest(object sender, EventArgs e)
        {
            var isEnabled = FileNestingPackage.Options.EnableAutoNesting;

            _dte.StatusBar.Text = "Automatic file nesting " + (isEnabled ? "disabled" : "enabled");

            FileNestingPackage.Options.EnableAutoNesting = !isEnabled;
        }
    }
}