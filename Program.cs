using System.Text.Json;
using System.Text.Json.Serialization;
using CdxViz.Models;
using CdxViz.Services;
using CdxViz.Options;
using CommandLine;
using CdxViz.Models.Bom;
using CdxViz.Models.Common;
using CdxViz.Models.Cytoscape;
using CdxViz.Models.ThreeDForceGraph;

namespace CdxViz
{
    /// <summary>
    /// Main program class for the CdxViz application
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point of the application
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <returns>Exit code: 0 for success, 1 for error</returns>
        public static async Task<int> Main(string[] args)
        {
            // Parse command line arguments using CommandLineParser
            var result = Parser.Default.ParseArguments<CommandLineOptions>(args);

            return await result.MapResult(
                async (CommandLineOptions opts) =>
                {
                    // Validate the options before running the application
                    if (!opts.Validate())
                    {
                        return 1;
                    }
                    return await RunApplicationAsync(opts);
                },
                errors => Task.FromResult(1));
        }

        /// <summary>
        /// Creates the appropriate graph factory based on the output format
        /// </summary>
        /// <param name="format">The output format (cytoscape or 3dforce)</param>
        /// <returns>An instance of IGraphFactory for the specified format</returns>
        private static IGraphFactory CreateGraphFactory(string format)
        {
            var normalizedFormat = format.ToLowerInvariant();

            return normalizedFormat switch
            {
                "3dforce" => new ThreedDForceGraphFactory(),
                "cytoscape" => new CystoscapeGraphFactory(),
                _ => new CystoscapeGraphFactory() // Default to cytoscape
            };
        }

