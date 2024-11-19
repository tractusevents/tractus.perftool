// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
public class GlobalNetworkMetrics : IDisposable
{
    private PerformanceCounter tcpSegmentsSentCounter;
    private PerformanceCounter tcpSegmentsRetransmittedCounter;
    private PerformanceCounter tcpSegmentsRecvCounter;

    private PerformanceCounter udpDatagramsSentCounter;
    private PerformanceCounter udpDatagramsReceivedErrorsCounter;
    private PerformanceCounter udpDatagramsReceivedSecCounter;
    private PerformanceCounter tcpConnEstablishedCounter;

    public float TcpSegmentsSent { get; private set; }
    public float TcpSegmentsRecv { get; private set; }
    public float TcpSegmentsRetransmitted { get; private set; }
    public float TcpConnectionsEstablished { get; private set; }

    public float UdpDatagramsSent { get; private set; }
    public float UdpDatagramsReceivedErrors { get; private set; }
    public float UdpDatagramsReceivedSec { get; private set; }

    public GlobalNetworkMetrics()
    {
        // Initialize performance counters for TCP and UDP (no instance name required)
        this.tcpSegmentsSentCounter = new PerformanceCounter("TCPv4", "Segments Sent/sec");
        this.tcpSegmentsRecvCounter = new PerformanceCounter("TCPv4", "Segments Received/sec");
        this.tcpSegmentsRetransmittedCounter = new PerformanceCounter("TCPv4", "Segments Retransmitted/sec");
        this.tcpConnEstablishedCounter = new PerformanceCounter("TCPv4", "Connections Established");

        this.udpDatagramsSentCounter = new PerformanceCounter("UDPv4", "Datagrams Sent/sec");
        this.udpDatagramsReceivedErrorsCounter = new PerformanceCounter("UDPv4", "Datagrams Received Errors");
        this.udpDatagramsReceivedSecCounter = new PerformanceCounter("UDPv4", "Datagrams Received/sec");

        this.tcpSegmentsSentCounter.NextValue();
        this.tcpSegmentsRecvCounter.NextValue();
        this.tcpSegmentsRetransmittedCounter.NextValue();
        this.tcpConnEstablishedCounter.NextValue();

        this.udpDatagramsSentCounter.NextValue();
        this.udpDatagramsReceivedErrorsCounter.NextValue();
        this.udpDatagramsReceivedSecCounter.NextValue();
    }

    // Refresh method to update metric values
    public void Refresh()
    {
        this.TcpSegmentsSent = this.tcpSegmentsSentCounter.NextValue();
        this.TcpSegmentsRecv = this.tcpSegmentsRecvCounter.NextValue();
        this.TcpSegmentsRetransmitted = this.tcpSegmentsRetransmittedCounter.NextValue();
        this.TcpConnectionsEstablished = this.tcpConnEstablishedCounter.NextValue();

        this.UdpDatagramsSent = this.udpDatagramsSentCounter.NextValue();
        this.UdpDatagramsReceivedErrors = this.udpDatagramsReceivedErrorsCounter.NextValue();
        this.UdpDatagramsReceivedSec = this.udpDatagramsReceivedSecCounter.NextValue();
    }

    // Implement IDisposable
    public void Dispose()
    {
        this.tcpSegmentsSentCounter?.Dispose();
        this.tcpSegmentsRetransmittedCounter?.Dispose();
        this.tcpSegmentsRecvCounter?.Dispose();
        this.tcpConnEstablishedCounter.Dispose();

        this.udpDatagramsSentCounter?.Dispose();
        this.udpDatagramsReceivedErrorsCounter?.Dispose();
        this.udpDatagramsReceivedSecCounter?.Dispose();
    }
}
