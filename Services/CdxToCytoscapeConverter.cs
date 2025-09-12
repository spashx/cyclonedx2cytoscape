using Cdx2Cyto.Models;

namespace Cdx2Cyto.Services;

/// <summary>
/// Converts CycloneDX SBOM data to Cytoscape graph format.
/// This converter handles components, vulnerabilities, licenses, and their relationships.
/// The converter can be configured to include or exclude vulnerabilities and licenses
/// based on the options provided.
/// </summary>
public class CdxToCytoscapeConverter
{
    private ConversionOptions _options = new ConversionOptions();

    /// <summary>
    /// Converts a CycloneDX SBOM to a Cytoscape graph format with default options
    /// </summary>
    /// <param name="bom">The SBOM to convert</param>
    /// <returns>A graph representation suitable for Cytoscape visualization</returns>
    public CytoscapeGraph Convert(SimpleBom bom)
    {
        return Convert(bom, new ConversionOptions());
    }

    /// <summary>
    /// Converts a CycloneDX SBOM to a Cytoscape graph format with specified options
    /// </summary>
    /// <param name="bom">The SBOM to convert</param>
    /// <param name="options">Options to configure the conversion</param>
    /// <returns>A graph representation suitable for Cytoscape visualization</returns>
    public CytoscapeGraph Convert(SimpleBom bom, ConversionOptions options)
    {
        _options = options;
        var graph = new CytoscapeGraph();
      
        // Add root component if it exists
        // The root component represents the main application or project
        if (bom.Metadata?.Component != null)
        {
            AddComponentNode(graph.Elements, bom.Metadata.Component);
        }

        // Add all components as nodes
        // Components represent libraries, frameworks, or other dependencies
        if (bom.Components != null)
        {
            foreach (var component in bom.Components)
            {
                AddComponentNode(graph.Elements, component);
            }
        }

        // Process licenses from all components if enabled
        // Only generates license nodes and edges if IncludeLicenses is true
        if (options.IncludeLicenses && bom.Components != null)
        {
            var uniqueLicenses = new HashSet<string>();
            
            // Extract unique licenses from all components
            // This ensures each license is only represented once in the graph
            // even if multiple components use the same license
            foreach (var component in bom.Components)
            {
                if (component.Licenses != null)
                {
                    foreach (var licenseEntry in component.Licenses)
                    {
                        if (licenseEntry.License?.Id != null)
                        {
                            uniqueLicenses.Add(licenseEntry.License.Id);
                        }
                    }
                }
            }
            
            // Add license nodes
            // Each unique license is represented as a separate node
            foreach (var licenseId in uniqueLicenses)
            {
                AddLicenseNode(graph.Elements, licenseId);
            }
            
            // Connect components to their licenses
            // Creates edges from licenses to components to show which licenses apply to each component
            foreach (var component in bom.Components)
            {
                if (component.Licenses != null && component.BomRef != null)
                {
                    foreach (var licenseEntry in component.Licenses)
                    {
                        if (licenseEntry.License?.Id != null)
                        {
                            AddLicenseEdge(graph.Elements, component.BomRef, licenseEntry.License.Id);
                        }
                    }
                }
            }
        }

        // Add vulnerabilities as nodes if enabled
        // Only generates vulnerability nodes and edges if IncludeVulnerabilities is true
        if (options.IncludeVulnerabilities && bom.Vulnerabilities != null)
        {
            foreach (var vulnerability in bom.Vulnerabilities)
            {
                AddVulnerabilityNode(graph.Elements, vulnerability);
            }
            
            // Add vulnerability affects edges
            // Creates edges from vulnerabilities to components to show which components are affected
            foreach (var vulnerability in bom.Vulnerabilities)
            {
                AddVulnerabilityEdges(graph.Elements, vulnerability);
            }
        }

        // Add dependencies as edges
        // Dependencies represent relationships between components
        // For example, if A depends on B, there will be an edge from A to B
        if (bom.Dependencies != null)
        {
            foreach (var dependency in bom.Dependencies)
            {
                AddDependencyEdges(graph.Elements, dependency);
            }
        }

        // Propagate vulnerability severity up the dependency tree if vulnerabilities are included
        // This ensures that if a component has a vulnerable dependency, the component itself
        // is marked with the highest severity of any of its dependencies
        if (options.IncludeVulnerabilities)
        {
            PropagateSeverityToParents(graph);
        }

        return graph;
    }
    
