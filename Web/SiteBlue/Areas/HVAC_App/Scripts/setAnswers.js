function Submit() {
    setAnswerSign(32, "Authorized");
    //SendDataToServer();
}

function setAnswerSign(id, text, agreed) {
    var signdata = $("#sign" + id).jSignature("getData");
    var answ = "Important";
    var agree = true;
    if (text != undefined)
        answ = text;
    if (agreed != undefined)
        agree = agreed;
    var emptysign;
    if (id>=26&&id<32) {
        emptysign = "data:image/png;base64,data:image/png;base64,VBORw0KGgoAAAANSUhEUgAAAUAAAABkCAYAAAD32uk+AAAC+ElEQVR4Ae3UgREAIAgDMXX/neGc48MGTbnemTmOAAECRYFXDC0zAQIEvoAB9AcECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEDCAfoAAgayAAcxWLzgBAgbQDxAgkBUwgNnqBSdAwAD6AQIEsgIGMFu94AQIGEA/QIBAVsAAZqsXnAABA+gHCBDIChjAbPWCEyBgAP0AAQJZAQOYrV5wAgQMoB8gQCArYACz1QtOgIAB9AMECGQFDGC2esEJEFgXgQPFi6H9qQAAAABJRU5ErkJggg==";
    }
    if (id == 32) {
        emptysign = "data:image/png;base64,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";
    }
    if (agree) {
        if (signdata == emptysign) {
            dhx.notice("Please, enter signature");
            return null;
        }
        setAnswer(answ, signdata);
    } else {
        $("#sign" + id).jSignature("clear");
        setAnswer(answ);
    }
    return true;
}

function setAnswer(ans, data) {
    answer = ans;
    var index = parseInt($$('list_questions').o[0]); //id of selected item;
    //var index = $$("multiview_3").index($$("multiview_3").getActive());
    listQuestions.dhtmlx().blockEvent();
    if (data) {
        listQuestions.dhtmlx().update(index.toString(), { id: index.toString(), question: $$('view' + index + '_question').data.label, answer: answer, data: data });
    }
    else {
        listQuestions.dhtmlx().update(index.toString(), { id: index.toString(), question: $$('view' + index + '_question').data.label, answer: answer });
    }

    listQuestions.dhtmlx().select(index.toString());
    listQuestions.dhtmlx().showItem(index.toString());
   
    $$("view" + index + "_answer").setValue(answer);

    listQuestions.dhtmlx().unblockEvent();
    showButtonNext();
}


function setAnswerText(idview) {
    setAnswer($$(idview + "_answer").getValue());
}

//function setAnswer12() {
//    var ratings = new Ratings();
//    ratings.electric = $$("view12_lbl_kwh").getValue();
//    ratings.gas = $$("view12_lbl_ccf").getValue();
//    var text1 = "Electric rate: " + ratings.electric + " $/KWH";
//    var text2 = "Gas rate: " + ratings.gas + " $/ccf";
//    setAnswer(text1 + "\n" + text2, ratings);
//}

//function setAnswerCalendar() {
//    var date = formatdDate($$("view18_answer").getValue());
//    var data = $$("view18_answer").getValue();
//    $$("view18_lblDate").setValue(date);
//    setAnswer(date, data);
//    $$("view18_answer").setValue(data);
//}

function setAnswer19() {
    var size = new SystemSize();
    size.loss = $$("view19_lbl_loss").getValue();
    size.gain = $$("view19_lbl_gain").getValue();
    var text1 = "Heating System Size: " + size.loss + " BTUH";
    var text2 = "Cooling System Size: " + size.gain + " Ton";
   
    setAnswer(text1 + "\n" + text2, size);
}

function setAnswer25() {
    var sys = new System();
    switch ($$("view20_segm").getValue()) {
        case 'view20_1':
            {
                sys.seer_rating = 16;
                sys.id = 1;
                sys.name = "SE";
                sys.system = $$("view20_1").data.system;
                break;
            }
        case 'view20_2':
            {
                sys.seer_rating = 15;
                sys.id = 2;
                sys.name = "ME"; 
                sys.system = $$("view20_2").data.system;
                break;
            }
        case 'view20_3':
            {
                sys.seer_rating = 13;
                sys.id = 3;
                sys.name = "LE";
                sys.system = $$("view20_3").data.system;
                break;
            }
        case 'view20_4':
            {
                sys.seer_rating = 16;
                sys.id = 4;
                sys.name = "UE"; 
                sys.system = $$("view20_4").data.system;
                break;
            }
        case 'view20_5':
            {
                sys.seer_rating = 16;
                sys.id = 5;
                sys.name = "HE";
                sys.system = $$("view20_5").data.system;
                //sys.total_amount = $$("lbl_view20_5_3").getValue();
                //view35.loadList($$("view20_5").data.system);
                break;
            }
        default:
    }
    if (listQuestions.dhtmlx().item(35) != undefined) {
        listQuestions.dhtmlx().item(35).data = undefined;
        listQuestions.dhtmlx().item(35).answer = "";
    }
    setAnswer(sys.name, sys);
}

var cs = new CoolingSavings();
var ghs = new GasHeatSavings();
var hp = new HeatPumpHeatingSavings();

