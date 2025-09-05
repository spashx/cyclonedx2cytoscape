namespace Cdx2Cyto.Services;

/// <summary>
/// Options for configuring the CycloneDX to Cytoscape conversion
/// </summary>
public class ConversionOptions
{
    /// <summary>
    /// Whether to include vulnerability nodes and edges in the output
    /// </summary>
    public bool IncludeVulnerabilities { get; set; } = false;

    /// <summary>
    /// Whether to include license nodes and edges in the output
    /// </summary>
    public bool IncludeLicenses { get; set; } = false;
    
    /// <summary>
    /// Whether to include group names in node labels
    /// When false, node labels will be more readable but won't show group information
    /// </summary>
    public bool ShowGroupsInNodeLabels { get; set; } = false;
}
