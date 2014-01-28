using System.ComponentModel;
using Microsoft.VisualStudio.Shell;

namespace MadsKristensen.FileNesting
{
    public class NestingOptions : DialogPage
    {
        private bool _enableAutoNesting = false;
        private bool _enableExtensionRule = true;
        private bool _enablePathSegmentRule = true;
        private bool _enableBundleRule = true;
        private bool _enableSpriteRule = true;
        private bool _enableKnownFileTypeRule = true;

        [LocDisplayName("Enable auto-nesting")]
        [Description("Enables automatic nesting when files are added or renamed")]
        [Category("File Nesting")]
        public bool EnableAutoNesting
        {
            get { return _enableAutoNesting; }
            set { _enableAutoNesting = value; }
        }

        // Rules
        [LocDisplayName("Enable extention rule")]
        [Description("Files with an added extension nests under parent. Example: foo.js.map nests under foo.js")]
        [Category("Nesting rules")]
        public bool EnableExtensionRule
        {
            get { return _enableExtensionRule; }
            set { _enableExtensionRule = value; }
        }

        [LocDisplayName("Enable path segment rule")]
        [Description("Files with an added path segment nests under parent. Example: foo.min.js nests under foo.js")]
        [Category("Nesting rules")]
        public bool EnablePathSegmentRule
        {
            get { return _enablePathSegmentRule; }
            set { _enableExtensionRule = value; }
        }

        [LocDisplayName("Enable .bundle file rule")]
        [Description("Files with .bundle extension will be parent. Example: foo.js.bundle will nest foo.js under it")]
        [Category("Nesting rules")]
        public bool EnableBundleRule
        {
            get { return _enableBundleRule; }
            set { _enableBundleRule = value; }
        }

        [LocDisplayName("Enable .sprite file rule")]
        [Description("Files with .sprite extension will be parent. Example: foo.sprite will nest foo.png under it")]
        [Category("Nesting rules")]
        public bool EnableSpriteRule
        {
            get { return _enableSpriteRule; }
            set { _enableSpriteRule = value; }
        }

        [LocDisplayName("Enable known file type rule")]
        [Description("Some known file types will be nestd. Example: foo.css will nested under foo.less")]
        [Category("Nesting rules")]
        public bool EnableKnownFileTypeRule
        {
            get { return _enableKnownFileTypeRule; }
            set { _enableKnownFileTypeRule = value; }
        }
    }
}
