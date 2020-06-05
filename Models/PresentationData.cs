using System.Collections.Generic;
using System.Linq;

namespace PptxExtractor.Models {
    public interface IPresentationData
    {
        string FileName { get; set; }
        int SlideCount { get; }
        IList<SlideData> Slides { get; set; }
    }

    public class PresentationData : IPresentationData
    {
        public PresentationData()
        {
            Slides = new List<SlideData>();
        }
        public string FileName { get; set; }
        public int SlideCount { get => Slides.Count(); }
        public IList<SlideData> Slides { get; set; }
    }

    public interface ISlideData
    {
        int Number { get; set; }
        IList<VideoData> Files { get; set; }
    }

    public class SlideData : ISlideData
    {
        public SlideData()
        {
            Files = new List<VideoData>();
        }
        public int Number { get; set; }
        public IList<VideoData> Files { get; set; }
    }

    public interface IVideoData
    {
        string FileName { get; set; }
        string ContentType { get; set; }
    }

    public class VideoData : IVideoData
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}