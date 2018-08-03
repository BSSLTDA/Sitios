(function ($) {
    $(document).ready(function () {

        $("#cssmenu").menumaker({
            title: "",
            breakpoint: 948,
            format: "multitoggle"
        });

        $("#cssmenu").prepend("<div id='menu-line'></div>");

        var foundActive = false, activeElement, linePosition = 0, menuLine = $("#cssmenu #menu-line"), lineWidth, defaultPosition, defaultWidth;

        $("#cssmenu > ul > li").each(function () {
            if ($(this).hasClass('active')) {
                activeElement = $(this);
                foundActive = true;
            }
        });

        if (foundActive === false) {
            activeElement = $("#cssmenu > ul > li").first();
        }

        //defaultWidth = lineWidth = activeElement.width();
        defaultWidth = lineWidth = 0;

        defaultPosition = linePosition = activeElement.position().left;

        menuLine.css("width", lineWidth);
        menuLine.css("left", linePosition);

        $("#cssmenu > ul > li").hover(function () {
            activeElement = $(this);
            //console.log(activeElement[0].id);
            if (activeElement[0].id != "infosesion") {
                lineWidth = activeElement.width();
                linePosition = activeElement.position().left;
                menuLine.css("width", lineWidth);
                menuLine.css("left", linePosition);
            }
        }, function () {
            menuLine.css("left", defaultPosition);
            menuLine.css("width", defaultWidth);
        });
    });
})(jQuery);
