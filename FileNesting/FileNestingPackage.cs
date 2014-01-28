using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace MadsKristensen.FileNesting
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidFileNestingPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(NestingOptions), "File Nesting", "General", 101, 100, true, new[] { "File Nesting in Solution Explorer" })]
    public sealed class FileNestingPackage : Package
    {
        public static DTE2 DTE { get; private set; }
        public static NestingOptions Options { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            DTE = GetService(typeof(DTE)) as DTE2;

            FileNestingFactory.Enable(DTE);
            
            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID commandId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.Nestingmenu);
                OleMenuCommand menuCommand = new OleMenuCommand((s, e) => { }, commandId);
                menuCommand.BeforeQueryStatus += ShowMenu;
                mcs.AddCommand(menuCommand);

                UnNestButton.Register(DTE, mcs);
                NestButton.Register(DTE, mcs);
                EnableAutoNestButton.Register(DTE, mcs);
                RunAutoNestingButton.Register(DTE, mcs);
            }

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                Options = (NestingOptions)GetDialogPage(typeof(NestingOptions));
                FileNestingFactory.Enabled = true;

            }), DispatcherPriority.ApplicationIdle, null);
        }

        private void ShowMenu(object sender, EventArgs e)
        {
            OleMenuCommand menu = (OleMenuCommand)sender;
            ProjectItem item = Helpers.GetSelectedItems().FirstOrDefault();

            menu.Visible = item != null && !(item.ContainingProject.Object is VsWebSite.VSWebSite);
        }
    }
}