// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text.Json;

public static class Program
{
    public static List<NetworkMetrics> NetworkCards { get; private set; } = [];
    public static List<ProcessorMetrics> ProcessorCounters { get; private set; } = [];
    public static GlobalNetworkMetrics GlobalNetworkMetrics { get; private set; } = new();
    public static ComputerDetails PCDetails { get; private set; } = new();

    public static bool EnableLog { get; set; } = false;
    public static string LogCode { get; set; } = "perflog";

    public static bool EnableWebLogging { get; set; } = false;
    public static bool EnableConsoleLog { get; set; } = true;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("Tractus PerfTool v2024.11.18.1 Starting up...");

        if (args.Any(x => x == "--debugweb"))
        {
            Console.WriteLine("WARNING - Enabling ASP.NET Core logging. (--debugweb set)");
            EnableWebLogging = true;
        }

        if(args.Any(x => x == "--disableconsole"))
        {
            Console.WriteLine("WARNING - Metrics will not be printed to the console. (--disableconsole set)");
            EnableConsoleLog = false;
        }

        var rootDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csv");
        Console.WriteLine($"Writing CSV files to '{rootDir}'");

        Directory.CreateDirectory(rootDir);
        Directory.SetCurrentDirectory(rootDir);

        var pcDetails = new ComputerDetails();
        var networkAdapters = pcDetails.GetAllNetworkAdapterInfo();
        var detailsSnapshot = pcDetails.GetComputerDetailSnapshot();
        PCDetails = pcDetails;