        /// <summary>
        /// Runs the main application logic with the provided command line options
        /// </summary>
        /// <param name="options">Parsed command line options</param>
        /// <returns>Exit code: 0 for success, 1 for error</returns>
        private static async Task<int> RunApplicationAsync(CommandLineOptions options)
        {
            try
            {
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };
                jsonOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));

                // Read the CycloneDX SBOM file using streaming async
                await using var fs = File.OpenRead(options.InputFile);
                var bom = await System.Text.Json.JsonSerializer.DeserializeAsync<SimpleBom>(fs, jsonOptions);

                if (bom == null)
                {
                    Console.Error.WriteLine("Error: Could not parse input file as CycloneDX SBOM");
                    return 1;
                }

                // Create the appropriate graph factory based on the output format
                var factory = CreateGraphFactory(options.OutputFormat);

                // Convert to the specified format with the factory
                var converter = new CdxConverter(factory, options);

                // Filter the BOM according to the requested mode
                var filteredBom = FilterBomByMode(bom, options.OnlyVex, options.OnlyVdr);

                var graph = converter.Convert(filteredBom);

                // Handle VEX/VDR mode display
                if (options.OnlyVex || options.OnlyVdr)
                {
                    // Display the main component name if available
                    if (bom.Metadata?.Component?.Name != null)
                    {
                        Console.WriteLine();
                        var description = bom.Metadata?.Component?.Description != null ? $" - {bom.Metadata.Component.Description}" : "";
                        Console.WriteLine($"SBOM Component: {bom.Metadata?.Component?.Name}{description}");
                    }

                    if (filteredBom.Vulnerabilities == null || filteredBom.Vulnerabilities.Length == 0)
                    {
                        Console.WriteLine("No vulnerabilities found in the SBOM.");
                    }
                    else
                    {
                        var vulnCount = filteredBom.Vulnerabilities?.Length ?? 0;
                        var componentCount = filteredBom.Components?.Length ?? 0;

                        if (options.OnlyVex)
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

                // Write the output using streaming async
                await using var ofs = File.Create(options.OutputFile);
                await System.Text.Json.JsonSerializer.SerializeAsync(ofs, graph, jsonOptions);

                var formatName = options.OutputFormat.ToLowerInvariant() == "3dforce" ? "3D Force Graph" : "Cytoscape";
                Console.WriteLine($"Successfully generated {formatName} file: {options.OutputFile}");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return 1;
            }
        }

        /// <summary>
        /// Filters the BOM based on the specified mode (VEX or VDR)
        /// </summary>
        /// <param name="originalBom">The original BOM to filter</param>
        /// <param name="onlyVex">Whether to apply VEX mode filtering</param>
        /// <param name="onlyVdr">Whether to apply VDR mode filtering</param>
        /// <returns>A filtered BOM according to the specified mode</returns>
        private static SimpleBom FilterBomByMode(SimpleBom originalBom, bool onlyVex, bool onlyVdr)
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
                var affectedComponentRefs = originalBom.Vulnerabilities?
                    .SelectMany(v => v.Affects ?? Array.Empty<VulnerabilityAffects>())
                    .Where(a => !string.IsNullOrEmpty(a.Ref))
                    .Select(a => a.Ref!)
                    .ToHashSet() ?? new HashSet<string>();

                // Filter components to keep only affected ones
                var affectedComponents = originalBom.Components?.Where(c =>
                    !string.IsNullOrEmpty(c.BomRef) && affectedComponentRefs.Contains(c.BomRef)
                ).ToArray() ?? new SimpleComponent[0];

                filteredBom.Components = affectedComponents;
                filteredBom.Vulnerabilities = uniqueVulnerabilities; // Use deduplicated vulnerabilities
            }

            return filteredBom;
        }

        /// <summary>
        /// Displays a summary of vulnerabilities grouped by severity
        /// </summary>
        /// <param name="bom">The BOM containing vulnerabilities to summarize</param>
        private static void DisplayVulnerabilitySummary(SimpleBom bom)
        {
            if (bom.Vulnerabilities == null || bom.Vulnerabilities.Length == 0)
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

        /// <summary>
        /// Gets a colorized representation of a number based on severity
        /// </summary>
        /// <param name="number">The number to colorize</param>
        /// <param name="severity">The severity level for color selection</param>
        /// <returns>A colorized string representation of the number</returns>
        private static string GetColorizedNumber(int number, string severity)
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

        /// <summary>
        /// Displays vulnerabilities in a formatted table
        /// </summary>
        /// <param name="bom">The BOM containing vulnerabilities to display</param>
        /// <param name="showComponents">Whether to include affected components in the table</param>
        private static void DisplayVulnerabilitiesTable(SimpleBom bom, bool showComponents)
        {
            if (bom.Vulnerabilities == null || bom.Vulnerabilities.Length == 0)
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
                .OrderByDescending(v =>
                {
                    // Sort first by severity (main priority)
                    var highestSeverity = v.Ratings?.Where(r => !string.IsNullOrEmpty(r.Severity))
                        .Max(r => severityOrder.ContainsKey(r.Severity!.ToLowerInvariant())
                            ? severityOrder[r.Severity.ToLowerInvariant()]
                            : 0) ?? 0;
                    return highestSeverity;
                })
                .ThenByDescending(v =>
                {
                    // Then by CVSS score to break ties between vulnerabilities of the same severity
                    var highestScore = v.Ratings?.Max(r => r.Score) ?? 0;
                    return highestScore;
                })
                .ToList();

            // Calculate column widths
            int maxIdWidth = Math.Max("CVE ID".Length, sortedVulns.Max(v => v.Id?.Length ?? 0));
            int maxSeverityWidth = Math.Max("Severity".Length, 8); // "CRITICAL"
            int maxScoreWidth = Math.Max("CVSS Score".Length, 10);
            int maxMethodWidth = Math.Max("Method".Length, sortedVulns.Max(v =>
            {
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

                var scoreStr = score?.ToString("F1") ?? "N/A";
                var idStr = vuln.Id ?? "N/A";
                var methodStr = method;

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

        /// <summary>
        /// Creates a mapping of component references to their display names
        /// </summary>
        /// <param name="bom">The BOM containing components to map</param>
        /// <returns>A dictionary mapping component references to display names</returns>
        private static Dictionary<string, string> CreateComponentMap(SimpleBom bom)
        {
            var componentMap = new Dictionary<string, string>();

            if (bom.Components != null)
            {
                foreach (var component in bom.Components.Where(c => !string.IsNullOrEmpty(c.BomRef)))
                {
                    var name = component.Name ?? "Unknown";
                    if (!string.IsNullOrEmpty(component.Version))
                    {
                        name += $"@{component.Version}";
                    }
                    componentMap[component.BomRef!] = name;
                }
            }

            return componentMap;
        }

        /// <summary>
        /// Gets the components affected by a specific vulnerability
        /// </summary>
        /// <param name="vulnerability">The vulnerability to check</param>
        /// <param name="componentMap">The mapping of component references to display names</param>
        /// <returns>A string representation of affected components</returns>
        private static string GetComponentsForVulnerability(SimpleVulnerability vulnerability, Dictionary<string, string> componentMap)
        {
            if (vulnerability.Affects?.Length > 0)
            {
                var componentNames = vulnerability.Affects
                    .Where(a => !string.IsNullOrEmpty(a.Ref) && a.Ref != null && componentMap.ContainsKey(a.Ref))
                    .Select(a => componentMap[a.Ref!])
                    .ToList();

                if (componentNames.Any())
                {
                    return string.Join(", ", componentNames);
                }
            }

            return "N/A";
        }

        /// <summary>
        /// Gets a colorized representation of a severity level
        /// </summary>
        /// <param name="severity">The severity level to colorize</param>
        /// <returns>A colorized string representation of the severity</returns>
        private static string GetColorizedSeverity(string severity)
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
    }
}
