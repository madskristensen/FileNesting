// Guids.cs
// MUST match guids.h
using System;

namespace MadsKristensen.FileNesting
{
    static class GuidList
    {
        public const string guidFileNestingPkgString = "6c799bc4-0d4c-4172-98bc-5d464b612dca";
        public const string guidFileNestingCmdSetString = "a5bb8f41-d79a-4de2-bd13-857f39dd0f3f";

        public static readonly Guid guidFileNestingCmdSet = new Guid(guidFileNestingCmdSetString);
    };

    static class PkgCmdIDList
    {
        public const uint Nestingmenu = 0x1000;
        public const uint cmdUnNest = 0x1030;
        public const uint cmdNest = 0x1040;
        public const uint cmdAutoNesting = 0x01060;
        public const uint cmdRunNesting = 0x01070;
    };
}