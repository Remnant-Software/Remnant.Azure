using Azure.Data.AppConfiguration;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Remnant.Azure.Configuration;

/// <summary>
/// Derive from this class to implement any additional config settings
/// </summary>
public class AzAppSetting
{

	[JsonIgnore]
	public ConfigurationSetting RawSetting { get; set; }

	public Azure azure { get; set; }

	public static TAppSetting? Deserialize<TAppSetting>(string json)
		where TAppSetting : AzAppSetting, new()
	{
		return JsonSerializer.Deserialize<TAppSetting>(json);
	}

	public static string Serialize(AzAppSetting config) => JsonSerializer.Serialize(config);
}

public class Azure
{
	public string entraId { get; set; }
	public string environment { get; set; }
	public List<Resource> resources { get; set; }
}

public class Resource
{
	public string endPoint { get; set; }
	public string name { get; set; }
	public List<Source> sources { get; set; }
	public string type { get; set; }
}

public class Source
{
	public string name { get; set; }
	public string type { get; set; }
}