using Microsoft.AspNetCore.Mvc;
using SMS.Domain.DTOs;
using SMS.Services.Interface;

namespace Student_management_system.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentApplication _service;
        public StudentController(IStudentApplication service)
        {
            _service = service;
        }

        // Get all students
        [HttpGet]
        public async Task<IActionResult> GetAllStudents()
        {
            var students = await _service.GetAllStudentsAsync();
            return Ok(students);
        }

        // Get student by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _service.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound(new { Message = $"Student with ID {id} not found." });

            return Ok(student);
        }

        // Create new student
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] StudentCreateDto studentDto)
        {
            var result = await _service.CreateStudentWithCoursesAsync(studentDto);
            return CreatedAtAction(nameof(GetStudent), new { id = result.StudentId }, result);
        }

        // Update student
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentDto student)
        {
            var updatedStudent = await _service.UpdateStudentAsync(id, student);
            if (updatedStudent == null) return NotFound();
            return Ok(updatedStudent);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var result = await _service.DeleteStudentAsync(id);
            if (!result) return NotFound(new { Message = $"Student with ID {id} not found." });

            return NoContent();
        }

    }
}
