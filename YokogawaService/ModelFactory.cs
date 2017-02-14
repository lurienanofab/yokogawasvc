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
using YokogawaService.Models;

namespace YokogawaService
{
    public static class ModelFactory
    {
        public static ImportFileModel CreateImportFileModel(ImportFile importFile)
        {
            if (importFile == null) return null;

            return new ImportFileModel()
            {
                Index = importFile.Index,
                FilePath = importFile.FilePath,
                Granularity = importFile.Granularity,
                LineCount = importFile.LineCount,
                ImportDate = importFile.ImportDate
            };
        }

        public static YokogawaFileData CreateYokogawaFileData(ImportFileData importFileData)
        {
            if (importFileData == null) return null;

            return new YokogawaFileData()
            {
                FileIndex = importFileData.FileIndex,
                LineIndex = importFileData.LineIndex,
                TimeStamp = importFileData.TimeStamp,
                Header = importFileData.Header,
                Value = importFileData.Value
            };
        }
    }
}
