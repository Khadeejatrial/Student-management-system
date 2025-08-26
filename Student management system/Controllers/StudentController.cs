using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        [HttpGet("get-student/{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid student ID received: {Id}", id);
                throw new ArgumentException("Student ID must be greater than zero.");
            }

            _logger.LogInformation("Returning student details for ID: {Id}", id);
            return Ok(new { Id = id, Name = "John Doe" });
        }
    }
}
