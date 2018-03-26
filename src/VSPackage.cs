using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Task = System.Threading.Tasks.Task;

namespace MadsKristensen.FileNesting
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidFileNestingPkgString)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasSingleProject, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideAutoLoad(UIContextGuids80.SolutionHasMultipleProjects, PackageAutoLoadFlags.BackgroundLoad)]
    [ProvideOptionPage(typeof(NestingOptions), Vsix.Name, "General", 101, 100, true, new[] { "File Nesting in Solution Explorer" })]
    public sealed class FileNestingPackage : AsyncPackage
    {
        public static DTE2 DTE { get; private set; }
        public static NestingOptions Options { get; private set; }

        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            DTE = await GetServiceAsync(typeof(DTE)) as DTE2;
            FileNestingFactory.Enable(DTE);

            await JoinableTaskFactory.SwitchToMainThreadAsync();
            await Logger.Initialize(this, Vsix.Name);

            if (GetService(typeof(IMenuCommandService)) is OleMenuCommandService mcs)
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

            Options = (NestingOptions)GetDialogPage(typeof(NestingOptions));
            FileNestingFactory.Enabled = true;
        }

        private void ShowMenu(object sender, EventArgs e)
        {
            OleMenuCommand menu = (OleMenuCommand)sender;
            ProjectItem item = Helpers.GetSelectedItems().FirstOrDefault();

            // We need to exclude website projects, since they don't support file nesting
            menu.Visible = item != null &&
                           !(item.ContainingProject.Kind.Equals("{E24C65DC-7377-472B-9ABA-BC803B73C61A}", StringComparison.OrdinalIgnoreCase)) &&
                           !item.ContainingProject.Kind.Equals("{8BB2217D-0F2D-49D1-97BC-3654ED321F3B}", StringComparison.OrdinalIgnoreCase); // ASP.NET 5
        }
    }
}