using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
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
                UnNestButton.Register(_dte, mcs);
                NestButton.Register(_dte, mcs);
                EnableAutoNestButton.Register(_dte, mcs);
                RunAutoNestingButton.Register(_dte, mcs);                
            }
        }
    }
}