using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ConsultationApplication.Data;

public sealed class ConsultationAppDbContextFactory
    : IDesignTimeDbContextFactory<ConsultationAppDbContext>
{
    public ConsultationAppDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable(
            "ConnectionStrings__DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Set ConnectionStrings__DefaultConnection before running Entity Framework migrations.");
        }

        var options = new DbContextOptionsBuilder<ConsultationAppDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new ConsultationAppDbContext(options);
    }
}