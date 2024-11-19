<template>
    <div class="w-100" :style="`height: ${graphHeight}px`">
        <Bar :options="graphOptions" 
             :data="data"></Bar>
    </div>
</template>
<script>
import { Bar } from 'vue-chartjs'

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
                        display: true,
                        grid: {
                            color: '#444444'
                        }
                    },
                    y: {
                        position: 'left',
                        min: 0,
                        ticks: {
                            color: 'white',
                        },
                        grid: {
                            color: '#444444'
                        }
                    },
                    y2: {
                        stacked: true,
                        position: 'right',
                        min: 0,
                        max: 100,
                        ticks: {
                            color: 'white',
                        },
                        grid: {
                            color: 'transparent'
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
        Bar
    },
}
</script>
