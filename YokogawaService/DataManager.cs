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

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using FluentNHibernate.Conventions.Helpers;
using NHibernate;
using NHibernate.Context;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using YokogawaService.Models;

namespace YokogawaService
{
    public class DataManager
    {
        public static DataManager Current { get; }

        static DataManager()
        {
            Current = new DataManager();
        }

        private readonly ISessionFactory _sessionFactory;

        private DataManager()
        {

            _sessionFactory = Fluently.Configure()
                .Database(() =>
                {
                    var result = MsSqlConfiguration.MsSql2012.ConnectionString(c => c.FromConnectionStringWithKey("YokogawaData"));

                    if (Config.Current.ShowSql)
                        result.ShowSql();

                    return result;
                })
                .Mappings(m =>
                {
                    m.FluentMappings
                        .AddFromAssembly(GetType().Assembly)
                        .Conventions.Add(
                            ForeignKey.EndsWith("ID"),
                            LazyLoad.Always()
                        );
                })
                .CurrentSessionContext<ThreadStaticSessionContext>()
                .BuildSessionFactory();
        }

        public UnitOfWork StartUnitOfWork()
        {
            return new UnitOfWork(_sessionFactory);
        }
    }

    public class UnitOfWork : IDisposable
    {
        private ISessionFactory _sessionFactory;
        private ISession _session;
        private ITransaction _transaction;

        internal UnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;

            if (!CurrentSessionContext.HasBind(_sessionFactory))
            {
                _session = _sessionFactory.OpenSession();
                CurrentSessionContext.Bind(_session);
            }
            else
            {
                _session = _sessionFactory.GetCurrentSession();
            }

            _transaction = _session.BeginTransaction();
        }

        public int DeleteFileImports()
        {
            // delete all imports
            int result = _session.CreateQuery("delete FileImport f").ExecuteUpdate();

            // delete all import data
            _session.CreateQuery("delete MeterData m").ExecuteUpdate();

            return result;
        }

        public int GetFileImportCount()
        {
            return _session.Query<FileImport>().Count();
        }

        public int GetMeterDataCount()
        {
            return _session.Query<MeterData>().Count();
        }

        public bool ImportExists(string filePath)
        {
            return _session.Query<FileImport>().Any(x => x.FilePath == filePath);
        }

        public FileImport GetFileImport(int fileIndex)
        {
            return _session.Query<FileImport>().FirstOrDefault(x => x.FileIndex == fileIndex);
        }

        public IQueryable<FileImport> AllImportFiles(int skip, int limit)
        {
            return _session.Query<FileImport>().Skip(skip).Take(limit);
        }

        public IQueryable<MeterData> AllMeterData(int skip, int limit)
        {
            return _session.Query<MeterData>().Skip(skip).Take(limit);
        }

        public IQueryable<MeterData> QueryMeterData(int fileIndex)
        {
            return _session.Query<MeterData>().Where(x => x.FileIndex == fileIndex);
        }

        public IQueryable<MeterData> QueryMeterData(DataQueryCriteria criteria)
        {
            var result = _session.Query<MeterData>();

            if (criteria != null)
            {
                if (!string.IsNullOrEmpty(criteria.HeaderPattern))
                    result = result.Where(x => x.Header.Like(criteria.HeaderPattern));

                if (criteria.StartDate.HasValue)
                    result = result.Where(x => x.TimeStamp >= criteria.StartDate.Value);

                if (criteria.EndDate.HasValue)
                    result = result.Where(x => x.TimeStamp < criteria.EndDate.Value);
            }

            return result;
        }

        public int? GetMaxFileIndex()
        {
            return _session.Query<FileImport>().Max(x => (int?)x.FileIndex);
        }

        public bool DeleteFileImport(int fileIndex)
        {
            FileImport fileImport = GetFileImport(fileIndex);

            if (fileImport != null)
            {
                // delete all the meter data for this import
                var query = _session.CreateQuery("delete from MeterData where FileImportID = :fileImportId");
                query.SetInt32("fileImportId", fileImport.FileImportID);
                query.ExecuteUpdate();

                // delete the import
                _session.Delete(fileImport);

                return true;
            }

            return false;
        }

