$(document).ready(function () {

    $("#FormModel_Bookmaker").on('change', function () {
        onBookmakerChanged();
    });

    function onBookmakerChanged() {
        var data = $("#football-form").serialize();
        $.post({
            url: "/Football",
            data: data,
            success: function (result) {
                location.reload();
            }
        });
    };
});
