// See https://aka.ms/new-console-template for more information
using LibreHardwareMonitor.Hardware;
using System.Diagnostics;
public class NetworkMetrics : IDisposable
{
    // Public property for the network interface instance name
    public string InstanceName { get; }

    // Private performance counter fields
    public Counter BytesTotalSec { get; private set; }
    public Counter PacketsSentUnicastSec { get; private set; }
    public Counter PacketsSentNonUnicastSec { get; private set; }
    public Counter PacketsOutboundDiscarded { get; private set; }
    public Counter PacketsOutboundErrors { get; private set; }

    public Counter PacketsReceivedDiscarded { get; private set; }
    public Counter PacketsReceivedErrors { get; private set; }

    public Counter PacketsReceivedUnicastSec { get; private set; }
    public Counter PacketsReceivedNonUnicastSec { get; private set; }

    public Counter PacketsSec { get; private set; }

    public Counter OutputQueueLength { get; private set; }
    public Counter OffloadedConnections { get; private set; }

    public NetworkMetrics(string instanceName)
    {
        this.InstanceName = instanceName;

        // Initialize performance counters for the specific network interface
        this.BytesTotalSec= new Counter("Network Interface", "Bytes Total/sec", instanceName);
        this.PacketsSentUnicastSec = new Counter("Network Interface", "Packets Sent Unicast/sec", instanceName);
        this.PacketsSentNonUnicastSec = new Counter("Network Interface", "Packets Sent Non-Unicast/sec", instanceName);
        this.PacketsOutboundDiscarded = new Counter("Network Interface", "Packets Outbound Discarded", instanceName);
        this.PacketsOutboundErrors = new Counter("Network Interface", "Packets Outbound Errors", instanceName);

        this.PacketsReceivedDiscarded = new Counter("Network Interface", "Packets Received Discarded", instanceName);
        this.PacketsReceivedErrors = new Counter("Network Interface", "Packets Received Errors", instanceName);
        this.PacketsReceivedUnicastSec = new Counter("Network Interface", "Packets Received Unicast/sec", instanceName);
        this.PacketsReceivedNonUnicastSec = new Counter("Network Interface", "Packets Received Non-Unicast/sec", instanceName);

        this.PacketsSec = new Counter("Network Interface", "Packets/sec", instanceName);


        this.OutputQueueLength = new Counter("Network Interface", "Output Queue Length", instanceName);
        this.OffloadedConnections = new Counter("Network Interface", "Offloaded Connections", instanceName);
    }

    // Refresh method to update metric values
    public void Refresh()
    {
        this.BytesTotalSec.NextValue();
        this.PacketsSentUnicastSec.NextValue();
        this.PacketsSentNonUnicastSec.NextValue();
        this.PacketsOutboundDiscarded.NextValue();
        this.PacketsOutboundErrors.NextValue();
        this.PacketsReceivedDiscarded.NextValue();
        this.PacketsReceivedErrors.NextValue();
        this.PacketsReceivedUnicastSec.NextValue();
        this.PacketsReceivedNonUnicastSec.NextValue();
        this.PacketsSec.NextValue();
        this.OutputQueueLength.NextValue();
        this.OffloadedConnections.NextValue();
    }

    // Implement IDisposable
    public void Dispose()
    {
        this.BytesTotalSec.Dispose();
        this.PacketsSentUnicastSec.Dispose();
        this.PacketsSentNonUnicastSec.Dispose();
        this.PacketsOutboundDiscarded.Dispose();
        this.PacketsOutboundErrors.Dispose();
        this.PacketsReceivedDiscarded.Dispose();
        this.PacketsReceivedErrors.Dispose();
        this.PacketsReceivedUnicastSec.Dispose();
        this.PacketsReceivedNonUnicastSec.Dispose();
        this.PacketsSec.Dispose();
        this.OutputQueueLength.Dispose();
        this.OffloadedConnections.Dispose();
    }
}
public class UpdateVisitor : IVisitor
{
    public void VisitComputer(IComputer computer)
    {
        computer.Traverse(this);
    }
    public void VisitHardware(IHardware hardware)
    {
        hardware.Update();
        foreach (var subHardware in hardware.SubHardware) subHardware.Accept(this);
    }
    public void VisitSensor(ISensor sensor) { }
    public void VisitParameter(IParameter parameter) { }
}
