using Newtonsoft.Json;

namespace Cdx2Cyto.Services;

public class SimpleBom
{
    [JsonProperty("metadata")]
    public BomMetadata? Metadata { get; set; }

    [JsonProperty("components")]
    public SimpleComponent[]? Components { get; set; }

    [JsonProperty("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }

    [JsonProperty("vulnerabilities")]
    public SimpleVulnerability[]? Vulnerabilities { get; set; }
}

public class BomMetadata
{
    [JsonProperty("component")]
    public SimpleComponent? Component { get; set; }
}

public class SimpleComponent
{
    [JsonProperty("bom-ref")]
    public string? BomRef { get; set; }

    [JsonProperty("name")]
    public string? Name { get; set; }

    [JsonProperty("version")]
    public string? Version { get; set; }

    [JsonProperty("type")]
    public string? Type { get; set; }

    [JsonProperty("group")]
    public string? Group { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("purl")]
    public string? Purl { get; set; }
    
    [JsonProperty("licenses")]
    public SimpleLicense[]? Licenses { get; set; }
}

public class SimpleLicense
{
    [JsonProperty("license")]
    public LicenseContent? License { get; set; }
}

public class LicenseContent
{
    [JsonProperty("id")]
    public string? Id { get; set; }
    
    [JsonProperty("name")]
    public string? Name { get; set; }
    
    [JsonProperty("url")]
    public string? Url { get; set; }
}

public class SimpleDependency
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }

    [JsonProperty("dependsOn")]
    public string[]? DependsOn { get; set; }

    [JsonProperty("dependencies")]
    public SimpleDependency[]? Dependencies { get; set; }
}

public class SimpleVulnerability
{
    [JsonProperty("id")]
    public string? Id { get; set; }

    [JsonProperty("source")]
    public VulnerabilitySource? Source { get; set; }

    [JsonProperty("ratings")]
    public VulnerabilityRating[]? Ratings { get; set; }

    [JsonProperty("description")]
    public string? Description { get; set; }

    [JsonProperty("affects")]
    public VulnerabilityAffects[]? Affects { get; set; }
}

public class VulnerabilitySource
{
    [JsonProperty("url")]
    public string? Url { get; set; }
    [JsonProperty("name")]
    public string? Name { get; set; }
}

public class VulnerabilityRating
{
    [JsonProperty("score")]
    public double? Score { get; set; }

    [JsonProperty("severity")]
    public string? Severity { get; set; }
    [JsonProperty("method")]
    public string? Method { get; set; }
    [JsonProperty("vector")]
    public string? Vector { get; set; }

}

public class VulnerabilityAffects
{
    [JsonProperty("ref")]
    public string? Ref { get; set; }
}
