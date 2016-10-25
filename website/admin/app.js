$(document).ready(function(){
    $.ajax({
        url: "data.php",
        method: "GET",
        success: function(data) {
            console.log(data);
            var birthYear = [];

            var postcodeRanges = [
                {state: "QLD", range: {start: 4000, end: 4999}},
                {state: "VIC", range: {start: 3000, end: 3999}},
                {state: "ACT", range: {start: 2600, end: 2999}},
                {state: "NSW", range: {start: 2000, end: 2599}},
                {state: "SA", range: {start: 5000, end: 5999}},
                {state: "WA", range: {start: 6000, end: 6999}},
                {state: "TAS", range: {start: 7000, end: 7999}},
                {state: "NT", range: {start: 800, end: 999}}
            ];
            var skill = [0, 0, 0, 0, 0];
            var states = [0, 0, 0, 0, 0, 0, 0, 0];
            var languages = [0,0,0,0,0,0];
            var profession = [0,0,0,0,0,0];

            var dateBuckets = {};
            var dateLabels = [];

            for(var i in data) {
                //dateAdded.push(data[i].dateAdded);
                var datetimeAdded = new Date(data[i].dateAdded);
                var dateAdded = new Date(datetimeAdded.getFullYear(),datetimeAdded.getMonth(),datetimeAdded.getDate(),0,0,0,0);


                if (!dateBuckets[dateAdded]) {
                    dateBuckets[dateAdded] = 1;
                } else {
                    dateBuckets[dateAdded] += 1;
                }

                birthYear.push(data[i].birthYear);

                var dataPostcode = data[i].postcode;
                for (var j = 0; j < postcodeRanges.length; j++) {
                    var range = postcodeRanges[j].range;
                    if (range.start <= dataPostcode && dataPostcode <= range.end) {
                        states[j] += 1;
                    }
                }

                if (profession.hasOwnProperty(data[i].profession)) {
                    profession[data[i].profession - 1] += 1;
                }

                if (skill.hasOwnProperty(data[i].skill)) {
                    skill[data[i].skill - 1] += 1;
                }

                var itemLanguages = data[i].languages.split(',');
                console.log('itemLanguages' + itemLanguages);
                for (var x = 0; x < itemLanguages.length; x++) {
                    if (itemLanguages[x] !== "") {
                        languages[itemLanguages[x] - 1] += 1;
                    }
                }
            }

            console.log('skill: ' + skill);
            console.log('languages: ' + languages);

            // for each new chart, copy from here

            var chartdata1 = {
                labels: ['QLD', 'VIC', 'ACT', 'NSW', 'SA', 'WA', 'TAS', 'NT'],
                datasets : [
                    {
                        label: 'Location',
                        backgroundColor: 'rgba(200, 200, 200, 0.75)',
                        borderColor: 'rgba(200, 200, 200, 0.75)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: states
                    }
                ],

                fillColor: "rgba(207,42,175,0.5)",
            };

            var ctx1 = $("#canvas1");

            var barGraph1 = new Chart(ctx1, {
                type: 'bar',
                data: chartdata1,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: 'Location'
                    },
                    legend: {
                        display: false
                    },
                    fillColor: "rgba(207,42,175,0.5)"
                }
            });
            // to here


            // for each new chart, copy from here
            /*var languagesArray = [];
            for (i in languages) {
                languagesArray.push(languages[i]);
            }*/
            var chartdata2 = {
                labels: ["C/C++", "Python", "Java", "Go", "Perl", "PHP"],
                datasets : [
                    {
                        label: 'Languages Known',
                        backgroundColor: 'rgba(200, 200, 200, 0.75)',
                        borderColor: 'rgba(200, 200, 200, 0.75)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: languages
                    }
                ],

                fillColor: "rgba(207,42,175,0.5)",
            };

            var ctx2 = $("#canvas2");

            var barGraph2 = new Chart(ctx2, {
                type: 'bar',
                data: chartdata2,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: 'Languages'
                    },
                    legend: {
                        display: false
                    },
                    fillColor: "rgba(207,42,175,0.5)"
                }
            });
            // to here


            // for each new chart, copy from here
            var professionArray = [];
            for (i in profession) {
                professionArray.push(profession[i]);
            }
            var chartdata3 = {
                labels: ["Backend", "Frontend", "Student", "BA", "Tech", "Non-Tech"],
                datasets : [
                    {
                        label: 'Profession',
                        backgroundColor: 'rgba(200, 200, 200, 0.75)',
                        borderColor: 'rgba(200, 200, 200, 0.75)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: professionArray
                    }
                ],

                fillColor: "rgba(207,42,175,0.5)",
            };

            var ctx3 = $("#canvas3");

            var barGraph3 = new Chart(ctx3, {
                type: 'bar',
                data: chartdata3,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: 'Profession'
                    },
                    legend: {
                        display: false
                    },
                    fillColor: "rgba(207,42,175,0.5)"
                }
            });
            // to here


            // for each new chart, copy from here
            var skillArray = [];
            for (i in skill) {
                skillArray.push(skill[i]);
            }
            var chartdata4 = {
                labels: ['1 Poor', '2', '3', '4', '5 Excellent'],
                datasets : [
                    {
                        label: 'Skill Level',
                        backgroundColor: 'rgba(200, 200, 200, 0.75)',
                        borderColor: 'rgba(200, 200, 200, 0.75)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: skillArray
                    }
                ],

                fillColor: "rgba(207,42,175,0.5)",
            };

            var ctx4 = $("#canvas4");

            var barGraph4 = new Chart(ctx4, {
                type: 'bar',
                data: chartdata4,
                options: {
                    scales: {
                        yAxes: [{
                            ticks: {
                                beginAtZero:true
                            }
                        }]
                    },
                    title: {
                        display: true,
                        text: 'Skill Level'
                    },
                    legend: {
                        display: false
                    },
                    fillColor: "rgba(207,42,175,0.5)"
                }
            });
            // to here

            // for each new chart, copy from here

            var dateLabels = [];
            for (var item in dateBuckets) {
                dateLabels.push(new Date(item));
            }

            dateLabels = dateLabels.sort(function(a,b){
                return a - b;
            });
            var dateValues = [];
            for (var i = 0; i < dateLabels.length; i++) {
                dateValues.push(dateBuckets[dateLabels[i]]);
            }

            var chartdata5 = {
                labels: dateLabels,
                datasets : [
                    {
                        label: 'Times Accessed',
                        backgroundColor: 'rgba(200, 200, 200, 0.75)',
                        borderColor: 'rgba(200, 200, 200, 0.75)',
                        hoverBackgroundColor: 'rgba(200, 200, 200, 1)',
                        hoverBorderColor: 'rgba(200, 200, 200, 1)',
                        data: dateValues
                    }
                ],

                fillColor: "rgba(207,42,175,0.5)",
            };

            var ctx5 = $("#canvas5");

            var barGraph5 = new Chart(ctx5, {
                type: 'bar',
                data: chartdata5,
                options: {
                    title: {
                        display: true,
                        text: 'Date Accessed'
                    },
                    legend: {
                        display: false
                    },
                    scales: {
                        xAxes: [{
                            type: 'time',
                            time: {
                                displayFormats: {
                                    'millisecond': 'MMM DD',
                                    'second': 'MMM DD',
                                    'minute': 'MMM DD',
                                    'hour': 'MMM DD',
                                    'day': 'MMM DD',
                                    'week': 'MMM DD',
                                    'month': 'MMM DD',
                                    'quarter': 'MMM DD',
                                    'year': 'MMM DD',
                                }
                            }
                        }],
                    },
                    fillColor: "rgba(207,42,175,0.5)"
                }
            });
            // to here

        },
        error: function(data) {
            console.log(data);
        }
    });
});