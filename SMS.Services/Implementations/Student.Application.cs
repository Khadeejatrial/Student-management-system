using Microsoft.EntityFrameworkCore;
using SMS.Domain.DTOs;
using SMS.Domain.Model;
using SMS.Infrastructure;
using SMS.Services.Interface;
using System;

namespace SMS.Services.Implementations
{
    public class StudentApplication : IStudentApplication
    {
        private readonly AppDbContext _context;
        public StudentApplication(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .Select(s => new StudentDto
                {
                    StudentId = s.StudentId,
                    FirstName = s.FirstName,
                    LastName = s.LastName,
                    Email = s.Email,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    Enrollments = s.Enrollments.Select(e => new EnrollmentDto
                    {
                        EnrollmentId = e.EnrollmentId,
                        CourseId = e.CourseId,
                        Course = new CourseDto
                        {
                            CourseId = e.Course.CourseId,
                            CourseName = e.Course.CourseName,
                            
                        }
                    }).ToList()
                }).ToListAsync();
        }

        
        public async Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            var s = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(x => x.StudentId == id);

            if (s == null) return null;

            return new StudentDto
            {
                StudentId = s.StudentId,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                Gender = s.Gender,
                Enrollments = s.Enrollments.Select(e => new EnrollmentDto
                {
                    EnrollmentId = e.EnrollmentId,
                    CourseId = e.CourseId,
                    Course = new CourseDto
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                       
                    }
                }).ToList()
            };
        }

        
        public async Task<StudentDto> CreateStudentAsync(StudentDto student)
        {

            var entity = new Student
            {

                FirstName = GetFirstName(student),
                LastName = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(entity);
            await _context.SaveChangesAsync();

            return await GetStudentByIdAsync(entity.StudentId)
                ?? throw new Exception("Student creation failed");
        }

        private static string? GetFirstName(StudentDto student)
        {
            return student.FirstName;
        }

        public async Task<StudentDto?> UpdateStudentAsync(int id, StudentDto student)
        {
            var entity = await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (entity == null) return null;

            
            entity.FirstName = student.FirstName;
            entity.LastName = student.LastName;
            entity.Email = student.Email;
            entity.DateOfBirth = student.DateOfBirth;
            entity.Gender = student.Gender;
            entity.UpdatedAt = DateTime.UtcNow;

            
            

            foreach (var e in student.Enrollments)
            {
                var enrollment = entity.Enrollments.FirstOrDefault(en => en.StudentId == id && en.CourseId == e.CourseId);
                if (enrollment != null)
                {
                    enrollment.EnrollmentDate = e.EnrollmentDate ?? enrollment.EnrollmentDate;
                    enrollment.IsActive = e.IsActive;
                    
                }
                else
                {
                    enrollment = new Enrollment
                    {

                        CourseId = e.CourseId,
                        StudentId = id,
                        EnrollmentDate = DateTime.Now,
                        IsActive = e.IsActive
                    };
                    entity.Enrollments.Add(enrollment);

                }
                    

                

                
            }

            await _context.SaveChangesAsync();
            return await GetStudentByIdAsync(entity.StudentId);
        }


        public async Task<StudentDto> CreateStudentWithCoursesAsync(StudentCreateDto dto)
        {
            var student = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                CreatedAt = DateTime.UtcNow
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            var enrollments = dto.Enrollments?.Select(e => new Enrollment
            {
                StudentId = student.StudentId, 
                CourseId = e.CourseId,
                EnrollmentDate = e.EnrollmentDate ?? DateTime.UtcNow,
                IsActive = e.IsActive
            }).ToList() ?? new List<Enrollment>();

            _context.Enrollments.AddRange(enrollments);
            await _context.SaveChangesAsync();

            return await GetStudentByIdAsync(student.StudentId)
                ?? throw new Exception("Student creation failed");
        }
        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
