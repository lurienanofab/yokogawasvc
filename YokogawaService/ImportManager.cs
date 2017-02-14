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

using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using YokogawaService.Models;

namespace YokogawaService
{
    public class ImportManager
    {
        public static ImportManager Current { get; }

        static ImportManager()
        {
            Current = new ImportManager();
        }

        private MongoClient _client;
        private IMongoDatabase _database;

        private ImportManager()
        {
            MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(Config.Current.ConnectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            _client = new MongoClient(settings);
            _database = _client.GetDatabase("yokogawasvc");
        }

        public long GetImportCount()
        {
            return GetImportFilesCollection().Find(FilterDefinition<ImportFile>.Empty).Count();
        }

        public long GetImportDataCount()
        {
            return GetImportFileDataCollection().Find(FilterDefinition<ImportFileData>.Empty).Count();
        }

        private IMongoCollection<ImportFile> GetImportFilesCollection()
        {
            return _database.GetCollection<ImportFile>("importFiles");
        }

        private IMongoCollection<ImportFileData> GetImportFileDataCollection()
        {
            return _database.GetCollection<ImportFileData>("importFileData");
        }

        private IMongoCollection<ImportFileIndex> GetImportFileIndexCollection()
        {
            return _database.GetCollection<ImportFileIndex>("importFileIndex");
        }

        public bool ImportExists(string filePath)
        {
            return GetImportFilesCollection().Count(x => x.FilePath == filePath) > 0;
        }

        public ImportFile GetImportFile(int index)
        {
            return GetImportFilesCollection().Find(Builders<ImportFile>.Filter.Eq(x => x.Index, index)).FirstOrDefault();
        }

        public IQueryable<ImportFile> QueryImportFiles()
        {
            return GetImportFilesCollection().AsQueryable();
        }

        public IQueryable<ImportFileData> QueryImportFileData()
        {
            return GetImportFileDataCollection().AsQueryable();
        }

        public IList<ImportFileData> QueryImportFileData(DataQueryCriteria criteria)
        {
            var importData = GetImportFileDataCollection();

            if (criteria != null)
            {
                var defs = new List<FilterDefinition<ImportFileData>>();

                if (!string.IsNullOrEmpty(criteria.HeaderPattern))
                    defs.Add(Builders<ImportFileData>.Filter.Regex(x => x.Header, new BsonRegularExpression(criteria.HeaderPattern)));

                if (criteria.StartDate.HasValue)
                    defs.Add(Builders<ImportFileData>.Filter.Gte(x => x.TimeStamp, criteria.StartDate.Value));

                if (criteria.EndDate.HasValue)
                    defs.Add(Builders<ImportFileData>.Filter.Lt(x => x.TimeStamp, criteria.EndDate.Value));

                if (defs.Count == 1)
                    return importData.Find(defs[0]).ToList();
                else if (defs.Count > 1)
                    return importData.Find(Builders<ImportFileData>.Filter.And(defs)).ToList();
            }

            return importData.Find(FilterDefinition<ImportFileData>.Empty).ToList();
        }

        public ImportFileIndex GetIndex()
        {
            // There should never be more than one document in this collection.
            return GetImportFileIndexCollection().Find(FilterDefinition<ImportFileIndex>.Empty).FirstOrDefault();
        }

        public void SetIndex(ImportFileIndex lastIndex, int nextIndex)
        {
            var importIndex = new ImportFileIndex() { Index = nextIndex };

            if (lastIndex == null)
                GetImportFileIndexCollection().InsertOne(importIndex);
            else
            {
                importIndex.Id = lastIndex.Id;
                GetImportFileIndexCollection().ReplaceOne(Builders<ImportFileIndex>.Filter.Eq(x => x.Id, lastIndex.Id), importIndex);
            }
        }

        public bool DeleteIndex()
        {
            var result = GetImportFileIndexCollection().DeleteMany(FilterDefinition<ImportFileIndex>.Empty);
            return result.IsAcknowledged && result.DeletedCount > 0;
        }

        public bool DeleteFileImport(int index)
        {
            var importFiles = GetImportFilesCollection();

            var import = importFiles.Find(x => x.Index == index).FirstOrDefault();

            if (import == null) return false;

            importFiles.DeleteOne(x => x.Index == index);

            GetImportFileDataCollection().DeleteMany(x => x.FileIndex == index);

            return true;
        }

        public ImportFile ImportFile(YokogawaFile yokoFile, SampleGranularity gran = SampleGranularity.Day)
        {
            var data = yokoFile.GetData(gran);

            var count = 0;

            if (data != null)
                count = data.Count();

            var document = new ImportFile()
            {
                Index = yokoFile.Index,
                FilePath = yokoFile.FilePath,
                ImportDate = DateTime.UtcNow,
                Granularity = gran,
                LineCount = count
            };

            GetImportFilesCollection().InsertOne(document);

            if (count > 0)
            {
                var documents = data.Select(x => new ImportFileData()
                {
                    FileIndex = x.FileIndex,
                    LineIndex = x.LineIndex,
                    Header = x.Header,
                    TimeStamp = x.TimeStamp,
                    Value = x.Value,
                    ImportDate = DateTime.UtcNow
                });

                GetImportFileDataCollection().InsertMany(documents);
            }

            return document;
        }
    }
}
