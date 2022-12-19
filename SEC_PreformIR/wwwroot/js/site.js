const baseUrl = document.location.origin + document.location.pathname;
var res = baseUrl.substring(0, baseUrl.lastIndexOf('/'));


const connection = new signalR.HubConnectionBuilder()
    .withUrl(res + "/preformtemperature/temperatureHub")
    .withAutomaticReconnect({
        nextRetryDelayInMilliseconds: retryContext => {
            if (retryContext.elapsedMilliseconds < 60000) {
                // If we've been reconnecting for less than 60 seconds so far,
                // wait between 0 and 10 seconds before the next reconnect attempt.
                return Math.random() * 10000;
            } else {
                // If we've been reconnecting for more than 60 seconds so far, stop reconnecting.
                return null;
            }
        }
    })
    .build();

async function start() {
    try {

        await connection.start();
        console.log("SignalR Connected.");
    } catch (err) {
        console.log(err);
        setTimeout(start, 5000);
    }
};

connection.on("NotifyTempChange", function (status) {
    //change gauge on receive
    console.log(status);
    var progress_width = status.temperatureValue / 160 * 100;
    document.getElementById("gauge-container").style.width = progress_width + "%";
    document.getElementById("temperature-data").innerHTML = status.temperatureValue;

    myChart.data.datasets[0].data.push({
        x: Date.now(),
        y: status.temperatureValue
    });
    myChart.update('quiet');
});

const config = {
    type: 'line',
    data: {
        datasets: [
            {
                label: 'Husky 8 Preform Temperatures',
                backgroundColor: 'rgb(255, 99, 132)',
                borderColor: 'rgb(255, 99, 132)',
                cubicInterpolationMode: 'monotone',
                data: []
            }
        ]
    },
    options: {
        scales: {
            x: {
                type: 'realtime',
                realtime: {



                    //onRefresh: chart => {



                    //    chart.data.datasets(dataset => {
                    //        dataset.data.push({
                    //            x: Date.now(),
                    //            y: Math.random()

                    //        });
                    //    });
                    //}
                }
            }
        }
    }
};

var myChart = new Chart(
    document.getElementById('myChart'),
    config
);

connection.onclose(start);

// Start the connection.
start();









