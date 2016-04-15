using System.Collections.Generic;
using System.IO;
using System.Linq;
using EnvDTE;

namespace MadsKristensen.FileNesting
{
    internal class InterfaceImplementationNester : IFileNester
    {
        public NestingResult Nest(string fileName)
        {
            if (!IsSupported(fileName))
                return NestingResult.Continue;

            IEnumerable<string> possibleInterfaceNames = PossibleInterfaceNames(fileName);

            foreach (string interfaceName in possibleInterfaceNames)
            {
                string directory = Path.GetDirectoryName(fileName);
                string parentFileName = Path.Combine(directory, interfaceName);

                ProjectItem parent = VSPackage.DTE.Solution.FindProjectItem(parentFileName);
                if (parent != null)
                {
                    parent.ProjectItems.AddFromFile(fileName);
                    return NestingResult.StopProcessing;
                }
            }

            return NestingResult.Continue;
        }

        private static IEnumerable<string> PossibleInterfaceNames(string fileName)
        {
            string fileNameOnly = Path.GetFileNameWithoutExtension(fileName);

            List<string> possibleNames = new List<string>();

            for (int i = 0; i < fileNameOnly.Length; i++)
            {
                string letter = fileNameOnly.Substring(i, 1);

                if (letter == letter.ToUpperInvariant())
                {
                    possibleNames.Add(fileNameOnly.Substring(i, fileNameOnly.Length - i));
                }
            }

            string extension = Path.GetExtension(fileName);

            return possibleNames.Select(n => "I" + n + extension);
        }

        private static bool IsSupported(string fileName)
        {
            return (IsAllowedFileType(fileName)) && (!IsInterface(fileName));
        }

        private static bool IsAllowedFileType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            string[] allowed = { ".cs" };

            return allowed.Contains(extension);
        }

        private static bool IsInterface(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            bool firstLetterIsI = ('I' == fileName[0]);

            string secondLetter = fileName.Substring(1, 1);
            bool secondLetterPeriodOrLowercase = (("." == secondLetter) || (secondLetter == secondLetter.ToLowerInvariant()));

            return ((firstLetterIsI) && (!secondLetterPeriodOrLowercase));
        }

        public bool IsEnabled()
        {
            return VSPackage.Options.EnableInterfaceImplementationRule;
        }
    }
}