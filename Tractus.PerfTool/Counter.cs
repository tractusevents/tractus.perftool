// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
public class Counter : IDisposable
{
    public string Category { get; }
    public string CounterName { get; set; }
    public string InstanceName { get; set; }

    public float Current { get; private set; }

    private PerformanceCounter counter;
    private bool disposedValue;

    public static implicit operator float(Counter counter) => counter.Current;

    public Counter(string category, string counterName, string instanceName)
    {
        this.Category = category;
        this.CounterName = counterName;
        this.InstanceName = instanceName;

        this.counter = new PerformanceCounter(category, counterName, instanceName);
        this.counter.NextValue();
    }

    public void NextValue()
    {
        this.Current = this.counter.NextValue();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposedValue)
        {
            if (disposing)
            {
                this.counter.Dispose();
            }

            this.disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        this.Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