    /// <summary>
    /// Adds a license node to the graph
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="licenseId">The license ID to add</param>
    private void AddLicenseNode(CytoscapeElements elements, string licenseId)
    {
        // Create a license node with a consistent ID format
        // The "license-" prefix ensures uniqueness and identification in the graph
        var nodeData = new LicenseNodeData
        {
            Id = $"license-{licenseId}",
            Label = licenseId
        };

        elements.AddNode(new CytoscapeNode(nodeData));
    }
    
    /// <summary>
    /// Adds an edge between a license and a component
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="componentBomRef">The component BOM reference</param>
    /// <param name="licenseId">The license ID</param>
    private void AddLicenseEdge(CytoscapeElements elements, string componentBomRef, string licenseId)
    {
        // Create an edge from the license to the component
        // This represents that the license applies to the component
        // Direction is from license to component, indicating the license "applies to" the component
        var edge = new CytoscapeEdge
        {
            Data = new CytoscapeEdgeData
            {
                Id = $"license-{licenseId}-applies-to-{componentBomRef}",
                Source = $"license-{licenseId}",
                Target = componentBomRef,
                Class = "license"
            }
        };

        elements.AddEdge(edge);
    }
    
    /// <summary>
    /// Adds a component node to the graph
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="component">The component to add</param>
    private void AddComponentNode(CytoscapeElements elements, SimpleComponent component)
    {
        // Skip components without a valid BOM reference as they can't be properly identified
        if (string.IsNullOrEmpty(component.BomRef))
            return;

        // Create a component node using the BOM reference as the unique identifier
        // The label is a formatted string that includes relevant component metadata
        var nodeData = new ComponentNodeData
        {
            Id = component.BomRef,
            Label = FormatNodeLabel(component),
            Type = component.Type ?? "",
            Version = component.Version ?? "",
            Group = component.Group ?? ""
        };

        elements.AddNode(new CytoscapeNode(nodeData));
    }

    /// <summary>
    /// Adds a vulnerability node to the graph
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="vulnerability">The vulnerability to add</param>
    private void AddVulnerabilityNode(CytoscapeElements elements, SimpleVulnerability vulnerability)
    {
        // Skip vulnerabilities without a valid ID
        if (string.IsNullOrEmpty(vulnerability.Id))
            return;

        // Get the vulnerability rating (if available) to extract severity and score
        var rating = vulnerability.Ratings?.FirstOrDefault();
        
        // Create the vulnerability node with its metadata
        var nodeData = new VulnerabilityNodeData
        {
            Id = vulnerability.Id,
            Label = vulnerability.Id,          
            SourceUrl = vulnerability.Source?.Url ?? "",      // URL to vulnerability details
            SourceName = vulnerability.Source?.Name ?? "", // Name of the rating source (e.g., NVD)

            Score = rating?.Score,                     // CVSS score (numerical value)
            Severity = rating?.Severity ?? "unknown",  // Severity level (text)
            Method = rating?.Method ?? "",              // Scoring method (e.g., CVSSv3)
            Vector = rating?.Vector ?? "",              // Scoring vector string
        };

        elements.AddNode(new CytoscapeNode(nodeData));
    }

    /// <summary>
    /// Formats a component's label using its group, name and version
    /// </summary>
    /// <param name="component">The component to format</param>
    /// <returns>A formatted label string</returns>
    private string FormatNodeLabel(SimpleComponent component)
    {
        var parts = new List<string>();

        // Build a hierarchical label including available metadata
        // This creates a more informative node label in the graph
        if (_options.ShowGroupsInNodeLabels && !string.IsNullOrEmpty(component.Group))
            parts.Add(component.Group);
        
        if (!string.IsNullOrEmpty(component.Name))
            parts.Add(component.Name);
            
        if (!string.IsNullOrEmpty(component.Version))
            parts.Add(component.Version);

        // Join all parts with "/" to create a label like "group/name/version"
        return string.Join("/", parts);
    }

