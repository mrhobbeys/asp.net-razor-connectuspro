var SiteBlue = new function () {
    this.franchiseId = 0;
    this.allFranchiseLabel = "All Franchises";
    this.franchiseName = "";
    this.isCorporate = false;
    this.renderMenu = function (allowAllOption) {
        SiteBlue.franchiseId = $("#current > a").attr("fid");
        SiteBlue.franchiseName = $.trim($("#current > a").text());
        SiteBlue.isCorporate = $("#franchiseMenu").attr("isCorp") == "true";
        SiteBlue.isPlumber = $("#franchiseMenu").attr("isPlumber") == "true";
        if (allowAllOption && SiteBlue.isCorporate) {
            $("#menuItemContainer li:first").before('<li><a fid="0">' + SiteBlue.allFranchiseLabel + '</a></li>');
        }

        if (!SiteBlue.isCorporate)
            $("#options").hide();

        $('.menu').fixedMenu();

        $('#menuItemContainer > li > a').click(function (index) {
            SiteBlue.changeFranchise($(this).attr("fid"), $(this).text(), $(this).attr("href"));
            $('.menu').find('.active').removeClass('active');
            return false;
        });
    };
    this.changeFranchise = function (fId, franchiseName, actionUrl, callback) {
        this.franchiseId = fId;
        this.franchiseName = franchiseName;
        $("#current > a").text(franchiseName).attr('fid', fId);
        if (actionUrl) {
            $.ajax({
                url: actionUrl,
                data: JSON.stringify({ id: fId, showInactiveFranchises: $("#showInactiveCompanies").is(":checked") }),
                contentType: "application/json; charset=utf-8",
                success: function (msg) { if (callback) callback(); },
                type: "POST",
                async: false,
                dataType: "json",
                success: function (result) {
                },
                error: function (e) {
                    alert('Could not change franchise selection.');
                }

            });
        }
        $(this).trigger("FranchiseChanged", { franchiseId: fId });
    };
};

$(function ($) {
    $.fn.fixedMenu = function () {
        return this.each(function () {
            var menu = $(this);

            //close dropdown when clicked anywhere else on the document
            $("html").click(function () {
                menu.find('.active').removeClass('active');
            });

            var allowSelection = menu.find('ul li ul li').length > 1;
            if (allowSelection) {
                menu.find('ul li > a').bind('click', function (event) {
                    event.stopPropagation();
                    //check whether the particular link has a dropdown
                    if (!$(this).parent().hasClass('single-link') && !$(this).parent().hasClass('current')) {
                        //hiding drop down menu when it is clicked again

                        if ($(this).parent().hasClass('active')) {
                            $(this).parent().removeClass('active');
                        } else {
                            //displaying the drop down menu
                            $(this).parent().parent().find('.active').removeClass('active');
                            $(this).parent().addClass('active');
                        }
                    } else {
                        //hiding the drop down menu when some other link is clicked
                        $(this).parent().parent().find('.active').removeClass('active');

                    }
                });
            }
            else {
                $(".menu .dropDownItem").hide();
            }
        });
    };
});

$(document).ready(function () {
    $("#showInactiveCompanies").click(function () {
    SiteBlue.changeFranchise(SiteBlue.franchiseId, SiteBlue.franchiseName, $(this).attr("url"), function () { window.location.href=window.location.href; }); });
});
