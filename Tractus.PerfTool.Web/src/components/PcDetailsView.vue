<template>
    <!-- PC Specifications -->
    <div class="mb-4">
        <h2>PC Specifications - <span class="font-monospace">{{ pcDetails.machineName }}</span></h2>
        <div class="row">
            <div class="col-md-4"><strong>OS:</strong> {{ pcDetails.computerInfo.productName }}</div>
            <div class="col-md-4"><strong>Version:</strong> {{ pcDetails.computerInfo.osVersion }}</div>
            <div class="col-md-4"><strong>Build:</strong> {{ pcDetails.computerInfo.buildNumber }}</div>
        </div>
        <div class="row">
            <div class="col-md-4"><strong>Release ID:</strong> {{ pcDetails.computerInfo.releaseID }}</div>
            <div class="col-md-4"><strong>Update Revision:</strong> {{ pcDetails.computerInfo.updateBuildRevision }}</div>
        </div>
        <div class="row">
            <div class="col-md-6"><strong>Motherboard:</strong> {{ motherboard.name }}</div>
            <div class="col-md-6"><strong>CPU:</strong> {{ cpu.name }} </div>
        </div>
        <div class="row">
            <div class="col-md-6"><strong>GPU:</strong> {{ gpu.name }}</div>
        </div>
    </div>

<!-- NIC Details -->
<div v-for="nic in pcDetails.nicInfo" :key="nic.instanceId" class="mb-4">
    <h3>{{ nic.name }} ({{ nic.description }})</h3>
    <div class="row">
    <!-- NIC Details -->
    <div class="col-md-6">
        <h5>Details</h5>
        <table class="table table-sm">
        <thead>
          <tr>
            <th scope="col">Property</th>
            <th scope="col">Value</th>
          </tr>
        </thead>
        <tbody>
            <tr>
                <td><b>Network Address</b></td>
                <td><input class="font-monospace form-control-sm form-control" readonly :value='nic.properties.NetworkAddresses'></td>
            </tr>
          <tr>
            <td><b>Instance ID</b></td>
            <td>{{ nic.instanceId }}</td>
          </tr>
          <tr>
            <td><b>Device Name</b></td>
            <td>{{ nic.properties.DeviceName }}</td>
          </tr>
          <tr>
            <td><b>Driver</b></td>
            <td>{{ nic.properties.DriverDescription }}</td>
          </tr>
          <tr>
            <td><b>Driver Version</b></td>
            <td>{{ nic.properties.DriverVersionString }}</td>
          </tr>
          <tr>
            <td><b>Driver Date</b></td>
            <td>{{ nic.properties.DriverDate }}</td>
          </tr>
          <tr>
            <td><b>Driver Provider</b></td>
            <td>{{ nic.properties.DriverProvider }}</td>
          </tr>
          <tr>
            <td><b>Driver Name</b></td>
            <td>{{ nic.properties.DriverName }}</td>
          </tr>
          <tr>
            <td><b>Speed</b></td>
            <td>{{ formatSpeed(nic.properties.Speed) }}</td>
          </tr>
          <tr>
            <td><b>Virtual</b></td>
            <td>{{ nic.properties.Virtual }}</td>
          </tr>
          <tr>
            <td><b>Wake-up Enabled</b></td>
            <td>{{ nic.properties.DeviceWakeUpEnable }}</td>
          </tr>
          <tr>
            <td><b>Media Disconnected</b></td>
            <td>{{ nic.properties.OperationalStatusDownMediaDisconnected }}</td>
          </tr>
          <tr>
            <td><b>Default Port Not Authenticated</b></td>
            <td>{{ nic.properties.OperationalStatusDownDefaultPortNotAuthenticated }}</td>
          </tr>
          <tr>
            <td><b>Interface Paused</b></td>
            <td>{{ nic.properties.OperationalStatusDownInterfacePaused }}</td>
          </tr>
          <tr>
            <td><b>Low Power State</b></td>
            <td>{{ nic.properties.OperationalStatusDownLowPowerState }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Advanced Driver Settings -->
    <div class="col-md-6">
        <h5>Advanced Settings</h5>
        <table class="table table-sm">
        <thead>
            <tr>
            <th>Setting</th>
            <th>Value</th>
            <th>Default</th>
            </tr>
        </thead>
        <tbody>
            <tr v-for="adv in nic.advancedProperties" :key="adv.registryKeyword">
            <td :class="{'bg-danger-subtle': adv.defaultDisplayValue && adv.isValueDifferentFromDefault}">{{ adv.displayName }}</td>
            <td :class="{'bg-danger': adv.defaultDisplayValue && adv.isValueDifferentFromDefault}">{{ adv.displayValue }}</td>
            <td :class="{'bg-danger-subtle': adv.defaultDisplayValue && adv.isValueDifferentFromDefault}">{{ adv.defaultDisplayValue }}</td>
            </tr>
        </tbody>
        </table>
    </div>
    </div>
</div>
</template>
<script>
export default {
    props: ['pcDetails'],

    computed: {
        motherboard() {
            return this.pcDetails.computerInfo.hardware.find(x => x.type == 'Motherboard');
        },

        cpu() {
            return this.pcDetails.computerInfo.hardware.find(x => x.type == 'Cpu');
        },

        memory() {
            return this.pcDetails.computerInfo.hardware.find(x => x.type == 'Memory')
        },

        gpu() {
            return this.pcDetails.computerInfo.hardware.find(x => x.type.indexOf("Gpu") != -1);
        }
    },

    methods: {
        formatSpeed(speed) {
            if (!speed) return "Unknown";
            const num = parseInt(speed);
            if (num >= 1e9) return `${num / 1e9} Gbps`;
            if (num >= 1e6) return `${num / 1e6} Mbps`;
            return `${num} bps`;
        },        
    }
}
</script>