// See https://aka.ms/new-console-template for more information
public class ComputerDetailNode
{
    public string Name { get; set; }
    public string Type { get; set; }
    public List<ComputerDetailNode> SubHardware { get; } = [];
    public List<ComputerDetailValue> Sensors { get; } = [];

    public ComputerDetailNode(
        string name,
        string type,
        IEnumerable<ComputerDetailNode>? subHardware,
        IEnumerable<ComputerDetailValue>? sensors)
    {
        this.Name = name;
        this.Type = type;
        if (subHardware is not null)
        {
            this.SubHardware.AddRange(subHardware);
        }

        if (sensors is not null)
        {
            this.Sensors.AddRange(sensors);
        }
    }
}
