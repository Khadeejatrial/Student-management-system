using SMS.Domain.Model;

namespace SMS.Services.Interface
{
    public interface IStudent
    {
        IEnumerable<Student> GetAllStudents();
        Student? GetStudentById(int id);
        Student CreateStudent(Student student);
        Student? UpdateStudent(int id, Student student);
        Student? PatchStudent(int id, string? name, int? age, string? department);
        bool DeleteStudent(int id);
    }
}