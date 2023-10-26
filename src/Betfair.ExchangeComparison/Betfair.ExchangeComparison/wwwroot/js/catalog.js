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
});
