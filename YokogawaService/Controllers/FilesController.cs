using System.Collections.Generic;
using System.Web.Http;
using YokogawaService.Models;

namespace YokogawaService.Controllers
{
    public class FilesController : ApiController
    {
        [Route("files")]
        public IEnumerable<YokogawaFile> GetFiles(int skip = 0, int take = 100)
        {
            return FileUtility.GetAllFiles(skip, take);
        }

        [Route("files/{index}")]
        public YokogawaFile GetFile(int index)
        {
            return FileUtility.GetFile(index);
        }

        [Route("files/{index}/content")]
        public string GetFileContent(int index)
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.GetContent();
        }

        [Route("files/{index}/data")]
        public IEnumerable<MeterData> GetFileData(int index)
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.GetData();
        }

        [HttpPost, Route("files/{index}/data")]
        public IEnumerable<MeterData> QueryFileData([FromBody] DataQueryCriteria criteria, int index)
        {
            YokogawaFile yokoFile = FileUtility.GetFile(index);
            return yokoFile.QueryData(criteria);
        }

        [HttpPost, Route("files/{index}/import")]
        public FileImport ImportFile(int index)
        {
            using (var uow = DataManager.Current.StartUnitOfWork())
            {
                YokogawaFile yokoFile = FileUtility.GetFile(index);

                if (yokoFile == null) return null;

                FileImport result = uow.ImportFile(yokoFile);

                return result;
            }
        }
    }
}
