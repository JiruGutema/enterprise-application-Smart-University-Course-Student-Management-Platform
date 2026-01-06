namespace SmartUniversity.Modules.Enrollment.Api.DTOs
{
    public class PrerequisiteValidationResponse
    {
        public bool IsEligible { get; set; }
        public List<string> MissingPrerequisites { get; set; } = new List<string>();
    }
}
