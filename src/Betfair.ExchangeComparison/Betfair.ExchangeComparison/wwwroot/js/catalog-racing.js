$(document).ready(function () {

    $("#FormModel_Bookmaker").on('change', function () {
        onBookmakerChanged();
    });

    function onBookmakerChanged() {
        var data = $("#racing-form").serialize();
        $.post({
            url: "/Racing",
            data: data,
            success: function (result) {
                location.reload();
            }
        });
    };

    $("#refresh-btn-racing").on('click', function (e) {
        e.preventDefault();
        refreshPageRacing();
    });

    let refreshRate = 5;
    $("#FormModel_RefreshRateSeconds").on('change', function (e) {
        e.preventDefault();
        refreshRate = $(this).val();
        restartInterval();
    });

    function refreshPageRacing() {
        $.get({
            url: '?handler=RacingCatalog',
            beforeSend: function () {

                //$("#loading-selected").show();
            },
            success: function (result) {
                var parser = new DOMParser();
                var parsed = parser.parseFromString(result, 'text/html');
                var element = parsed.querySelector('#racing-catalog');
                $("#meetings").html('');
                $("#meetings").html(element.innerHTML);
                $("#last-updated-time").fadeOut(80).fadeIn(80).fadeOut(80).fadeIn(80);
            }
        });
    }

    let myBoolean = $("#FormModel_RefreshIsOn").val() == "True" ? true : false;
    let intervalId;

    if (myBoolean) {
        restartInterval();
    }

    $("#refresh-auto-racing").on('click', function (e) {
        myBoolean = !myBoolean;

        if (myBoolean) {
            $("#refresh-auto-racing").removeClass('btn-danger');
            $("#refresh-auto-racing").addClass('btn-success');
            $("#refresh-auto-racing").text('Auto Refresh On');
            $("#FormModel_RefreshIsOn").val('True')

            refreshPageRacing();
            intervalId = setInterval(refreshPageRacing, refreshRate * 1000);
        }
        else {
            $("#refresh-auto-racing").removeClass('btn-success');
            $("#refresh-auto-racing").addClass('btn-danger');
            $("#refresh-auto-racing").text('Auto Refresh Off');
            $("#FormModel_RefreshIsOn").val('False')

            clearInterval(intervalId);
        }
    });

    function restartInterval() {
        if (myBoolean) {
            $("#refresh-auto-racing").removeClass('btn-danger');
            $("#refresh-auto-racing").addClass('btn-success');
            $("#refresh-auto-racing").text('Auto Refresh On');
            $("#FormModel_RefreshIsOn").val('True')
            refreshPageRacing();
            clearInterval(intervalId);
            intervalId = setInterval(refreshPageRacing, refreshRate * 1000);
        }
    };
});
