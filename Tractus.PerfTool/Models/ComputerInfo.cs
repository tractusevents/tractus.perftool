// See https://aka.ms/new-console-template for more information
using Microsoft.Win32;
using System.Management;
public class ComputerInfo
{
    public List<ComputerDetailNode> Hardware { get; } = [];

    public string OSVersion { get; set; }
    public string ProductName { get; set; }
    public string ReleaseID { get; set; }
    public string BuildNumber { get; set; }
    public string UpdateBuildRevision { get; set; }

    public ComputerInfo(
        IEnumerable<ComputerDetailNode> hardware)
    {
        this.Hardware.AddRange(hardware);

        using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
        foreach (ManagementObject obj in searcher.Get())
        {
            this.ProductName = obj["Caption"]?.ToString();
            this.OSVersion = obj["Version"]?.ToString();
            this.BuildNumber = obj["BuildNumber"]?.ToString();
        }


        // Registry-based method for build details
        var registryKey = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
        using var key = Registry.LocalMachine.OpenSubKey(registryKey);
        if (key != null)
        {
            var releaseId = key.GetValue("ReleaseId")?.ToString();
            var buildNumber = key.GetValue("CurrentBuildNumber")?.ToString();
            var ubr = key.GetValue("UBR")?.ToString(); // Update Build Revision

            this.ReleaseID = releaseId;
            this.BuildNumber = buildNumber;
            this.UpdateBuildRevision = ubr;
        }
    }
}
