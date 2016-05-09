
var source_list_jobs;
var dp;
var dp2;
var arraylist = new Array();
var answer;

function clearSign(id) {
    $("#sign" + id).jSignature("clear");
}

function SendDataToServer() {
    dp.send();
}
 
function showButtonNext() {
    $$("btnNext").show();
    clearInterval(interval);
    timer = 0;
    interval = setInterval(function () {
        if (timer < popup_settings.timer_total)
            timer = timer + 100;
        else {
            clearInterval(interval);
            click_btn_next();
        }
    }, 100);
}

function formatdDate(date) {
    var currdate = date.getDate();
    var currmonth = date.getMonth();
    currmonth++;
    var curryear = date.getFullYear();
    return (currmonth + "/" + currdate + "/" + curryear);
}

function increase(id, step, n, min, max) {
    var currentMax = Globalize.parseFloat(max);
    var currentvalue = Globalize.parseFloat($$(id).getValue().toString());
    var currentstep = Globalize.parseFloat(step);
    if ((currentvalue + currentstep)<=currentMax)
        $$(id).setValue(Globalize.format(currentvalue + currentstep, n));
}

function decrease(id, step, n, min) {
    var currentmin = Globalize.parseFloat(min);
    var currentvalue = Globalize.parseFloat($$(id).getValue().toString());
    var currentstep = Globalize.parseFloat(step);
    if ((currentvalue - currentstep) >= currentmin)
        $$(id).setValue(Globalize.format(currentvalue - currentstep,n));
}

function increaseGain(id, step, n, min, max) {
    var currentMax = Globalize.parseFloat(max);
    var currentvalue = Globalize.parseFloat($$(id).getValue().toString());
    if (currentvalue == 4)
        currentvalue = 4.5;
    var currentstep = Globalize.parseFloat(step);
    if ((currentvalue + currentstep) <= currentMax)
        $$(id).setValue(Globalize.format(currentvalue + currentstep, n));
}

function decreaseGain(id, step, n, min) {
    var currentmin = Globalize.parseFloat(min);
    var currentvalue = Globalize.parseFloat($$(id).getValue().toString());
    if (currentvalue == 5)
        currentvalue = 4.5;
    var currentstep = Globalize.parseFloat(step);
    if ((currentvalue - currentstep) >= currentmin)
        $$(id).setValue(Globalize.format(currentvalue - currentstep, n));
}

function mainShow(id, index) {
    $$("btnBack").hide();
    $$("btnWait").hide();
    $$("view" + id).show();
    $$("multiview_3").i[getIndexbyIdMW(id)].show();
    $$("btnNext").show();
    $$("btnPrev").show();

    listQuestions.dhtmlx().blockEvent();
    listQuestions.dhtmlx().select(id.toString());
    listQuestions.dhtmlx().unblockEvent();

    var answer = listQuestions.dhtmlx().item(id).answer;
    $$("view" + id + "_answer").setValue(answer);

    $$("slider").setValue(Math.round(index * 100 / $$("multiview_3").i.length));
    $$("slider").setSliderTitle(Math.round(index * 100 / $$("multiview_3").i.length) + "%");
    
    if (index == 0) {
        $$("btnPrev").hide();
    }
    if (index == $$("multiview_3").i.length || answer == '') {
        $$("btnNext").hide();
    }
};

