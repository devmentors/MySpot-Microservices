using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Xunit.Abstractions;

namespace Micro.Testing;

[ExcludeFromCodeCoverage]
public sealed class TestServer : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly Process _process;
    
    public Uri Url { get; }

    public TestServer(string project, ITestOutputHelper output, string url = "http://localhost:9222")
    {
        _output = output;
        Url = new Uri(url);
        var path = Path.Join("..", "..", "..", "..", project);
        _process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --urls {url} --launch-profile {project}.Test",
                WorkingDirectory = path,
                UseShellExecute = false,
                RedirectStandardOutput = true
            }
        };
    }

    public async Task StartAsync()
    {
        _output.WriteLine("Starting the API...");
        _process.Start();
        
        while (!_process.StandardOutput.EndOfStream)
        {
            var line = await _process.StandardOutput.ReadLineAsync();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            _output.WriteLine(line);
            if (line.Contains("Application started"))
            {
                break;
            }
        }

        _output.WriteLine("API has been started.");
    }

    public void Dispose()
    {
        _output.WriteLine("Stopping the API...");
        _process.Kill();
    }
}