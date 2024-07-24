using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace NbomberTest;


internal class Program
{
    static void Main(string[] args)
    {
        #region get_by_id_tarantool
        var scenarioTarantoolId = Scenario.Create("get_by_id_tarantool", async context =>
                {
                    // Generate a random id
                    var random = new Random();
                    var id = random.Next(1, 1000000);

                    // Define HTTP request
                    var request = Http.CreateRequest("GET", $"https://localhost:59686/tarantool/get?id={id}");

                    // Send the HTTP request
                    using var httpClient = new HttpClient();
                    var response = await Http.Send(httpClient, request);

                    // Check if request was successful
                    if (!response.IsError)
                    {
                        return Response.Ok();
                    }
                    else
                    {
                        return Response.Fail();
                    }
                }).WithLoadSimulations(
                    // it creates 20 copies and keeps them running
                    // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
                    Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30))
                );
        #endregion

        #region get_by_email_tarantool
        var scenarioTarantoolEmail = Scenario.Create("get_by_email_tarantool", async context =>
        {
            // Generate a random id
            var random = new Random();
            var id = random.Next(0, 99);
            var email = Emails.Data100[id];

            // Define HTTP request
            var request = Http.CreateRequest("GET", $"https://localhost:59686/tarantool/get?email={email}");

            // Send the HTTP request
            using var httpClient = new HttpClient();
            var response = await Http.Send(httpClient, request);

            // Check if request was successful
            if (!response.IsError)
            {
                return Response.Ok();
            }
            else
            {
                return Response.Fail();
            }
        }).WithLoadSimulations(
                   // it creates 20 copies and keeps them running
                   // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
                   Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30))
               );
        #endregion

        #region get_by_id_postgres
        var scenarioPostgresId = Scenario.Create("get_by_id_postgres", async context =>
        {
            // Generate a random id
            var random = new Random();
            var id = random.Next(1, 1000000);

            // Define HTTP request
            var request = Http.CreateRequest("GET", $"https://localhost:59686/postgres/get?id={id}");

            // Send the HTTP request
            using var httpClient = new HttpClient();
            var response = await Http.Send(httpClient, request);

            // Check if request was successful
            if (!response.IsError)
            {
                return Response.Ok();
            }
            else
            {
                return Response.Fail();
            }
        }).WithLoadSimulations(
                   // it creates 20 copies and keeps them running
                   // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
                   Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30))
               );
        #endregion

        #region get_by_email_postgres
        var scenarioPostgresEmail = Scenario.Create("get_by_email_postgres", async context =>
        {
            // Generate a random id
            var random = new Random();
            var id = random.Next(0, 99);
            var email = Emails.Data100[id];

            // Define HTTP request
            var request = Http.CreateRequest("GET", $"https://localhost:59686/postgres/get?email={email}");

            // Send the HTTP request
            using var httpClient = new HttpClient();
            var response = await Http.Send(httpClient, request);

            // Check if request was successful
            if (!response.IsError)
            {
                return Response.Ok();
            }
            else
            {
                return Response.Fail();
            }
        }).WithLoadSimulations(
                  // it creates 20 copies and keeps them running
                  // duration: 30 seconds (it executes from [00:00:00] to [00:00:30])
                  Simulation.KeepConstant(copies: 20, during: TimeSpan.FromSeconds(30))
              );
        #endregion

        // Run Scenario
        NBomberRunner
            .RegisterScenarios(scenarioTarantoolEmail)
            .Run();

        Console.WriteLine("Press any key ...");
        Console.ReadKey();
    }
}