function showView(index, scrollTo) {
    var id = listQuestions.dhtmlx().idByIndex(index);
    if (scrollTo) listQuestions.dhtmlx().showItem(id.toString());
    clearInterval(interval);

    mainShow(id, index);
    
    if (id == 1
        || id == 2) {
        var ind = listQuestions.dhtmlx().indexById(id);
        arraylist[ind].showView();
    }

    if (id == 12) {
        view12.showView();
    }

    if ((id >= 26 && id <= 30) || id == 36 || id == 37 ) {
        $("#sign" + id).jSignature({ color: "#00f", lineWidth: 5, width: 320, height: 100, cssclass: 'box' });
        if (listQuestions.dhtmlx().item(id).data != undefined) {
            $("#sign" + id).jSignature("importData", listQuestions.dhtmlx().item(id).data);
        } else {
            clearSign(id);
        }
    }

    if (id == 32) {
        $$("slider").setValue(97);
        $$("slider").setSliderTitle("99%");
        $("#sign" + id).jSignature({ color: "#00f", lineWidth: 5, width: 370, height: 175, cssclass: 'box' });
        if (listQuestions.dhtmlx().item(id).data != undefined) {
            $("#sign" + id).jSignature("importData", listQuestions.dhtmlx().item(id).data);
        }
        else {
            clearSign(32);
        }

        var data35 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(35).data);                
        $$("view" + id + "_lbl3").setValue(GlobalizeDoubleToString(data35.GrandTotal,"c"));
        $$("view" + id + "_lbl4").setValue((Globalize.format(Globalize.parseFloat(data35.GrandTotal.toString()) / 200, "c")));
    }

    if (id == 33) {
        $$("slider").setValue(98);
        $$("slider").setSliderTitle("100%");

        $$("btnBack").hide();
        $$("btnWait").hide();
        $$("view33_richselect").setValue('2');
        dhx.ajax().post("../../hvac_app/wizard/GetPayments",
        null,
        {
            error: function () {
                popupntfc.HidePopup();
                alert("Error in get payments");
            },
            success: view33.loadData
        });
    }

    if (id == 18)
        view18.showView();
    
    if (id == 19) {
        var data19 = (new SystemSize()).toObject(listQuestions.dhtmlx().item(19).data);
        view19.lbl_loss().setValue(Globalize.format(data19.loss, "n0"));
        view19.lbl_gain().setValue(Globalize.format(data19.gain, "n1"));
    }
    if (id == 20) {
        GetSystemInforms(ShowView20);
    }
    
    if (id == 21) {
        ShowView21();
    }
    
    if (id==22) {
        ShowView22();
    }

    if (id == 23) {
        view23.showView();
    }

    if (id == 31) {
        view31.showView();
    }

    if (id == 35) {
        view35.showView();
    }
}

function GetSystemInforms(funct) {
    var data19 = (new SystemSize()).toObject(listQuestions.dhtmlx().item(19).data);
    var data17 = listQuestions.dhtmlx().item(17).data;
    popupntfc.ChangeText("Loading Packages info...");
    var ac = listQuestions.dhtmlx().item(7) == undefined ? "N/A" : listQuestions.dhtmlx().item(7).data;
    var uv = listQuestions.dhtmlx().item(8) == undefined ? "N/A" : listQuestions.dhtmlx().item(8).data;
    var hm = listQuestions.dhtmlx().item(9) == undefined ? "N/A" : listQuestions.dhtmlx().item(9).data;
    var dw = listQuestions.dhtmlx().item(34) == undefined ? "N/A" : listQuestions.dhtmlx().item(34).data;
    popupntfc.ShowPopup();
    dhx.ajax().post("../../hvac_app/hvacconfig/GetSystemInforms",
        { code: data17, tons: data19.gain, ac: ac, uv: uv, hm: hm, dw: dw },
        {
            error: function () {
                popupntfc.HidePopup();
                alert("Error in get systems");
            },
            success: funct
        });
}

function ShowView20(data) {
    var obj = dhx.DataDriver.json.toObject(data);
    view20.HideAllPackages();
    for (i in obj.system_ids) {
        switch (obj.system_ids[i]) {
            case "SE":
                {
                    $$("lbl_view20_1_2").setValue(obj.systems[i].TotalDescription);
                    $$("lbl_view20_1_2").refresh();
                    $$("lbl_view20_1_3").setValue(obj.systems[i].TotalAmount);
                    $$("view20_1").data.system = obj.systems[i];
                    view20.ShowPackage(0);
                    break;
                }
        case "ME":
            {
                $$("lbl_view20_2_2").setValue(obj.systems[i].TotalDescription);
                $$("lbl_view20_2_3").setValue(obj.systems[i].TotalAmount);
                $$("view20_2").data.system = obj.systems[i];
                view20.ShowPackage(1);
                break;
            }
        case "LE":
            {
                $$("lbl_view20_3_2").setValue(obj.systems[i].TotalDescription);
                $$("lbl_view20_3_3").setValue(obj.systems[i].TotalAmount);
                $$("view20_3").data.system = obj.systems[i];
                view20.ShowPackage(2);
                break;
            }
        case "UE":
            {
                $$("lbl_view20_4_2").setValue(obj.systems[i].TotalDescription);
                $$("lbl_view20_4_3").setValue(obj.systems[i].TotalAmount);
                $$("view20_4").data.system = obj.systems[i];
                view20.ShowPackage(3);
                break;
            }
        case "HE":
            {
                $$("lbl_view20_5_2").setValue(obj.systems[i].TotalDescription);
                $$("lbl_view20_5_3").setValue(obj.systems[i].TotalAmount);
                $$("view20_5").data.system = obj.systems[i];
                view20.ShowPackage(4);
                break;
            }
        default:
        }
    }
    var data2031 = (new System()).toObject(listQuestions.dhtmlx().item(20).data);
    if (data2031.length != undefined || data2031.length == 0) {
        view20.showView(1);
    } else {
        view20.showView(data2031.id);
    }
    popupntfc.HidePopup();
}

