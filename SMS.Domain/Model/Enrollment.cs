namespace SMS.Domain.Model
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;

        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
