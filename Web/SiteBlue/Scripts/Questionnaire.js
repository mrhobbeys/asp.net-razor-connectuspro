$(function () {

    // binds form submission and fields to the validation engine
    $("#form").validationEngine();

    $(".form .formSection:first").fadeIn(1000); // Show the first div

    // highlights the parent div when focus a child field
    $("input").focus(function () {
        $('.formComponent').removeClass('formComponentHighlight');
        $(this).parents('.formComponent').addClass('formComponentHighlight');
    });

    $("textarea").focus(function () {
        $('.formComponent').removeClass('formComponentHighlight');
        $(this).parents('.formComponent').addClass('formComponentHighlight');
    });

    $("#GPS").click(function () {
    });

    $("#AddMarketingType").click(function () {
        var marketingType = getMarketingType();

        var json = $.toJSON(marketingType);

        $.ajax({
            url: '/Questionnaire/SaveMarketingType',
            type: 'POST',
            dataType: 'json',
            data: json,
            contentType: 'application/json; charset=utf-8',
            succes: function (data) {
                var message = data.Message;
                $("#marketingResultMessage").html(message);
            }
        });
    });

    // attach back step button handler
    // hide on first step
    $("#back").hide().click(function () {
        var $step = $(".formSection:visible"); // Get current step
        if ($step.prev().hasClass("formSection")) {
            $step.hide().prev().fadeIn(1000);

            // disable back step button
            if (!$step.prev().prev().hasClass("formSection")) $("#back").hide();
        }
    });

//    // attach next step button handler
//    $("#next").click(function () {
//        $('.formComponent').removeClass('formComponentHighlight ');
//        $('.formComponent').removeClass('formComponentValidationFailed');
//        var $step = $(".formSection:visible"); // Get current step
//        var validator = $("form").validate(); // Obtain validator
//        var anyError = false;

//        $step.find("input").each(function () {
//            if (!validator.element(this)) {
//                // apply error style
//                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
//                anyError = true;
//            }
//        });

//        $step.find("textarea").each(function () {
//            if (!validator.element(this)) {
//                // apply error style
//                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
//                anyError = true;
//            }
//        });

//        if (anyError) return false; // Exit if any error found

//        if ($step.next().hasClass("formSection")) { // is there any next step?
//            $step.hide().next().fadeIn(1000);  // show it and hide current step
//            $("#back").show();   // recall to show backStep button
//        }

//        //            else { // this is last step, submit form
//        //                $("form").submit();
//        //            }
//    });

    //    // attach next step button handler
    //    $("#owner-next").click(function () {
    //        $('.formComponent').removeClass('formComponentHighlight ')
    //        $('.formComponent').removeClass('formComponentValidationFailed')
    //        var $step = $(".formSection:visible"); // Get current step
    //        var validator = $("form").validate(); // Obtain validator
    //        var anyError = false;

    //        $step.find("input").each(function () {
    //            if (!validator.element(this)) {
    //                // apply error style
    //                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
    //                anyError = true;
    //            }
    //        });

    //        $step.find("textarea").each(function () {
    //            if (!validator.element(this)) {
    //                // apply error style
    //                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
    //                anyError = true;
    //            }
    //        });

    //        if (anyError) return false; // Exit if any error found

    //        if ($step.next().hasClass("formSection")) { // is there any next step?
    //            $step.hide().next().fadeIn(1000);  // show it and hide current step
    //            $("#back").show();   // recall to show backStep button
    //        }

    //        //            else { // this is last step, submit form
    //        //                $("form").submit();
    //        //            }
    //    });


    //    // attach next step button handler
    //    $("#business-next").click(function () {
    //        $('.formComponent').removeClass('formComponentHighlight ')
    //        $('.formComponent').removeClass('formComponentValidationFailed')
    //        var $step = $(".formSection:visible"); // Get current step
    //        var validator = $("form").validate(); // Obtain validator
    //        var anyError = false;

    //        $step.find("input").each(function () {
    //            if (!validator.element(this)) {
    //                // apply error style
    //                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
    //                anyError = true;
    //            }
    //        });

    //        $step.find("textarea").each(function () {
    //            if (!validator.element(this)) {
    //                // apply error style
    //                $(this).parents('.formComponent').addClass('formComponentValidationFailed');
    //                anyError = true;
    //            }
    //        });

    //        if (anyError) return false; // Exit if any error found

    //        if ($step.next().hasClass("formSection")) { // is there any next step?
    //            $step.hide().next().fadeIn(1000);  // show it and hide current step
    //            $("#back").show();   // recall to show backStep button
    //        }

    //        else { // this is last step, submit form
    //            //$("form").submit();
    //            var business = $("#business-form");
    //            var businessaction = business.attr("action");
    //            alert("action: " + businessaction);
    //            var businessSerializedForm = business.serialize();
    //            alert("Serialized form: " + businessSerializedForm);
    //            $.post(businessaction,
    //                    businessSerializedForm,
    //                    function () {
    //                        alert('error occured when trying to submit the form, please try again!');
    //                    });
    //            return false;

    //            jQuery().ajaxStart(function () {
    //                $("#business-form").fadeOut("slow");
    //            });

    //            jQuery().ajaxStop(function () {
    //                $("#business-form").fadeIn("fast");
    //            });
    //        }
    //    });
});

function getMarketingType() {
    var questionnaireId = $("#QuestionnaireId").val();
    var marketingTypeName = $("#MarketingTypeName").val();
    var comment = $("#MarketingTypeComment").val();

    return (marketingTypeName == "") ? null : { QuestionnaireId: questionnaireId, MarketingTypeName: marketingTypeName, Comment: comment };
}
