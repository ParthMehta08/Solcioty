using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ModelBaseCopyResponse
    {
        public ModelBaseCopyResponse()
        {
            result = new List<ModelCopySiteResponse>();
        }
        public string Code { get; set; }
        public string Message { get; set; }
        public IList<ModelCopySiteResponse> result { get; set; }
    }
    public class ModelCopySiteResponse
    {
        public int SourceId { get; set; }
        public int DestinationId { get; set; }
        public string GroupName { get; set; }
    }
}