    /// <summary>
    /// Adds edges between vulnerabilities and affected components
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="vulnerability">The vulnerability that affects components</param>
    private void AddVulnerabilityEdges(CytoscapeElements elements, SimpleVulnerability vulnerability)
    {
        // Skip if the vulnerability doesn't have a valid ID or doesn't affect any components
        if (string.IsNullOrEmpty(vulnerability.Id) || vulnerability.Affects == null)
            return;

        // Create edges from the vulnerability to each affected component
        // These edges represent the "affects" relationship
        foreach (var affect in vulnerability.Affects)
        {
            if (string.IsNullOrEmpty(affect.Ref))
                continue;

            // Create an edge from the vulnerability to the component
            // Direction is from vulnerability to component, indicating the vulnerability "affects" the component
            var edge = new CytoscapeEdge
            {
                Data = new CytoscapeEdgeData
                {
                    Id = $"{vulnerability.Id}-affects-{affect.Ref}",
                    Source = vulnerability.Id,
                    Target = affect.Ref,
                    Class = "vulnerability"
                }
            };

            elements.AddEdge(edge);
        }
    }

    /// <summary>
    /// Recursively adds dependency edges between components
    /// </summary>
    /// <param name="elements">The graph elements collection</param>
    /// <param name="dependency">The dependency relationship to process</param>
    private void AddDependencyEdges(CytoscapeElements elements, SimpleDependency dependency)
    {
        // Skip if the source component reference is missing or there are no dependencies
        if (string.IsNullOrEmpty(dependency.Ref) || dependency.DependsOn == null)
            return;

        // Create edges from the source component to each of its dependencies
        foreach (var target in dependency.DependsOn)
        {
            // Create an edge representing the dependency relationship
            // Direction is from source to target, indicating the source "depends on" the target
            // For example: A -> B means A depends on B
            var edge = new CytoscapeEdge
            {
                Data = new CytoscapeEdgeData
                {
                    Id = $"{dependency.Ref}->{target}",
                    Source = dependency.Ref,
                    Target = target,
                    Class = "component"
                }
            };

            elements.AddEdge(edge);
        }

        // Recursively process nested dependencies if any
        // This handles complex dependency trees with multiple levels
        if (dependency.Dependencies != null)
        {
            foreach (var childDependency in dependency.Dependencies)
            {
                AddDependencyEdges(elements, childDependency);
            }
        }
    }
    