function ShowView21() {
    //se,me,le - 20
    var d1 = (new System()).toObject(listQuestions.dhtmlx().item(20).data);
    $$('view21_cs_input3').data.label = d1.system.MainSystem.SEER;
    $$('view21_cs_input3').refresh();
    $$('view21_ghs_input3').data.label = d1.system.MainSystem.AFUE;
    $$('view21_ghs_input3').refresh();
    //BTU - 19      
    var d2 = (new SystemSize()).toObject(listQuestions.dhtmlx().item(19).data);
    $$('view21_cs_input4').data.label = Globalize.format(d2.gain * 12000, "n0");
    $$('view21_cs_input4').refresh();
    $$("view21_ghs_input1").data.label = Globalize.format(d2.loss, "n0");
    $$("view21_ghs_input1").refresh();
    $$("view21_hp_input4").data.label = Globalize.format(d2.gain * 12000, "n0");
    $$("view21_hp_input4").refresh();
    //rate electic - 12
    var d3 = (new Ratings()).toObject(listQuestions.dhtmlx().item(12).data);
    $$('view21_cs_input5').data.label = d3.electric;
    $$('view21_cs_input5').refresh();
    $$("view21_ghs_input4").data.label = d3.gas;
    $$("view21_ghs_input4").refresh();
    $$("view21_hp_input5").data.label = d3.electric;
    $$("view21_hp_input5").refresh();
    //----
    
    if (listQuestions.dhtmlx().item(21).data) {
        var savings = (new Savings()).toObject($$("list_questions").item(21).data);
        if (Globalize.parseFloat(savings.cs.calculated_value.toString()) != 0) {
            $$('view21_cs_input1').setValue(Globalize.format(Globalize.parseFloat(savings.cs.seer_rating_exist.toString()), "n0"));
            $$('view21_cs_input2').setValue(Globalize.format(Globalize.parseFloat(savings.cs.btu_capacity_exist.toString()), "n0"));
            $$('view21_cs_input6').setValue(Globalize.format(Globalize.parseFloat(savings.cs.average_cooling_hours.toString()), "n0"));
            $$("view21_lbl_cs").setValue('Cooling Saving: ' + Globalize.format(Globalize.parseFloat(savings.cs.calculated_value.toString()), "c"));
        }
        else {
            $$("view21_lbl_cs").setValue('');
        }
        if (Globalize.parseFloat(savings.ghs.calculated_value.toString()) != 0) {       
            $$("view21_ghs_input2").setValue(savings.ghs.afue_exist);
            $$("view21_ghs_input3").setValue(savings.ghs.afue_selected);
            $$("view21_ghs_input5").setValue(Globalize.format(Globalize.parseFloat(savings.ghs.average_heating_hours.toString()), "n0"));
            $$("view21_lbl_ghs").setValue('Gas Heat Savings: ' + Globalize.format(Globalize.parseFloat(savings.ghs.calculated_value.toString()), "c"));
        }
        else {
            $$("view21_lbl_ghs").setValue('');
        }
        if (Globalize.parseFloat(savings.hp.calculated_value.toString()) != 0) {        
            $$("view21_hp_input1").setValue(Globalize.format(Globalize.parseFloat(savings.hp.hspf_exist.toString()), "n0"));
            $$("view21_hp_input2").setValue(Globalize.format(Globalize.parseFloat(savings.hp.btu_capacity_exist.toString()), "n0"));
            $$("view21_hp_input3").setValue(Globalize.format(Globalize.parseFloat(savings.hp.hspf_selected.toString()), "n0"));
            $$("view21_hp_input6").setValue(Globalize.format(Globalize.parseFloat(savings.hp.average_cooling_hours.toString()), "n0"));
            $$("view21_lbl_hp").setValue("Heat Pump Heating Savings: " + Globalize.format(Globalize.parseFloat(savings.hp.calculated_value.toString()), "c"));
        }
        else {
            $$("view21_lbl_hp").setValue('');
        }
    }
}

