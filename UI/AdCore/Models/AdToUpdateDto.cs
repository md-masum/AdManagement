namespace AdCore.Models
{
    public class AdToUpdateDto : BaseDto
    {
        public string Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public List<string> ImageUrl { get; set; }
        public List<string> VideoUrl { get; set; }
    }
}
