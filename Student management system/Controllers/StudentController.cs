using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using SMS.Domain.Model;

namespace StudentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private static readonly List<Student> _students = new();
        private readonly ILogger<StudentsController> _logger;

        public StudentsController(ILogger<StudentsController> logger)
        {
            _logger = logger;
        }

        
        [HttpPost]
        public IActionResult CreateStudent([FromBody] Student student)
        {
            if (string.IsNullOrWhiteSpace(student.Name) || student.Age <= 0)
            {
                _logger.LogWarning("Invalid student data submitted");
                return BadRequest(new { Message = "Name cannot be empty and Age must be greater than zero." });
            }

            student.Id = _students.Count + 1;
            _students.Add(student);

            _logger.LogInformation("Student created with ID: {Id}", student.Id);

            return CreatedAtAction(nameof(GetStudentById), new { id = student.Id }, student);
        }

        
        [HttpGet]
        public IActionResult GetAllStudents()
        {
            _logger.LogInformation("Retrieved all students");
            return Ok(_students);
        }

       
        [HttpGet("{id}")]
        public IActionResult GetStudentById(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                _logger.LogWarning("Student with ID {Id} not found", id);
                return NotFound(new { Message = $"Student with ID {id} not found." });
            }
            return Ok(student);
        }

        
        [HttpPut("{id}")]
        public IActionResult UpdateStudent(int id, [FromBody] Student updatedStudent)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(new { Message = $"Student with ID {id} not found." });
            }

            if (string.IsNullOrWhiteSpace(updatedStudent.Name) || updatedStudent.Age <= 0)
            {
                return BadRequest(new { Message = "Name cannot be empty and Age must be greater than zero." });
            }

            student.Name = updatedStudent.Name;
            student.Age = updatedStudent.Age;
            student.Department = updatedStudent.Department;

            _logger.LogInformation("Student with ID {Id} updated", id);
            return Ok(student);
        }

        
        [HttpPatch("{id}")]
        public IActionResult PartiallyUpdateStudent(int id, [FromBody] JsonPatchDocument<Student> patchDoc)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(new { Message = $"Student with ID {id} not found." });
            }

            patchDoc.ApplyTo(student);
            _logger.LogInformation("Student with ID {Id} partially updated", id);
            return Ok(student); 
        }

       
        [HttpDelete("{id}")]
        public IActionResult DeleteStudent(int id)
        {
            var student = _students.FirstOrDefault(s => s.Id == id);
            if (student == null)
            {
                return NotFound(new { Message = $"Student with ID {id} not found." });
            }

            _students.Remove(student);
            _logger.LogInformation("Student with ID {Id} deleted", id);
            return NoContent();
        }
    }
}
