using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using YokogawaService.Models;

namespace YokogawaService
{
    public static class FileUtility
    {
        public const string FILE_FILTER = "*_DAD_TABULAR.csv";

        public static int GetFileCount()
        {
            DirectoryInfo di = new DirectoryInfo(Config.Current.FolderPath);

            if (di.Exists)
                return di.EnumerateFiles(FILE_FILTER).Count();
            else
                return 0;
        }


        public static IEnumerable<YokogawaFile> GetAllFiles(int skip, int take)
        {
            DirectoryInfo di = new DirectoryInfo(Config.Current.FolderPath);

            if (di.Exists)
            {
                IEnumerable<FileInfo> files = di.EnumerateFiles(FILE_FILTER).Skip(skip).Take(take);

                foreach (FileInfo fi in files)
                {
                    yield return Create(fi.FullName);
                }
            }
        }

        public static YokogawaFile GetFirstFile()
        {
            DirectoryInfo di = new DirectoryInfo(Config.Current.FolderPath);

            if (di.Exists)
            {
                FileInfo fi = di.EnumerateFiles(FILE_FILTER).FirstOrDefault();
                if (fi == null || !fi.Exists) return null;
                return Create(fi.FullName);
            }

            return null;
        }

        public static YokogawaFile GetFile(int index)
        {
            string filter = string.Format("{0:000000}_{1}", index, FILE_FILTER);

            if (Directory.Exists(Config.Current.FolderPath))
            {
                string filePath = Directory.GetFiles(Config.Current.FolderPath, filter).FirstOrDefault();

                if (!string.IsNullOrEmpty(filePath))
                    return Create(filePath);
            }

            return null;
        }

        public static YokogawaFile Create(string filePath)
        {
            // "F:\\yokogawa-reports\\recorder-01\\002567_170202_065400_DAD_TABULAR.csv";

            if (!File.Exists(filePath))
                throw new FileNotFoundException(string.Format("File not found: {0}", filePath));

            var fileInfo = new FileInfo(filePath);

            if (fileInfo.Length == 0)
                throw new Exception(string.Format("File is empty: {0}", filePath));

            var fileName = Path.GetFileNameWithoutExtension(filePath);

            var matches = Regex.Match(fileName, "([0-9]{1,6})_([0-9]{1,6})_([0-9]{1,6})_DAD_TABULAR");

            if (matches.Success)
            {
                YokogawaFile result = new YokogawaFile();

                result.FilePath = filePath;

                result.Index = int.Parse(matches.Groups[1].Value);

                var datePart = DateTime.ParseExact(matches.Groups[2].Value, "yyMMdd", CultureInfo.InvariantCulture);

                var timePart = TimeSpan.ParseExact(matches.Groups[3].Value, "hhmmss", CultureInfo.InvariantCulture); ;

                result.Date = datePart.Add(timePart);

                return result;
            }
            else
            {
                throw new ArgumentException("Invalid file path.", "filePath");
            }
        }

        public static bool TakeSample(DateTime timeStamp)
        {
            bool result = timeStamp.Second == 0
                && timeStamp.Hour % Config.Current.HourGranularity == 0
                && timeStamp.Minute % Config.Current.MinuteGranularity == 0;

            return result;
        }
    }
}
