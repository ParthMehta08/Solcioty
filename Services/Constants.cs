namespace Services
{
   public static class RoleTypes
    {
        public const string SuperAdmin = "UDC01";
        public const string ClientAdmin = "UDC02";
        public const string BranchAdmin = "UDC03";
        public const string BranchInstructor = "UDC04";
        public const string Client = "UDC05";
    }

    public static class Permission
    {
        public const string Client = "PRMS_Client";
        public const string Branch = "PRMS_Branch";
        public const string Roles = "PRMS_Role";
        public const string Permissions = "PRMS_Permission";
        public const string Tags = "PRMS_Tag";
        public const string Videos = "PRMS_Video";
        public const string Images = "PRMS_Image";
        public const string Templates = "PRMS_Template";
        public const string Workouts = "PRMS_Workout";
        public const string Workouts_Schedule = "PRMS_Workout_Schedule";
        public const string Branch_Workout_Schedule = "PRMS_Branch_Workout_Schedule";
        public const string User = "PRMS_User";
		public const string TrainingPortal = "PRMS_TrainingPortal";
		public const string Category = "PRMS_Category";
        public const string BoxerWorkouts = "PRMS_BoxerWorkout";
        public const string BoxerWorkouts_Schedule = "PRMS_BoxerWorkout_Schedule";
        public const string BoxerTemplates = "PRMS_BoxerTemplate";
    }



    public static class EmailTemplate
    {
        public static string UserRegistration = "UserRegistration";
        public static string ForgotPassword = "ForgotPassword";
    }

    public static class EmailSubject
    {
        public static string WelcomeUser = "Welcome to Solcioty";
        public static string ResetPassword = "Reset Password";
    }

    public static class ErrorMessage
    {
        public static string InvalidRequest = "Invalid request.";
        public static string EmailAlreadyExist = "Email already exist.";
        public static string OldPasswordWrong = "Old password is wrong.";
        public static string PasswordChanged = "Password changed successfully.";
        public static string UsernamePasswordWrong = "Username/Password may incorrect.";
        public static string GeneralError = "Something went wrong.";
        public static string WorkoutType = "Please Select Workout Type.";
        public static string AccountDeactivated = "Your account is deactivated, please contact to site Administrator.";
        public static string LocationAdded = "Location detail added.";
        public static string LocationUpdated = "Location detail updated.";
        public static string LocationDeleted = "Location detail deleted.";
        public static string TryLater = "Please try again later.";
        public static string UserSaved = "User detail saved.";
        public static string UserDeleted = "User detail deleted.";
        public static string ClientAdded = "Client detail added.";
        public static string ClientUpdated = "Client detail updated.";
        public static string ClientDeleted = "Client detail deleted.";
        public static string DuplicateLocation = "Please try again with different location name.";
        public static string TemplateAdded = "Template detail added.";
        public static string TemplateUpdated = "Template detail updated.";
        public static string TemplateDeleted = "Template detail deleted.";
        public static string DuplicateTemplate = "Please try again with different template name.";

        public static string VideoAdded = "Video detail added.";
        public static string VideoUpdated = "Video detail updated.";
        public static string VideoDeleted = "Video detail deleted.";
		public static string VideoMoved = "Video detail moved.";
		public static string DuplicateVideo = "Please try again with different video name.";

        public static string ImageAdded = "Image detail added.";
        public static string ImageUpdated = "Image detail updated.";
        public static string ImageDeleted = "Image detail deleted.";
        public static string DuplicateImage = "Please try again with different Image name.";


        public static string TagAdded = "Tag detail added.";
        public static string TagUpdated = "Tag detail updated.";
        public static string TagDeleted = "Tag detail deleted.";
        public static string DuplicateTag = "Please try again with different tag name.";

        public static string WorkoutAdded = "Workout detail added.";
        public static string WorkoutUpdated = "Workout detail updated.";
        public static string WorkoutDeleted = "Workout detail deleted.";
		public static string WorkoutMoved = "Workout detail moved.";
		public static string DuplicateWorkout = "Please try again with different workout name.";
        public static string WorkoutSelected = "Workout selected successfully.";
        public static string WorkoutScheduled = "Workout scheduled successfully.";
        public static string ConfigurationSaved = "Configuration saved successfully.";
		public static string SelectLocation = "Please select location.";

		public static string RoleStatusChanged = "Role activation status changed.";

		public static string TrainingPortalAdded ="Training Portal detail added.";
		public static string TrainingPortalUpdated = "Training Portal detail updated.";
		public static string TrainingPortalDeleted = "Training Portal detail deleted.";

		public static string TrainingPortalCategoryAdded =   "Training Portal Category detail added.";
		public static string TrainingPortalCategoryUpdated = "Training Portal Category detail updated.";
		public static string TrainingPortalCategoryDeleted = "Training Portal Category detail deleted.";

		public static string TrainingPortalSubCategoryAdded =   "Training Portal Sub Category detail added.";
		public static string TrainingPortalSubCategoryUpdated = "Training Portal Sub Category detail updated.";
		public static string TrainingPortalSubCategoryDeleted = "Training Portal Sub Category detail deleted.";

		public static string TrainingPortalSubCategoryVideoAdded =   "Training Portal Sub Category Video detail added.";
		public static string TrainingPortalSubCategoryVideoUpdated = "Training Portal Sub Category Video detail updated.";
		public static string TrainingPortalSubCategoryVideoDeleted = "Training Portal Sub Category Video detail deleted.";

		public static string CategoryAdded = "Category detail added.";
		public static string CategoryUpdated = "Category detail updated.";
		public static string CategoryDeleted = "Category detail deleted.";
		public static string DuplicateCategory = "Please try again with different category name.";

        public static string BoxerWorkoutAdded = "Boxer Workout detail added.";
        public static string BoxerWorkoutUpdated = "Boxer Workout detail updated.";
        public static string BoxerWorkoutDeleted = "Boxer Workout detail deleted.";
        public static string BoxerWorkoutMoved = "Boxer Workout detail moved.";
        public static string BoxerDuplicateWorkout = "Please try again with different boxer workout name.";
        public static string BoxerWorkoutSelected = "Boxer Workout selected successfully.";
        public static string BoxerWorkoutScheduled = "Boxer Workout scheduled successfully.";
        public static string BoxerConfigurationSaved = "Configuration saved successfully.";
        public static string BoxerSelectLocation = "Please select location.";
    }

}
