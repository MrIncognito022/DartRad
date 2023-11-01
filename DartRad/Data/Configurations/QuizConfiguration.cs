using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DartRad.Data.Configurations
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasMany(x => x.Notes)
                .WithOne(y => y.Quiz)
                .HasForeignKey(y => y.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
