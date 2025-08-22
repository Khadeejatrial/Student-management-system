using Microsoft.AspNetCore.Mvc;

namespace StudentManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentController : ControllerBase
    {
        [HttpGet("get-student/{id}")]
        public IActionResult GetStudent(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Student ID must be greater than zero.");
            }

            return Ok(new { Id = id, Name = "John Doe" });
        }
    }
}
