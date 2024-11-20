// See https://aka.ms/new-console-template for more information
using LibreHardwareMonitor.Hardware;
using Microsoft.Management.Infrastructure;
public class ComputerDetails : IDisposable
{
    private UpdateVisitor updateVisitor = new();
    private Computer? computer;
    public void Refresh()
    {
        
    }

    public ComputerDetails()
    {
        this.computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true,
            IsControllerEnabled = true,
            IsMotherboardEnabled = true,
            IsNetworkEnabled = false,
        };

        this.computer.Open();
    }

    public void Dispose()
    {
        if(this.computer is null)
        {
            return;
        }

        this.computer.Close();
    }

    public ComputerInfo? GetComputerDetailSnapshot()
    {
        if(this.computer is null)
        {
            throw new ObjectDisposedException("ComputerInfo was null when a snapshot was requested.");
        }

        this.computer.Accept(this.updateVisitor);
        var allHardware = this.computer.Hardware.Select(CreateDetailNodeFromIHardware()).ToList();
        var toReturn = new ComputerInfo(allHardware);
        return toReturn;
    }

    private static Func<IHardware, ComputerDetailNode> CreateDetailNodeFromIHardware()
    {
        return x =>
        {
            var name = x.Name;
            var type = x.HardwareType.ToString();

            var subHardware = x.SubHardware.Select(CreateDetailNodeFromIHardware());
            var sensors = x.Sensors.Select(s => new ComputerDetailValue(s.Name, s.Value));

            return new ComputerDetailNode(name, type, subHardware, sensors);
        };
    }

    public List<NetworkAdapterInfo> GetAllNetworkAdapterInfo()
    {
        var namespaceName = @"root\standardcimv2";
        var className = "MSFT_NetAdapterAdvancedPropertySettingData";

        using var netAdapterSession = CimSession.Create(null);

        var netAdapterProperties = netAdapterSession.QueryInstances(namespaceName, "WQL", "SELECT * FROM MSFT_NetAdapter");

        var adapters = new List<NetworkAdapterInfo>();

        foreach(var networkAdapter in netAdapterProperties)
        {
            var name = networkAdapter.CimInstanceProperties["Name"]?.Value?.ToString();
            var description = networkAdapter.CimInstanceProperties["InterfaceDescription"]?.Value?.ToString();
            var instanceId = networkAdapter.CimInstanceProperties["InstanceID"]?.Value?.ToString();

            var netAdapterPropertiesDict = networkAdapter.CimInstanceProperties.Where(x => x.Name != "Name" && x.Name != "InterfaceDescription" && x.Name != "InstanceID")
                .OrderBy(x=>x.Name)
                .ToDictionary(k => k.Name, v =>
                {
                    return SerializeValue(v?.Value);   
                });

            // Connect to the CIM session
            using var session = CimSession.Create(null);
            // Query the MSFT_NetAdapterAdvancedProperty class
            var properties = session.QueryInstances(namespaceName, "WQL", $"SELECT * FROM {className} WHERE InstanceID LIKE '{instanceId}%'");

            var advancedProperties = new List<NetworkAdapterAdvancedProperty>();

            foreach (var property in properties)
            {
                var interfaceFriendlyName = property.CimInstanceProperties["InterfaceDescription"]?.Value?.ToString();
                var adapterName = property.CimInstanceProperties["InstanceID"]?.Value?.ToString();
                var propertyName = property.CimInstanceProperties["Name"]?.Value?.ToString();
                var displayName = property.CimInstanceProperties["DisplayName"]?.Value?.ToString();
                var displayValue = property.CimInstanceProperties["DisplayValue"]?.Value?.ToString();
                var registryValue = SerializeValue(property.CimInstanceProperties["RegistryValue"]?.Value);
                var registryKeyword = property.CimInstanceProperties["RegistryKeyword"]?.Value?.ToString();

                var defaultRegistryValue = property.CimInstanceProperties["DefaultRegistryValue"]?.Value?.ToString();
                var defaultDisplayValue = property.CimInstanceProperties["DefaultDisplayValue"]?.Value?.ToString();

                var toAdd = new NetworkAdapterAdvancedProperty
                {
                    DisplayName = displayName,
                    DisplayValue = displayValue,
                    RegistryValue = registryValue,
                    RegistryKeyword = registryKeyword,
                    DefaultDisplayValue = defaultDisplayValue,
                    DefaultRegistryValue = defaultRegistryValue
                };

                advancedProperties.Add(toAdd);
            }

            var adapterToAdd = new NetworkAdapterInfo(name, description, instanceId, netAdapterPropertiesDict, advancedProperties.OrderBy(x=>x.DisplayName).ToArray());
            adapters.Add(adapterToAdd);
        }

        return adapters;
    }

    public static string? SerializeValue(object? o)
    {
        var toReturn = o;
        if (toReturn is null)
        {
            return "";
        }

        if (toReturn is string[] v)
        {
            return string.Join(';', v);
        }

        return toReturn.ToString();
    }
}

