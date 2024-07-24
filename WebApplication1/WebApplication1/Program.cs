
using Bogus;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Npgsql;

using ProGaudi.MsgPack.Light;
using ProGaudi.Tarantool.Client;
using ProGaudi.Tarantool.Client.Model;

using System.Configuration;
using System.Globalization;

using WebApplication1.Database;
using WebApplication1.Model;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                              {
                                  policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                              });
        });
        builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        const string connectionStringPostgres = "Host=postgres;Port=5432;Database=postgres;Username=postgres;Password=postgres;Include Error Detail=True";
        var dbDataSource = new NpgsqlDataSourceBuilder(connectionStringPostgres).EnableDynamicJson().Build();
        builder.Services.AddDbContext<PostgresDbContext>(opt => opt.UseNpgsql(dbDataSource).UseSnakeCaseNamingConvention());

        builder.Services.AddSingleton<ITarantoolService, TarantoolService>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthorization();

        app.MapGet("/tarantool/reload", async (ITarantoolService tarantool) =>
        {
            var res = await tarantool.Init();
        })
        .WithName("Reload users from postgres")
        .WithOpenApi();

        app.MapGet("/tarantool/get", async (ITarantoolService tarantool, long? id, string? email) =>
        {
            UserTar? res = null;
            if (id != null)
            {
                res = await tarantool.GetById(id.Value);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                res = await tarantool.GetByEmail(email);
            }
            if (res != null) return Results.Ok(res);
            return Results.NotFound();
        })
       .WithName("Get user from tarantool")
       .WithOpenApi();

        app.MapGet("/tarantool/insert", async (ITarantoolService tarantool, int count) =>
        {
            // Create an instance of Faker<User>
            var faker = new Faker<UserTar>()
                .RuleFor(u => u.Id, f => f.IndexGlobal + 1)
                .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                .RuleFor(u => u.LastName, f => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName, f.Internet.DomainName(), f.IndexGlobal.GetHashCode().ToString()))
                .RuleFor(u => u.Gender, f => f.Person.Gender.ToString())
                .RuleFor(u => u.IpAddress, f => f.Internet.Ip())
                .RuleFor(u => u.IsActive, f => f.Random.Bool())
                .RuleFor(u => u.Birthdate, f => f.Date.Past(30).ToUniversalTime().ToString("o", CultureInfo.InvariantCulture))
                .RuleFor(u => u.Score, f => f.Random.Number(0, 100))
                .RuleFor(u => u.UniqueId, f => f.Random.Guid().ToString())
                .RuleFor(u => u.Created, f => f.Date.Past().ToUniversalTime().ToString("o", CultureInfo.InvariantCulture));

            var items = faker.Generate(count);
            await tarantool.Insert(items);
            return Results.Ok();
        })
        .WithName("Insert into tarantool")
        .WithOpenApi();

        app.MapGet("/postgres/get", async (PostgresDbContext db, long? id, string? email) =>
        {
            User? res = null;
            if (id != null)
            {
                res = await db.Users.FirstOrDefaultAsync(o => o.Id == id);
            }
            else if (!string.IsNullOrEmpty(email))
            {
                res = await db.Users.FirstOrDefaultAsync(o => o.Email == email);
            }
            if (res != null)
                return Results.Ok(res);
            return Results.NotFound();
        })
       .WithName("Get user from postgres")
       .WithOpenApi();

        app.MapGet("/postgres/emails", async (PostgresDbContext db) =>
        {
            var items = await db.Users.Select(o => o.Email).ToListAsync();
            return Results.Ok(items);
        })
       .WithName("Get emails from postgres")
       .WithOpenApi();

        // Seed data 
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<DbSeeder>>();
            var seeder = new DbSeeder(context, logger);
            seeder.SeedData();
        }
        app.UseCors();
        app.Run();
    }
}
