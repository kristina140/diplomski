using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Blokic.Models
{
    public partial class BlokicContext : DbContext
    {
        public BlokicContext()
        {
        }

        public BlokicContext(DbContextOptions<BlokicContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Course> Course { get; set; }
        public virtual DbSet<CourseInstance> CourseInstance { get; set; }
        public virtual DbSet<Enrolment> Enrolment { get; set; }
        public virtual DbSet<Exam> Exam { get; set; }
        public virtual DbSet<Semester> Semester { get; set; }
        public virtual DbSet<Student> Student { get; set; }
        public virtual DbSet<StudentExam> StudentExam { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    if (!optionsBuilder.IsConfigured)
        //    {
        //        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Blokic;Trusted_Connection=True;");
        //    }
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<CourseInstance>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.SemesterId });

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CourseInstance)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseInstance_Course");

                entity.HasOne(d => d.Semester)
                    .WithMany(p => p.CourseInstance)
                    .HasForeignKey(d => d.SemesterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CourseInstance_Semester");
            });

            modelBuilder.Entity<Enrolment>(entity =>
            {
                entity.HasIndex(e => new { e.CourseId, e.SemesterId, e.StudentId })
                    .HasName("IX_Enrolment");

                entity.HasOne(d => d.Student)
                    .WithMany(p => p.Enrolment)
                    .HasForeignKey(d => d.StudentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrolment_Student");

                entity.HasOne(d => d.CourseInstance)
                    .WithMany(p => p.Enrolment)
                    .HasForeignKey(d => new { d.CourseId, d.SemesterId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Enrolment_CourseInstance");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.HasOne(d => d.CourseInstance)
                    .WithMany(p => p.Exam)
                    .HasForeignKey(d => new { d.CourseId, d.SemesterId })
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Exam_CourseInstance");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.Property(e => e.Firstname).IsRequired();

                entity.Property(e => e.IndexNmb).HasMaxLength(50);

                entity.Property(e => e.Jmbag)
                    .IsRequired()
                    .HasColumnName("JMBAG")
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.Lastname).IsRequired();
            });

            modelBuilder.Entity<StudentExam>(entity =>
            {
                entity.HasKey(e => new { e.EnrolmentId, e.ExamId });

                entity.HasIndex(e => e.Id)
                    .HasName("IX_StudentExam")
                    .IsUnique();

                entity.HasOne(d => d.Enrolment)
                    .WithMany(p => p.StudentExam)
                    .HasForeignKey(d => d.EnrolmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentExam_Enrolment");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.StudentExam)
                    .HasForeignKey(d => d.ExamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_StudentExam_Exam");
            });
        }
    }
}
