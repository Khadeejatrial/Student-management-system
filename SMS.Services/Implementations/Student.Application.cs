using SMS.Services.Interface;
using SMS.Domain.Model;

namespace SMS.Services.Implementations
{
    public class StudentApplication : IStudent
    {
        private readonly List<Student> _students = new();

        public IEnumerable<Student> GetAllStudents() => _students;

        public Student? GetStudentById(int id) => _students.FirstOrDefault(s => s.Id == id);

        public Student CreateStudent(Student student)
        {
            student.Id = _students.Count + 1;
            _students.Add(student);
            return student;
        }

        public Student? UpdateStudent(int id, Student student)
        {
            var existing = _students.FirstOrDefault(s => s.Id == id);
            if (existing == null) return null;

            existing.Name = student.Name;
            existing.Age = student.Age;
            existing.Department = student.Department;
            return existing;
        }

        public Student? PatchStudent(int id, string? name, int? age, string? department)
        {
            var existing = _students.FirstOrDefault(s => s.Id == id);
            if (existing == null) return null;

            if (!string.IsNullOrEmpty(name)) existing.Name = name;
            if (age.HasValue) existing.Age = age.Value;
            if (!string.IsNullOrEmpty(department)) existing.Department = department;
            return existing;
        }

        public bool DeleteStudent(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null) return false;

            _students.Remove(student);
            return true;
        }
    }
}
