using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WebAPI.Models
{
    public partial class LicenseExamDBContext : DbContext
    {
        public LicenseExamDBContext()
        {
        }

        public LicenseExamDBContext(DbContextOptions<LicenseExamDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Accounts { get; set; } = null!;
        public virtual DbSet<AccountExam> AccountExams { get; set; } = null!;
        public virtual DbSet<Answer> Answers { get; set; } = null!;
        public virtual DbSet<Exam> Exams { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Progress> Progresses { get; set; } = null!;
        public virtual DbSet<Question> Questions { get; set; } = null!;
        public virtual DbSet<QuestionType> QuestionTypes { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=localhost\\SQLEXPRESS;Initial Catalog=LicenseExamDB;User ID=sa;Password=sa;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasKey(e => e.UserId)
                    .HasName("PK__account__1788CCAC7F5704C3");

                entity.ToTable("account");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Fullname).HasMaxLength(255);

                entity.Property(e => e.Password).HasMaxLength(255);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.Username).HasMaxLength(50);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Accounts)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__account__RoleID__38996AB5");
            });

            modelBuilder.Entity<AccountExam>(entity =>
            {
                entity.HasKey(e => e.UserExamId)
                    .HasName("PK__AccountE__37688871A4EA21B6");

                entity.Property(e => e.UserExamId).HasColumnName("UserExamID");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Exam)
                    .WithMany(p => p.AccountExams)
                    .HasForeignKey(d => d.ExamId)
                    .HasConstraintName("FK__AccountEx__ExamI__5070F446");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AccountExams)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__AccountEx__UserI__4F7CD00D");
            });

            modelBuilder.Entity<Answer>(entity =>
            {
                entity.ToTable("answer");

                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Answers)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__answer__Question__4316F928");
            });

            modelBuilder.Entity<Exam>(entity =>
            {
                entity.ToTable("exam");

                entity.Property(e => e.ExamId).HasColumnName("ExamID");

                entity.Property(e => e.Name).HasMaxLength(100);

                entity.HasMany(d => d.Questions)
                    .WithMany(p => p.Exams)
                    .UsingEntity<Dictionary<string, object>>(
                        "ExamQuestion",
                        l => l.HasOne<Question>().WithMany().HasForeignKey("QuestionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__examQuest__Quest__46E78A0C"),
                        r => r.HasOne<Exam>().WithMany().HasForeignKey("ExamId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__examQuest__ExamI__45F365D3"),
                        j =>
                        {
                            j.HasKey("ExamId", "QuestionId").HasName("PK__examQues__F9A9275F167E8508");

                            j.ToTable("examQuestions");

                            j.IndexerProperty<int>("ExamId").HasColumnName("ExamID");

                            j.IndexerProperty<int>("QuestionId").HasColumnName("QuestionID");
                        });
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.ToTable("Image");

                entity.Property(e => e.ImageId).HasColumnName("ImageID");

                entity.Property(e => e.Link).HasMaxLength(255);
            });

            modelBuilder.Entity<Progress>(entity =>
            {
                entity.ToTable("progress");

                entity.Property(e => e.ProgressId).HasColumnName("ProgressID");

                entity.Property(e => e.AnswerId).HasColumnName("AnswerID");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.UserExamId).HasColumnName("UserExamID");

                entity.HasOne(d => d.Answer)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.AnswerId)
                    .HasConstraintName("FK__progress__Answer__5535A963");

                entity.HasOne(d => d.Question)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.QuestionId)
                    .HasConstraintName("FK__progress__Questi__5441852A");

                entity.HasOne(d => d.UserExam)
                    .WithMany(p => p.Progresses)
                    .HasForeignKey(d => d.UserExamId)
                    .HasConstraintName("FK__progress__UserEx__534D60F1");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.ToTable("questions");

                entity.Property(e => e.QuestionId).HasColumnName("QuestionID");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Questions)
                    .HasForeignKey(d => d.TypeId)
                    .HasConstraintName("FK__questions__TypeI__403A8C7D");

                entity.HasMany(d => d.Images)
                    .WithMany(p => p.Questions)
                    .UsingEntity<Dictionary<string, object>>(
                        "QuestionImage",
                        l => l.HasOne<Image>().WithMany().HasForeignKey("ImageId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__questionI__Image__4CA06362"),
                        r => r.HasOne<Question>().WithMany().HasForeignKey("QuestionId").OnDelete(DeleteBehavior.ClientSetNull).HasConstraintName("FK__questionI__Quest__4BAC3F29"),
                        j =>
                        {
                            j.HasKey("QuestionId", "ImageId").HasName("PK__question__DA9100C21964C984");

                            j.ToTable("questionImage");

                            j.IndexerProperty<int>("QuestionId").HasColumnName("QuestionID");

                            j.IndexerProperty<int>("ImageId").HasColumnName("ImageID");
                        });
            });

            modelBuilder.Entity<QuestionType>(entity =>
            {
                entity.HasKey(e => e.TypeId)
                    .HasName("PK__Question__516F0395EE876BE3");

                entity.ToTable("QuestionType");

                entity.Property(e => e.TypeId).HasColumnName("TypeID");

                entity.Property(e => e.Name).HasMaxLength(50);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("role");

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.RoleName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