function setAnswer21(save) {
    if (save) {
        var savings = new Savings(cs, ghs, hp);
        if ($$("view21_lbl_cs").getValue() == '') {
            savings.cs.calculated_value = 0;
        }
        if ($$("view21_lbl_ghs").getValue() == '') {
            savings.ghs.calculated_value = 0;
        }
        if ($$("view21_lbl_hp").getValue() == '') {
            savings.hp.calculated_value = 0;
        }
        setAnswer("Calculated", savings);
    }
    else {
        var f6;
        var f7;
        var f8;
        var f9;
        var f10;
        var f11;
        switch ($$("view21_segm").getValue()) {
        case 'view21_1':
            {
                if ($$("view21_1_form").validate() == false) {
                    dhx.alert("Some of fields are empty!");
                    break;
                }
                f6 = Globalize.parseFloat($$("view21_cs_input1").getValue().toString());
                cs.seer_rating_exist = f6;
                f7 = Globalize.parseFloat($$("view21_cs_input2").getValue().toString());
                cs.btu_capacity_exist = f7;
                f8 = Globalize.parseFloat($$("view21_cs_input3").data.label.toString());
                f9 = Globalize.parseFloat($$("view21_cs_input4").data.label.toString());
                f10 = Globalize.parseFloat($$("view21_cs_input5").data.label.toString());
                f11 = Globalize.parseFloat($$("view21_cs_input6").getValue().toString());
                cs.average_cooling_hours = f11;
                var valueCs = Globalize.format(abs((((((f7 / f6) / 1000) * f10) * f11)) - ((((f9 / f8) / 1000) * f10) * f11)), "n2");
                cs.calculated_value = valueCs;
                $$("view21_lbl_cs").setValue('Cooling Saving: $' + cs.calculated_value);
                break;
            }
        case 'view21_2':
            {
                if ($$("view21_2_form").validate() == false) {
                    dhx.alert("Some of fields are empty!");
                    break;
                }
                f6 = Globalize.parseFloat($$("view21_ghs_input1").data.label.toString());
                f7 = Globalize.parseFloat($$("view21_ghs_input2").getValue().toString());
                ghs.afue_exist = f7;
                f8 = Globalize.parseFloat($$("view21_ghs_input3").data.label.toString());
                ghs.afue_selected = f8;
                f9 = Globalize.parseFloat($$("view21_ghs_input4").data.label.toString());
                f10 = Globalize.parseFloat($$("view21_ghs_input5").getValue().toString());
                ghs.average_heating_hours = f10;
                var valueGhs = Globalize.format(abs(((((f6 / f7) - (f6 / f8)) / (1000)) * f9) * f10), "n2");
                ghs.calculated_value = valueGhs;
                $$("view21_lbl_ghs").setValue('Gas Heat Savings: $' + ghs.calculated_value);
                break;
            }
        case 'view21_3':
            {
                if ($$("view21_3_form").validate() == false) {
                    dhx.alert("Some of fields are empty!");
                    break;
                }
                f6 = Globalize.parseFloat($$("view21_hp_input1").getValue());
                hp.hspf_exist = f6;
                f7 = Globalize.parseFloat($$("view21_hp_input2").getValue());
                hp.btu_capacity_exist = f7;
                f8 = Globalize.parseFloat($$("view21_hp_input3").getValue());
                hp.hspf_selected = f8;
                f9 = Globalize.parseFloat($$("view21_hp_input4").data.label.toString());
                f10 = Globalize.parseFloat($$("view21_hp_input5").data.label.toString());
                f11 = Globalize.parseFloat($$("view21_hp_input6").getValue());
                hp.average_cooling_hours = f11;
                var valueHp = Globalize.format(abs((((((f7 / f6) / 1000) * f10) * f11)) - ((((f9 / f8) / 1000) * f10) * f11)), "n2");
                hp.calculated_value = valueHp;
                $$("view21_lbl_hp").setValue("Heat Pump Heating Savings: $" + hp.calculated_value);
                break;
            }
        default:
        }
    }
    return null;
}

function abs(a) { return a >= 0 ? a : -a; }

function SetAnswer22(save) {
    if ($$("view22_form").validate()==false) {
        dhx.alert("Some of fields are empty!");
        return null;
    }
    var cost = new CostSystem();
    cost.saving = Globalize.parseFloat($$("view22_input2").data.label);
    cost.remaing = Globalize.parseFloat($$("view22_input1").getValue());
    cost.repears = Globalize.parseFloat($$("view22_input3").getValue());
    $$("view22_input3").setValue(GlobalizeDoubleToString(cost.repears, "c"));
    var inflation = Globalize.parseFloat($$("view22_input5").getValue().toString());

    if (inflation < 1) {
        cost.inflation = inflation;
    }
    else {
        cost.inflation = inflation / 100;
    }
    //var subtotal = saving * remaing + repears;
    $$("view22_input4").data.label = Globalize.format(cost.subtotal(), "c");
    $$("view22_input4").refresh();
    cost.CalculateTotalValue();

    $$("view22_answer").setValue(Globalize.format(cost.total_value, "c"));
    if (save) {
        setAnswer(Globalize.format(cost.total_value, "c"), cost);
    }
    return null;
}

function CalcSubtotal() {
    if ($$("view22_input1").getValue() == "" || $$("view22_input2").data.label == "" || $$("view22_input3").getValue()=="") {
        dhx.alert("Some of fields are empty!");
    }
    else {
        var cost = new CostSystem();
        cost.saving = Globalize.parseFloat($$("view22_input2").data.label);
        cost.remaing = Globalize.parseFloat($$("view22_input1").getValue());
        cost.repears = Globalize.parseFloat($$("view22_input3").getValue());
        $$("view22_input3").setValue(GlobalizeDoubleToString(cost.repears, "c"));
        $$("view22_input4").data.label = Globalize.format(cost.subtotal(), "c");
        $$("view22_input4").refresh();
    }
}