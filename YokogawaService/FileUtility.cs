/*
   Copyright 2017 University of Michigan

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License. 
*/

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

        public static SampleGranularity GetGranularity(string gran)
        {
            return (SampleGranularity)Enum.Parse(typeof(SampleGranularity), gran, true);
        }

        public static bool TakeSample(DateTime timeStamp, SampleGranularity gran)
        {
            bool result = false;

            if (gran == SampleGranularity.Day)
                result = timeStamp.Hour == 0 && timeStamp.Minute == 0 && timeStamp.Second == 0;
            else if (gran == SampleGranularity.Hour)
                result = timeStamp.Minute == 0 && timeStamp.Second == 0;
            else if (gran == SampleGranularity.Minute)
                result = timeStamp.Second == 0;

            return result;
        }
    }
}
