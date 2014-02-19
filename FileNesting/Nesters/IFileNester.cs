
namespace MadsKristensen.FileNesting
{
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
