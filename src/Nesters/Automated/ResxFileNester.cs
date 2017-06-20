using System;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class ResxFileNester : IFileNester
    {
        private const string _resxExtension = ".resx";
        private static readonly string[] _sortedCultureNames = GetSortedCultureNames();

        public NestingResult Nest(string fileName)
        {
            string extension = Path.GetExtension(fileName);

            if (!_resxExtension.Equals(extension, StringComparison.OrdinalIgnoreCase))
                return NestingResult.Continue;

            string baseName = Path.ChangeExtension(fileName, null);
            string language = Path.GetExtension(baseName);

            if (!IsValidLanguage(language))
                return NestingResult.Continue;

            string neutralFileName = Path.ChangeExtension(baseName, _resxExtension);

            ProjectItem item = FileNestingPackage.DTE.Solution.FindProjectItem(neutralFileName);

            if (item != null)
            {
                item.ProjectItems.AddFromFile(fileName);
                return NestingResult.StopProcessing;
            }

            return NestingResult.Continue;
        }

        public bool IsEnabled()
        {
            return FileNestingPackage.Options.EnableResxResourceFileRule;
        }

        private bool IsValidLanguage(string language)
        {
            if (string.IsNullOrEmpty(language))
                return false;

            return Array.BinarySearch(_sortedCultureNames, language.TrimStart('.'), StringComparer.OrdinalIgnoreCase) >= 0;
        }

        private static string[] GetSortedCultureNames()
        {
            CultureInfo[] allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            Contract.Assume(allCultures != null);
            string[] cultureNames = allCultures
                .SelectMany(culture => new[] { culture.IetfLanguageTag, culture.Name })
                .Distinct()
                .ToArray();

            Array.Sort(cultureNames, StringComparer.OrdinalIgnoreCase);

            return cultureNames;
        }
    }
}
