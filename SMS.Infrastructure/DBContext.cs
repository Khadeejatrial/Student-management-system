using Microsoft.EntityFrameworkCore;
using SMS.Domain.Model;

namespace SMS.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Student>()
                .Property(s => s.StudentId);
                

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .Property(s => s.Gender)
                .HasMaxLength(1)
                .IsRequired();

            
            modelBuilder.Entity<Course>()
                .HasKey(c => c.CourseId);

            
            modelBuilder.Entity<Enrollment>()
                .HasKey(e => e.EnrollmentId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique();

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);




            modelBuilder.Entity<Student>()
                .ToTable(t => t.HasCheckConstraint("CK_Student_Gender", "Gender IN ('M', 'F', 'O')"));

        }
    }
}
