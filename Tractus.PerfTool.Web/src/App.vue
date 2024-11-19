<!--TODO:

- Azure logging

-->

<template>
    <nav class="navbar navbar-expand-lg fixed-top bg-body-tertiary">
        <div class="container-fluid">
            <a class="navbar-brand" href="#">Tractus PerfTool</a>
            <div class="collapse navbar-collapse">
                <div class="navbar-nav me-auto">
                    <a class="nav-link"
                        :class="{'active': currentMode === 'all'}"
                        @click="currentMode = 'all'">All</a>
                    <a class="nav-link"
                        :class="{'active': currentMode === 'cpu'}"
                        @click="currentMode = 'cpu'">CPU</a>
                    <a class="nav-link"
                        :class="{'active': currentMode === 'nic'}"
                        @click="currentMode = 'nic'">NIC</a>
                    <a class="nav-link"
                        :class="{'active': currentMode === 'networkglobal'}"
                        @click="currentMode = 'networkglobal'">TCP/UDP</a>
                    <a class="nav-link"
                        :class="{'active': currentMode === 'pcdetails'}"
                        @click="currentMode = 'pcdetails'">PC Info</a>

                </div>
            </div>

            <div class="d-flex">
                <div class="input-group">
                    <input v-model="candidateLogCode"
                        class="form-control form-control-sm font-monospace"
                        placeholder="Log Code"
                        :disabled="isLogging">
                    <button class="btn btn-sm"
                            :class="{'btn-danger': isLogging, 'btn-outline-success': !isLogging}"
                            @click="requestToggleLog">
                        {{ isLogging ? 'Stop CSV' : 'Start CSV' }}
                    </button>
                </div>
            </div>
        </div>
    </nav>

    <div class="container-fluid mt-5 pt-3" v-if="cpuMetrics.length">

        <div class="row"
             v-if="currentMode == 'pcdetails'">
            <div class="col"
                 v-if="pcDetails">
                <PcDetailsView :pcDetails="pcDetails">
                    
                </PcDetailsView>

            </div>
            <div class="col"
                 v-else>
                 <div class="spinner-border" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
                <h5>Awaiting data...</h5>
            </div>
        </div>

        <div class="row"
             v-if="currentMode == 'all' || currentMode == 'cpu'">
            <div class="col-md-3"
                 v-for="cpu in cpuMetrics">
                <h4>CPU #{{ cpu.cpu }}</h4>
                <AreaChart :data="getCpuGraphData(cpuMetrics[cpu.cpu])"
                           height="300">
                </AreaChart>
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>
                                % User
                            </th>
                            <th>
                                % Kernel
                            </th>
                            <th>
                                % Interrupt
                            </th>
                            <th>
                                % DPC
                            </th>
                            <th>
                                Total
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <td>
                                {{ cpu.data[cpu.data.length - 1].userTime.toFixed(2) }}
                            </td>
                            <td>
                                {{ cpu.data[cpu.data.length - 1].privilegedTime.toFixed(2) }}
                            </td>
                            <td>
                                {{ cpu.data[cpu.data.length - 1].interruptTime.toFixed(2) }}
                            </td>
                            <td>
                                {{ cpu.data[cpu.data.length - 1].dpcTime.toFixed(2) }}
                            </td>
                            <td>
                                <strong>{{ cpu.data[cpu.data.length - 1].cpuTotal.toFixed(2) }}</strong>
                            </td>
                        </tr>
                        
                    </tbody>
                </table>    
                <table class="table table-sm td-right">
                    <tbody>
                        <tr>
                            <th>
                                Interrupts/s
                            </th>
                            <td class="text-end">
                                {{ cpu.data[cpu.data.length - 1].interruptsPerSec.toFixed(2) }}
                            </td>
                            <th>
                                DPC Queued/s
                            </th>
                            <td class="text-end">
                                {{ cpu.data[cpu.data.length - 1].dpCsQueuedPerSec.toFixed(2) }}
                            </td>
                        </tr>                        
                    </tbody>
                </table>
            </div>
        </div>

        <div class="row mt-2"
             v-if="currentMode == 'all' || currentMode == 'cpu'">
            <div class="col-12">
                <h4>Interrupts by CPU</h4>
                <BarChart :data="cpuInterruptData">

                </BarChart>
            </div>
        </div>


        <template v-if="currentMode == 'all' || currentMode == 'nic'">

            <div v-for="nic in networkMetrics"
                class="row">
                <div class="col-12">
                    <h3>
                        {{ nic.nicName }} - {{ nic.data[nic.data.length - 1].megabitsPerSecond.toFixed(2) }} mbps
                    </h3>
                </div>

                <div class="col-md-6">
                    <AreaChart :data="getNicBandwidthGraphData(nic)"
                                :height="500"
                                :options="chartOptions">
                    </AreaChart>

                </div>

                <div class="col-md-6">
                    <AreaChart :data="getNicPacketGraphData(nic)"
                                :height="500"
                                :options="chartOptions">
                    </AreaChart>
                
                </div>
                <div class="col-12">
                    <table class="table table-sm td-end">
                        <tbody>
                            <tr>
                                <th>Packets/s</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsSec.toFixed(2) }}</td>
                                <th>Packets Sent Unicast/s</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsSentUnicastSec.toFixed(2) }}</td>
                                <th>Packets Sent Non-Unicast/s</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsSentNonUnicastSec.toFixed(2) }}</td>
                            </tr>
                            <tr>
                                <th>Packets Sent Discarded</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsOutboundDiscarded.toFixed(2) }}</td>
                                <th>Packets Sent Errors</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsOutboundErrors.toFixed(2) }}</td>
                                <th>Packets Recv Unicast/s</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsReceivedUnicastSec.toFixed(2) }}</td>
                            </tr>
                            <tr>
                                <th>Packets Recv Non-Unicast/s</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsReceivedNonUnicastSec.toFixed(2) }}</td>
                                <th>Packets Recv Discarded</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsReceivedDiscarded.toFixed(2) }}</td>
                                <th>Packets Recv Errors</th>
                                <td>{{ nic.data[nic.data.length - 1].packetsReceivedErrors.toFixed(2) }}</td>
                            </tr>
                            <tr>
                                <th>Output Queue Length</th>
                                <td>{{ nic.data[nic.data.length - 1].outputQueueLength.toFixed(2) }}</td>
                                <th>Offloaded Connections</th>
                                <td>{{ nic.data[nic.data.length - 1].offloadedConnections.toFixed(2) }}</td>
                            </tr>
                        </tbody>
                    </table>
                </div>

            </div>
            
        </template>


        <div class="row"
             v-if="currentMode == 'all' || currentMode == 'networkglobal'">
            <div class="col-md-6">
                <h4>TCP</h4>
                <AreaChart :data="tcpData"
                       :height="500"
                       :useY2="true"
                       :options="chartOptions">
                </AreaChart>
                <table class="table table-sm td-right">
                    <tbody>
                        <tr>
                            <th>
                                Sent
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].tcpSegmentsSent.toFixed(2) }}
                            </td>
                            <th>
                                Recv
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].tcpSegmentsRecv.toFixed(2) }}
                            </td>
                            <th>
                                Retransmit
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].tcpSegmentsRetransmitted.toFixed(2) }}
                            </td>
                            <th>
                                Established Conns
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].tcpConnectionsEstablished.toFixed(2) }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
            <div class="col-md-6">
                <h4>UDP</h4>
                <AreaChart :data="udpData"
                        :height="500"
                        :options="chartOptions">
                </AreaChart>            
                <table class="table table-sm td-right">
                    <tbody>
                        <tr>
                            <th>
                                Sent
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].udpDatagramsSent.toFixed(2) }}
                            </td>
                            <th>
                                Recv
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].udpDatagramsReceivedSec.toFixed(2) }}
                            </td>
                            <th>
                                Recv Errors
                            </th>
                            <td>
                                {{ this.globalNetworkMetrics[this.globalNetworkMetrics.length - 1].udpDatagramsReceivedErrors.toFixed(2) }}
                            </td>
                        </tr>
                    </tbody>
                </table>
            </div>
        </div>
    </div>
    <div v-else class="container text-center vh-max mt-5 pt-3">
        <div class="spinner-border" role="status">
            <span class="visually-hidden">Loading...</span>
        </div>
        <h5>Awaiting data...</h5>
    </div>
    <footer class="border-top border-2 mt-2 pt-3 pb-3">
        <div class="container text-center">
            <small>&copy; <a href="https://www.tractusevents.com">Tractus Events</a></small>
        </div>        
    </footer>
