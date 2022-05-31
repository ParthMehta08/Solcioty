using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BoxerTemplateModel
    {
        public int BoxerTemplateID { get; set; }
        public string TemplateName { get; set; }
        public string TimeText { get; set; }
        public string Reps { get; set; }
        public string Note { get; set; }
        public int VideoID { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
    }
}
