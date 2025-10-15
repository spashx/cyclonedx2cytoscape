```mermaid
classDiagram
    %% Abstract base classes
    class BaseNodeData {
        <<abstract>>
        +string Id
        +string Label
        +string Class
    }

    class BaseLinkData {
        +string Id
        +string Source
        +string Target
        +string Class
        +BaseLinkData(string id, string source, string target, string edgeClass)
    }

    %% Cytoscape Models
    class CytoscapeNode {
        +BaseNodeData Data
        +CytoscapeNode(BaseNodeData data)
    }

    class CytoscapeEdge {
        +BaseLinkData Data
        +CytoscapeEdge(BaseLinkData data)
    }

    class CytoscapeElements {
        -List~CytoscapeNode~ nodes
        -List~CytoscapeEdge~ edges
        +List~CytoscapeNode~ Nodes
        +List~CytoscapeEdge~ Edges
        +AddNode(CytoscapeNode node)
        +AddEdge(CytoscapeEdge edge)
    }

    class CytoscapeGraph {
        +CytoscapeElements Elements
    }

    %% Node Data Types (inherit from BaseNodeData)
    class ComponentNodeData {
        +string Type
        +string Version
        +string Group
        +string Severity
        +bool IsTopParent
        +ComponentNodeData()
    }

    class VulnerabilityNodeData {
        +double? Score
        +string Severity
        +string SourceUrl
        +string Method
        +string Vector
        +string SourceName
        +VulnerabilityNodeData()
    }

    class LicenseNodeData {
        +string Url
        +LicenseNodeData()
    }

    class ParentNodeData {
        +ParentNodeData(string id, string label)
    }

    %% SBOM Models
    class SimpleBom {
        +BomMetadata? Metadata
        +SimpleComponent[]? Components
        +SimpleDependency[]? Dependencies
        +SimpleVulnerability[]? Vulnerabilities
    }

    class BomMetadata {
        +SimpleComponent? Component
    }

    class SimpleComponent {
        +string? BomRef
        +string? Name
        +string? Version
        +string? Type
        +string? Group
        +string? Description
        +string? Purl
        +SimpleLicense[]? Licenses
    }

    class SimpleLicense {
        +LicenseContent? License
    }

    class LicenseContent {
        +string? Id
        +string? Name
        +string? Url
    }

    class SimpleDependency {
        +string? Ref
        +string[]? DependsOn
        +SimpleDependency[]? Dependencies
    }

    class SimpleVulnerability {
        +string? Id
        +VulnerabilitySource? Source
        +VulnerabilityRating[]? Ratings
        +string? Description
        +VulnerabilityAffects[]? Affects
    }

    class VulnerabilitySource {
        +string? Url
        +string? Name
    }

    class VulnerabilityRating {
        +double? Score
        +string? Severity
        +string? Method
        +string? Vector
    }

    class VulnerabilityAffects {
        +string? Ref
    }

    %% Relationships - Cytoscape hierarchy
    BaseNodeData <|-- ComponentNodeData
    BaseNodeData <|-- VulnerabilityNodeData
    BaseNodeData <|-- LicenseNodeData
    BaseNodeData <|-- ParentNodeData

    CytoscapeNode *-- BaseNodeData : contains
    CytoscapeEdge *-- BaseLinkData : contains
    CytoscapeElements o-- CytoscapeNode : aggregates
    CytoscapeElements o-- CytoscapeEdge : aggregates
    CytoscapeGraph *-- CytoscapeElements : contains

    %% Relationships - SBOM hierarchy
    SimpleBom o-- BomMetadata : aggregates
    SimpleBom o-- SimpleComponent : aggregates
    SimpleBom o-- SimpleDependency : aggregates
    SimpleBom o-- SimpleVulnerability : aggregates

    BomMetadata *-- SimpleComponent : contains
    SimpleComponent o-- SimpleLicense : aggregates
    SimpleLicense *-- LicenseContent : contains
    SimpleDependency o-- SimpleDependency : recursive

    SimpleVulnerability *-- VulnerabilitySource : contains
    SimpleVulnerability o-- VulnerabilityRating : aggregates
    SimpleVulnerability o-- VulnerabilityAffects : aggregates

    %% Styling
    classDef abstract fill:#e1f5fe,stroke:#01579b,stroke-width:2px
    classDef cytoscape fill:#f3e5f5,stroke:#4a148c,stroke-width:2px
    classDef sbom fill:#e8f5e8,stroke:#1b5e20,stroke-width:2px
    classDef nodeData fill:#fff3e0,stroke:#e65100,stroke-width:2px
