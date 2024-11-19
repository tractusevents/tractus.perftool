// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
public class ProcessorMetrics : IDisposable
{
    // Public property for the processor instance name
    public string ProcessorInstanceName { get; }

    // Private performance counter fields (removed underscores)
    private PerformanceCounter interruptTimeCounter;
    private PerformanceCounter dpcTimeCounter;
    private PerformanceCounter interruptsPerSecCounter;
    private PerformanceCounter dpcsQueuedPerSecCounter;
    private PerformanceCounter userTimeCounter;
    private PerformanceCounter privilegedTimeCounter;

    // Public properties for metric values
    public float InterruptTime { get; private set; }
    public float DPCTime { get; private set; }
    public float InterruptsPerSec { get; private set; }
    public float DPCsQueuedPerSec { get; private set; }
    public float UserTime { get; private set; }
    public float PrivilegedTime { get; private set; }

    // Constructor
    public ProcessorMetrics(string processorInstanceName)
    {
        this.ProcessorInstanceName = processorInstanceName;

        // Initialize performance counters
        this.interruptTimeCounter = new PerformanceCounter("Processor", "% Interrupt Time", processorInstanceName);
        this.dpcTimeCounter = new PerformanceCounter("Processor", "% DPC Time", processorInstanceName);
        this.interruptsPerSecCounter = new PerformanceCounter("Processor", "Interrupts/sec", processorInstanceName);
        this.dpcsQueuedPerSecCounter = new PerformanceCounter("Processor", "DPCs Queued/sec", processorInstanceName);
        this.userTimeCounter = new PerformanceCounter("Processor", "% User Time", processorInstanceName);
        this.privilegedTimeCounter = new PerformanceCounter("Processor", "% Privileged Time", processorInstanceName);

        // Warm-up readings
        this.interruptTimeCounter.NextValue();
        this.dpcTimeCounter.NextValue();
        this.interruptsPerSecCounter.NextValue();
        this.dpcsQueuedPerSecCounter.NextValue();
        this.userTimeCounter.NextValue();
        this.privilegedTimeCounter.NextValue();
    }

    // Refresh method to update metric values
    public void Refresh()
    {
        this.InterruptTime = this.interruptTimeCounter.NextValue();
        this.DPCTime = this.dpcTimeCounter.NextValue();
        this.InterruptsPerSec = this.interruptsPerSecCounter.NextValue();
        this.DPCsQueuedPerSec = this.dpcsQueuedPerSecCounter.NextValue();
        this.UserTime = this.userTimeCounter.NextValue();
        this.PrivilegedTime = this.privilegedTimeCounter.NextValue();
    }

    // Implement IDisposable
    public void Dispose()
    {
        this.interruptTimeCounter?.Dispose();
        this.dpcTimeCounter?.Dispose();
        this.interruptsPerSecCounter?.Dispose();
        this.dpcsQueuedPerSecCounter?.Dispose();
        this.userTimeCounter?.Dispose();
        this.privilegedTimeCounter?.Dispose();
    }
}
