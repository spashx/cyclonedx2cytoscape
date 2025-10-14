using System;
using System.IO;
using CommandLine;

namespace CdxViz.Options;

/// <summary>
/// Command line options for the CdxViz application
/// </summary>
public class CommandLineOptions
{
    /// <summary>
    /// Path to the CycloneDX SBOM JSON input file
    /// </summary>
    [Option("input", HelpText = "Path to CycloneDX SBOM JSON input file", Required = true)]
    public string InputFile { get; set; } = string.Empty;

    /// <summary>
    /// Path to write the Cytoscape.js JSON output file
    /// </summary>
    [Option("output", HelpText = "Path to output file", Required = true)]
    public string OutputFile { get; set; } = string.Empty;

    /// <summary>
    /// Output format: cytoscape or 3dforce
    /// </summary>
    [Option("format", HelpText = "Output format: cytoscape or 3dforce (default: cytoscape)", Default = "cytoscape")]
    public string OutputFormat { get; set; } = "cytoscape";

    /// <summary>
    /// Include vulnerability nodes and edges in the output
    /// </summary>
    [Option("with-vulns", HelpText = "Include vulnerability nodes and edges into cytoscape output")]
    public bool IncludeVulnerabilities { get; set; }

    /// <summary>
    /// Include license nodes and edges in the output
    /// </summary>
    [Option("with-lic", HelpText = "Include license nodes and edges")]
    public bool IncludeLicenses { get; set; }

    /// <summary>
    /// Include group names in node labels
    /// </summary>
    [Option("show-groups-in-nodes-labels", HelpText = "Include group names in nodes labels (default: off)")]
    public bool ShowGroupsInNodesLabels { get; set; }

    /// <summary>
    /// Output only vulnerability information (VEX mode)
    /// </summary>
    [Option("only-vex", HelpText = "Output only vulnerability information (VEX mode)")]
    public bool OnlyVex { get; set; }

    /// <summary>
    /// Output vulnerabilities and affected components (VDR mode)
    /// </summary>
    [Option("only-vdr", HelpText = "Output vulnerabilities and affected components (VDR mode)")]
    public bool OnlyVdr { get; set; }

    public bool Validate()
    {
        // Mutually exclusive options
        if (OnlyVex && OnlyVdr)
        {
            Console.Error.WriteLine("Error: --only-vex and --only-vdr options are mutually exclusive");
            return false;
        }

        // Validate output format
        var normalizedFormat = OutputFormat.ToLowerInvariant();
        if (normalizedFormat != "cytoscape" && normalizedFormat != "3dforce")
        {
            Console.Error.WriteLine("Error: --format must be either 'cytoscape' or '3dforce'");
            return false;
        }

        // If VEX/VDR mode requested, ensure vulnerabilities are included
        if (OnlyVex || OnlyVdr)
        {
            IncludeVulnerabilities = true;
        }

        // Validate input file
        if (string.IsNullOrWhiteSpace(InputFile))
        {
            Console.Error.WriteLine("Error: input file path is required.");
            return false;
        }

        string inputFull;
        try
        {
            inputFull = Path.GetFullPath(InputFile);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: cannot resolve input file path '{InputFile}': {ex.Message}");
            return false;
        }

        if (!File.Exists(inputFull))
        {
            Console.Error.WriteLine($"Error: input file not found: {inputFull}");
            return false;
        }

        // Validate output file path
        if (string.IsNullOrWhiteSpace(OutputFile))
        {
            Console.Error.WriteLine("Error: output file path is required.");
            return false;
        }

        string outputFull;
        try
        {
            outputFull = Path.GetFullPath(OutputFile);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: cannot resolve output file path '{OutputFile}': {ex.Message}");
            return false;
        }

        var outDir = Path.GetDirectoryName(outputFull);
        if (!string.IsNullOrEmpty(outDir))
        {
            try
            {
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: cannot create output directory '{outDir}': {ex.Message}");
                return false;
            }
        }

        return true;
    }
}