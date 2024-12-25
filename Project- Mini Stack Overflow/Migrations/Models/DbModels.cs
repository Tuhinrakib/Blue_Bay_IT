using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project__Mini_Stack_Overflow.Models
{
    public class User
    {
        public int Id { get; set; }
        [Required]
        [StringLength(255, ErrorMessage = "User Name Must be of 255 Characters")]
        [Display(Name = "Username")]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Tags { get; set; }
        public string TrackingId { get; set; }
       
        public DateTime? CreatedAt { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        [NotMapped]
        public long AnswerCount { get; set; }
        public ICollection<Answer>? Answers { get; set; }
        public ICollection<Vote>? Votes { get; set; }
        public int VotesCount
        {
            get
            {
                // The number of upvotes for this question
                return Votes?.Count(v => v.IsUpvote) ?? 0;
            }
        }
    }

    public class Answer
    {
        public int Id { get; set; }
        public string AnswerText { get; set; }
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        public ICollection<Vote>? Votes { get; set; }
    }
    public class Vote
    {
        public int Id { get; set; }
        public bool IsUpvote { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }

        public int QuestionId { get; set; }
        public Question? Question { get; set; }

        public int AnswerId { get; set; }
        public Answer? Answer { get; set; }
    }

    public class QuestionDbContext : DbContext
    {
        public QuestionDbContext(DbContextOptions<QuestionDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Answer -> User
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Configure Answer -> Question
            modelBuilder.Entity<Answer>()
                .HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading delete

            // Configure Vote -> User
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete

            // Configure Vote -> Question
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Question)
                .WithMany(q => q.Votes)
                .HasForeignKey(v => v.QuestionId)
                .OnDelete(DeleteBehavior.Cascade); // Allow cascading delete

            // Configure Vote -> Answer
            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Answer)
                .WithMany(a => a.Votes)
                .HasForeignKey(v => v.AnswerId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascading delete
        }


    }

}
