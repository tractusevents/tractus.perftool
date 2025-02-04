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

    private PerformanceCounter ip4DatagramsOutDiscarded;
    private PerformanceCounter ip4DatagramsRecvDiscarded;
    private PerformanceCounter ip4DatagramsRecvDelivered;


    public float TcpSegmentsSent { get; private set; }
    public float TcpSegmentsRecv { get; private set; }
    public float TcpSegmentsRetransmitted { get; private set; }
    public float TcpConnectionsEstablished { get; private set; }

    public float UdpDatagramsSent { get; private set; }
    public float UdpDatagramsReceivedErrors { get; private set; }
    public float UdpDatagramsReceivedSec { get; private set; }

    public float IPV4DatagramsOutDiscarded { get; set; }
    public float IPV4DatagramsRecvDeliveredSec { get; set; }
    public float IPV4DatagramsRecvDiscarded { get; set; }

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

        this.ip4DatagramsOutDiscarded = new PerformanceCounter("IPv4", "Datagrams Outbound Discarded");
        this.ip4DatagramsRecvDelivered = new PerformanceCounter("IPv4", "Datagrams Received Delivered/sec");
        this.ip4DatagramsRecvDiscarded = new PerformanceCounter("IPV4", "Datagrams Received Discarded");


        this.tcpSegmentsSentCounter.NextValue();
        this.tcpSegmentsRecvCounter.NextValue();
        this.tcpSegmentsRetransmittedCounter.NextValue();
        this.tcpConnEstablishedCounter.NextValue();

        this.udpDatagramsSentCounter.NextValue();
        this.udpDatagramsReceivedErrorsCounter.NextValue();
        this.udpDatagramsReceivedSecCounter.NextValue();

        this.ip4DatagramsOutDiscarded.NextValue();
        this.ip4DatagramsRecvDiscarded.NextValue();
        this.ip4DatagramsRecvDelivered.NextValue();
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

        this.IPV4DatagramsOutDiscarded = this.ip4DatagramsOutDiscarded.NextValue() ;
        this.IPV4DatagramsRecvDiscarded = this.ip4DatagramsRecvDiscarded.NextValue();
        this.IPV4DatagramsRecvDeliveredSec = this.ip4DatagramsRecvDelivered.NextValue();
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

        this.ip4DatagramsOutDiscarded?.Dispose();
        this.ip4DatagramsRecvDelivered?.Dispose();
        this.ip4DatagramsRecvDiscarded?.Dispose();
    }
}