        public FileImport ImportFile(YokogawaFile yokoFile)
        {
            FileImport result = GetFileImport(yokoFile.Index);

            if (result != null) return result;

            var data = yokoFile.GetData();

            var count = 0;

            if (data != null)
                count = data.Count();

            using (var statelessSession = _sessionFactory.OpenStatelessSession())
            using (var statelessTransaction = statelessSession.BeginTransaction())
            {
                try
                {
                    result = new FileImport()
                    {
                        FileIndex = yokoFile.Index,
                        FilePath = yokoFile.FilePath,
                        ImportDate = DateTime.Now,
                        LineCount = count
                    };

                    statelessSession.Insert(result);

                    if (count > 0)
                    {
                        var items = data.Select(x => new MeterData()
                        {
                            FileIndex = yokoFile.Index,
                            LineIndex = x.LineIndex,
                            Header = x.Header,
                            TimeStamp = x.TimeStamp,
                            Value = x.Value
                        });

                        foreach (var item in items)
                            statelessSession.Insert(item);
                    }

                    statelessTransaction.Commit();

                    return result;
                }
                catch
                {
                    statelessTransaction.Rollback();
                    throw;
                }
                finally
                {
                    statelessSession.Close();
                }
            }
        }

        public IQueryable<Report> GetReports()
        {
            return _session.Query<Report>();
        }

        public ReportModel RunReport(DataQueryCriteria criteria, string reportType)
        {
            if (!criteria.StartDate.HasValue)
                throw new Exception("Criteria must have a StartDate value.");

            if (!criteria.EndDate.HasValue)
                throw new Exception("Criteria must have a EndDate value.");

            if (criteria.StartDate.Value >= criteria.EndDate.Value)
                throw new Exception("Criteria must have a StartDate that comes before the EndDate.");

            IList<MeterData> meterData = QueryMeterData(criteria)
                .Where(x => x.TimeStamp.Day == 1 && x.TimeStamp.Hour == 0 && x.TimeStamp.Minute == 0 && x.TimeStamp.Second == 0).ToList();

            IList<Report> reports;

            if (reportType != "all")
                reports = GetReports().Where(x => x.ReportType == reportType).ToList();
            else
                reports = GetReports().ToList();

            var datasets = new List<ReportDataset>();

            var labels = new List<string>();
            var d = criteria.StartDate.Value;
            while (d < criteria.EndDate.Value)
            {
                labels.Add(d.ToString("MMM \\'yy"));

                foreach (var rep in reports)
                {
                    ReportDataset ds = datasets.FirstOrDefault(x => x.Label == rep.ReportName);

                    if (ds == null)
                    {
                        ds = new ReportDataset()
                        {
                            Label = rep.ReportName,
                            Data = new List<double>(),
                            BorderColor = rep.BorderColor,
                            BackgroundColor = rep.BackgroundColor,
                            PointBorderColor = rep.PointBorderColor,
                            PointBackgroundColor = rep.PointBackgroundColor,
                            UnitCost = rep.UnitCost,
                            Fill = false
                        };
                        
                        datasets.Add(ds);
                    }

                    var md = meterData.FirstOrDefault(x => x.TimeStamp == d && x.Header == rep.Header);
                    var value = md == null ? 0 : md.Value;
                    ds.Data.Add(value);
                }

                d = d.AddMonths(1);
            }


            var result = new ReportModel()
            {
                Labels = labels,
                Datasets = datasets.Where(x => x.Data.Sum() > 0)
            };

            return result;
        }

        public void Dispose()
        {
            try
            {
                _transaction.Commit();
            }
            catch
            {
                _transaction.Rollback();
                throw;
            }
            finally
            {
                if (CurrentSessionContext.HasBind(_sessionFactory))
                {
                    ISession session = CurrentSessionContext.Unbind(_sessionFactory);
                    session.Close();
                    session.Dispose();
                }
            }
        }
    }


}
