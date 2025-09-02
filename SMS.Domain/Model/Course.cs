namespace SMS.Domain.Model
{
    public class Course
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public int DurationMonths { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
