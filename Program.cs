using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Superbotti.Services;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        // Pitäisikö docker käynnistää samalla?

        var builder = Host.CreateApplicationBuilder(args);
        builder.Services.AddHttpClient(LocalAIClient.HttpClientName, (sp, client) =>
        {
            var config = sp.GetRequiredService<IConfiguration>();
            client.BaseAddress = new Uri(config["LocalAI:URL"]!.TrimEnd('/') + "/");
            client.Timeout = TimeSpan.FromMinutes(5);
        });

        builder.Services
            .AddDiscordGateway()
            .AddApplicationCommands()
            // Omat luokka instancet
            .AddSingleton<SpeechService>()
            .AddSingleton<LocalAIClient>()
            .AddSingleton<Models>()
            .AddSingleton<SummaryService>()
            .AddSingleton<JokeService>();

        var host = builder.Build();

        // Tarkistus että LocalAI hostiin saa yhteyden?

        host.AddSlashCommand("ping", "Ping!", () => "Pong!"); //testikomento
        host.AddModules(typeof(Program).Assembly); // Lisää kaikki komentomoduulit botille

        await host.RunAsync();
    }
}