function ShowView22() {
    var savings = (new Savings()).toObject(listQuestions.dhtmlx().item(21).data);
    cs = savings.cs;
    ghs = savings.ghs;
    hp = savings.hp;
    var savingReaming = Globalize.parseFloat(savings.cs.calculated_value.toString()) + Globalize.parseFloat(savings.ghs.calculated_value.toString()) + Globalize.parseFloat(savings.hp.calculated_value.toString());
    $$("view22_input2").data.label = Globalize.format(savingReaming, "c");
    $$("view22_input2").refresh();
    if ($$("list_questions").item(22).data) {
        var cost = (new CostSystem()).toObject($$("list_questions").item(22).data);
        $$("view22_input1").setValue(Globalize.format(cost.remaing));
        $$("view22_input3").setValue(Globalize.format(cost.repears, "c"));
        var subtotal = cost.saving * cost.remaing + cost.repears;
        $$("view22_input4").data.label = Globalize.format(subtotal,"c");
        $$("view22_input4").refresh();
        $$("view22_input5").setValue(Globalize.format((cost.inflation*100)));
    }
}


function click_btn_prev() {
    var id = listQuestions.dhtmlx().getSelected();
    //var indexcurrent = listQuestions.dhtmlx().indexById(id);
    //showView(indexcurrent - 1, true);
    var idprev = getIdPreviousView(id);
    var indexcurrent = listQuestions.dhtmlx().indexById(idprev);
    if (idprev == 6 && $$("view5_answer").getValue() == "") {
        indexcurrent = listQuestions.dhtmlx().indexById(getIdPreviousView(idprev));
    }
    showView(indexcurrent, true);
}

function click_btn_next() {
    var id = listQuestions.dhtmlx().getSelected();
    var indexcurrent = listQuestions.dhtmlx().indexById(id);
    var idnext = getIdNextView(id);
    
    if (id == 5 && $$("view5_answer").getValue() == "") {
        $$('list_questions').remove("6");
        idnext = getIdNextView(idnext);
    }

    $$('list_questions').blockEvent();
    $$('list_questions').add({ id: idnext, question: $$('view' + idnext + '_question').data.label, answer: '' }, (indexcurrent+1).toString());
    $$('list_questions').unblockEvent();
   
    answer = null;
    indexcurrent = listQuestions.dhtmlx().indexById(idnext);
    showView(indexcurrent, true);
}

function click_btn_back() {
    dhx.confirm({
        title: "Complete",
        message: "Do you really want to complete job?",
        callback: GoToJobList
    });
}

function click_btn_wait() {
    dhx.confirm({
        title: "Information",
        message: "Completing this as an esimate will remove this job from your device. Do you want to continue?",
        callback: AfterConfirmEstimate
    });
}

function GoToJobList(ok) {
    if (ok) {
        SendDataToServer();
        //TODO send data to server
        dhx.ajax().sync().post("../../hvac_app/wizard/CompleteJob", null, {
            error: function () {
                alert("Error in complete job");
            },
            success: afterCompleteJob
        });
    }
}

function AfterConfirmEstimate(ok) {
    if (ok) {
        SendDataToServer();
        dhx.ajax().sync().post("../../hvac_app/wizard/EstimateJob", null, {
            error: function () {
                alert("Error in estimate job");
            },
            success: afterEstimateJob
        });
    }
}

function afterCompleteJob(ok) {
    var text = "";
    if (ok) {
        text = "Job completed successfully.";
        winCompleteJob.showbackToJobList();
        winCompleteJob.ChangeText(text);
        winCompleteJob.dhtmlx().show();
        //$$("preparation_app").show();
        //job_list.updateList();
        //backTojobList();
    } else {
        text = "Some thing wrong!";
        winCompleteJob.ChangeText(text);
        winCompleteJob.dhtmlx().show();
    }
}

function afterEstimateJob(ok) {
    var text = "";
    if (ok) {
        text = "Waiting Estimate";
        winCompleteJob.showbackToJobList();
        winCompleteJob.ChangeText(text);
        winCompleteJob.dhtmlx().show();
        //$$("preparation_app").show();
        //job_list.updateList();
        //backTojobList();
    } else {
        text = "Some thing wrong!";
        winCompleteJob.ChangeText(text);
        winCompleteJob.dhtmlx().show();
    }
}

function exitClick() {
    dhx.confirm({
        title: "Close",
        message: "Do you really want to exit?",
        callback: ConfirmClick
    });
}

