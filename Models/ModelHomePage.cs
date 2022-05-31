using System.Collections.Generic;

namespace Models
{
    public class ModelHomePage
    {
        public ModelGymConfiguration ClientConfiguration { get; set; }
        public WorkoutDetailModel WorkoutInfo { get; set; }
        public BoxerWorkoutDetailModel BoxerWorkoutInfo { get; set; }
        public List<WorkoutDetailModel> WorkoutInfos { get; set; }
        public List<ModelWorkout> WorkoutList { get; set; }
        public List<ModelWorkout> FutureWorkoutList { get; set; }
		public bool IsTrainingPortalAccessible { get; set; }
    }

    public class ModelBoxerHomePage
    {
        public ModelGymConfiguration ClientConfiguration { get; set; }
        public BoxerWorkoutDetailModel BoxerWorkoutInfo { get; set; }
        public List<BoxerWorkoutDetailModel> BoxerWorkoutInfos { get; set; }
        public List<ModelBoxerWorkout> BoxerWorkoutList { get; set; }
        public List<ModelBoxerWorkout> FutureBoxerWorkoutList { get; set; }
        public IList<ModelBoxerTemplate> BoxerTempalteList { get; set; }
        public bool IsTrainingPortalAccessible { get; set; }
    }

    public class LocationDetail
    {
        public int LocationId { get; set; }
        public string LocationName { get; set; }

        public WorkoutDetail SelectedWorkout { get; set; }
    }

    public class WorkoutDetail
    {
        public int WorkoutId { get; set; }
        public string WorkoutName { get; set; }

        public List<TemplateInfo> TemplateList { get; set; }

    }

    public class TemplateInfo
    {
        public int TemplateId { get; set; }
        public string TemplateName { get; set; }
        
        public List<ModelTemplateVideoMapping> PrimaryVideos { get; set; }

        public List<ModelTemplateVideoMapping> AlternateVideos { get; set; }

    }
}
