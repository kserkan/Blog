using Blog.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore;
using Blog.Data.Concrete.EfCore;

public class BlogAppContextFactory : IDesignTimeDbContextFactory<BlogAppContext>
{
    public BlogAppContext CreateDbContext(string[] args)
    {
        var connectionString = "server=localhost;port=3306;database=blogappdb;user=bloguser;password=blogpass;";
        var optionsBuilder = new DbContextOptionsBuilder<BlogAppContext>();
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new BlogAppContext(optionsBuilder.Options);
    }
}
