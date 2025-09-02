using SMS.Domain.Model;
using System;
namespace SMS.Domain.DTOs
{
    public class StudentDto
    {
        public int StudentId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public List<EnrollmentDto> Enrollments { get; set; } = new List<EnrollmentDto>();
    }

    public class EnrollmentDto
    {
        public int EnrollmentId { get; set; }
        public int CourseId { get; set; }
        public CourseDto? Course { get; set; }
        public DateTime? EnrollmentDate { get; set; } 
        public bool IsActive { get; set; } = true;
    }


    public class CourseDto
    {
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        
    }

    public class StudentCreateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public List<EnrollmentCreateDto> Enrollments { get; set; } = new List<EnrollmentCreateDto>();
    }

    public class EnrollmentCreateDto
    {
        public int CourseId { get; set; }
        public DateTime? EnrollmentDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
