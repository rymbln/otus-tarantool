using Bogus;

using Microsoft.EntityFrameworkCore;

using WebApplication1.Database;
using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class DbSeeder
    {
        private PostgresDbContext _db;
        private ILogger _logger;

        public DbSeeder(PostgresDbContext db, ILogger<DbSeeder> logger)
        {
            _db = db;
            _logger = logger;
        }

        public void SeedData()
        {
            _db.Database.EnsureCreated();
            _logger.LogInformation("Database created.");

            var usersCount = _db.Users.Count();
            if (usersCount == 0)
            {
                _logger.LogInformation("Generate 1,000,000 user profiles.");
                var profiles = GenerateUserProfiles(1000000);

                _logger.LogInformation("Inserting profiles");
                _db.Users.BulkInsertOptimized(profiles);
                _db.SaveChanges();
            }
            _logger.LogInformation("User profiles inserted successfully.");
        }

        public List<User> GenerateUserProfiles(int count)
        {
            // Create an instance of Faker<User>
            var faker = new Faker<User>()
                .RuleFor(u => u.Id, f => f.IndexGlobal + 1)
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, f.Internet.DomainName(), f.IndexGlobal.GetHashCode().ToString()))
                .RuleFor(u => u.Gender, f => f.Person.Gender.ToString())
                .RuleFor(u => u.IpAddress, f => f.Internet.Ip())
                .RuleFor(u => u.IsActive, f => f.Random.Bool())
                .RuleFor(u => u.Birthdate, f => f.Date.Past(30).ToUniversalTime())
                .RuleFor(u => u.Score, f => f.Random.Number(0, 100))
                .RuleFor(u => u.UniqueId, f => f.Random.Guid())
                .RuleFor(u => u.Created, f => f.Date.Past().ToUniversalTime())
               ;


            return faker.Generate(count);
        }
    }
}
