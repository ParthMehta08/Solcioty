using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class BoxerTemplateModel
    {
        public int ID { get; set; }
        public string TemplateName { get; set; }
        public string TimeText { get; set; }
        public int NumberOfBasicVideos { get; set; }
        public int NumberOfAlterVideos { get; set; }

        public List<BoxerWorkoutModelList> boxerWorkoutModelLists { get; set; }
    }

    public class BoxerWorkoutModelList
    {
        public int ID { get; set; }
        public string WorkoutName { get; set; }
        public string PDFName { get; set; }
        
    }
}
