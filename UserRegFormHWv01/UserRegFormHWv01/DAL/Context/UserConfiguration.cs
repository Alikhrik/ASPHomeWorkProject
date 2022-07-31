using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UserRegFormHWv01.DAL.Context
{
    public class UserConfiguration
        : IEntityTypeConfiguration<Entities.User>
    {
        public void Configure(
            EntityTypeBuilder<Entities.User> builder)
        {
            builder.HasData(new Entities.User
            {
                Id = System.Guid.NewGuid(),
                RealName = "Корневой администратор",
                Login = "Admin",
                PassHash = "",
                Email = "",
                PassSalt = "",
                RegMoment = System.DateTime.Now,
                Avatar = ""
            });
        }
    }
}