function ConfirmClick(ok) {
    if (ok)
        $$("preparation_app").show();
}

function listselect(ids) {
    var indexcurrent = listQuestions.dhtmlx().indexById(ids);
    showView(indexcurrent, false);
}


function changeListItem() {
    $$('list_questions').select(0);
    $$('view18_answer').setValue(new Date());
}

function addPayment() {
    var email = $$("view33_input_email").getValue();
    if (!validateEmail(email)) {
        dhx.notice("Email is not valid!");
        return null;
    }
    if ($$("view33_form_7").validate() == false) {
        dhx.alert("Amount value is empty!");
        return null;
    }
    var typeId = $$("view33_segmented_payment").getValue();
    var type =view33.getTypeNamebyId(typeId);
    var amount = Globalize.parseFloat($$("view33_input_amount").getValue().toString());
    var code = $$("view33_input_number").getValue();
    var count = $$("view33_payments").dataCount();
    var totalRemaing = Globalize.parseFloat($$("view33_total_amount").getValue().toString());
    var totalAmount = totalRemaing - amount;
    $$("view33_total_amount").setValue(Globalize.format(totalAmount, "c"));
    $$("view33_input_amount").setValue(Globalize.format(totalAmount, "c"));
    //$$("view33_input_amount").data.placeholder = "Balance Amount: " + $$("view33_total_amount").getValue();
   // $$("view33_input_amount").refresh();
    $$("view33_input_number").setValue('');
    $$("view33_payments").add({ type: type, typeId: typeId, payment: Globalize.format(amount, "c"), code: code });
    var datajs = $$("view33_payments").serialize();
    listQuestions.dhtmlx().update("33", { id: "33", question: "Payment", answer: "", data: JSON.stringify({ email: email, payments: datajs, total_amount: totalAmount})});
    return true;
}

function removePayment(id) {
    var email = $$("view33_input_email").getValue();
    if (!validateEmail(email)) {
        dhx.notice("Email is not valid!");
        return null;
    }
    var data = $$("view33_payments").item(id);
    var totalRemaing = Globalize.parseFloat($$("view33_total_amount").getValue().toString());
    var addpayment = Globalize.parseFloat(data.payment.toString());
    var totalAmount = totalRemaing + addpayment;
    $$("view33_total_amount").setValue(Globalize.format(totalAmount, "c"));
    $$("view33_input_amount").setValue(Globalize.format(totalAmount, "c"));
    //$$("view33_input_amount").data.placeholder = "Balance Amount: " + $$("view33_total_amount").getValue();
    //$$("view33_input_amount").refresh();
    $$("view33_payments").remove(id);
    var datajs = $$("view33_payments").serialize();
    listQuestions.dhtmlx().update("33", { id: "33", question: "Payment", answer: "", data: JSON.stringify({ email: email, payments: datajs, total_amount: totalAmount}) });
    return null;
}

function validateEmail(email) {
    if (email != "") {
        var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/ ;
        return re.test(email);
    }
    return true;
}

function openFinanceWindow(num) {
    var data = finance_options_controller.data;
    var systype = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(20).data);
    for (i in data) {
        if (data[i].sys == systype.name) {
            if (num == 1) 
                $$("iframeFinOpt").load(data[i].FinanceOption1);
            if (num == 2)
                $$("iframeFinOpt").load(data[i].FinanceOption2);
            if (num == 3)
                $$("iframeFinOpt").load(data[i].FinanceOption3);
            winFinOpt.dhtmlx().show();
        }
    }
}


var interval;
var timer = 0;

function getIdNextView(idview) {
    for (var i in arraylist) {
        if (arraylist[i].id == "view" + idview) {
            return arraylist[++i].id.replace("view", "");
        }
    }
    return -1;
}

function getIdPreviousView(idview) {
    for (var i in arraylist) {
        if (arraylist[i].id == "view" + idview) {
            return arraylist[--i].id.replace("view", "");
        }
    }
    return -1;
}

function getIndexbyIdMW(idview) {
    for (var i in $$("multiview_3").i) {
        if ($$("multiview_3").i[i].config.id == "view" + idview) {
            return i;
        }
    }
    return -1;
}

function GlobalizeDoubleToString(doublenum, format)
{
    return Globalize.format(Globalize.parseFloat(doublenum.toString()), format);
}

function GlobalizeToDouble(doublenum) {
    return Globalize.parseFloat(doublenum.toString());
}
