<template>
    <div class="w-100" :style="`height: ${graphHeight}px`">
        <Line :options="graphOptions" 
              :data="data"></Line>
    </div>
</template>
<script>
import { Line } from 'vue-chartjs'

export default {
    props: [
        'height', 
        'data', 
        'options',
        'useY2'],

    computed: {
        graphHeight() {
            if(!this.height) {
                return 300;
            }

            return this.height;
        },

        graphOptions() {
            if(!this.options) {

                let scales = {
                    x: {
                        display: false,
                        stacked: true,
                        grid: {
                            color: '#444444'
                        }
                    },
                    y: {
                        stacked: true,
                        position: 'left',
                        min: 0,
                        max: 100,
                        ticks: {
                            color: 'white',
                        },
                        grid: {
                            color: '#444444'
                        }
                    }
                };


                return {
                    animation: {
                        duration: 0
                    },
                    scales: scales,
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

                };
            }

            return this.options;
        }
    },

    components: {
        Line
    },
}
</script>