</template>
<script>
import AreaChart from './components/AreaChart.vue'
import BarChart from './components/BarChart.vue';
import PcDetailsView from './components/PcDetailsView.vue';

export default {
    components: {
        AreaChart,
        BarChart,
        PcDetailsView
    },

    computed: {

        cpuInterruptData() {
            let labels = [...Object.keys(this.cpuMetrics)];
            let datasets = [ { 
                label: "Interrupts/Sec",
                backgroundColor: '#f87979',
                data: [...this.cpuMetrics.map(x => x.data[x.data.length - 1].interruptsPerSec)]
            }, { 
                label: "DPCs Queued/Sec",
                backgroundColor: '#187979',
                data: [...this.cpuMetrics.map(x => x.data[x.data.length - 1].dpCsQueuedPerSec)]
            }, { 
                label: "% Usage",
                backgroundColor: '#187919',
                yAxisID: 'y2',
                data: [...this.cpuMetrics.map(x => x.data[x.data.length - 1].totalTime)]
            },]

            return {
                labels: labels,
                datasets: datasets
            };
        },

        bandwidthData() {
            return {
                labels: [...this.networkMetrics.map(x => x.timestamp)],
                datasets: [ {
                    label: 'Mbps',
                    backgroundColor: '#f87979',
                    borderColor: 'transparent',
                    pointRadius: 0,
                    fill: 'stack',
                    data: [...this.networkMetrics.map(x => x.megabitsPerSecond) ]
                } ]
            }
        },

        tcpData() {
            return {
                labels: [...this.globalNetworkMetrics.map(x => x.timestamp)],
                datasets: [ 
                    {
                        label: 'TCP Segments Sent',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        yAxisID: 'y',
                        data: [...this.globalNetworkMetrics.map(x => x.tcpSegmentsSent) ]
                    },
                    {
                        label: 'TCP Segments Recv',
                        backgroundColor: '#181979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        yAxisID: 'y',
                        data: [...this.globalNetworkMetrics.map(x => x.tcpSegmentsRecv) ]
                    },
                    {
                        label: 'TCP Segments Retransmitted',
                        backgroundColor: '#187979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        yAxisID: 'y',
                        data: [...this.globalNetworkMetrics.map(x => x.tcpSegmentsRetransmitted) ]
                    },
                ]
            }
        },

        udpData() {
            return {
                labels: [...this.globalNetworkMetrics.map(x => x.timestamp)],
                datasets: [ 
                    {
                        label: 'UDP Datagrams Sent',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        data: [...this.globalNetworkMetrics.map(x => x.udpDatagramsSent) ]
                    },
                    {
                        label: 'UDP Datagrams Recv',
                        backgroundColor: '#187979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        data: [...this.globalNetworkMetrics.map(x => x.udpDatagramsReceivedSec) ]
                    },
                    {
                        label: 'UDP Datagrams Recv Errors',
                        backgroundColor: '#A81979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        data: [...this.globalNetworkMetrics.map(x => x.udpDatagramsReceivedErrors) ]
                    },
                ]
            }
        },

        packetData() {
            return {
                labels: [...this.networkMetrics.map(x => x.timestamp)],
                datasets: [ 
                    {
                        label: 'Packets/Sec',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...this.networkMetrics.map(x => x.packetsSec) ],
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Recv Discarded',
                        backgroundColor: '#187979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...this.networkMetrics.map(x => x.packetsReceivedDiscarded) ],
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Outbound Discarded',
                        backgroundColor: '#A81979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        fill: 'stack',
                        data: [...this.networkMetrics.map(x => x.packetsOutboundDiscarded) ],
                    },
                ]
            }
        },
    },

    data() {
        return {
            currentMode: 'all',
            pcDetails: null,
            networkMetrics: [],
            cpuMetrics: [],
            globalNetworkMetrics: [],

            candidateLogCode: '',
            isLogging: false,
            logCode: '',

            chartOptions: {
                animation: {
                    duration: 0
                },
                scales: {
                    x: {
                        display: false,
                        stacked: true,
                        grid: {
                            color: '#444444'
                        }
                    },
                    y: {
                        min: 0,
                        stacked: true,
                        ticks: {
                            color: 'white',
                        },
                        grid: {
                            color: '#444444'
                        }
                    }
                },
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    tooltip: {
                        mode: 'index'
                    },

                    filler: {
                        propagate: true
                    }
                }
            },

            fetchDataTimer: null,
        }
    },

    methods: {
        getNicBandwidthGraphData(nicData) {
            if(!nicData || !nicData.data) {
                return;
            }

            return {
                labels: [...nicData.data.map(x => x.timestamp)],
                datasets: [ 
                    {
                        label: 'Mbps',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        
                        data: [...nicData.data.map(x => x.megabitsPerSecond) ],
                        fill: 'stack'
                    }

                ]
            }            
        },

        getNicPacketGraphData(nicData) {
            if(!nicData || !nicData.data) {
                return;
            }
            return {
                labels: [...nicData.data.map(x => x.timestamp)], // Timestamps for the X-axis
                datasets: [
                    {
                        label: 'Packets/sec',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsSec)], // Data for packets/sec
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Sent Unicast/sec',
                        backgroundColor: '#79f879',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsSentUnicastSec)], // Data for unicast packets sent/sec
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Sent Non-Unicast/sec',
                        backgroundColor: '#7987f8',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsSentNonUnicastSec)], // Data for non-unicast packets sent/sec
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Outbound Discarded',
                        backgroundColor: '#f8d879',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsOutboundDiscarded)], // Data for outbound discarded packets
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Outbound Errors',
                        backgroundColor: '#f879d8',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsOutboundErrors)], // Data for outbound packet errors
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Received Unicast/sec',
                        backgroundColor: '#d8f879',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsReceivedUnicastSec)], // Data for unicast packets received/sec
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Received Non-Unicast/sec',
                        backgroundColor: '#87f8f8',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsReceivedNonUnicastSec)], // Data for non-unicast packets received/sec
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Received Discarded',
                        backgroundColor: '#7987a8',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsReceivedDiscarded)], // Data for discarded received packets
                        fill: 'stack'
                    },
                    {
                        label: 'Packets Received Errors',
                        backgroundColor: '#a879f8',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...nicData.data.map(x => x.packetsReceivedErrors)], // Data for received packet errors
                        fill: 'stack'
                    }
                ]
            };
          
        },


        getCpuGraphData(cpuData) {
            if(!cpuData || !cpuData.data) {
                return;
            }

            return {
                labels: [...cpuData.data.map(x => x.timestamp)],
                datasets: [ 
                    {
                        label: '% User',
                        backgroundColor: '#f87979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        
                        data: [...cpuData.data.map(x => x.userTime) ],
                        fill: 'stack'
                    },
                    {
                        label: '% Kernel',
                        backgroundColor: '#187979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...cpuData.data.map(x => x.privilegedTime) ],
                        fill: 'stack'
                    },
                    {
                        label: '% Interrupt',
                        backgroundColor: '#A81979',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...cpuData.data.map(x => x.interruptTime) ],
                        fill: 'stack'
                    },
                    {
                        label: '% DPC',
                        backgroundColor: '#189929',
                        borderColor: 'transparent',
                        pointRadius: 0,
                        data: [...cpuData.data.map(x => x.dpcTime) ],
                        fill: 'stack'
                    }
                ]
            }
        },

        async build() {
            if(!this.fetchDataTimer) {
                this.fetchDataTimer = setInterval(this.fetchData, 1000);
            }

            let dataRaw = await fetch(`http://${location.hostname}:9000/pcdetails`);
            this.pcDetails = await dataRaw.json();
        },

        async fetchData() {
            let timestamp = new Date();
            let dataRaw = await fetch(`http://${location.hostname}:9000/stats`);

            let data = await dataRaw.json();

            this.logCode = data.logCode;
            this.isLogging = data.enableLog;

            if(this.isLogging) {
                this.candidateLogCode = data.logCode;
            }

            if(this.globalNetworkMetrics.length >= 300) {
                this.globalNetworkMetrics.splice(0, 1);
            }

            data.globalNetworkMetrics.timestamp = timestamp;
            this.globalNetworkMetrics.push(data.globalNetworkMetrics);

            // TODO: list out the various adapters

            if(this.networkMetrics.length >= 300) {
                this.networkMetrics.splice(0, 1);
            } 

            for(let i = 0; i < data.networkMetrics.length; i++) {
                if(this.networkMetrics.length <= i) {
                    let toAdd = {
                        nic: i,
                        nicName: data.networkMetrics[i].instanceName,
                        data: [],
                        graphData: [],
                    }

                    this.networkMetrics.push(toAdd);
                }

                let nic = this.networkMetrics[i];
                if(nic.data.length >= 300) {
                    nic.data.splice(0, 1);
                }

                nic.data.push({
                    timestamp: timestamp,
                    ...data.networkMetrics[i]
                });
            }


            if(this.cpuMetrics.length >= 300) {
                this.cpuMetrics.splice(0, 1);
            }

            for(let i = 0; i < data.processorMetrics.length; i++) {

                if(this.cpuMetrics.length <= i) {
                    let toAdd = {
                        cpu: i,
                        data: [],

                        graphData: [],
                    };

                    this.cpuMetrics.push(toAdd);
                }
            
                let cpu = this.cpuMetrics[i];

                if(cpu.data.length >= 300) {
                    cpu.data.splice(0, 1);
                }

                cpu.data.push({
                    timestamp: timestamp,
                    ...data.processorMetrics[i],
                    cpuTotal: data.processorMetrics[i].userTime + data.processorMetrics[i].privilegedTime + data.processorMetrics[i].interruptTime + data.processorMetrics[i].dpcTime
                });
            }

        },

        async requestToggleLog() {
            if(this.isLogging) {
                await fetch(`http://${location.hostname}:9000/log/stop`);
            } else {
                if(!this.candidateLogCode) {
                    alert("Log code cannot be blank.");
                    return;
                }

                this.candidateLogCode = this.candidateLogCode.toUpperCase();
                await fetch(`http://${location.hostname}:9000/log/start/${encodeURIComponent(this.candidateLogCode)}`);
            }

            await this.fetchData();
        },
    },

    beforeUnmount() {
        if(this.fetchDataTimer) {
            clearInterval(this.fetchDataTimer);
            this.fetchDataTimer = null;
        }
    },

    mounted() {
        this.build();
    }
}
</script>
