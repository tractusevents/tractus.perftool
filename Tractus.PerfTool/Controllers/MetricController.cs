// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
[ApiController]
public class MetricController : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [Route("/pcdetails")]
    public IActionResult GetPCDetails()
    {
        var toReturn = new
        {
            ComputerInfo = Program.PCDetails.GetComputerDetailSnapshot(),
            NICInfo = Program.PCDetails.GetAllNetworkAdapterInfo(),
            MachineName = Environment.MachineName,

            CsvLoggingEnabled = Program.EnableLog,
            CsvSessionCode = Program.LogCode
        };

        return this.Ok(toReturn);
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/log/start/{code}")]
    public IActionResult StartLogSession(string code)
    {
        var finalCode = code.ToUpperInvariant();

        Program.LogCode = finalCode;
        Program.EnableLog = true;

        return this.Ok();
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("/log/stop")]
    public IActionResult StopLogSession()
    {
        Program.EnableLog = false;
        return this.Ok();
    }


    [HttpGet]
    [AllowAnonymous]
    [Route("/stats")]
    public IActionResult GetMetrics()
    {
        var metricsData = new
        {
            Timestamp = DateTime.Now,
            MachineName = Environment.MachineName,
            LogCode = Program.EnableLog ? Program.LogCode : "",
            EnableLog = Program.EnableLog,

            NetworkMetrics = Program.NetworkCards.Select(x => new
            {
                x.InstanceName,
                BytesTotalSec = x.BytesTotalSec.Current,

                MegabitsPerSecond = (x.BytesTotalSec.Current * 8) / 1_000_000,

                PacketsSec = x.PacketsSec.Current,
                PacketsSentUnicastSec = x.PacketsSentUnicastSec.Current,
                PacketsSentNonUnicastSec = x.PacketsSentNonUnicastSec.Current,
                PacketsOutboundDiscarded =x.PacketsOutboundDiscarded.Current,
                PacketsOutboundErrors = x.PacketsOutboundErrors.Current,

                PacketsReceivedUnicastSec = x.PacketsReceivedUnicastSec.Current,
                PacketsReceivedNonUnicastSec = x.PacketsReceivedNonUnicastSec.Current,
                PacketsReceivedDiscarded = x.PacketsReceivedDiscarded.Current,
                PacketsReceivedErrors = x.PacketsReceivedErrors.Current,

                OutputQueueLength = x.OutputQueueLength.Current,
                OffloadedConnections = x.OffloadedConnections.Current
            }),
            GlobalNetworkMetrics = new
            {
                Program.GlobalNetworkMetrics.TcpConnectionsEstablished,
                Program.GlobalNetworkMetrics.TcpSegmentsSent,
                Program.GlobalNetworkMetrics.TcpSegmentsRetransmitted,
                Program.GlobalNetworkMetrics.TcpSegmentsRecv,

                Program.GlobalNetworkMetrics.UdpDatagramsSent,
                Program.GlobalNetworkMetrics.UdpDatagramsReceivedSec,
                Program.GlobalNetworkMetrics.UdpDatagramsReceivedErrors,
            },
            ProcessorMetrics = Program.ProcessorCounters.Select(metrics => new
            {
                metrics.ProcessorInstanceName,
                metrics.UserTime,
                metrics.PrivilegedTime,
                metrics.InterruptTime,
                metrics.DPCTime,
                TotalTime = metrics.UserTime + metrics.PrivilegedTime + metrics.InterruptTime + metrics.DPCTime,
                metrics.InterruptsPerSec,
                metrics.DPCsQueuedPerSec
            })
        };

        return this.Ok(metricsData);
    }
}