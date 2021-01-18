using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetizenApi.Models;
using PetizenApi.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetizenApi.Controllers
{

    public class FileUploadController : ControllerBase
    {
        private readonly UploadRepository _uploadRepository;

        public FileUploadController(UploadRepository uploadRepository)
        {
            _uploadRepository = uploadRepository;
        }


        [Route("UploadFile")]
        [RequestFormLimits(MultipartBodyLengthLimit = 4294967295)]
        [RequestSizeLimit(4294967295)]
        [HttpPost]
        public async Task<object> UploadFileAsync([FromForm] IFormFile file)
        {
            try
            {
                var response = await _uploadRepository.UploadFilesAsync(file).ConfigureAwait(false);

                if (!string.IsNullOrEmpty(response.Id))
                {
                    return Ok(response);
                }
                else
                {

                    return StatusCode(412, "Error while uploading file..");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return StatusCode(500, $"Internal server error: {ex}");
            }
        }


        [Route("MultipleUpload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 4294967295)]
        [RequestSizeLimit(4294967295)]
        [HttpPost]
        public async Task<object> MultipleUploadAsync([FromForm] List<IFormFile> files)
        {
            try
            {
                var result = new List<FileClass>();
                if (files == null) throw new ArgumentNullException(nameof(files));


                foreach (var item in files)
                {
                    var response = await _uploadRepository.UploadFilesAsync(item).ConfigureAwait(false);
                    result.Add(response);
                }

                if (result.Count > 0)
                {
                    return Ok(result);
                }
                else
                {

                    return StatusCode(412, "Error while uploading files..");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
                //return StatusCode(500, $"Internal server error: {ex}");
            }
        }
    }
}