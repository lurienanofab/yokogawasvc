using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class DefaultController : ApiController
    {
        [Route("")]
        public string Get()
        {
            return "yokogawasvc";
        }

        [Route("summary")]
        public ServiceSummary GetSummary()
        {
            var uow = Request.GetUnitOfWork();
            ServiceSummary result = new ServiceSummary();
            result.TotalFiles = FileUtility.GetFileCount();
            result.TotalImports = uow.GetFileImportCount();
            result.TotalDataRecords = uow.GetMeterDataCount();
            result.CurrentIndex = uow.GetMaxFileIndex().GetValueOrDefault(-1);
            result.LastFile = FileUtility.GetFile(result.CurrentIndex);
            result.LastImport = uow.GetFileImport(result.CurrentIndex);
            return result;
        }

        [Route("index")]
        public ImportIndex GetIndex()
        {
            lock (ImportIndex.Instance)
            {
                return ImportIndex.Instance;
            }
        }

        [Route("index")]
        public void PutIndex([FromBody] ImportIndex index)
        {
            lock (ImportIndex.Instance)
            {
                ImportIndex.Instance.Value = index.Value;
            }
        }

        [HttpGet, Route("index/increment")]
        public ImportIndex IncrementIndex()
        {
            lock (ImportIndex.Instance)
            {
                ImportIndex.Instance.Increment();
                return ImportIndex.Instance;
            }
        }
    }
}
