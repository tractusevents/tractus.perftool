// See https://aka.ms/new-console-template for more information
public class ComputerDetailValue
{
    public string Name { get; set; }

    public float? Value { get; set; }

    public ComputerDetailValue(string name, float? value)
    {
        this.Name = name;
        this.Value = value;
    }
}
