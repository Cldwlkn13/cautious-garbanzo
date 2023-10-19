$.fn.flashMessage = function (message) {
    console.log(message);
    var target = this;
    var options = { message: message, timeout: 6000 };
    if (options.message) {
        if (typeof options.message === "string" && options.message != "null") {

            if (options.message.includes("Success")) {
                target.addClass('bg-success');
                target.html("<span>" + options.message + "</span>");
            }
            else if (options.message.includes("Fail") && options.message != "null") {
                target.addClass('bg-danger');
                target.html("<span>" + options.message + "</span>");
            }

        } else {
            if (options.message != "null") {
                target.empty().append(options.message);
            }
        }
    }

    if (target.children().length === 0) return;

    target.fadeIn().one("click", function () {
        $(this).fadeOut();
    });

    if (options.timeout > 0) {
        setTimeout(function () { target.fadeOut(); }, options.timeout);
    }

    return this;
};




