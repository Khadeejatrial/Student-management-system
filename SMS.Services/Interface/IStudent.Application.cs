using SMS.Domain.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SMS.Services.Interface
{
    public interface IStudentApplication
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto?> CreateStudentAsync(StudentDto student);
        Task<StudentDto?> UpdateStudentAsync(int id, StudentDto student);
        Task<StudentDto> CreateStudentWithCoursesAsync(StudentCreateDto dto);
        Task<bool> DeleteStudentAsync(int id);
    }
}
