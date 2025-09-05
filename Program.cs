using System.Text.Json;
using Cdx2Cyto.Models;
using Cdx2Cyto.Services;
using Newtonsoft.Json;

// Parse command line arguments
if (args.Length < 2)
{
    PrintUsage();
    return 1;
}

var inputFile = "";
var outputFile = "";
bool includeVulnerabilities = false;
bool includeLicenses = false;
bool showGroupsInNodeLabels = false;

// Parse arguments
for (int i = 0; i < args.Length; i++)
{
    var arg = args[i];
    
    if (arg == "--vulns")
    {
        includeVulnerabilities = true;
    }
    else if (arg == "--lic")
    {
        includeLicenses = true;
    }
    else if (arg == "--showGroupsInNodeLabels")
    {
        showGroupsInNodeLabels = true;
    }
    else if (string.IsNullOrEmpty(inputFile))
    {
        inputFile = arg;
    }
    else if (string.IsNullOrEmpty(outputFile))
    {
        outputFile = arg;
    }
}

// Validate required arguments
if (string.IsNullOrEmpty(inputFile) || string.IsNullOrEmpty(outputFile))
{
    PrintUsage();
    return 1;
}

try
{
    // Read the CycloneDX SBOM file
    var cdxJson = File.ReadAllText(inputFile);
    var bom = JsonConvert.DeserializeObject<SimpleBom>(cdxJson);

    if (bom == null)
    {
        Console.WriteLine("Error: Could not parse input file as CycloneDX SBOM");
        return 1;
    }

    // Convert to Cytoscape.js format with specified options
    var converter = new CdxToCytoscapeConverter();
    var options = new ConversionOptions
    {
        IncludeVulnerabilities = includeVulnerabilities,
        IncludeLicenses = includeLicenses,
        ShowGroupsInNodeLabels = showGroupsInNodeLabels
    };
    
    var cytoscapeElements = converter.Convert(bom, options);

    // Write the output
    var cytoscapeJson = JsonConvert.SerializeObject(cytoscapeElements, Formatting.Indented);
    File.WriteAllText(outputFile, cytoscapeJson);

    Console.WriteLine($"Successfully converted {inputFile} to {outputFile}");
    Console.WriteLine($"Options: Vulnerabilities={includeVulnerabilities}, Licenses={includeLicenses}, ShowGroupsInNodeLabels={showGroupsInNodeLabels}");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}

void PrintUsage()
{
    Console.WriteLine("Usage: cdx2cyto [options] <input-file> <output-file>");
    Console.WriteLine("  input-file: Path to CycloneDX SBOM JSON file");
    Console.WriteLine("  output-file: Path to write Cytoscape.js JSON output");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --vulns                Include vulnerability nodes and edges");
    Console.WriteLine("  --lic                  Include license nodes and edges");
    Console.WriteLine("  --showGroupsInNodeLabels  Include group names in node labels (default: off)");
    Console.WriteLine();
    Console.WriteLine("If no options are specified, only components and their dependencies will be included.");
}