    /// <summary>
    /// Propagates vulnerability severity up the dependency tree to parent components
    /// </summary>
    /// <param name="graph">The graph with components, dependencies and vulnerabilities</param>
    private void PropagateSeverityToParents(CytoscapeGraph graph)
    {
        var elements = graph.Elements;
        
        // Create a dictionary of all component nodes by id for efficient lookup
        var componentNodes = elements.Nodes
            .Where(n => n.Data is ComponentNodeData)
            .ToDictionary(n => n.Data.Id, n => n);
        
        // Create a dictionary to track dependency relationships: child -> parents
        // This represents the inverse of the dependency direction in the graph
        var parentsByChild = new Dictionary<string, HashSet<string>>();
        
        // Build the dependency relationships
        // In this step, we identify which components depend on which other components
        foreach (var edge in elements.Edges)
        {
            // Only consider dependency edges between components, not vulnerability-component edges
            if (componentNodes.ContainsKey(edge.Data.Source) && componentNodes.ContainsKey(edge.Data.Target))
            {
                // In SBOM format, if A depends on B (edge A->B), then A is affected if B has a vulnerability
                // So B is a dependency of A, meaning A is a parent of B in the propagation tree
                if (!parentsByChild.ContainsKey(edge.Data.Target))
                {
                    parentsByChild[edge.Data.Target] = new HashSet<string>();
                }
                parentsByChild[edge.Data.Target].Add(edge.Data.Source);
            }
        }
        
        // Track which components are directly affected by vulnerabilities and their severity
        var componentSeverities = new Dictionary<string, List<string>>();
        
        // Collect direct vulnerabilities
        // Identify which components are directly affected by vulnerabilities
        foreach (var edge in elements.Edges)
        {
            // Check if this is a vulnerability-component edge
            var vulnNode = elements.Nodes
                .FirstOrDefault(n => n.Data.Id == edge.Data.Source && n.Data is VulnerabilityNodeData);
                
            if (vulnNode != null && componentNodes.ContainsKey(edge.Data.Target))
            {
                // Extract the severity from the vulnerability node
                var severity = (vulnNode.Data as VulnerabilityNodeData)?.Severity ?? "unknown";
                
                // Add the severity to the component's list of severities
                if (!componentSeverities.ContainsKey(edge.Data.Target))
                {
                    componentSeverities[edge.Data.Target] = new List<string>();
                }
                componentSeverities[edge.Data.Target].Add(severity);
            }
        }
        
        // Set direct severity for components with vulnerabilities
        // Assign the highest severity level to each directly affected component
        foreach (var componentId in componentSeverities.Keys)
        {
            if (componentNodes.TryGetValue(componentId, out var node) && node.Data is ComponentNodeData compData)
            {
                compData.Severity = GetMaxSeverity(componentSeverities[componentId]);
            }
        }
        
        // Now propagate severity up the dependency tree
        // This ensures that components depending on vulnerable components also show the severity
        var processedNodes = new HashSet<string>();
        
        // First, identify components that have direct vulnerabilities to start propagation from
        var startingNodes = componentSeverities.Keys.ToList();
        
        // Propagate from each directly affected component
        // This will follow the dependency chain upwards, setting severity on parent components
        foreach (var startingNodeId in startingNodes)
        {
            PropagateToAllParents(startingNodeId, componentNodes, parentsByChild, processedNodes);
        }
        
        // Mark top parent components (components that are not dependencies of any other component)
        // These are the root nodes in the dependency tree
        MarkTopParentComponents(componentNodes, parentsByChild);
        
        // Ensure all edges involving a vulnerability node have class="vulnerability"
        // This is for consistent styling in the visualization
        EnsureVulnerabilityEdgeClasses(elements, componentNodes);
    }
    
    /// <summary>
    /// Propagates severity from a component to all its parent components
    /// </summary>
    private void PropagateToAllParents(
        string componentId,
        Dictionary<string, CytoscapeNode> componentNodes,
        Dictionary<string, HashSet<string>> parentsByChild,
        HashSet<string> processedForPropagation)
    {
        // Get the component's severity
        if (!componentNodes.TryGetValue(componentId, out var node) || !(node.Data is ComponentNodeData compData))
            return;
            
        string severity = compData.Severity;
        
        // Don't propagate if the component has no severity or unknown severity
        if (severity == "none" || severity == "unknown")
            return;
            
        // Get all parents of this component (components that depend on this one)
        if (!parentsByChild.TryGetValue(componentId, out var parentIds))
            return;
            
        // Propagate to each parent
        foreach (var parentId in parentIds)
        {
            if (componentNodes.TryGetValue(parentId, out var parentNode) && 
                parentNode.Data is ComponentNodeData parentData)
            {
                // Update parent severity if the current component's severity is higher
                string oldSeverity = parentData.Severity;
                parentData.Severity = GetMaxSeverity(new[] { oldSeverity, severity });
                
                // If severity changed, we need to propagate further up
                // This ensures that the entire dependency chain is updated
                if (oldSeverity != parentData.Severity && !processedForPropagation.Contains(parentId))
                {
                    // Mark this node as processed to avoid infinite loops in circular dependencies
                    processedForPropagation.Add(parentId);
                    
                    // Recursively propagate to this parent's parents
                    PropagateToAllParents(parentId, componentNodes, parentsByChild, processedForPropagation);
                }
            }
        }
    }