        var serialized = JsonSerializer.Serialize(networkAdapters, new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters =
            {
                new SemicolonSeparatedStringArrayConverter(),
            }
        });

        var pcDetailsSerialized = JsonSerializer.Serialize(detailsSnapshot, new JsonSerializerOptions
        {
            NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowNamedFloatingPointLiterals,
            Converters =
            {
                new SemicolonSeparatedStringArrayConverter(),
            }
        });

        File.WriteAllText("nic_settings.json", serialized);
        File.WriteAllText("pc_details.json", pcDetailsSerialized);

        var interfacesCategory = new PerformanceCounterCategory("Network Interface");
        var interfaces = interfacesCategory.GetInstanceNames();
        var machineName = Environment.MachineName;

        foreach(var item in interfaces)
        {
            var toAdd = new NetworkMetrics(item);
            NetworkCards.Add(toAdd);
            toAdd.Refresh();
        }

        var processorCategory = new PerformanceCounterCategory("Processor");
        var processorInstanceNames = processorCategory.GetInstanceNames().Where(x => x != "_Total").ToArray();
        
        foreach (var processorInstance in processorInstanceNames.OrderBy(x => x.PadLeft(2, '0')))
        {
            var toAdd = new ProcessorMetrics(processorInstance);
            ProcessorCounters.Add(toAdd);
            toAdd.Refresh();
        }

        GlobalNetworkMetrics.Refresh();

        Thread.Sleep(1000);

        var host = Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(builder =>
            {
                builder.UseUrls("http://+:9000")
                       .UseStartup<Startup>();
            })
            .Build();

        var webHostTask = host.RunAsync();

        var marker = "";

        var logEnabledLastRun = false;

        while (true)
        {
            Console.Clear();
            if (Console.KeyAvailable)
            {
                var keyInfo = Console.ReadKey(intercept: true);
                if (keyInfo.Key == ConsoleKey.M)
                {
                    marker = "M"; // Set marker
                }
                else if (keyInfo.Key == ConsoleKey.Q)
                {
                    break;
                }
            }

            GlobalNetworkMetrics.Refresh();
            foreach(var card in NetworkCards)
            {
                card.Refresh();
            }

            foreach (var cpuMetrics in ProcessorCounters)
            {
                cpuMetrics.Refresh();
            }

            if (!string.IsNullOrEmpty(marker))
            {
                Console.WriteLine($"*** MARKER '{marker}' SET AT {DateTime.Now} ***");
                Console.WriteLine();
            }

            if (EnableConsoleLog)
            {
                LogMetricsToConsole();
            }

            if (EnableLog && logEnabledLastRun != EnableLog)
            {
                Console.WriteLine($"Enable Log State Change - from {logEnabledLastRun} to {EnableLog} - {LogCode}");
                logEnabledLastRun = EnableLog;
            }

            if (EnableLog)
            {
                LogMetrics(marker);
            }

            marker = "";
            Thread.Sleep(1000);
        }

        await host.StopAsync();

        foreach (var procCounters in ProcessorCounters)
        {
            procCounters.Dispose();
        }

        foreach(var item in NetworkCards)
        {
            item.Dispose();
        }
    }

    static void LogMetricsToConsole()
    {
        // Network Metrics Output
        Console.WriteLine("=== Network Metrics ===");
        foreach (var nic in NetworkCards)
        {
            var megabitsPerSec = (nic.BytesTotalSec * 8) / 1_000_000;
            Console.WriteLine($"NIC: {nic.InstanceName}");
            Console.WriteLine($"Mb/s: {megabitsPerSec:F2}, Packets/sec: {nic.PacketsSec.Current:F2}");
            Console.WriteLine($"Packets Out (Unicast): {nic.PacketsSentUnicastSec.Current:F2}, Packets Out (Non-Unicast): {nic.PacketsSentNonUnicastSec.Current:F2}");
            Console.WriteLine($"Packets Out (Errors): {nic.PacketsOutboundErrors.Current:F2}, Packets Out (Discarded): {nic.PacketsOutboundDiscarded.Current:F2}");
            Console.WriteLine($"Packets In (Unicast): {nic.PacketsReceivedUnicastSec.Current:F2}, Packets In (Non-Unicast): {nic.PacketsReceivedNonUnicastSec.Current:F2}");
            Console.WriteLine($"Packets In (Errors): {nic.PacketsReceivedErrors.Current:F2}, Packets In (Discarded): {nic.PacketsReceivedDiscarded.Current:F2}");
            Console.WriteLine();
        }

        // Global Network Metrics
        Console.WriteLine("=== Global Network Stats ===");
        Console.WriteLine($"TCP Segments Sent/sec: {GlobalNetworkMetrics.TcpSegmentsSent:F2}");
        Console.WriteLine($"TCP Segments Received/sec: {GlobalNetworkMetrics.TcpSegmentsRecv:F2}");
        Console.WriteLine($"TCP Retransmitted Segments/sec: {GlobalNetworkMetrics.TcpSegmentsRetransmitted:F2}");
        Console.WriteLine($"TCP Connections Established/sec: {GlobalNetworkMetrics.TcpConnectionsEstablished:F2}");
        Console.WriteLine($"UDP Datagrams Sent/sec: {GlobalNetworkMetrics.UdpDatagramsSent:F2}");
        Console.WriteLine($"UDP Datagrams Received/sec: {GlobalNetworkMetrics.UdpDatagramsReceivedSec:F2}");
        Console.WriteLine($"UDP Errors: {GlobalNetworkMetrics.UdpDatagramsReceivedErrors:F2}");
        Console.WriteLine();

        // CPU Metrics in Table Format
        Console.WriteLine("=== CPU Metrics ===");
        Console.WriteLine("CPU Core | User Time | Kernel Time | Interrupt Time | DPC Time | Interrupts/sec | DPCs/sec | Total Usage");
        Console.WriteLine("---------|-----------|-------------|----------------|----------|----------------|----------|------------");
        foreach (var metrics in ProcessorCounters)
        {
            var totalCpuUsage = metrics.UserTime + metrics.PrivilegedTime + metrics.InterruptTime + metrics.DPCTime;
            Console.WriteLine(
                $"{metrics.ProcessorInstanceName,-8} | {metrics.UserTime,10:F2} | {metrics.PrivilegedTime,11:F2} | {metrics.InterruptTime,14:F2} | {metrics.DPCTime,8:F2} | {metrics.InterruptsPerSec,14:F2} | {metrics.DPCsQueuedPerSec,8:F2} | {totalCpuUsage,10:F2}");
        }
        Console.WriteLine();
    }


    static void LogMetrics(string marker)
    {
        var outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "csv", DateTime.Now.ToString("yyyyMMdd"), LogCode);
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var timestamp = DateTime.Now.ToString("o"); // ISO 8601 format
        var writeHeader = false;

        foreach (var nic in NetworkCards)
        {
            var filePath = Path.Combine(outputDirectory, $"log_{nic.InstanceName}.csv");
            writeHeader = !File.Exists(filePath);

            using var writer = new StreamWriter(filePath, true);
            
            if (writeHeader)
            {
                writer.Write("Timestamp,Marker,Mb/s,PacketsSec,PacketsOutUniSec,PacketsOutNonUniSec,PacketsOutErrors,PacketsOutDisc,PacketsInUniSec,PacketsInNonUniSec,PacketsInErrors,PacketsInDisc,OffloadedConn,OutputQueueLength\n");
            }

            var megabitsPerSec = (nic.BytesTotalSec * 8) / 1_000_000;

            // Write network metrics
            writer.Write($"{timestamp},{marker},{megabitsPerSec:F2},{nic.PacketsSec.Current:F2},{nic.PacketsSentUnicastSec.Current:F2},{nic.PacketsSentNonUnicastSec.Current:F2},{nic.PacketsOutboundErrors.Current:F2},{nic.PacketsOutboundDiscarded.Current:F2},{nic.PacketsReceivedUnicastSec.Current:F2},{nic.PacketsReceivedNonUnicastSec.Current:F2},{nic.PacketsReceivedErrors.Current:F2},{nic.PacketsReceivedDiscarded.Current:F2},{nic.OffloadedConnections.Current:F2},{nic.OutputQueueLength.Current:F2}\n");
        }

        var networkFilePath = Path.Combine(outputDirectory, $"log_tcpudp.csv");
        writeHeader = !File.Exists(networkFilePath);
        using var networkWriter = new StreamWriter(networkFilePath, true);

        if (writeHeader)
        {
            networkWriter.Write("Timestamp,Marker,TcpSegmentsSent,TcpSegmentsRecv,TcpSegmentsRetransmitted,TcpConnectionsEstablished,UdpDatagramsSent,UdpDatagramsReceivedErrors,UdpDatagramsReceivedSec\n");
        }
        networkWriter.WriteLine($"{timestamp},{marker},{GlobalNetworkMetrics.TcpSegmentsSent:F2},{GlobalNetworkMetrics.TcpSegmentsRecv:F2},{GlobalNetworkMetrics.TcpSegmentsRetransmitted:F2},{GlobalNetworkMetrics.TcpConnectionsEstablished:F2},{GlobalNetworkMetrics.UdpDatagramsSent:F2},{GlobalNetworkMetrics.UdpDatagramsReceivedErrors:F2},{GlobalNetworkMetrics.UdpDatagramsReceivedSec:F2}");


        var cpuFilePath = Path.Combine(outputDirectory, "log_cpu.csv");
        writeHeader |= !File.Exists(cpuFilePath);
        using var cpuWriter = new StreamWriter(cpuFilePath, true);

        if (writeHeader)
        {
            cpuWriter.Write("Timestamp,Marker");
            foreach (var metrics in ProcessorCounters)
            {
                cpuWriter.Write($",CPU{metrics.ProcessorInstanceName}_UserTime,CPU{metrics.ProcessorInstanceName}_KernelTime,CPU{metrics.ProcessorInstanceName}_IntTime,CPU{metrics.ProcessorInstanceName}_DPCTime,CPU{metrics.ProcessorInstanceName}_IntPerSec,CPU{metrics.ProcessorInstanceName}_DPCsPerSec,CPU{metrics.ProcessorInstanceName}_TotalUsage");
            }
            cpuWriter.WriteLine();
        }

        cpuWriter.Write($"{timestamp},{marker}");
        foreach (var metrics in ProcessorCounters)
        {
            var totalCpuUsage = metrics.UserTime + metrics.PrivilegedTime + metrics.InterruptTime + metrics.DPCTime;
            cpuWriter.Write($",{metrics.UserTime:F2},{metrics.PrivilegedTime:F2},{metrics.InterruptTime:F2},{metrics.DPCTime:F2},{metrics.InterruptsPerSec:F2},{metrics.DPCsQueuedPerSec:F2},{totalCpuUsage:F2}");
        }
        cpuWriter.WriteLine();
    }
}

