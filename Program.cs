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
bool onlyVex = false;
bool onlyVdr = false;

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
    else if (arg == "--only-vex")
    {
        onlyVex = true;
    }
    else if (arg == "--only-vdr")
    {
        onlyVdr = true;
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

// Validate mutually exclusive options
if (onlyVex && onlyVdr)
{
    Console.WriteLine("Error: --only-vex and --only-vdr options are mutually exclusive");
    return 1;
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
        ShowGroupsInNodeLabels = showGroupsInNodeLabels,
        OnlyVex = onlyVex,
        OnlyVdr = onlyVdr
    };
    
    // Filter the BOM according to the requested mode
    var filteredBom = FilterBomByMode(bom, onlyVex, onlyVdr);
    
    var cytoscapeElements = converter.Convert(filteredBom, options);

    // Handle VEX/VDR mode display
    if (onlyVex || onlyVdr)
    {
        // Display the main component name if available
        if (bom.Metadata?.Component?.Name != null)
        {
            Console.WriteLine();
            Console.WriteLine($"SBOM Component: {bom.Metadata.Component.Name}");            
        }

        if (filteredBom.Vulnerabilities?.Length == 0)
        {
            Console.WriteLine("No vulnerabilities found in the SBOM.");
        }
        else
        {
            var vulnCount = filteredBom.Vulnerabilities?.Length ?? 0;
            var componentCount = filteredBom.Components?.Length ?? 0;
            
            if (onlyVex)
            {
                Console.WriteLine($"Found {vulnCount} vulnerabilities:");
                DisplayVulnerabilitySummary(filteredBom);
                Console.WriteLine();
                DisplayVulnerabilitiesTable(filteredBom, false); // false = do not display components
            }
            else // onlyVdr
            {
                Console.WriteLine($"Found {vulnCount} vulnerabilities affecting {componentCount} components:");
                DisplayVulnerabilitySummary(filteredBom);
                Console.WriteLine();
                DisplayVulnerabilitiesTable(filteredBom, true); // true = display components
            }
        }
    }

    // Write the output
    var cytoscapeJson = JsonConvert.SerializeObject(cytoscapeElements, Formatting.Indented);
    File.WriteAllText(outputFile, cytoscapeJson);

    Console.WriteLine($"Successfully converted {inputFile} to {outputFile}");
    Console.WriteLine($"Options: Vulnerabilities={includeVulnerabilities}, Licenses={includeLicenses}, ShowGroupsInNodeLabels={showGroupsInNodeLabels}, OnlyVex={onlyVex}, OnlyVdr={onlyVdr}");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}

SimpleBom FilterBomByMode(SimpleBom originalBom, bool onlyVex, bool onlyVdr)
{
    if (!onlyVex && !onlyVdr)
    {
        return originalBom; // No filtering necessary
    }

    var filteredBom = new SimpleBom
    {
        Metadata = originalBom.Metadata,
        Dependencies = originalBom.Dependencies
    };

    // Deduplication of vulnerabilities by ID
    var uniqueVulnerabilities = originalBom.Vulnerabilities?
        .GroupBy(v => v.Id ?? "Unknown")
        .Select(g => g.First()) // Take the first occurrence of each unique vulnerability
        .ToArray() ?? new SimpleVulnerability[0];

    if (onlyVex)
    {
        // VEX mode: keep only vulnerabilities (deduplicated)
        filteredBom.Vulnerabilities = uniqueVulnerabilities;
        // No components in VEX mode
        filteredBom.Components = new SimpleComponent[0];
    }
    else if (onlyVdr)
    {
        // VDR mode: keep vulnerabilities AND affected components
        var affectedComponentRefs = new HashSet<string>();
        
        // Identify all components affected by vulnerabilities (use original vulnerabilities for this analysis)
        if (originalBom.Vulnerabilities != null)
        {
            foreach (var vulnerability in originalBom.Vulnerabilities)
            {
                if (vulnerability.Affects != null)
                {
                    foreach (var affect in vulnerability.Affects)
                    {
                        if (!string.IsNullOrEmpty(affect.Ref))
                        {
                            affectedComponentRefs.Add(affect.Ref);
                        }
                    }
                }
            }
        }
        
        // Filter components to keep only affected ones
        var affectedComponents = originalBom.Components?.Where(c => 
            !string.IsNullOrEmpty(c.BomRef) && affectedComponentRefs.Contains(c.BomRef)
        ).ToArray() ?? new SimpleComponent[0];
        
        filteredBom.Components = affectedComponents;
        filteredBom.Vulnerabilities = uniqueVulnerabilities; // Use deduplicated vulnerabilities
    }

    return filteredBom;
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
    Console.WriteLine("  --only-vex             Output only vulnerability information (VEX mode)");
    Console.WriteLine("  --only-vdr             Output vulnerabilities and affected components (VDR mode)");
    Console.WriteLine();
    Console.WriteLine("Note: --only-vex and --only-vdr options are mutually exclusive.");
    Console.WriteLine("If no options are specified, only components and their dependencies will be included.");
}

