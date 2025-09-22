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
    
    /// <summary>
    /// Whether to output only vulnerability information (VEX mode)
    /// When true, only vulnerability nodes are included in the output
    /// </summary>
    public bool OnlyVex { get; set; } = false;
    
    /// <summary>
    /// Whether to output only vulnerabilities and affected components (VDR mode)
    /// When true, only vulnerability nodes and their affected component nodes are included in the output
    /// </summary>
    public bool OnlyVdr { get; set; } = false;
}
