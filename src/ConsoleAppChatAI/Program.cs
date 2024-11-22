using ConsoleAppChatAI.AI;
using ConsoleAppChatAI.Inputs;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;
using System.Text;

var configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();

var logger = new LoggerConfiguration()
    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
    .CreateLogger();

var modelAI = InputHelper.GetModelAI();

using var chatClient = ChatClientFactory.CreateClient(configuration, modelAI);

while (true)
{
    Console.WriteLine();
    logger.Information("Pressione CTRL + C para encerrar a aplicacao...");
    var request = InputHelper.GetRequest();

    var responseStream = chatClient.CompleteStreamingAsync(request);
    var oldColor = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine();
    StringBuilder responseContent = new();
    await foreach (var chunk in responseStream)
    {
        responseContent.Append(chunk.Contents);
        Console.Write(chunk);
    }
    Console.WriteLine();
    Console.ForegroundColor = oldColor;
}