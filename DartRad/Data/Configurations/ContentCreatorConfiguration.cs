using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DartRad.Data.Configurations
{
    public class ContentCreatorConfiguration : IEntityTypeConfiguration<ContentCreator>
    {
        public void Configure(EntityTypeBuilder<ContentCreator> builder)
        {
            builder.HasMany(x => x.Quizzes)
                .WithOne(q => q.ContentCreator)
                .HasForeignKey(q => q.ContentCreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