    /// <summary>
    /// Gets the maximum severity from a list of severity values
    /// </summary>
    /// <param name="severities">List of severity strings</param>
    /// <returns>The highest severity value</returns>
    private string GetMaxSeverity(IEnumerable<string> severities)
    {
        // Define severity levels with their numeric values
        // Higher value = higher severity
        var severityValues = new Dictionary<string, int>
        {
            { "none", 0 },
            { "unknown", 0 },
            { "low", 1 },
            { "medium", 2 },
            { "high", 3 },
            { "critical", 4 }
        };
        
        int maxValue = 0;
        
        // Find the maximum severity value
        foreach (var severity in severities)
        {
            string normalizedSeverity = severity.ToLowerInvariant();
            if (severityValues.TryGetValue(normalizedSeverity, out int value))
            {
                maxValue = Math.Max(maxValue, value);
            }
        }
        
        // Convert the numeric value back to its string representation
        // Find the key (severity name) that corresponds to the maximum value
        foreach (var kvp in severityValues)
        {
            if (kvp.Value == maxValue)
            {
                return kvp.Key;
            }
        }
        
        // Default return if no match is found
        return "none";
    }

    /// <summary>
    /// Marks components that are at the top of the dependency chain (not a dependency of any other component)
    /// </summary>
    /// <param name="componentNodes">Dictionary of all component nodes</param>
    /// <param name="parentsByChild">Mapping of child components to their parent components</param>
    private void MarkTopParentComponents(
        Dictionary<string, CytoscapeNode> componentNodes,
        Dictionary<string, HashSet<string>> parentsByChild)
    {
        // We need to identify components that other components depend on (targets)
        // and components that depend on others (sources)
        
        // First, collect all components that appear as dependencies of other components
        // These are the "target" nodes in dependency edges
        var dependencyTargets = new HashSet<string>();
        
        // Also collect all components that depend on others
        // These are the "source" nodes in dependency edges
        var dependencySources = new HashSet<string>();
        
        // In parentsByChild, key is the child/target and values are parents/sources
        foreach (var entry in parentsByChild)
        {
            // The key is a target/dependency
            dependencyTargets.Add(entry.Key);
            
            // The values are sources/parents that depend on the target
            foreach (var source in entry.Value)
            {
                dependencySources.Add(source);
            }
        }
        
        // A component is a top parent if it's a source but not a target
        // meaning it depends on other components but no one depends on it
        // These are the leaf nodes in the inverted dependency tree
        foreach (var node in componentNodes.Values)
        {
            if (node.Data is ComponentNodeData compData)
            {
                compData.IsTopParent = dependencySources.Contains(node.Data.Id) && 
                                       !dependencyTargets.Contains(node.Data.Id);
            }
        }
    }

    /// <summary>
    /// Ensures that all edges involving a vulnerability node have class="vulnerability"
    /// </summary>
    /// <param name="elements">The graph elements</param>
    /// <param name="componentNodes">Dictionary of component nodes by ID</param>
    private void EnsureVulnerabilityEdgeClasses(CytoscapeElements elements, Dictionary<string, CytoscapeNode> componentNodes)
    {
        // Get all vulnerability node IDs for efficient lookup
        var vulnerabilityNodeIds = elements.Nodes
            .Where(n => n.Data is VulnerabilityNodeData)
            .Select(n => n.Data.Id)
            .ToHashSet();
        
        // Update edge classes for all edges involving vulnerability nodes
        // This ensures consistent styling for vulnerability-related edges in visualization
        foreach (var edge in elements.Edges)
        {
            // If either source or target is a vulnerability node, set class to "vulnerability"
            if (vulnerabilityNodeIds.Contains(edge.Data.Source) || vulnerabilityNodeIds.Contains(edge.Data.Target))
            {
                edge.Data.Class = "vulnerability";
            }
        }
    }
}
