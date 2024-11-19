// See https://aka.ms/new-console-template for more information
public class NetworkAdapterAdvancedProperty
{
    // Properties from MSFT_NetAdapterAdvancedPropertySettingData
    public string DisplayName { get; set; }
    public string DisplayValue { get; set; }
    public string RegistryValue { get; set; }
    public string RegistryKeyword { get; set; }
    public string DefaultRegistryValue { get; set; }
    public string DefaultDisplayValue { get; set; }

    // Computed flag to check if current value is different from the default
    public bool IsValueDifferentFromDefault =>
        !string.Equals(this.DisplayValue, this.DefaultDisplayValue, StringComparison.OrdinalIgnoreCase);
}
