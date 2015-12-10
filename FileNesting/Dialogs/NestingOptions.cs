using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    public class NestingOptions : DialogPage
    {
        [LocDisplayName("Enable auto-nesting")]
        [Description("Enables automatic nesting when files are added or renamed")]
        [Category("File Nesting")]
        [DefaultValue(true)]
        public bool EnableAutoNesting { get; set; }

        // Rules
        [LocDisplayName("Enable extension rule")]
        [Description("Files with an added extension nests under parent. Example: foo.js.map nests under foo.js")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnableExtensionRule { get; set; }

        [LocDisplayName("Enable path segment rule")]
        [Description("Files with an added path segment nests under parent. Example: foo.min.js nests under foo.js")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnablePathSegmentRule { get; set; }

        [LocDisplayName("Enable .bundle file rule")]
        [Description("Files with .bundle extension will be parent. Example: foo.js.bundle will nest foo.js under it")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnableBundleRule { get; set; }

        [LocDisplayName("Enable .sprite file rule")]
        [Description("Files with .sprite extension will be parent. Example: foo.sprite will nest foo.png under it")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnableSpriteRule { get; set; }

        [LocDisplayName("Enable known file type rule")]
        [Description("Some known file types will be nested. Example: foo.css will nest under foo.less")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnableKnownFileTypeRule { get; set; }

        [LocDisplayName("Enable -vsdoc.js rule")]
        [Description("Nest JavaScript files ending in -vsdoc.js under corresponding .js files")]
        [Category("Nesting rules")]
        [DefaultValue(true)]
        public bool EnableVsDocRule { get; set; }

        [LocDisplayName("Enable interface implementation rule")]
        [Description("Nest C# interface implementations under their respective interfaces (by filename only)")]
        [Category("Nesting rules")]
        [DefaultValue(false)]
        public bool EnableInterfaceImplementationRule { get; set; }
    }
}