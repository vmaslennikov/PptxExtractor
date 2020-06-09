using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using PptxExtractor.Models;
using PptxExtractor.Services;

namespace PptxExtractor.Controllers {

    [ApiController]
    [Route ("api/Files")]
    public class FilesController : Controller {

        private readonly ILogger<FilesController> _logger;
        private readonly IPptxReaderService _pptxReaderService;
        private readonly IFileSystemReaderService _fileSystemReaderService;
        public FilesController (
            IPptxReaderService pptxReaderService,
            IFileSystemReaderService fileSystemReaderService,
            ILogger<FilesController> logger
        ) {
            _logger = logger;
            _pptxReaderService = pptxReaderService;
            _fileSystemReaderService = fileSystemReaderService;
        }

        public string GetContentType (string fileName) {
            var provider = new FileExtensionContentTypeProvider ();
            string contentType;
            if (!provider.TryGetContentType (fileName, out contentType)) {
                contentType = "application/octet-stream";
            }
            return contentType;
        }

        [Route ("Download"), HttpGet]
        public async Task<IActionResult> Download ([FromQuery] string fn, [FromQuery] int sn, [FromQuery] string vn) {
            byte[] fileContents = null;
            string filename = vn;
            string contentType = GetContentType(vn);
            try {
                if (string.IsNullOrEmpty (fn)) {
                    return BadRequest ("Не указано имя файла");
                }

                if (!_fileSystemReaderService.TryGetFile(fn, out string fullPath)) {
                    return BadRequest ("Не найден файл");
                }

                fileContents = await _pptxReaderService.GetFile(fn,vn);

            } catch (Exception error) {
                _logger.LogError(error, error.Message);
            }
            return File (
                fileContents: fileContents,
                contentType:  contentType, 
                fileDownloadName : filename
            );
        }

        [Route ("Upload"), HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult> Upload () {
            try {
                var file = Request.Form.Files.FirstOrDefault ();
                if (file == null || file?.Length == 0) {
                    return BadRequest ();
                }
                var fileName = ContentDispositionHeaderValue.Parse (file.ContentDisposition).FileName.Trim ('"');

                var fullPath = _fileSystemReaderService.GetStoragePathForSave (fileName);

                using (var stream = new FileStream (fullPath, FileMode.OpenOrCreate)) {
                    await file.CopyToAsync (stream);
                }

                return Ok (new { fullPath });
            } catch (Exception error) {
                _logger.LogError(error, error.Message);
                return StatusCode (500, $"Internal server error: {error.Message}");
            }
        }

        [Route ("extract"), HttpGet]
        public async Task<IActionResult> Extract ([FromQuery] string fn) {
            try {
                if (string.IsNullOrEmpty (fn)) {
                    return BadRequest ("Не указано имя файла");
                }

                if (!_fileSystemReaderService.TryGetFile(fn, out string fullPath)) {
                    return BadRequest ("Не найден файл");
                }

                PresentationData result = _pptxReaderService.Extract (fn, fullPath);

                return Ok (result);
            } catch (Exception error) {
                _logger.LogError(error, error.Message);
                return StatusCode (500, $"Internal server error: {error.Message}");
            }
        }

    }
}