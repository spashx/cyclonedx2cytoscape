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
    [Option("output", HelpText = "Path to Cytoscape.js JSON output file", Required = true)]
    public string OutputFile { get; set; } = string.Empty;

    /// <summary>
    /// Include vulnerability nodes and edges in the output
    /// </summary>
    [Option("vulns", HelpText = "Include vulnerability nodes and edges")]
    public bool IncludeVulnerabilities { get; set; }

    /// <summary>
    /// Include license nodes and edges in the output
    /// </summary>
    [Option("lic", HelpText = "Include license nodes and edges")]
    public bool IncludeLicenses { get; set; }

    /// <summary>
    /// Include group names in node labels
    /// </summary>
    [Option("showGroupsInNodeLabels", HelpText = "Include group names in node labels (default: off)")]
    public bool ShowGroupsInNodeLabels { get; set; }

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
        // Validate mutually exclusive options
        if (OnlyVex && OnlyVdr)
        {
            Console.WriteLine("Error: --only-vex and --only-vdr options are mutually exclusive");
            return false;
        }
        return true;
    }
}