using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using PptxExtractor.Models;

namespace PptxExtractor.Services {
    public interface IPptxReaderService {
        public PresentationData Extract (string fileName, string fullPath);
        public Task<byte[]>  GetFile (string pptxFileName, string fileName);
    }
    public class PptxReaderService : IPptxReaderService {

        private readonly IFileSystemReaderService _fileSystemReaderService;
        public PptxReaderService (IFileSystemReaderService fileSystemReaderService) {
            _fileSystemReaderService = fileSystemReaderService;
        }

        // Get a list of the titles of all the slides in the presentation.
        public PresentationData Extract (string fileName, string fullPath) {
            using (PresentationDocument presentationDocument = PresentationDocument.Open (fullPath, false)) {
                if (presentationDocument == null) {
                    throw new ArgumentNullException ("presentationDocument");
                }
                var result = new PresentationData {
                    FileName = fileName
                };
                // Get a PresentationPart object from the PresentationDocument object.
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                if (presentationPart?.Presentation?.SlideIdList != null) {
                    // Get a Presentation object from the PresentationPart object.
                    presentationPart
                        .SlideParts
                        .Select ((s, i) => new { Number = i, Data = s })
                        .ToList ()
                        .ForEach (slide => {
                            var slideData = new SlideData { Number = slide.Number };
                            //var videos = slide.Data.Slide.CommonSlideData.ShapeTree.Descendants<VideoFromFile> ();
                            foreach (var videoPart in slide.Data.DataPartReferenceRelationships.OfType<VideoReferenceRelationship> ()) {
                                slideData.Files.Add (new VideoData {
                                    FileName = videoPart?.DataPart?.Uri?.OriginalString?.Substring (videoPart.DataPart.Uri.OriginalString.LastIndexOf ('/') + 1),
                                        ContentType = videoPart.DataPart?.ContentType
                                });
                            }
                            result.Slides.Add (slideData);
                        });
                }
                return result;
            }
        }

        public async Task<byte[]>  GetFile (string pptxFileName, string fileName) {
            byte[] result = null;
            var tempPath = _fileSystemReaderService.GetStoragePathTemp(fileName);
            var fullPath = _fileSystemReaderService.GetStoragePathForSave(pptxFileName);
            using (var file = ZipFile.OpenRead (fullPath)) {
                var temp = file.Entries.FirstOrDefault (entry => entry.FullName.EndsWith ($"/{fileName}"));
                temp.ExtractToFile(tempPath);
                result = await File.ReadAllBytesAsync(tempPath);
                File.Delete(tempPath);
            }
            return result;
        }

    }
}