void DisplayVulnerabilitySummary(SimpleBom bom)
{
    if (bom.Vulnerabilities?.Length == 0)
    {
        return;
    }

    var vulnerabilities = bom.Vulnerabilities ?? new SimpleVulnerability[0];

    // Count vulnerabilities by severity
    var severityCounts = new Dictionary<string, int>
    {
        { "critical", 0 },
        { "high", 0 },
        { "medium", 0 },
        { "low", 0 },
        { "unknown", 0 }
    };

    foreach (var vuln in vulnerabilities)
    {
        var highestRating = vuln.Ratings?.OrderByDescending(r => r.Score ?? 0).FirstOrDefault();
        var severity = (highestRating?.Severity ?? "unknown").ToLowerInvariant();
        
        if (severityCounts.ContainsKey(severity))
        {
            severityCounts[severity]++;
        }
        else
        {
            severityCounts["unknown"]++;
        }
    }

    // Display summary with colors
    var summaryParts = new List<string>();
    
    if (severityCounts["critical"] > 0)
        summaryParts.Add($"{GetColorizedNumber(severityCounts["critical"], "critical")} {GetColorizedSeverity("CRITICAL")}");
    if (severityCounts["high"] > 0)
        summaryParts.Add($"{GetColorizedNumber(severityCounts["high"], "high")} {GetColorizedSeverity("HIGH")}");
    if (severityCounts["medium"] > 0)
        summaryParts.Add($"{GetColorizedNumber(severityCounts["medium"], "medium")} {GetColorizedSeverity("MEDIUM")}");
    if (severityCounts["low"] > 0)
        summaryParts.Add($"{GetColorizedNumber(severityCounts["low"], "low")} {GetColorizedSeverity("LOW")}");
    if (severityCounts["unknown"] > 0)
        summaryParts.Add($"{severityCounts["unknown"]} UNKNOWN");

    if (summaryParts.Any())
    {
        Console.WriteLine($"  {string.Join(", ", summaryParts)}");
    }
}

string GetColorizedNumber(int number, string severity)
{
    var numberStr = number.ToString();
    
    return severity.ToLowerInvariant() switch
    {
        "critical" => $"\u001b[91m{numberStr}\u001b[0m", // Red
        "high" => $"\u001b[38;5;208m{numberStr}\u001b[0m", // Orange
        "medium" => $"\u001b[93m{numberStr}\u001b[0m",   // Yellow
        "low" => $"\u001b[92m{numberStr}\u001b[0m",      // Green
        _ => numberStr // No color for unknown
    };
}

