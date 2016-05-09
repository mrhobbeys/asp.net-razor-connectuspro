$.expr[':'].contains = function (a, i, m) {
    return jQuery(a).text().toUpperCase().indexOf(m[3].toUpperCase()) >= 0;
};

var tmp;

$(document).ready(function () {

    $("body").ajaxComplete(function (e, resp) {
        if (resp.status == 404 || resp.status == 500) {
            alert("Error occured: please update page and try again");
            return false;
        }
    });

    //$("body").layout({ west__size: 400, south__size: 140 });

    $("#searchbtn").click(function () {
        search();
    });

    $("#searchcancel").click(function () {
        $("#searchtxt").val("");
        $("#searchmessages").hide();
        $(".edittreenode").remove();

        $("#sidetreecontrol").children("a").click();

        $("span.section").removeClass("boldnode");
        $("span.subsection").removeClass("boldnode");
        $("span.task").removeClass("boldnode");
    });

    $("#taskdetail").ajaxComplete(function () {

        $('.tabledetail').dataTable({
            "bJQueryUI": true,
            "bPaginate": false,
            "bLengthChange": true,
            "bFilter": true,
            "bSort": true,
            "bInfo": false,
            "bAutoWidth": false,
            "bRetrieve": true,
            "fnInitComplete": function (oSettings, json) {
            }
        });

    });

    $(".franchpostback").change(function () {
        var url = $(this).attr("url");
        var id = $(this).val();

        $.ajax({
            url: url,
            type: 'POST',
            data: { fid: id },
            success: function (result) {
                var select = $('.autopostback');
                select.empty();
                select.append($('<option></option>').val("-1").html("Select PriceBook"));

                if (result.ok) {
                    $.each(result.data, function (index, itemData) {
                        select.append($('<option></option>').val(itemData.PriceBookID).html(itemData.BookName));
                    });

                    select.fadeIn("slow");
                } else {
                    window.alert(' error : ' + result.message);
                }
            }
        });
    });

    $(".autopostback").change(function () {
        $('#searchFrm').submit();
    });

    $(".renpricebook").change(function () {
        var pid = $('select.renpricebook').val();

        if (pid == -1) {
            $("#txtRename").val("");
        } else {
            var bname = $('select.renpricebook option:selected').text();
            $("#txtRename").val(bname);
        }

        $("#txtRename").select();
    });

    $("#maintainchk").click(function () {
        $("#MFlag").val(this.checked);

        if ($('select.franchpostback option:selected').val() == "-1") {
            return;
        }

        if ($('select.autopostback option:selected').val() == "-1") {
            return;
        }

        $('#searchFrm').submit();
    });

    $("#Recalcbtn").click(function () {

        if ($('select.franchpostback option:selected').val() == "-1") {
            alert("Please select Franchise");
            return;
        }

        if ($('select.autopostback option:selected').val() == "-1") {
            alert("Please select PriceBook");
            return;
        }

        var pid = $("#PriceBookID").val();
        var pbname = $("#PriceBookID option:selected").text();

        var url = $(this).attr("url") + "/" + pid;
        var per = $("#percenttxt").val();

        per = parseFloat(per);
        if (isNaN(per)) {
            alert("Please enter a valid number.");
            return;
        } else {
            if (per > 1 && per < 99) {

            } else {
                alert("Please enter 1 ~ 99 number.");
                return;
            }
        }

        prettyPrompt({
            title: "Recalculate Membership Prices",
            message: "Are you sure you want to recalculate membership prices for " + pbname + "?",
            onOkay: function () {
                $("#searchprocess").show();
                $.ajax({
                    url: url,
                    data: { per: per },
                    success: function (result1) {
                        $("#taskgrid").empty();

                        $("#searchprocess").hide();
                    }
                });
            }
        });

    });

    $('#pricebookdlg').dialog({
        modal: false,
        autoOpen: false,
        buttons: {
            "Save": function () {
                var pid = $('select.autopostback option:selected').val();
                var url = $(this).attr("url").replace("id", pid);
                var bname = $("#BookName").val();

                $("#searchprocess").show();
                $("#copypricebook").attr("disabled", true);

                $.ajax({
                    url: url,
                    type: "POST",
                    data: { bookname: bname },
                    success: function (result) {
                        if (result.ok) {
                            var item = "<option value='" + result.data.pid + "'>" + result.data.pname + "</option>";
                            $(item).appendTo("select.autopostback");
                            $(item).appendTo("select.renpricebook");
                            $(item).appendTo("select.delpricebook");
                        } else {
                            window.alert(' error : ' + result.message);
                        }

                        $("#searchprocess").hide();
                        $("#copypricebook").attr("disabled", false);
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#BookName').val('');
        },
        height: 170,
        width: 400,
        resizable: true,
        title: "Copy PriceBook"
    });

    $('#sendpaddlg').dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            "Send": function () {
                var techId = $('#techList').val();

                if (!techId || techId == null || techId == "") {
                    alert("Please select a technician.");
                    return;
                }

                var pid = $('select.autopostback option:selected').val();
                var url = $(this).attr("url");

                $("#searchprocess").show();

                $.ajax({
                    url: url,
                    type: "POST",
                    data: { id: pid, techId: techId },
                    success: function (result) {
                        if (result.Success) {
                            window.alert('Price book "' + result.Name + "' sent to tablet '" + result.Tablet + '".');
                        } else {
                            window.alert(' error : ' + result.message);
                        }

                        $("#searchprocess").hide();
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () { },
        height: 170,
        width: 400,
        resizable: true,
        title: "Send To Tablet"
    });

    $('#sectiondlg').dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            "Save": function () {
                var sectionname = $('#SectionName').val();
                if (sectionname == "") {
                    alert("Input the Section Name");
                    return;
                }

                var pbid = $('select.autopostback option:selected').val();
                var active = $('#ActiveYN').is(':checked');
                var url = $(this).attr("url");

                var params = 
                {
                    PriceBookID: pbid,
                    SectionName: sectionname,
                    ActiveYN: active
                };

                $.ajax({
                    url: url,
                    type: 'POST',
                    data: params,
                    success: function (result) {
                        $('#searchFrm').submit();
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#SectionName').val('');
        },
        height: 170,
        width: 400,
        resizable: true,
        title: "Add Section"
    });

    $('#subsectiondlg').dialog({
        modal: false,
        autoOpen: false,
        buttons: {
            "Save": function () {
                var form = $('#subsectionFrm');
                var sid = $('#hSectionID').val();

                $.ajax({
                    url: form.attr('action'),
                    type: form.attr('method'),
                    data: form.serialize(),
                    success: function (result) {
                        $('#searchFrm').submit();
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#SubSectionName').val('');
        },
        height: 170,
        width: 400,
        resizable: true,
        title: "Add SubSection"
    });

    $('#taskdlg').dialog({
        modal: false,
        autoOpen: false,
        buttons: {
            "Save": function () {
                var form = $('#taskFrm');

                $.ajax({
                    url: form.attr('action'),
                    type: form.attr('method'),
                    data: form.serialize(),
                    success: function (result) {
                        $('#searchFrm').submit();
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () {
            $('#JobCode').val('');
            $('#JobCodeDescription').val('');
            $('#JobCost').val('');
            $('#JobStdPrice').val('');
            $('#JobMemberPrice').val('');
            $('#JobAddonStdPrice').val('');
            $('#JobAddonMemberPrice').val('');
            $('#ResAccountCode').val('');
            $('#ComAccountCode').val('');
        },
        height: 470,
        width: 600,
        resizable: true,
        title: "Add Task"
    });

    $('#rendlg').dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            "Rename": function () {
                var pbid = $('.renpricebook').val();

                if (pbid == -1) {
                    alert("Please select a PriceBook.");
                    return;
                }

                var renname = $('#txtRename').val();

                var url = $(this).attr("url");

                $("#searchprocess").show();
                $("#renpricebook").attr("disabled", true);

                $.ajax({
                    url: url,
                    type: "POST",
                    data: { id: pbid, renname: renname },
                    success: function (result) {
                        if (result == "True") {
                            $('select.autopostback').val(pbid);
                            $('select.autopostback option:selected').text(renname);
                            $('select.renpricebook option:selected').text(renname);

                            $('#searchFrm').submit();
                        } else {
                            alert("Error");
                        }

                        $("#searchprocess").hide();
                        $("#renpricebook").attr("disabled", false);
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () { },
        height: 190,
        width: 410,
        resizable: true,
        title: "Rename PriceBook"
    });

    $('#deldlg').dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            "Delete": function () {
                var pbid = $('.delpricebook').val();

                if (pbid == -1) {
                    alert("Please select a PriceBook.");
                    return;
                }

                var url = $(this).attr("url");

                $("#searchprocess").show();
                $("#delpricebook").attr("disabled", true);

                $.ajax({
                    url: url,
                    type: "POST",
                    data: { id: pbid },
                    success: function (result) {
                        if (result == "True") {
                            //$("select.autopostback option[value='" + pbid + "']").remove();

                            document.location.href = $("#searchFrm").attr("action") + "/Index?frid=" + SiteBlue.franchiseId;
                        } else {
                            alert("Error");
                        }

                        $("#searchprocess").hide();
                        $("#delpricebook").attr("disabled", false);
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () { },
        height: 150,
        width: 410,
        resizable: true,
        title: "Delete PriceBook"
    });

    $("#partadjustdlg").dialog({
        modal: true,
        autoOpen: false,
        buttons: {
            "Submit": function () {
                var catid = $('#categoryList').val();

                if (!catid || catid == null || catid == "") {
                    alert("Please select a Category.");
                    return;
                }

                var pbid = $('select.autopostback option:selected').val();
                var url = $(this).attr("url");

                $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

                $.ajax({
                    url: url,
                    type: "POST",
                    cache: false,
                    data: { pbid: pbid, catid: catid },
                    success: function (result) {
                        $("#taskgrid").empty().html(result);
                    }
                });

                $(this).dialog("close");
            },
            "Cancel": function () {
                $(this).dialog("close");
            }
        },
        close: function () { },
        height: 170,
        width: 400,
        resizable: true,
        title: "Part Adjustment"
    });

    $("#searchFrm").submit(function () {
        $("#searchmessages").hide();
        ClearAllDivs();

        if ($('select.autopostback').val() == -1)
            return;

        $("#pricebooktree").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

        $.ajax({
            url: $(this).attr("action"),
            cache: false,
            type: $(this).attr("method"),
            data: $(this).serialize(),
            success: function (result) {
                $("#pricebooktree").empty().html(result);

                $("#example").treeview({ collapsed: true, control: "#sidetreecontrol" });

                clearMaintain();

                $("#laborpricebook").click(function () {
                    var pid = $("#PriceBookID").val();
                    var url = $(this).attr("url") + "/" + pid;

                    $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');
                    $.ajax({
                        url: url,
                        success: function (result1) {
                            $("#taskgrid").empty().html(result1);
                        }
                    });
                });

                $("#copypricebook").click(function (event) {
                    var bname = $('select.autopostback option:selected').text() + " - Copy";
                    $("#BookName").val(bname);

                    $('#pricebookdlg').dialog('open');
                });

                $("#renpricebook").click(function (event) {
                    var pid = $('.autopostback').val();
                    $(".renpricebook").val(pid);

                    var bname = $('select.autopostback option:selected').text();
                    $("#txtRename").val(bname);
                    $("#txtRename").select();

                    $('#rendlg').dialog('open');
                });

                $("#delpricebook").click(function (event) {
                    var pid = $('.autopostback').val();
                    $(".delpricebook").val(pid);

                    $('#deldlg').dialog('open');
                });

                var franchiseId = SiteBlue.franchiseId;
                var url = $("#sendtoipad").attr("techurl");
                $("#sendtoipad").click(function (event) {
                    $.ajax({
                        url: url,
                        type: 'POST',
                        data: { id: franchiseId },
                        success: function (result) {
                            var select = $('#techList');
                            select.empty();
                            select.append($('<option></option>').val("").html("Select Technician"));
                            $.each(result, function (index, r) {
                                select.append($('<option></option>').val(r.key).html(r.label));
                            });

                        },
                        error: function (result) {
                            alert('error getting technicians.');
                        }
                    });

                    $('#sendpaddlg').dialog('open');
                });

                $("#PartAdjustbtn").click(function (event) {
                    var url = $(this).attr("url");
                    $.ajax({
                        url: url,
                        type: 'POST',
                        success: function (result) {
                            var select = $('#categoryList');
                            select.empty();
                            select.append($('<option></option>').val("").html("Select Category"));
                            $.each(result, function (index, r) {
                                select.append($('<option></option>').val(r.key).html(r.label));
                            });

                        },
                        error: function (result) {
                            alert('error getting technicians.');
                        }
                    });

                    $('#partadjustdlg').dialog('open');
                });

                $("#createSection").click(function () {
                    $('#sectiondlg').dialog('open');
                });

                $(".DeleteSection").click(function () {
                    var sid = this.id;
                    var url = $(this).attr("url");

                    prettyPrompt({
                        title: "Delete Section",
                        message: "Are you sure you want to delete this <" + sid + "> ?",
                        onOkay: function () {
                            $.ajax({
                                url: url,
                                success: function (result) {
                                    if (result == "success") {
                                        $('#searchFrm').submit();
                                    }
                                }
                            });
                        }
                    });
                });

                $(".CopySection").click(function () {
                    var sid = this.id;
                    var url = $(this).attr("url");

                    prettyPrompt({
                        title: "Copy Section",
                        message: "Are you sure you want to copy this <" + sid + "> ?",
                        onOkay: function () {
                            $.ajax({
                                url: url,
                                success: function (result) {
                                    if (result == "success") {
                                        $('#searchFrm').submit();
                                    }
                                }
                            });
                        }
                    });
                });

                $(".LaborSection").click(function () {
                    var url = $(this).attr("url");

                    $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');
                    $.ajax({
                        url: url,
                        data: { tlevel: 1 },
                        success: function (result1) {
                            $("#taskgrid").empty().html(result1);
                        }
                    });

                    return false;
                });

                $(".AddSubSection").click(function () {
                    $("#hSectionID").val(this.id);
                    $('#subsectiondlg').dialog('open');
                });

                //Both Section and Sub Section
                $(".EditSection").click(function () {
                    //$(".edittreenode").remove();
                    $(".edittreenode.cancel").click();
                    $(".EditSection").show();
                    var val;
                    if ($(this).parent().children("span").children("a").size() > 0) {
                        val = $(this).parent().children("span").children("a").text();
                        $(this).parent().children("span").children("a").text("");
                    }
                    else {
                        val = $(this).parent().children("span").text();
                        $(this).parent().children("span").text("");
                    }

                    $(this).parent().children("span").append("<input maxlength='50' class='edittreenode' type='text' origvalue='" + $.trim(val) + "' value='" + $.trim(val) + "' style='width:250px;left:45px;position:absolute;' /><img title='" + this.title + "' src='/Areas/PriceBook/images/updatetreenode.jpg' class='edittreenode update' style='left:305px;position:absolute;margin-top:5px;'/><img src='/Areas/PriceBook/images/cancel.jpg' class='edittreenode cancel' style='left:320px;position:absolute;margin-top:5px;'/>");

                    $(this).hide();
                    $(".edittreenode.update").click(function () {
                        $.ajax({
                            url: $(this).attr("title"),
                            context: this,
                            data: "name=" + encodeURIComponent($(".edittreenode").val()),
                            success: function (resultupdate) {
                                if ($(this).parent().children("a").size() > 0) {
                                    $(this).parent().children("a").text($(".edittreenode").val());
                                }
                                else {
                                    $(this).parent().text($(".edittreenode").val());
                                }
                                $(".edittreenode").remove();
                                $(".EditSection").show();
                            },
                            error: function (resultupdate) {
                                alert("Error: Saving values, it could be that name is too long");
                                return false;
                            }
                        });

                        return false;
                    });

                    $(".edittreenode.cancel").click(function () {
                        if ($(this).parent().children("a").size() > 0) {
                            $(this).parent().children("a").text($(".edittreenode").attr("origvalue"));
                        }
                        else {
                            $(this).parent().text($(".edittreenode").attr("origvalue"));
                        }
                        $(".edittreenode").remove();
                        $(".EditSection").show();
                        return false;
                    });

                    return false;
                });

                $(".sectiontext").click(function () {
                    if ($(this).children("input").size() == 0) {
                        $(this).parent("span").click();
                    }
                    else { return false; }
                });

                $(".ActiveYNSection:input:not(:checked)").parent().parent().addClass("inactive");
                $(".ActiveYNSection").click(function () {
                    $.ajax({
                        url: $(this).attr("href"),
                        data: "active=" + this.checked
                    });
                    if (this.checked) {
                        $(this).parent().parent().removeClass("inactive");
                    }
                    else {
                        $(this).parent().parent().addClass("inactive");
                    }
                });

                $("a.subsectionlink").click(function () {
                    $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

                    var mflag = $("#MFlag").val();

                    $.ajax({
                        url: this.href,
                        cache: false,
                        data: { mflag: mflag },
                        success: function (result1) {
                            $("#taskgrid").empty().html(result1);
                        }
                    });

                    $("#subtaskname").html($(this).text());
                    return false;
                });

                $(".sectionli").click(function () {
                    if ($("#" + this.id + "SubSections").children().length == 1 && $("#" + this.id + "SubSections").children(":first").attr("class") == "templi last") {

                        var mflag = $("#MFlag").val();

                        $.ajax({
                            url: this.title,
                            data: { mflag: mflag },
                            context: this,
                            success: function (result2) {

                                $("#" + this.id + "SubSections").empty();

                                var branches = $(result2).prependTo("#" + this.id + "SubSections");
                                $("#example").treeview({
                                    add: branches
                                });

                                clearMaintain();

                                setSubSectionFuncs();

                                $(".taskli").click(function () {
                                    if ($("#" + this.id + "Tasks").children().length == 1 && $("#" + this.id + "Tasks").children(":first").attr("class") == "templi last") {

                                        var mflag = $("#MFlag").val();

                                        $.ajax({
                                            url: this.title,
                                            cache: false,
                                            data: { mflag: mflag },
                                            context: this,
                                            success: function (result3) {
                                                $("#" + this.id + "Tasks").empty();

                                                var branches = $(result3).prependTo("#" + this.id + "Tasks");
                                                $("#example").treeview({
                                                    add: branches
                                                });

                                                clearMaintain();

                                                setTaskFuncs();
                                            }
                                        });

                                        $("#subtaskname").html($(this).text());
                                        //return false;
                                    }
                                });
                            }
                        });
                    }

                });
            }

        });

        return false;
    });

});

function ClearAllDivs() {
    $("#taskdetail").empty();
    $("#partname").empty();
    $("#taskgrid").empty();
    $("#subtaskname").empty();
    $("#uploadimg").empty();
    $("#viewimg").empty();
}

function search() {

    $("#searchmessages").hide();
    $(".edittreenode").remove();

    if ($('select.franchpostback option:selected').val() == "-1") {
        alert("Please select Franchise");
        return;
    }

    if ($('select.autopostback option:selected').val() == "-1") {
        alert("Please select PriceBook");
        return;
    }

    $("#searchprocess").show();
    $("#searchbtn").attr("disabled", true);
    $("#searchcancel").attr("disabled", true);

    $("#sidetreecontrol").children("a").click();

    $("span.section").removeClass("boldnode");
    $("span.subsection").removeClass("boldnode");
    $("span.task").removeClass("boldnode");

    $.ajax({
        url: $("#searchbtn").attr("url").replace("id", $('select.autopostback option:selected').val()) + "?searchstr=" + $.trim($("#searchtxt").val()),
        context: this,
        success: function (result) {
            $("#hiddendiv").html(result);

            var sbid = 0;
            for (var i = 1; i <= $("#hiddendiv").children("ul").size(); i++) {
                if (sbid != $("#hiddendiv").children("ul:nth-child(" + i + ")").attr("id")) {
                    sbid = $("#hiddendiv").children("ul:nth-child(" + i + ")").attr("id");
                    $("#" + sbid + "SubSections").empty();
                }

                //$("#" + sbid + "SubSections").append($("#hiddendiv").children("ul:nth-child(" + i + ")").html());

                var branches = $($("#hiddendiv").children("ul:nth-child(" + i + ")").html()).prependTo("#" + sbid + "SubSections");
                $("#example").treeview({
                    add: branches
                });
            }

            clearMaintain();

            setSubSectionFuncs();
            setTaskFuncs();

            $("#hiddendiv").empty();

            $(".taskli").children("ul").hide();

            $("span.section:contains('" + $("#searchtxt").val() + "')").addClass("boldnode");
            $("span.subsection:contains('" + $("#searchtxt").val() + "')").addClass("boldnode");
            $("span.task:contains('" + $("#searchtxt").val() + "')").addClass("boldnode");

            //alert($("span.boldnode").parent().parent().parent().parent().parent(".expandable").children("span").size());
            $("span.boldnode").parent().parent().parent().parent().parent(".expandable").children("span").click();

            $("span.boldnode").parent().parent().parent(".expandable").children("span").click();

            //$(".boldnode.task").parent().parent().parent(".expandable.taskli").children("div").click();

            $("#searchprocess").hide();
            $("#searchbtn").attr("disabled", false);
            $("#searchcancel").attr("disabled", false);

            if ($(".boldnode").size() == 0) {
                $("#searchmessages").html("No match found");
                $("#searchmessages").show();
            }
            else {
                $(".boldnode:first").append("<input type='text' id='txtfoucsnode' />");
                $("#txtfoucsnode").focus();
                $("#txtfoucsnode").remove();

                if ($(".boldnode").size() > 100) {
                    $("#searchmessages").html("You search returned too many records. Please try to narrow down your search. Only first few matching tasks were returned");
                    $("#searchmessages").show();
                }
                else {
                    $("#searchmessages").html("Total " + $(".boldnode").size() + " matches found");
                    $("#searchmessages").show();
                }
            }
        }
    });


}

function updatetask(obj) {
    //$(".edittreenode").remove();
    $(".edittreenode.cancel").click();
    $(".EditTask").show();
    var valorig = $(obj).parent().children("span").children("a").text();
    var jobcode = $(obj).parent().children("span").attr("jobcode");
    var val;
    val = valorig.replace(jobcode, "");

    $(obj).parent().children("span").children("a").text("");

    $(obj).parent().children("span").append("<input maxlength='50' class='edittreenode' type='text' origvalue='" + $.trim(valorig) + "' value='" + $.trim(val) + "' style='width:250px;left:45px;position:absolute;' /><img title='" + obj.title + "' src='/Areas/PriceBook/images/updatetreenode.jpg' class='edittreenode update' style='left:305px;position:absolute;margin-top:5px;'/><img src='/Areas/PriceBook/images/cancel.jpg' class='edittreenode cancel' style='left:320px;position:absolute;margin-top:5px;'/>");

    $(obj).hide();
    $(".edittreenode.update").click(function () {
        $.ajax({
            url: $(this).attr("title"),
            context: this,
            data: "name=" + encodeURIComponent($(".edittreenode").val()),
            success: function (resultupdate) {
                $(this).parent().children("a").text(jobcode + " " + $(".edittreenode").val());
                $(".edittreenode").remove();
                $(".EditTask").show();
            },
            error: function (resultupdate) {
                alert("Error: Saving values, it could be that name is too long");
            }
        });

        return false;
    });

    $(".edittreenode.cancel").click(function () {
        $(this).parent().children("a").text($(".edittreenode").attr("origvalue"));
        $(".edittreenode").remove();
        $(".EditTask").show();
        return false;
    });

    return false;
}

function deletetask(obj) {
    var jid = obj.id;
    var url = $(obj).attr("url");
    var liobj = $(obj).parent().parent();

    prettyPrompt({
        title: "Delete Task",
        message: "Are you sure you want to delete this <" + jid + "> ?",
        onOkay: function () {
            $.ajax({
                url: url,
                success: function (result) {
                    if (result == "success") {
                        $('#searchFrm').submit();
                    }
                }
            });
        }
    });
}

function copytask(obj) {
    var jid = obj.id;
    var url = $(obj).attr("url");

    prettyPrompt({
        title: "Copy Task",
        message: "Are you sure you want to copy this <" + jid + "> ?",
        onOkay: function () {
            $.ajax({
                url: url,
                success: function (result) {
                    if (result == "success") {
                        $('#searchFrm').submit();
                    }
                }
            });
        }
    });
}

function labortask(obj) {
    var url = $(obj).attr("url");

    $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');
    $.ajax({
        url: url,
        data: { tlevel: 3 },
        success: function (result1) {
            $("#taskgrid").empty().html(result1);
        }
    });
}

function clearMaintain() {
    if ($("#MFlag").val().toLowerCase() == "false") {
        $("#createSection").hide();
        $(".EditSection").hide();
        $(".DeleteSection").hide();
        $(".CopySection").hide();
        $(".LaborSection").hide();

        $(".AddSubSection").hide();
        $(".DeleteSubSection").hide();
        $(".CopySubSection").hide();
        $(".LaborSubSection").hide();
        
        $(".AddTask").hide();
        $(".EditTask").hide();
        $(".DeleteTask").hide();
        $(".CopyTask").hide();
        $(".LaborTask").hide();
    } else {
        $("#createSection").show();
    }
}

function prettyPrompt(options) {
    var o = $.extend({ message: "Hello!", title: "Please Note...", cancelText: "No", okayText: "Yes", onOkay: null, onCancel: null }, options);
    var btns = {};
    btns[o.okayText] = function () {
        $(this).dialog("close");
        if ($.isFunction(o.onOkay)) o.onOkay();
    };
    btns[o.cancelText] = function () {
        $(this).dialog("close");
        if ($.isFunction(o.onCancel)) o.onCancel();
    };
    $("<div title='" + o.title + "'>" + o.message + "</div>").dialog({
        modal: true,
        buttons: btns,
        width: 350,
        close: function () { $(this).dialog("destroy").remove(); }
    });
}

function showSingleTaskDetail(obj) {
    $("#partname").html($(obj).text());

    $("#taskdetail").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

    $.ajax({
        url: obj.href,
        cache: false,
        success: function (result) {
            $("#taskdetail").empty().html(result);
        }
    });

    var taskattr = $(obj).attr('taskattr');

    showImageDetail(taskattr);
}

function showTaskDetail(tid, obj) {
    $("#partname").html($(obj).attr("title"));
    var url = $(obj).attr("url");
    
    $("#taskdetail").empty().html("<img src='/Areas/PriceBook/images/ajax-loader.gif' />");

    $.ajax({
        url: url,
        cache: false,
        success: function (result) {
            $("#taskdetail").empty().html(result);
        }
    });

    showImageDetail(tid);
}

function showImageDetail(tid) {
    var uploadurl = $("#uploadimg").attr("url");
    var tmpurl = $("#uploadimg").attr("tmpurl");

    $("#uploadimg").empty().html('<div id="fileuploader"></div>');
    $("#fileuploader").fileUpload({
        'uploader': '/Areas/PriceBook/Scripts/uploader.swf',
        'cancelImg': '/Areas/PriceBook/images/cancel.png',
        'buttonText': 'Select Image',
        'script': uploadurl,
        'folder': '/uploads',
        'fileDesc': 'Image Files',
        'fileExt': '*.jpg;*.jpeg;*.gif;*.png',
        'multi': true,
        'auto': true,
        'scriptData': {
            'taskid': tid
        },
        onComplete: function (event, queueID, fileObj, response, data) {
            $('#viewimg').empty().html("<img src='/uploads/" + fileObj.name + "' width=250 />");

            tmp = fileObj.name;
        },
        onAllComplete: function () {
            $.ajax({
                url: tmpurl,
                data: { filename: tmp },
                success: function (result) {
                    if (result == "1") {
                    }
                }
            });
        }
    });

    var imgurl = $("#viewimg").attr("url");

    $('#viewimg').empty();
    $.ajax({
        url: imgurl,
        cache: false,
        data: { taskid: tid },
        success: function (result) {
            if (result != "") {
                $('#viewimg').empty().html("<img src='/uploads/" + result + "' width=250 />");
            }
        }
    });

}

function loadDialog(tag, event, target) {
    event.preventDefault();
    var $loading = $('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');
    var $url = $(tag).attr('url');
    var $title = $(tag).attr('title');
    var $dialog = $('<div></div>');
    $dialog.empty();
    $dialog
            .append($loading)
            .load($url)
		    .dialog({
		        autoOpen: false
			    , title: $title
			    , width: 500
                , modal: true
			    , minHeight: 200
                , show: 'fade'
                , hide: 'fade'
		    });

    $dialog.dialog("option", "buttons", {
        "Cancel": function () {
            $(this).dialog("close");
            $(this).empty();
        },
        "Submit": function () {
            var dlg = $(this);
            $.ajax({
                url: $url,
                type: 'POST',
                data: $("#target").serialize(),
                success: function (response) {
                    $(target).html(response);
                    dlg.dialog('close');
                    dlg.empty();
                    $("#ajaxResult").hide().html('Record saved').fadeIn(300, function () {
                        var e = this;
                        setTimeout(function () { $(e).fadeOut(400); }, 2500);
                    });
                },
                error: function (xhr) {
                    if (xhr.status == 400)
                        dlg.html(xhr.responseText, xhr.status);     /* display validation errors in edit dialog */
                    else
                        displayError(xhr.responseText, xhr.status); /* display other errors in separate dialog */

                }
            });
        }
    });

    $dialog.dialog('open');
};

function setSubSectionFuncs()
{
    $("a.subsectionlink").click(function () {
        $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

        var mflag = $("#MFlag").val();

        $.ajax({
            url: this.href,
            cache: false,
            data: { mflag: mflag },
            success: function (result1) {
                $("#taskgrid").empty().html(result1);
            }
        });

        $("#subtaskname").html($(this).text());

        return false;
    });

    $(".ActiveYNSubSection:input:not(:checked)").parent().parent().addClass("inactive");
    $(".ActiveYNSubSection").click(function () {
        $.ajax({
            url: $(this).attr("href"),
            data: "active=" + this.checked
        });
        if (this.checked) {
            $(this).parent().parent().removeClass("inactive");
        }
        else {
            $(this).parent().parent().addClass("inactive");
        }
    });

    $(".AddTask").click(function () {
        $("#hSubSectionID").val(this.id);
        $('#taskdlg').dialog('open');
    });

    $(".EditSection").click(function () {
        //$(".edittreenode").remove();
        $(".edittreenode.cancel").click();
        $(".EditSection").show();
        var val;
        if ($(this).parent().children("span").children("a").size() > 0) {
            val = $(this).parent().children("span").children("a").text();
            $(this).parent().children("span").children("a").text("");
        }
        else {
            val = $(this).parent().children("span").text();
            $(this).parent().children("span").text("");
        }

        $(this).parent().children("span").append("<input maxlength='50' class='edittreenode' type='text' origvalue='" + $.trim(val) + "' value='" + $.trim(val) + "' style='width:250px;left:45px;position:absolute;' /><img title='" + this.title + "' src='/Areas/PriceBook/images/updatetreenode.jpg' class='edittreenode update' style='left:305px;position:absolute;margin-top:5px;'/><img src='/Areas/PriceBook/images/cancel.jpg' class='edittreenode cancel' style='left:320px;position:absolute;margin-top:5px;'/>");

        $(this).hide();
        $(".edittreenode.update").click(function () {
            $.ajax({
                url: $(this).attr("title"),
                context: this,
                data: "name=" + encodeURIComponent($(".edittreenode").val()),
                success: function (resultupdate) {
                    if ($(this).parent().children("a").size() > 0) {
                        $(this).parent().children("a").text($(".edittreenode").val());
                    }
                    else {
                        $(this).parent().text($(".edittreenode").val());
                    }
                    $(".edittreenode").remove();
                    $(".EditSection").show();
                },
                error: function (resultupdate) {
                    alert("Error: Saving values, it could be that name is too long");
                    return false;
                }
            });

            return false;
        });

        $(".edittreenode.cancel").click(function () {
            if ($(this).parent().children("a").size() > 0) {
                $(this).parent().children("a").text($(".edittreenode").attr("origvalue"));
            }
            else {
                $(this).parent().text($(".edittreenode").attr("origvalue"));
            }
            $(".edittreenode").remove();
            $(".EditSection").show();
            return false;
        });

        return false;
    });

    $(".DeleteSubSection").click(function () {
        var ssid = this.id;
        var url = $(this).attr("url");

        prettyPrompt({
            title: "Delete SubSection",
            message: "Are you sure you want to delete this <" + ssid + "> ?",
            onOkay: function () {
                $.ajax({
                    url: url,
                    success: function (result) {
                        if (result == "success") {
                            $('#searchFrm').submit();
                        }
                    }
                });
            }
        });
    });

    $(".CopySubSection").click(function () {
        var ssid = this.id;
        var url = $(this).attr("url");

        prettyPrompt({
            title: "Copy SubSection",
            message: "Are you sure you want to copy this <" + ssid + "> ?",
            onOkay: function () {
                $.ajax({
                    url: url,
                    success: function (result) {
                        if (result == "success") {
                            $('#searchFrm').submit();
                        }
                    }
                });
            }
        });
    });

    $(".LaborSubSection").click(function () {
        var url = $(this).attr("url");

        $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');
        $.ajax({
            url: url,
            data: { tlevel: 2 },
            success: function (result1) {
                $("#taskgrid").empty().html(result1);
            }
        });

        return false;
    });
}

function setTaskFuncs() {
    $("a.tasklink").click(function () {
        $("#taskgrid").empty().html('<img src="/Areas/PriceBook/images/ajax-loader.gif" />');

        var url = $(this).attr("url");
        $.ajax({
            url: url,
            cache: false,
            success: function (result1) {
                $("#taskgrid").empty().html(result1);
            }
        });

        showSingleTaskDetail(this);

        return false;
    });

    $(".EditTask").click(function () {
        updatetask(this);
    });

    $(".DeleteTask").click(function () {
        deletetask(this);
    });

    $(".CopyTask").click(function () {
        copytask(this);
    });

    $(".LaborTask").click(function () {
        labortask(this);
    });

    $(".ActiveYNTask:input:not(:checked)").parent().parent().addClass("inactive");
    $(".ActiveYNTask").click(function () {
        $.ajax({
            url: $(this).attr("href"),
            data: "active=" + this.checked
        });
        if (this.checked) {
            $(this).parent().parent().removeClass("inactive");
        }
        else {
            $(this).parent().parent().addClass("inactive");
        }
    });
}
