using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MockQueryable.Moq;
using SMS.Infrastructure;
using SMS.Domain.Model;
using SMS.Domain.DTOs;
using SMS.Services.Implementations;
using SMS.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SMS.Tests.ServiceTests
{
    [TestClass]
    public class StudentApplicationTests
    {
        private Mock<AppDbContext> _mockContext;
        private IStudentApplication _studentService;
        private List<Student> _students;

        [TestInitialize]
        public void Setup()
        {
            _students = new List<Student>
            {
                new Student { StudentId = 1, FirstName = "Khadeeja", LastName = "Najeem", Email = "khadeeja@gmail.com", Gender = "F", DateOfBirth = new DateTime(2000,1,1), Enrollments = new List<Enrollment>() },
                new Student { StudentId = 2, FirstName = "Judith", LastName = "Abraham", Email = "judith@gmail.com", Gender = "F", DateOfBirth = new DateTime(2001,2,2), Enrollments = new List<Enrollment>() }
            };

            var mockDbSet = _students.BuildMockDbSet();

            _mockContext = new Mock<AppDbContext>(new DbContextOptions<AppDbContext>());
            _mockContext.Setup(c => c.Students).Returns(mockDbSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            _studentService = new StudentApplication(_mockContext.Object);
        }


        [TestMethod]
        public async Task GetAllStudentsAsync_ShouldReturnAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Judith", result.Last().FirstName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_ShouldReturnStudent_WhenExists()
        {
            var result = await _studentService.GetStudentByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Khadeeja", result.FirstName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var result = await _studentService.GetStudentByIdAsync(99);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task CreateStudentAsync_ShouldAddStudent()
        {
            var newStudent = new StudentDto
            {
                FirstName = "Nazrin",
                LastName = "Farhana",
                Email = "nazrin@gmail.com",
                Gender = "F",
                DateOfBirth = DateTime.UtcNow
            };

            _mockContext.Setup(c => c.Students.Add(It.IsAny<Student>()))
                .Callback<Student>(s =>
                {
                    s.StudentId = _students.Max(x => x.StudentId) + 1;
                    _students.Add(s);
                });

            var result = await _studentService.CreateStudentAsync(newStudent);

            Assert.IsNotNull(result);
            Assert.AreEqual("Nazrin", result.FirstName);
            Assert.AreEqual("nazrin@gmail.com", result.Email);
            Assert.IsTrue(_students.Any(s => s.Email == "nazrin@gmail.com"));
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.AtLeastOnce);
        }




        [TestMethod]
        public async Task CreateStudentAsync_ShouldFail_WhenFirstNameMissing()
        {
            var newStudent = new StudentDto
            {
                FirstName = "",
                LastName = "Farhana",
                Email = "valid@gmail.com"
            };

            var ex = await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _studentService.CreateStudentAsync(newStudent));

            Assert.AreEqual("First name cannot be empty", ex.Message);
        }

        [TestMethod]
        public async Task CreateStudentAsync_ShouldFail_WhenLastNameMissing()
        {
            var newStudent = new StudentDto
            {
                FirstName = "Nazrin",
                LastName = "",
                Email = "valid@gmail.com"
            };

            var ex = await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _studentService.CreateStudentAsync(newStudent));

            Assert.AreEqual("Last name cannot be empty", ex.Message);
        }

        [TestMethod]
        public async Task CreateStudentAsync_ShouldFail_WhenEmailNotGmail()
        {
            var newStudent = new StudentDto
            {
                FirstName = "Nazrin",
                LastName = "Farhana",
                Email = "nazrin@xxx.com"
            };

            var ex = await Assert.ThrowsExactlyAsync<ArgumentException>(() =>
                _studentService.CreateStudentAsync(newStudent));

            Assert.AreEqual("Only Gmail addresses are allowed", ex.Message);
        }


        [TestMethod]
        public async Task UpdateStudentAsync_ShouldUpdate_WhenStudentExists()
        {
            var updateDto = new StudentDto
            {
                StudentId = 1,
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@gmail.com",
                Gender = "M",
                DateOfBirth = DateTime.UtcNow
            };

            var result = await _studentService.UpdateStudentAsync(1, updateDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", result.FirstName);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task UpdateStudentAsync_ShouldReturnNull_WhenNotExists()
        {
            var updateDto = new StudentDto
            {
                StudentId = 99,
                FirstName = "Ghost",
                Email = "ghost@gmail.com"
            };

            var result = await _studentService.UpdateStudentAsync(99, updateDto);

            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task DeleteStudentAsync_ShouldDelete_WhenExists()
        {
            var result = await _studentService.DeleteStudentAsync(1);

            Assert.IsTrue(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.AtLeastOnce);
        }

        [TestMethod]
        public async Task DeleteStudentAsync_ShouldReturnFalse_WhenNotExists()
        {
            var result = await _studentService.DeleteStudentAsync(123);

            Assert.IsFalse(result);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }
    }
}
