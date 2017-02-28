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
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;

namespace YokogawaService.Models
{
    public class ApiClient : IDisposable
    {
        private HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient();

            string serviceHost = ConfigurationManager.AppSettings["ServiceHost"];

            if (string.IsNullOrEmpty(serviceHost))
                throw new Exception("Missing appSetting: ServiceHost");

            _httpClient.BaseAddress = new Uri(serviceHost);
        }

        private async Task<HttpResponseMessage> Execute(Func<HttpClient, Task<HttpResponseMessage>> fn)
        {
            try
            {
                var msg = await fn(_httpClient);
                msg.EnsureSuccessStatusCode();
                return msg;
            }
            catch (HttpRequestException reqex)
            {
                if (reqex.InnerException != null && typeof(WebException) == reqex.InnerException.GetType())
                {
                    var webex = (WebException)reqex.InnerException;
                    if (webex.Status == WebExceptionStatus.ConnectFailure)
                        throw new Exception(string.Format("Cannot connect to {0}. The service may not be running.", _httpClient.BaseAddress));
                    else
                        throw reqex.InnerException;
                }

                throw reqex;
            }
            catch
            {
                throw;
            }
        }

        public async Task<ServiceSummary> GetSummary()
        {
            var msg = await Execute(x => x.GetAsync("summary"));
            var result = await msg.Content.ReadAsAsync<ServiceSummary>();
            return result;
        }

        public async Task<IEnumerable<MeterData>> GetFileData(DataQueryCriteria criteria)
        {
            var msg = await Execute(x => x.PostAsJsonAsync("imports/data", criteria));
            var result = await msg.Content.ReadAsAsync<IEnumerable<MeterData>>();
            return result;
        }

        public async Task<ReportModel> RunReport(DataQueryCriteria criteria, string reportType)
        {
            var msg = await Execute(x => x.PostAsJsonAsync(string.Format("report/run/{0}", reportType.ToLower()), criteria));
            var result = await msg.Content.ReadAsAsync<ReportModel>();
            return result;
        }

        public async Task<ImportIndex> GetIndex()
        {
            var msg = await Execute(x => x.GetAsync("index"));
            var result = await msg.Content.ReadAsAsync<ImportIndex>();
            return result;
        }

        public async Task SetIndex(ImportIndex index)
        {
            await Execute(x => x.PutAsJsonAsync("index", index));
        }

        public async Task<ImportIndex> IncrementIndex()
        {
            var msg = await Execute(x => x.GetAsync("index/increment"));
            var result = await msg.Content.ReadAsAsync<ImportIndex>();
            return result;
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}
