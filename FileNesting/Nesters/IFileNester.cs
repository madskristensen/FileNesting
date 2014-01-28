
namespace MadsKristensen.FileNesting
{
    public interface IFileNester
    {
        NestingResult Nest(string fileName);
    }

    public enum NestingResult
    {
        Continue,
        StopProcessing,
    }
}
