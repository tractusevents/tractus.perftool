// See https://aka.ms/new-console-template for more information
public class NetworkAdapterInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string InstanceId { get; set; }
    public Dictionary<string, string> Properties { get; } = [];
    public NetworkAdapterAdvancedProperty[] AdvancedProperties { get; } = [];

    public NetworkAdapterInfo(
        string name,
        string description,
        string instanceId,
        IDictionary<string, string> properties,
        NetworkAdapterAdvancedProperty[] advancedProperties)
    {
        this.Name = name;
        this.Description = description;
        this.InstanceId = instanceId;

        foreach (var item in properties)
        {
            this.Properties[item.Key] = item.Value;
        }

        this.AdvancedProperties = advancedProperties;
    }
}
