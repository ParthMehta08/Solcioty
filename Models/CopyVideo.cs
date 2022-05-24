using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class CopyVideo
    {
        public Nullable<long> OldId { get; set; }
        public Nullable<long> NewId { get; set; }
        public string Message { get; set; }
        public Nullable<int> SourceId { get; set; }
        public Nullable<int> DestinationId { get; set; }
        public string VideoTitle { get; set; }
        public string VideoAttachment { get; set; }
        public string ContentType { get; set; }
    }

    public class CopyVideosModel
    {
        public CopyVideosModel()
        {
            modelVideos = new List<CopyVideo>();
        }
        public IList<CopyVideo> modelVideos { get; set; }
        public string Message { get; set; }
    }
}
