$(document).ready(function () {
    //$('#calendar').fullCalendar({
    //    header:
    //        {
    //            left: 'prev,next today',
    //            center: 'title',
    //            right: 'month,agendaWeek,agendaDay'
    //        },
    //    buttonText: {
    //        today: 'today',
    //        month: 'month',
    //        week: 'week',
    //        day: 'day'
    //    },

    //    events: function (start, end, timezone, callback) {
    //        $.ajax({
    //            url: '/Workout/GetScheduledWorkout',
    //            type: "GET",
    //            dataType: "JSON",

    //            success: function (result) {
    //                var events = [];
    //                if (result.data.length > 0) {
    //                    $.each(result.data, function (i, data) {
    //                        events.push(
    //                            {
    //                                title: data.WorkoutName,
    //                                description: data.WorkoutName,
    //                                start: moment(data.ScheduledDate).format('YYYY-MM-DD'),
    //                                end: moment(data.ScheduledDate).format('YYYY-MM-DD'),
    //                                backgroundColor: "#98f",
    //                                borderColor: "#000",
    //                                id: data.WorkoutScheduleId

    //                            });
    //                    });
    //                }
                   

    //                callback(events);
    //            }
    //        });
    //    },

    //    eventRender: function (event, element) {
    //        element.qtip(
    //            {
    //                content: event.description
    //            });
    //    },
    //    eventClick: function (calEvent, jsEvent, view) {
    //        var workoutShceduleId = calEvent.id;
    //        ScheduleWorkout(workoutShceduleId)

    //    },
    //    eventDrop: function (event) {
    //        var requestData = {};
    //        requestData.WorkoutScheduleId = parseInt(event.id);
    //        requestData.ScheduledDate = new Date(event.start._d);
    //        SaveWorkoutSchedule(requestData);
    //        //$.ajax({
    //        //    url: baseURL + 'vacation/update',
    //        //    data: 'title=' + event.title + '&start=' + start + '&end=' + end + '&id=' + event.id,
    //        //    type: "POST"
    //        //});
    //    },
    //    editable: true
    //});
});