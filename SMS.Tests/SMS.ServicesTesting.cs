using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        private AppDbContext _context;
        private IStudentApplication _studentService;

        [TestInitialize]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;

            _context = new AppDbContext(options);

            _context.Students.AddRange(
                new Student { StudentId = 1, FirstName = "Khadeeja", LastName = "Najeem", Email = "khadeeja@gmail.com", Gender = "F", DateOfBirth = new DateTime(2000, 1, 1) },
                new Student { StudentId = 2, FirstName = "Judith", LastName = "Abraham", Email = "judith@gmail.com", Gender = "F", DateOfBirth = new DateTime(2001, 2, 2) }
            );
            _context.SaveChanges();

            _studentService = new StudentApplication(_context);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [TestMethod]
        public async Task GetAllStudentsAsync_ReturnAllStudents()
        {
            var result = await _studentService.GetAllStudentsAsync();

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Judith", result.Last().FirstName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_ReturnStudent_WhenExists()
        {
            var result = await _studentService.GetStudentByIdAsync(1);

            Assert.IsNotNull(result);
            Assert.AreEqual("Khadeeja", result.FirstName);
        }

        [TestMethod]
        public async Task GetStudentByIdAsync_ReturnNull_WhenNotExists()
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

            var result = await _studentService.CreateStudentAsync(newStudent);

            Assert.IsNotNull(result);
            Assert.AreEqual("Nazrin", result.FirstName);
            Assert.AreEqual(3, _context.Students.Count());
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

            Assert.AreEqual("First name is required", ex.Message);
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

            Assert.AreEqual("Last name is required", ex.Message);
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

            Assert.AreEqual("Email must be a valid Gmail address", ex.Message);
        }

        [TestMethod]
        public async Task UpdateStudentAsync_ShouldUpdate_WhenStudentExists()
        {
            var updateDto = new StudentUpdateDto
            {
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated@gmail.com",
                Gender = "M",
                DateOfBirth = DateTime.UtcNow
            };

            var result = await _studentService.UpdateStudentAsync(1, updateDto);

            Assert.IsNotNull(result);
            Assert.AreEqual("Updated", result.FirstName);

            var updatedEntity = await _context.Students.FindAsync(1);
            Assert.AreEqual("Updated", updatedEntity.FirstName);
        }

        [TestMethod]
        public async Task UpdateStudentAsync_ShouldReturnNull_WhenNotExists()
        {
            var updateDto = new StudentUpdateDto
            {
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
            Assert.AreEqual(1, _context.Students.Count());
        }

        [TestMethod]
        public async Task DeleteStudentAsync_ShouldReturnFalse_WhenNotExists()
        {
            var result = await _studentService.DeleteStudentAsync(123);

            Assert.IsFalse(result);
            Assert.AreEqual(2, _context.Students.Count());
        }
    }
}
