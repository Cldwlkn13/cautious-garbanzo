$(document).ready(function () {
    $("#loading-meetings").hide();
    $("#loading-selected").hide();

    $("#load-meetings-0").click(function () { GetMeetingCatalog(0) });
    $("#load-meetings-1").click(function () { GetMeetingCatalog(1) });
    $("#load-meetings-2").click(function () { GetMeetingCatalog(2) });

    function GetMeetingCatalog(daysAdded) {
        $.get({
            url: "/Test/LoadMeetingCatalog/" + daysAdded,
            beforeSend: function () {
                $("#meetings").html('');
                $("#loading-meetings").show();
            },
            success: function (result) {
                $("#loading-meetings").hide();
                $("#meetings").html(result);
            }
        })
    };

    $("#meetings-container, #selected-container").on('click', ".runner-click", function () {
        var url = $(this).attr("value");
        AddOrRemoveFromSelected(url);
        $("html, body").animate({ scrollTop: 0 }, "slow");
    });

    $("#selected-container").on('click', ".selection-cb", function () {
        $(".selection-cb").on('change', function () {
            updateSelectionDomain();
        });
    });

    function AddOrRemoveFromSelected(url) {
        $.get({
            url: url,
            beforeSend: function () {
                $("#selected").html('');
                $("#loading-selected").show();
            },
            success: function (result) {
                $("#loading-selected").hide();
                $("#selected").html(result);
            }
        });
    }

    $("#selected-container").on('blur', ".domain-name-tb", function () {
        $("#event-created-name").on('blur', function () {
            onEventNameChanged();
        });
    });

    function onEventNameChanged() {
        var data = $("#create-form").serialize();

        $.post({
            url: "/SpecialsBuilder/UpdateEventDomain",
            data: data,
        });
    };

    $("#selected-container").on('blur', ".domain-name-tb", function () {
        $("#market-created-name").on('blur', function () {
            onEventNameChanged();
        });
    });

    function onMarketNameChanged() {
        var data = $("#create-form").serialize();

        $.post({
            url: "/SpecialsBuilder/UpdateMarketDomain",
            data: data,
        });
    };

    $("#selected-container").on('click', "#SelectedTemplate_Value", function () {
        $("#SelectedTemplate_Value").on('change', function () {
            onSelectedTemplateChange();
        });
    });

    function onSelectedTemplateChange() {
        var data = $("#create-form").serialize();

        $.post({
            url: "/SpecialsBuilder/SelectTemplate",
            data: data,
            beforeSend: function () {
                $("#selected").html('');
            },
            success: function (result) {
                $("#selected").html(result);
            }
        });
    };


    $("#selected-container").on('click', "#submit-create-only", function () {
        sendFormCreateOnly();
    });

    function sendFormCreateOnly() {
        var data = $("#create-form").serialize();
        $.post({
            url: "/SpecialsBuilder/CreateSelectedOnly",
            data: data,
            beforeSend: function () {
                $("#selected").html('');
            },
            success: function (result) {
                $("#selected").html(result);
            }
        });
    }

    $("#selected-container").on('click', "#submit-price-only", function () {
        sendFormPriceOnly();
    });

    function sendFormPriceOnly() {
        var data = $("#create-form").serialize();
        $.post({
            url: "/SpecialsBuilder/PriceSelectedOnly",
            data: data,
            beforeSend: function () {
                $("#selected").html('');
            },
            success: function (result) {
                $("#selected").html(result);
            }
        });
    }

    $("#selected-container").on('click', "#submit-create-and-price", function () {
        sendFormCreateAndPrice();
    });

    function sendFormCreateAndPrice() {
        var data = $("#create-form").serialize();
        $.post({
            url: "/SpecialsBuilder/CreateAndPriceSelected",
            data: data,
            beforeSend: function () {
                $("#selected").html('');
            },
            success: function (result) {
                $("#selected").html(result);
            }
        });
    }


    function updateSelectionDomain() {
        var data = $("#create-form").serialize();
        $.post({
            url: "/SpecialsBuilder/UpdateSelectionDomain",
            data: data,
            beforeSend: function () {
                $("#selected").html('');
            },
            success: function (result) {
                $("#selected").html(result);
            }
        });
    }
})