void DisplayVulnerabilitiesTable(SimpleBom bom, bool showComponents)
{
    if (bom.Vulnerabilities?.Length == 0)
    {
        Console.WriteLine("No vulnerabilities found.");
        return;
    }

    var vulnerabilities = bom.Vulnerabilities ?? new SimpleVulnerability[0];

    // Sort by CVSS score descending, then by severity order
    var severityOrder = new Dictionary<string, int>
    {
        { "critical", 4 },
        { "high", 3 },
        { "medium", 2 },
        { "low", 1 },
        { "unknown", 0 },
        { "none", 0 }
    };

    var sortedVulns = vulnerabilities
        .OrderByDescending(v => {
            // Sort first by severity (main priority)
            var highestSeverity = v.Ratings?.Where(r => !string.IsNullOrEmpty(r.Severity))
                .Max(r => severityOrder.ContainsKey(r.Severity!.ToLowerInvariant()) 
                    ? severityOrder[r.Severity.ToLowerInvariant()] 
                    : 0) ?? 0;
            return highestSeverity;
        })
        .ThenByDescending(v => {
            // Then by CVSS score to break ties between vulnerabilities of the same severity
            var highestScore = v.Ratings?.Max(r => r.Score) ?? 0;
            return highestScore;
        })
        .ToList();

    // Calculate column widths
    int maxIdWidth = Math.Max("CVE ID".Length, sortedVulns.Max(v => v.Id?.Length ?? 0));
    int maxSeverityWidth = Math.Max("Severity".Length, 8); // "CRITICAL"
    int maxScoreWidth = Math.Max("CVSS Score".Length, 10);
    int maxMethodWidth = Math.Max("Method".Length, sortedVulns.Max(v => {
        var highestRating = v.Ratings?.OrderByDescending(r => r.Score ?? 0).FirstOrDefault();
        var method = highestRating?.Method ?? "N/A";
        return method.Length;
    }));
    int maxComponentWidth = 0;

    // Calculate component width if necessary
    if (showComponents)
    {
        var compMap = CreateComponentMap(bom);
        maxComponentWidth = Math.Max("Components".Length, 
            sortedVulns.Max(v => GetComponentsForVulnerability(v, compMap).Length));
    }

    // Create table format
    string separator;
    if (showComponents)
    {
        separator = new string('-', maxIdWidth + maxSeverityWidth + maxScoreWidth + maxMethodWidth + maxComponentWidth + 16);
    }
    else
    {
        separator = new string('-', maxIdWidth + maxSeverityWidth + maxScoreWidth + maxMethodWidth + 12);
    }

    // Print header
    Console.WriteLine(separator);
    Console.Write("| ");
    Console.Write("CVE ID".PadRight(maxIdWidth));
    Console.Write(" | ");
    Console.Write("Severity".PadRight(maxSeverityWidth));
    Console.Write(" | ");
    Console.Write("CVSS Score".PadRight(maxScoreWidth));
    Console.Write(" | ");
    Console.Write("Method".PadRight(maxMethodWidth));
    if (showComponents)
    {
        Console.Write(" | ");
        Console.Write("Components".PadRight(maxComponentWidth));
    }
    Console.WriteLine(" |");
    Console.WriteLine(separator);

    // Print rows
    var componentMap = showComponents ? CreateComponentMap(bom) : new Dictionary<string, string>();
    
    foreach (var vuln in sortedVulns)
    {
        var highestRating = vuln.Ratings?.OrderByDescending(r => r.Score ?? 0).FirstOrDefault();
        var severity = highestRating?.Severity ?? "Unknown";
        var score = highestRating?.Score;
        var method = highestRating?.Method ?? "N/A";
        
        var coloredSeverity = GetColorizedSeverity(severity);
        var scoreStr = score?.ToString("F1") ?? "N/A";
        var idStr = vuln.Id ?? "N/A";
        var methodStr = method;
        
        // Calculate the extra length added by ANSI color codes for proper alignment
        var colorCodeLength = coloredSeverity.Length - severity.ToUpperInvariant().Length;
        
        Console.Write("| ");
        Console.Write(idStr.PadRight(maxIdWidth));
        Console.Write(" | ");
        // For severity, first pad the text without color, then apply the color
        var paddedSeverity = severity.ToUpperInvariant().PadRight(maxSeverityWidth);
        var coloredPaddedSeverity = GetColorizedSeverity(paddedSeverity);
        Console.Write(coloredPaddedSeverity);
        Console.Write(" | ");
        Console.Write(scoreStr.PadRight(maxScoreWidth));
        Console.Write(" | ");
        Console.Write(methodStr.PadRight(maxMethodWidth));
        
        if (showComponents)
        {
            var componentsStr = GetComponentsForVulnerability(vuln, componentMap);
            Console.Write(" | ");
            Console.Write(componentsStr.PadRight(maxComponentWidth));
        }
        
        Console.WriteLine(" |");
    }

    Console.WriteLine(separator);
}

Dictionary<string, string> CreateComponentMap(SimpleBom bom)
{
    var componentMap = new Dictionary<string, string>();
    
    if (bom.Components != null)
    {
        foreach (var component in bom.Components)
        {
            if (!string.IsNullOrEmpty(component.BomRef))
            {
                var name = component.Name ?? "Unknown";
                if (!string.IsNullOrEmpty(component.Version))
                {
                    name += $"@{component.Version}";
                }
                componentMap[component.BomRef] = name;
            }
        }
    }
    
    return componentMap;
}

string GetComponentsForVulnerability(SimpleVulnerability vulnerability, Dictionary<string, string> componentMap)
{
    if (vulnerability.Affects?.Length > 0)
    {
        var componentNames = vulnerability.Affects
            .Where(a => !string.IsNullOrEmpty(a.Ref) && componentMap.ContainsKey(a.Ref!))
            .Select(a => componentMap[a.Ref!])
            .ToList();
            
        if (componentNames.Any())
        {
            return string.Join(", ", componentNames);
        }
    }
    
    return "N/A";
}

string GetColorizedSeverity(string severity)
{
    var normalizedSeverity = severity.Trim().ToLowerInvariant();
    var upperSeverity = severity.ToUpperInvariant();
    
    return normalizedSeverity switch
    {
        "critical" => $"\u001b[91m{upperSeverity}\u001b[0m", // Red
        "high" => $"\u001b[38;5;208m{upperSeverity}\u001b[0m", // Orange (256 colors)
        "medium" => $"\u001b[93m{upperSeverity}\u001b[0m",   // Yellow
        "low" => $"\u001b[92m{upperSeverity}\u001b[0m",      // Green
        _ => upperSeverity // No color for unknown/none but still uppercase
    };
}
