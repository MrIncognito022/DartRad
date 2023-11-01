using DartRad.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DartRad.Data
{
    public class SuperAdminConfig : IEntityTypeConfiguration<SuperAdmin>
    {
        public void Configure(EntityTypeBuilder<SuperAdmin> builder)
        {
            builder.HasData(new[]
            {
                new SuperAdmin
                {
                    Id = 1,
                    Email = "info@somuchheart.com",
                    Password = AuthHelper.HashPassword("IAmDRSuperAdmin123!"),
                    CreatedAt = DateTime.Now,
                    Name = "Test Super Editor"
                }
            });
        }
    }
}
