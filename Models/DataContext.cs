namespace StudentExams.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Configuration;
    using System.Web;

    public partial class DataContext : DbContext
    {
        public DataContext()
            : base(ConfigurationManager.ConnectionStrings["MsSqlConnectionString"].ConnectionString.Replace("{ROOT_DIR}", HttpContext.Current.Server.MapPath("~/")))
        {
            Database.SetInitializer<DataContext>(null);
        }

        public virtual DbSet<ExamList> ExamList { get; set; }
        public virtual DbSet<ExamListDetail> ExamListDetail { get; set; }
        public virtual DbSet<ExamType> ExamType { get; set; }
        public virtual DbSet<Predmet> Predmet { get; set; }
        public virtual DbSet<StudentGroup> StudentGroup { get; set; }
        public virtual DbSet<StudentWork> StudentWork { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserGroup> UserGroup { get; set; }
        public virtual DbSet<WorkType> WorkType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.ExamList)
                .WithRequired(e => e.Teacher)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasMany(e => e.ExamListDetail)
                .WithRequired(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentWork)
                .WithRequired(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<User>()
                .HasMany(e => e.StudentWork1)
                .WithRequired(e => e.Teacher)
                .HasForeignKey(e => e.TeacherId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<WorkType>()
                .HasMany(e => e.StudentWork)
                .WithRequired(e => e.WorkType)
                .HasForeignKey(e => e.WorkTypeId)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Predmet>()
                .HasMany(e => e.StudentWork)
                .WithRequired(e => e.Predmet)
                .HasForeignKey(e => e.PredmetId)
                .WillCascadeOnDelete(true);
                
        }
    }
}
