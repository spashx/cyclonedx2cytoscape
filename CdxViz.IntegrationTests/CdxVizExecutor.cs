using System.Diagnostics;
using System.Text;

namespace CdxViz.IntegrationTests;

/// <summary>
/// Test scenario definition
/// </summary>
public record TestScenario
{
    public required string Name { get; init; }
    public required string InputFileName { get; init; }
    public required string OutputFormat { get; init; }
    public required string[] Options { get; init; }
    public required string ExpectedOutputFileName { get; init; }
}

/// <summary>
/// Result of executing CdxViz
/// </summary>
public record ExecutionResult
{
    public required int ExitCode { get; init; }
    public required string StandardOutput { get; init; }
    public required string StandardError { get; init; }
    public required string CommandLine { get; init; }
    public required string InputFile { get; init; }
    public required string OutputFile { get; init; }
}

/// <summary>
/// Helper class to execute CdxViz and capture results
/// </summary>
public class CdxVizExecutor
{
    private readonly string _executablePath;
    private readonly string _outputDirectory;

    public CdxVizExecutor(string executablePath, string outputDirectory)
    {
        _executablePath = executablePath;
        _outputDirectory = outputDirectory;
    }

    /// <summary>
    /// Execute CdxViz with the given scenario
    /// </summary>
    public async Task<ExecutionResult> ExecuteAsync(TestScenario scenario, string inputFile)
    {
        var outputFile = Path.Combine(_outputDirectory, scenario.ExpectedOutputFileName);
        
        // Delete output file if it exists
        if (File.Exists(outputFile))
        {
            File.Delete(outputFile);
        }

        var arguments = BuildCommandLine(inputFile, outputFile, scenario.OutputFormat, scenario.Options);
        
        var startInfo = new ProcessStartInfo
        {
            FileName = _executablePath,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = startInfo };
        var outputBuilder = new StringBuilder();
        var errorBuilder = new StringBuilder();

        process.OutputDataReceived += (s, e) => 
        { 
            if (e.Data != null) 
                outputBuilder.AppendLine(e.Data); 
        };
        
        process.ErrorDataReceived += (s, e) => 
        { 
            if (e.Data != null) 
                errorBuilder.AppendLine(e.Data); 
        };

        process.Start();
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        await process.WaitForExitAsync();

        return new ExecutionResult
        {
            ExitCode = process.ExitCode,
            StandardOutput = outputBuilder.ToString(),
            StandardError = errorBuilder.ToString(),
            CommandLine = $"{_executablePath} {arguments}",
            InputFile = inputFile,
            OutputFile = outputFile
        };
    }

    private string BuildCommandLine(string inputFile, string outputFile, string format, string[] options)
    {
        var args = new List<string>
        {
            "--input", $"\"{inputFile}\"",
            "--output", $"\"{outputFile}\"",
            "--format", format
        };
        
        args.AddRange(options);
        return string.Join(" ", args);
    }
}
