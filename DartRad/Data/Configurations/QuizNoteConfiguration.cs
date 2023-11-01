using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace DartRad.Data.Configurations
{
    public class QuizNoteConfiguration : IEntityTypeConfiguration<QuizNote>
    {
        public void Configure(EntityTypeBuilder<QuizNote> builder)
        {
            builder.HasOne(qn => qn.Editor)
                .WithMany()
                .HasForeignKey(qn => qn.AdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
