using System.ComponentModel.Composition;

namespace MadsKristensen.FileNesting
{
    [InheritedExport(typeof(IFileNester))]
    public interface IFileNester
    {
        NestingResult Nest(string fileName);
        bool IsEnabled();
    }

    public enum NestingResult
    {
        Continue,
        StopProcessing,
    }
}
