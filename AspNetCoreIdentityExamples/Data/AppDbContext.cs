using AspNetCoreIdentityExamples.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentityExamples.Data
{
    // Databasedə AspNetUsers table-ı AppUser, AspNetRoles table-ı isə AppRole əsasında təşkil olunmasını istədiyimiz üçün parametr kimi veririk.
    // Int parametrini isə AspNetUsers və AspNetRoles-in primary keyləri rəqəm tipində olsun, defaultda stringdir(mətn tipli)
    public class AppDbContext :IdentityDbContext<AppUser,AppRole,int >
    {

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Poests { get; set; }
        // Aşağıdaki konstruktor kodu isə EF Core-un ASP.NET CORE ilə birgə istifadəsində tipik bir yanaşmadır. Contextin konfiqruasiyaları məsələn ConnectionString, Provider və s, servis konfiqurasiyası kimi Startup.csdən(.net 6 sonrası isə Program.csdən) edilir.
        public AppDbContext(DbContextOptions<AppDbContext> dbContext) : base(dbContext) { }
    }

}

