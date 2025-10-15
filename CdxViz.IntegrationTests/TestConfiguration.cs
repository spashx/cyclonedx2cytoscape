using CdxViz.Options;

namespace CdxViz.IntegrationTests;

/// <summary>
/// Configuration for integration tests
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// Path to the CdxViz executable to test
    /// Default: looks for the executable in the bin directory
    /// Can be overridden via environment variable CDXVIZ_EXECUTABLE_PATH
    /// </summary>
    public static string ExecutablePath
    {
        get
        {
            var envPath = Environment.GetEnvironmentVariable("CDXVIZ_EXECUTABLE_PATH");
            if (!string.IsNullOrEmpty(envPath) && File.Exists(envPath))
            {
                return envPath;
            }

            // Default: look for the executable in the CdxViz project bin directory
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var projectRoot = Path.GetFullPath(Path.Combine(baseDirectory, "..", "..", "..", ".."));

            // Try to find in Debug or Release configuration
            var configurations = new[] { "Debug", "Release" };
            foreach (var config in configurations)
            {
                var exePath = Path.Combine(projectRoot, "CdxViz", "bin", config, "net9.0", "CdxViz.exe");
                if (File.Exists(exePath))
                {
                    return exePath;
                }

                // On Linux/Mac, the executable doesn't have .exe extension
                exePath = Path.Combine(projectRoot, "CdxViz", "bin", config, "net9.0", "CdxViz");
                if (File.Exists(exePath))
                {
                    return exePath;
                }
            }

            throw new FileNotFoundException(
                "CdxViz executable not found. Please build the CdxViz project first, or set CDXVIZ_EXECUTABLE_PATH environment variable.");
        }
    }

    /// <summary>
    /// Formats to test
    /// Can be overridden via environment variable CDXVIZ_TEST_FORMATS (comma-separated: e.g., "cytoscape,3dforce")
    /// </summary>
    public static string[] FormatsToTest
    {
        get
        {
            var envFormats = Environment.GetEnvironmentVariable("CDXVIZ_TEST_FORMATS");
            if (!string.IsNullOrEmpty(envFormats))
            {
                return envFormats.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            }

            // Default: test all formats
            return new[] { CommandLineOptions.FORMAT_CYTOSCAPE, CommandLineOptions.FORMAT_3DFORCE };
        }
    }

    /// <summary>
    /// Directory for test outputs
    /// </summary>
    public static string TestOutputDirectory
    {
        get
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var testsOutputDir = Path.Combine(baseDirectory, "TestsOutput");

            if (!Directory.Exists(testsOutputDir))
            {
                Directory.CreateDirectory(testsOutputDir);
            }

            return testsOutputDir;
        }
    }

    /// <summary>
    /// Directory containing test data files
    /// </summary>
    public static string TestDataDirectory
    {
        get
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "TestData");
        }
    }
}
