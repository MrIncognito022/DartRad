using DartRad.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace DartRad.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new SuperAdminConfig());
            modelBuilder.ApplyConfiguration(new ContentCreatorConfiguration());
            modelBuilder.ApplyConfiguration(new QuizConfiguration());
            modelBuilder.ApplyConfiguration(new QuizNoteConfiguration());
        }

        public DbSet<SuperAdmin> SuperAdmin { get; set; }
        public DbSet<Editor> Editor { get; set; }
        public DbSet<ContentCreator> ContentCreator { get; set; }
        public DbSet<ContentCreatorInvite> ContentCreatorInvite { get; set; }
        public DbSet<ResetPasswordRequest> ResetPasswordRequest { get; set; }
        public DbSet<Quiz> Quiz { get; set; }
        public DbSet<QuizNote> QuizNote { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<AnswerMultipleChoice> AnswerMultipleChoice { get; set; }
        public DbSet<AnswerShortAnswer> AnswerShortAnswer { get; set; }
        public DbSet<AnswerMatching> AnswerMatching { get; set; }
        public DbSet<AnswerHotspot> AnswerHotspot { get; set; }
        public DbSet<HotspotQuestionImage> HotspotQuestionImages { get; set; }
        public DbSet<AnswerSequence> AnswerSequence { get; set; }
        public DbSet<Subscriber> Subscriber { get; set; }
    }
}
