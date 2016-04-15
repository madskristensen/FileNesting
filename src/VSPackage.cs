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
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidFileNestingPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    [ProvideOptionPage(typeof(NestingOptions), "File Nesting", "General", 101, 100, true, new[] { "File Nesting in Solution Explorer" })]
    public sealed class VSPackage : Package
    {
        public static DTE2 DTE { get; private set; }
        public static NestingOptions Options { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            DTE = GetService(typeof(DTE)) as DTE2;

            Logger.Initialize(this, Vsix.Name, Vsix.Version, "2e1b5120-2d5c-4324-82a3-ac73aafad731");
            FileNestingFactory.Enable(DTE);

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID commandId = new CommandID(PackageGuids.guidFileNestingCmdSet, PackageIds.NestingMenu);
                OleMenuCommand menuCommand = new OleMenuCommand((s, e) => { }, commandId);
                menuCommand.BeforeQueryStatus += ShowMenu;
                mcs.AddCommand(menuCommand);

                UnNestButton.Register(mcs);
                NestButton.Register(mcs);
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

            // We need to exclude website projects, since they don't support file nesting
            menu.Visible = item != null &&
                           !(item.ContainingProject.Object is VsWebSite.VSWebSite) &&
                           !item.ContainingProject.Kind.Equals("{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}", StringComparison.OrdinalIgnoreCase); // ASP.NET 5
        }
    }
}