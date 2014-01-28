using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
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
    public sealed class FileNestingPackage : Package
    {
        private static DTE2 _dte;
        
        internal static DTE2 DTE
        {
            get
            {
                if (_dte == null)
                    _dte = ServiceProvider.GlobalProvider.GetService(typeof(DTE)) as DTE2;

                return _dte;
            }
        }

        protected override void Initialize()
        {
            base.Initialize();
            FileNestingFactory.Enable(DTE);

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID commandId = new CommandID(GuidList.guidFileNestingCmdSet, (int)PkgCmdIDList.Nestingmenu);
                OleMenuCommand menuCommand = new OleMenuCommand((s, e) => { }, commandId);
                menuCommand.BeforeQueryStatus += ShowMenu;
                mcs.AddCommand(menuCommand);

                UnNestButton.Register(_dte, mcs);
                NestButton.Register(_dte, mcs);
                EnableAutoNestButton.Register(_dte, mcs);
                RunAutoNestingButton.Register(_dte, mcs);                
            }
        }

        private void ShowMenu(object sender, EventArgs e)
        {
            OleMenuCommand menu = (OleMenuCommand)sender;
            ProjectItem item = Helpers.GetSelectedItems().FirstOrDefault();

            menu.Visible = item != null && !(item.ContainingProject.Object is VsWebSite.VSWebSite);
        }
    }
}