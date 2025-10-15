using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace CdxViz.IntegrationTests;

/// <summary>
/// Integration tests for CdxViz
/// These tests execute the actual CdxViz executable with various parameters
/// and validate the output files.
/// </summary>
public class CdxVizIntegrationTests : IDisposable
{
    private readonly ITestOutputHelper _output;
    private readonly CdxVizExecutor _executor;
    private readonly string _testOutputDirectory;

    public CdxVizIntegrationTests(ITestOutputHelper output)
    {
        _output = output;
        _testOutputDirectory = TestConfiguration.TestOutputDirectory;
        _executor = new CdxVizExecutor(TestConfiguration.ExecutablePath, _testOutputDirectory);

        _output.WriteLine($"Executable Path: {TestConfiguration.ExecutablePath}");
        _output.WriteLine($"Test Output Directory: {_testOutputDirectory}");
        _output.WriteLine($"Formats to Test: {string.Join(", ", TestConfiguration.FormatsToTest)}");
    }

    /// <summary>
    /// Test scenarios covering all supported options
    /// </summary>
    public static IEnumerable<object[]> GetTestScenarios()
    {
        var formatsToTest = TestConfiguration.FormatsToTest;

        foreach (var format in formatsToTest)
        {
            // Basic output without any options
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_Basic",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = Array.Empty<string>(),
                    ExpectedOutputFileName = $"output-{format}-basic.json"
                }
            };

            // With vulnerabilities
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_WithVulnerabilities",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--with-vulns" },
                    ExpectedOutputFileName = $"output-{format}-vulns.json"
                }
            };

            // With licenses
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_WithLicenses",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--with-lics" },
                    ExpectedOutputFileName = $"output-{format}-licenses.json"
                }
            };

            // With vulnerabilities and licenses
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_WithVulnsAndLicenses",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--with-vulns", "--with-lics" },
                    ExpectedOutputFileName = $"output-{format}-vulns-licenses.json"
                }
            };

            // Show groups in node labels
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_ShowGroups",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--show-groups-in-nodes-labels" },
                    ExpectedOutputFileName = $"output-{format}-show-groups.json"
                }
            };

            // VEX mode (only vulnerabilities)
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_OnlyVex",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--only-vex" },
                    ExpectedOutputFileName = $"output-{format}-only-vex.json"
                }
            };

            // VDR mode (vulnerabilities and affected components)
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_OnlyVdr",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--only-vdr" },
                    ExpectedOutputFileName = $"output-{format}-only-vdr.json"
                }
            };

            // Complex scenario: all options combined (except mutually exclusive VEX/VDR)
            yield return new object[]
            {
                new TestScenario
                {
                    Name = $"{format}_Complex",
                    InputFileName = "test-dependency-tree-full.json",
                    OutputFormat = format,
                    Options = new[] { "--with-vulns", "--with-lics", "--show-groups-in-nodes-labels" },
                    ExpectedOutputFileName = $"output-{format}-complex.json"
                }
            };
        }
    }

    [Theory]
    [MemberData(nameof(GetTestScenarios))]
    public async Task ExecuteCdxViz_WithParameters_GeneratesValidOutput(TestScenario scenario)
    {
        // Arrange
        var inputFile = Path.Combine(TestConfiguration.TestDataDirectory, scenario.InputFileName);
        Assert.True(File.Exists(inputFile), $"Input file not found: {inputFile}");

        // Act
        var result = await _executor.ExecuteAsync(scenario, inputFile);

        // Assert - Log execution details
        _output.WriteLine("=== Test Scenario ===");
        _output.WriteLine($"Name: {scenario.Name}");
        _output.WriteLine($"Command Line: {result.CommandLine}");
        _output.WriteLine($"Input File: {result.InputFile}");
        _output.WriteLine($"Output File: {result.OutputFile}");
        _output.WriteLine($"Exit Code: {result.ExitCode}");

        if (!string.IsNullOrWhiteSpace(result.StandardOutput))
        {
            _output.WriteLine($"Standard Output:\n{result.StandardOutput}");
        }

        if (!string.IsNullOrWhiteSpace(result.StandardError))
        {
            _output.WriteLine($"Standard Error:\n{result.StandardError}");
        }

        // Assert - Execution should succeed
        Assert.Equal(0, result.ExitCode);

        // Assert - Output file should exist
        Assert.True(File.Exists(result.OutputFile), $"Output file not created: {result.OutputFile}");

        // Assert - Validate output format
        ValidateOutputFile(result.OutputFile, scenario.OutputFormat);

        var fileInfo = new FileInfo(result.OutputFile);
        _output.WriteLine($"? Test passed - Output file size: {fileInfo.Length} bytes");
        _output.WriteLine("===================");
    }

    /// <summary>
    /// Validate the structure of the output file based on the format
    /// </summary>
    private void ValidateOutputFile(string outputFile, string format)
    {
        var content = File.ReadAllText(outputFile);
        Assert.False(string.IsNullOrWhiteSpace(content), "Output file is empty");

        // Parse as JSON to ensure it's valid
        JsonDocument jsonDoc;
        try
        {
            jsonDoc = JsonDocument.Parse(content);
        }
        catch (JsonException ex)
        {
            Assert.Fail($"Output is not valid JSON: {ex.Message}");
            return;
        }

        using (jsonDoc)
        {
            switch (format.ToLowerInvariant())
            {
                case "cytoscape":
                    ValidateCytoscapeFormat(jsonDoc.RootElement);
                    break;

                case "3dforce":
                    Validate3DForceFormat(jsonDoc.RootElement);
                    break;

                default:
                    Assert.Fail($"Unknown format: {format}");
                    break;
            }
        }
    }

    /// <summary>
    /// Validate Cytoscape JSON format
    /// </summary>
    private void ValidateCytoscapeFormat(JsonElement root)
    {
        Assert.True(root.TryGetProperty("elements", out var elements),
            "Cytoscape JSON should contain 'elements' property");

        // Elements should be an object with 'nodes' and 'edges' arrays
        if (elements.ValueKind == JsonValueKind.Object)
        {
            Assert.True(elements.TryGetProperty("nodes", out var nodes),
                "Cytoscape elements should contain 'nodes' property");
            Assert.Equal(JsonValueKind.Array, nodes.ValueKind);

            Assert.True(elements.TryGetProperty("edges", out var edges),
                "Cytoscape elements should contain 'edges' property");
            Assert.Equal(JsonValueKind.Array, edges.ValueKind);
        }
    }

    /// <summary>
    /// Validate 3D Force Graph JSON format
    /// </summary>
    private void Validate3DForceFormat(JsonElement root)
    {
        Assert.True(root.TryGetProperty("nodes", out var nodes),
            "3D Force JSON should contain 'nodes' property");
        Assert.Equal(JsonValueKind.Array, nodes.ValueKind);

        Assert.True(root.TryGetProperty("links", out var links),
            "3D Force JSON should contain 'links' property");
        Assert.Equal(JsonValueKind.Array, links.ValueKind);
    }

    public void Dispose()
    {
        // Cleanup is optional - we keep output files for inspection
        // If you want to clean up, uncomment the following:
        // if (Directory.Exists(_testOutputDirectory))
        // {
        //     Directory.Delete(_testOutputDirectory, true);
        // }
    }
}
