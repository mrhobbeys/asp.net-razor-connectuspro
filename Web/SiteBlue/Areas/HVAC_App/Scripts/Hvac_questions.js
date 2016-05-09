function PopupSettings() {
    this.id = "Menu1";
    this.view = {
        view: "popup",
        id: this.id,
        width: 400,
        heigth: 200,
        css: 'writeBackground',
        body: {
            view: 'form',
            scroll: true,
            elements: [
                {
                    view: 'toggle',
                    options: [
                        { value: '0', label: 'Yes', id: 'popup_lblYes' },
                        { value: '1', label: 'No', id: 'popup_lblNo' }
                    ],
                    label: 'Auto next',
                    labelPosition: 'right',
                    id: 'timer_onoff',
                    labelWidth: '120',
                    inputWidth: '135',
                    inputPostion: 'center'
                },
                { view: 'counter', label: 'Time in milliseconds', 
                    value: 1500, step: 100, 
                    labelPosition: 'right', 
                    labelAlign: 'left',
                    labelWidth: '200',
                    id: 'timer_counter',
                    min: '0', max: '30000',
                    inputWidth: '45', 
                    inputPosition: 'center',
                    css: 'timer_counter'
                },
                { view: 'view', id: 'temp_designer_view_31', heigth: 40 },
                { view: 'button', label: 'Clear answers', id: 'btn_clearAnswers', popup: '', inputWidth: '140', align: 'center', click: 'popup_settings.clearAnswers' }
            ],
            id: 'setting_form'
        }
    };
    this.GetToggle = function() {
        return $$("timer_onoff");
    };

    this.clearAnswers = function() {
        dhx.confirm({
            title: "Clear Answers",
            message: "Do you really want to remove answers?",
            callback: popup_settings.afterClickClearAnswer
        });
    };

    this.afterClickClearAnswer = function (ok) {
        if (ok) {
            popupntfc.ChangeText("Delete answers...");
            popupntfc.ShowPopup();
            dhx.ajax().post("../../hvac_app/wizard/clearAnswers", null, {
                error: function () {
                    popupntfc.HidePopup();
                    alert("Error in clear answers.");
                },
                success: popup_settings.afterClearAnswers
            });
        }
    };

    this.afterClearAnswers = function () {
        popupntfc.HidePopup();
        listQuestions.updateList();
    };
}

function PopupController() {
    this.attachEvents = function() {
        $$("timer_counter").attachEvent("onchange", this.changeTimer);
        $$("timer_onoff").attachEvent("onchange", this.changeToggleOff);
        $$("timer_onoff").setValue(1);
        popup_settings.timer_total = 4000000;
    };

    this.changeTimer = function (val) {
        $$("timer_onoff").setValue(0);
        popup_settings.timer_total = val;
    };

    this.changeToggleOff = function(val) {
        if (val == 0) {
            popup_settings.timer_total = $$("timer_counter").getValue();
        }
        else {
            popup_settings.timer_total = 4000000;
        }
    };
}

function WindowAccessories() {
    this.id = "winAccessories";
    this.view = {
        view: 'window',
        head: {
            view: 'toolbar',
            type: 'MainBar',
            elements: [
                { view: 'label', label: 'Addons', id: 'winAS_lbl_header' },
                { view: 'button', label: 'Close', id: 'winAS_btn_close', click: 'winAS.dhtmlx().hide()' }
            ],
            id: 'winAS_toolbar'
        },
        body: {
            view: 'layout',
            type: 'wide',
            rows: [
                {
                    view: 'radio',
                    height: 'auto',
                    width: 550,
                    labelPosition: 'right',
                    labelAlign: 'left',
                    labelWidth: 520,
                    value: '0',
                    options: [
                        { label: 'Loading...', value: '0', id: 'winAS_item_1' },
                        { label: 'Loading...', value: '1', id: 'winAS_item_2' },
                        { label: 'Loading...', value: '2', id: 'winAS_item_3' },
                        { label: 'Loading...', value: '3', id: 'winAS_item_4' },
                        { label: 'Loading...', value: '4', id: 'winAS_item_5' },
                        { label: 'Loading...', value: '5', id: 'winAS_item_6' }
                    ],
                    id: 'winAS_radio_buttons',
                    options_number: 6
                },
                {
                    view: 'layout',
                    type: 'clean',
                    id: 'layout_4',
                    cols: [
                        { view: 'counter', label: 'Count',
                            value: 1, step: 1,
                            labelPosition: 'right',
                            labelAlign: 'left',
                            id: 'winAS_count',
                            min: '1', max: '30000',
                            inputWidth: '45',
                            inputPosition: 'center',
                            css: 'inp_counter'
                        },
                        { view: 'button', label: 'Add', id: 'winAS_btn_add', type: 'round', click: "winAS.btn_add_click" }
                    ]
                }
            ],
            id: 'layout_3'
        },
        position: 'center',
        width: 600,
        height: 'auto',
        move: true,
        modal: true,
        id: this.id
    };

    this.btn_add_click = function () {
        view35.AddAS($$("winAS_radio_buttons").data.options[$$("winAS_radio_buttons").getValue()], $$("winAS_count").getValue());
        winAS.dhtmlx().hide();
    };

    this.loadAccessories = function (tons) {
        if (tons != undefined) {
            dhx.ajax().post("../../hvac_app/hvacconfig/GetAllAccessoriesForPriceList?tons=" + tons, null, {
                error: function () {
                    alert("Error in get list of accessories");
                },
                success: winAS.afterPost
            });
        }
        else {
            dhx.ajax().post("../../hvac_app/hvacconfig/GetAllAccessoriesForPriceList", null, {
                error: function () {
                    alert("Error in get list of accessories");
                },
                success: winAS.afterPost
            });
        }
    };

    this.afterPost = function (posteddata) {
        var len = $$("winAS_radio_buttons").data.options.length;
        for (i = 1; i <= len; i++) {
            $$("winAS_radio_buttons").data.options = $$("winAS_radio_buttons").data.options.slice(1);
        }
        posteddata = dhx.DataDriver.json.toObject(posteddata);
        $$("winAS_radio_buttons").data.options[0] = {};
        $$("winAS_radio_buttons").data.options[0].code = "Package";
        $$("winAS_radio_buttons").data.options[0].label = "Selected Package";
        $$("winAS_radio_buttons").data.options[0].value = "0";
        for (var i in posteddata) {
            var d = GlobalizeToDouble(i) + 1;
//            Code: a.code,
//                    JobCode: a.JobCode,
//                    JobCodeId: a.JobCodeId,
//                    Description: a.Description,
//                    ResAccountCode: a.ResAccountCode,
            $$("winAS_radio_buttons").data.options[d] = {};
            $$("winAS_radio_buttons").data.options[d].value = d;
            $$("winAS_radio_buttons").data.options[d].Price = posteddata[i].Price;
            $$("winAS_radio_buttons").data.options[d].Description = posteddata[i].Description;
            $$("winAS_radio_buttons").data.options[d].JobCode = posteddata[i].JobCode;
            $$("winAS_radio_buttons").data.options[d].ResAccountCode = posteddata[i].ResAccountCode;
            $$("winAS_radio_buttons").data.options[d].JobCodeId = posteddata[i].JobCodeId;
            $$("winAS_radio_buttons").data.options[d].label = posteddata[i].Description;  // + " (" + GlobalizeDoubleToString(posteddata[i].Price, "n2") + ")";
            $$("winAS_radio_buttons").data.options[d].code = posteddata[i].Code;
        }
        $$("winAS_radio_buttons").refresh();
    };
}

WindowAccessories.prototype = proto_dhtmlx;

function WindowTaxRate() {
    this.id = "winTax";
    this.view = {
        view: 'window',
        head: {
            view: 'toolbar',
            type: 'MainBar',
            elements: [
                { view: 'label', label: 'Tax Rates', id: 'winTax_lbl_header' },
                { view: 'button', label: 'Close', id: 'winTax_btn_close', click: 'winTax.dhtmlx().hide()' }
            ],
            id: 'toolbar_2'
        },
        body: {
            view: 'layout',
            type: 'wide',
            rows: [
                {
                    view: 'form',
                    scroll: true,
                    height: 300,
                    elements: [
                        {
                            view: 'radio',
                            height: 'auto',
                            name: '',
                            labelPosition: 'right',
                            labelAlign: 'left',
                            labelWidth: 300,
                            value: '0',
                            options: [
                                { label: 'option 1', value: '1', id: 'control_radio_item_14' }
                            ],
                            id: 'winTax_listoftaxes'
                        }],
                    id: 'winTax_from'
                },
                { view: 'button', label: 'Change tax', popup: '', click: 'winTax.changeTax', css: '', id: 'winTax_btn_add' }
            ],
            id: 'layout_3'
        },
        width: 400,
        height: 'auto',
        position: 'center',
        move: true,
        modal: true,
        id: this.id
    };

    this.changeTax = function() {
        view35.changeTax($$("winTax_listoftaxes").data.options[$$("winTax_listoftaxes").getValue()].taxrate);
    };

    this.loadTaxRates = function () {
        dhx.ajax().post("../../hvac_app/wizard/GetCompanyTaxes", null, {
            error: function () {
                alert("Error in get company taxes");
            },
            success: winTax.afterPost
        });
    };

    this.afterPost = function (posteddata) {
        posteddata = dhx.DataDriver.json.toObject(posteddata);
        for (var i in posteddata) {
            $$("winTax_listoftaxes").data.options[i] = {
                label: posteddata[i].Tax,
                taxrate: posteddata[i].Value,
                value: i
            };
        }
        $$("winTax_listoftaxes").refresh();
        
    };
}

WindowTaxRate.prototype = proto_dhtmlx;

function CompleteWindow() {
    this.id = "winCompleteJob";
    this.view = {
        view: 'window',
        head: {
            view: 'toolbar',
            type: 'MainBar',
            elements: [
                { view: 'label', label: 'Information', popup: '', click: '', id: 'cw_header' },
                { view: 'button', label: 'Back to Job List', id: 'winCJ_BtJL', click: 'winCompleteJob.BackToJobList()' },
                { view: 'button', label: 'Back', id: 'winCJ_Back', click: 'winCompleteJob.dhtmlx().hide()' }
            ],
            id: 'cw_tb1'
        },
        body: {
            view: 'layout',
            type: 'wide',
            rows: [
                { view: 'label', label: 'Label', popup: '', click: '', id: 'lblCW_text' },
                {
                    view: 'toolbar',
                    type: 'MainBar',
                    elements: [
                        { view: 'button', label: 'Print Invoice', id: 'btnCW_dwnld', click: 'winCompleteJob.DownloadInvoice()' },
                        { view: 'button', label: 'Send email', id: 'btnCW_snd', click: 'winCompleteJob.SendInvoice()' }
                    ],
                    id: 'cw_tb2'
                }
            ],
            id: 'layout_3'
        },
        position: 'center',
        width: 400,
        height: 'auto',
        move: true,
        modal: true,
        id: this.id,
        hidden: 1
    };
    this.showBack = function () {
        $$("winCJ_Back").show();
        $$("winCJ_BtJL").hide();
    };

    this.showbackToJobList = function() {
        $$("winCJ_Back").hide();
        $$("winCJ_BtJL").show();
    };

    this.BackToJobList = function() { 
        this.dhtmlx().hide();
        $$("preparation_app").show();
        job_list.updateList();
        backTojobList();
    };

    this.ChangeText = function (text) {
        $$("lblCW_text").setValue(text);
    };

    this.DownloadInvoice = function() {
        dhx.confirm({
            title: "Complete",
            message: "Do you want to print the invoice?",
            callback: winCompleteJob.GetLink
        });
    };

    this.GetLink = function () {
        location.href = "../../hvac_app/wizard/PrintInvoice/" + datastory.get("id_job");
    };

    this.SendInvoice = function () {
        var email = $$("view33_input_email").getValue();
        if (email == "") {
            dhx.notice({ delay: 10000, message: "Email is empty" });
            return null;
        }
        if (!validateEmail(email)) {
            dhx.notice("Email is not valid!");
            return null;
        }
        popupntfc.ChangeText("Sending email...");
        popupntfc.ShowPopup();
        dhx.ajax().post("../../hvac_app/wizard/SendInvoiceByEmail", null, {
            error: function () {
                popupntfc.HidePopup();
                alert("Error in send invoice.");
            },
            success: winCompleteJob.afterSendInvoiceByEmail
        });
    };

    this.afterSendInvoiceByEmail = function (ok) {
        popupntfc.HidePopup();
        var data = dhx.DataDriver.json.toObject(ok);
        if (data.result) {
            dhx.notice({ delay: 10000, message: "Email sent." });
        }
        else {
            dhx.notice({ delay: 10000, message: "Email not sent." });
        }
    };
}

CompleteWindow.prototype = proto_dhtmlx;

function FinanceOptionWindow() {
    this.id = "winFinOpt";
    this.view = {
        view: 'window',
        head: {
            view: 'toolbar',
            type: 'MainBar',
            elements: [
                { view: 'label', label: 'Fifance Option', id: 'winFO_lbl_header' },
                { view: 'button', label: 'Close', id: 'winFO_btn_close', click: 'winFinOpt.dhtmlx().hide()' }
            ],
            id: 'winFO_toolbar'
        },
        body: {
            view: "iframe",
            id: "iframeFinOpt",
            src: ""
        },
        position: 'center',
        width: 900,
        height: 600,
        move: true,
        modal: true,
        id: this.id
    };
}

FinanceOptionWindow.prototype = proto_dhtmlx;

function View0() {
    this.superclass = proto_dhtmlx;
    this.id = "view0";
    this.setAnswer = function(answer, data) {
        setAnswer(answer, data);
    };
    
    this.view = { view: 'layout', type: 'clean', css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', label: 'Approximately how old is your home?', id: 'view0_question', align: 'center', height: 50, css: 'quest_label' },
            { view: 'label', id: 'view0_answer', label: '', align: 'middle', height: 230, css: "fontSize32" },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            { view: 'layout', type: 'clean', id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115, css: 'leftEmptyCol' },
                    { view: 'button', label: '0 - 5\nYears', id: 'view0btn0', css: 'btn2line', type: 'round', click: "view0.setAnswer('0-5 Years','0')" },
                    { view: 'button', label: '6 - 10\nYears', id: 'view0btn1', css: 'btn2line', type: 'round', click: "view0.setAnswer('6-10 Years','1')" },
                    { view: 'button', label: '11 - 15\nYears', id: 'view0btn2', css: 'btn2line', type: 'round', click: "view0.setAnswer('11-15 Years', '2')" },
                    { view: 'button', label: '16 - 20\nYears', id: 'view0btn3', css: 'btn2line', type: 'round', click: "view0.setAnswer('16-20 Years', '3')" },
                    { view: 'button', label: 'Over 20\nYears', id: 'view0btn4', css: 'btn2line', type: 'round', click: "view0.setAnswer('Over 20 Years', '4')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115, css: 'leftEmptyCol' }
                ]
            }
        ], id: this.id
    };
}
View0.prototype = proto_dhtmlx;

function View1() {
    this.id = "view1";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', label: 'How long have you lived in your home?', id: 'view1_question', align: 'center', height: 50, css: 'quest_label' },
            { view: 'label', id: 'view1_answer', label: '', align: 'middle', height: 230, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'view1_layout',
                cols: [
                    { view: 'view', id: 'view1_left', width: 115 },
                    { view: 'button', label: '0 - 5\nYears', id: 'view1btn0', css: 'btn2line', type: 'round', click: "setAnswer('0-5 Years')" },
                    { view: 'button', label: '6 - 10\nYears', id: 'view1btn1', css: 'btn2line', type: 'round', click: "setAnswer('6-10 Years')" },
                    { view: 'button', label: '11 - 15\nYears', id: 'view1btn2', css: 'btn2line', type: 'round', click: "setAnswer('11-15 Years')" },
                    { view: 'button', label: '16 - 20\nYears', id: 'view1btn3', css: 'btn2line', type: 'round', click: "setAnswer('16-20 Years')" },
                    { view: 'button', label: 'Over 20\nYears', id: 'view1btn4', css: 'btn2line', type: 'round', click: "setAnswer('Over 20 Years')" },
                    { view: 'view', id: 'view1_right', width: 115 }
                ]
            }
        ], id: this.id
        };

    this.showView = function() {
        $$("view1_right").hide();
        for (var ir = 0; ir < 5; ++ir) {
            $$('view1btn' + ir).show();
        }
        $$("view1_right").show();
        var num = listQuestions.dhtmlx().item(0).data;
        num = ++num;
        for (var ik = num; ik < 5; ++ik) {
            $$('view1btn' + ik).hide();
        }
    };
}
View1.prototype = proto_dhtmlx;

function View2() {
    this.id = "view2";
    this.view = { view: 'layout', type: 'clean', css: "maninback", align: 'center',
        rows: [
            { view: 'view', id: 'temp_designer_view_244', height: 130 },
            { view: 'label', label: 'How long do you plan on staying in the home?', id: 'view2_question', align: 'center', height: 60, css: 'quest_label' },
            { view: 'label', id: 'view2_answer', label: '', align: 'middle', height: 220, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_243', height: 50 },
            { view: 'layout', type: 'clean', id: 'layout_85',
                cols: [
                    { view: 'view', id: 'temp_designer_view_363', width: 115 },
                    { view: 'button', label: '0 - 5\nYears', id: 'view2btn1', css: 'btn2line', type: 'round', click: "setAnswer('0-5 Years')" },
                    { view: 'button', label: '6 - 10\nYears', id: 'view2btn2', css: 'btn2line', type: 'round', click: "setAnswer('6-10 Years')" },
                    { view: 'button', label: 'Over 10\nYears', id: 'view2btn3', css: 'btn2line', type: 'round', click: "setAnswer('Over 10 Years')" },
                    { view: 'view', id: 'temp_designer_view_312', width: 115 }
                ]
            }
        ], id: this.id
        };

    this.showView = function() {
        $$("multiview_3").i[2].show();
    };
}
View2.prototype = proto_dhtmlx;

function View3() {
    this.id = "view3";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', label: 'Where are the hot and cold spots in your home?', id: 'view3_question', align: 'center', height: 75, css: 'quest_label' },
            { view: 'textarea', height: 195, inputWidth: 420, align: 'center', label: 'Use keyboard to enter text', popup: '', click: '', id: 'view3_answer', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', label: 'Save', popup: '', click: 'setAnswerText("view3")', css: '', id: 'view3_btn' }
                        ],
                        id: 'view3_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ], id: this.id
    };
}
View3.prototype = proto_dhtmlx;

function View5() {
    this.superclass = view5;
    this.id = "view5";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view5_question', label: 'Who in the home suffers from allergies or asthma?', align: 'center', height: 75, css: 'quest_label' },
            { view: 'textarea', id: 'view5_answer', height: 195, inputWidth: 420, align: 'center', label: 'Use keyboard to enter text', popup: '', click: '', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', id: 'view5_btn', label: 'Save', popup: '', click: 'view5.setAnswer()', css: '' }
                        ],
                        id: 'view5_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.setAnswer = function() {
        var ans = $$('view5_answer').getValue();
        if (ans.trim() != 0) {
            $$("view6_question").data.label = "What are you doing to control the symptoms of " + ans + "?";
            $$("view6_question").refresh();
        } else {
            $$('list_questions').remove("6");
        }
        setAnswer(ans);
    };
}
View5.prototype = proto_dhtmlx;

function View6() {
    this.superclass = proto_dhtmlx;
    this.id = "view6";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view6_question', label: 'What are you doing to control the symptoms?', align: 'center', height: 75, css: 'quest_label' },
            { view: 'textarea', id: 'view6_answer', height: 195, inputWidth: 420, align: 'center', label: 'Use keyboard to enter text', popup: '', click: '', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', id: 'view6_btn', label: 'Save', popup: '', click: 'setAnswerText("view6")', css: '' }
                        ],
                        id: 'view6_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View6.prototype = proto_dhtmlx;

function View7() {
    this.id = "view7";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view7_question', label: 'Do you currently have an Air Cleaner or are you interested in having cleaner air in your home?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view7_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', label: 'Have', id: 'view7btn1', css: 'btn2line', type: 'round', click: "setAnswer('We would like to keep our existing Air Cleaner when replacing the&nbsp;system.', '1')" },
                    { view: 'button', label: 'Interested', id: 'view7btn2', css: 'btn2line', type: 'round', click: "setAnswer('We would be interested in having pure air when replacing the&nbsp;system.', '2')" },
                    { view: 'button', label: 'Not Interested', id: 'view7btn3', css: 'btn2line', type: 'round', click: "setAnswer('We are not interested in pure air at this&nbsp;time.', 'N/A')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View7.prototype = proto_dhtmlx;

function View8() {
    this.id = "view8";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view8_question', label: 'Do you have an air purifier or are you interested in having pure air in your home?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view8_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', label: 'Have', id: 'view8btn1', css: 'btn2line', type: 'round', click: "setAnswer('We would like to keep our existing Air Purifier when replacing the&nbsp;system.','Have')" },
                    { view: 'button', label: 'Interested', id: 'view8btn2', css: 'btn2line', type: 'round', click: "setAnswer('We would be interested in including UV lights when replacing the&nbsp;system.','Interested')" },
                    { view: 'button', label: 'Not Interested', id: 'view8btn3', css: 'btn2line', type: 'round', click: "setAnswer('We are not interested in air purification at this&nbsp;time','N/A')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View8.prototype = proto_dhtmlx;

function View9() {
    this.id = "view9";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 110 },
            { view: 'label', id: 'view9_question', label: 'Do you currently have a whole house humidifier or are you interested in having whole house humidification in your home?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view9_answer', label: '', align: 'top', height: 170, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', label: 'Have', id: 'view9btn1', css: 'btn2line', type: 'round', click: "setAnswer('We would like to keep our existing Humidifier when replacing the&nbsp;system.','Have')" },
                    { view: 'button', label: 'Interested', id: 'view9btn2', css: 'btn2line', type: 'round', click: "setAnswer('We would be interested in including a Whole House Humidifier when replacing the&nbsp;system.','Interested')" },
                    { view: 'button', label: 'Not Interested', id: 'view9btn3', css: 'btn2line', type: 'round', click: "setAnswer('We are not interested in Humidification at&nbsp;this&nbsp;time.','N/A')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View9.prototype = proto_dhtmlx;

function View10() {
    this.id = "view10";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view10_question', label: 'What is it that you like most about your current system?', align: 'center', height: 75, css: 'quest_label' },
            { view: 'textarea', id: 'view10_answer', height: 195, inputWidth: 420, align: 'center', label: 'Use keyboard to enter text', popup: '', click: '', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', id: 'view10_btn', label: 'Save', popup: '', click: 'setAnswerText("view10")', css: '' }
                        ],
                        id: 'view10_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View10.prototype = proto_dhtmlx;

function View11() {
    this.id = "view11";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view11_question', label: 'What do you like least about your system?', align: 'center', height: 75, css: 'quest_label' },
            { view: 'textarea', id: 'view11_answer', height: 195, inputWidth: 420, align: 'center', label: 'Use keyboard to enter text', popup: '', click: '', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', id: 'view11_btn', label: 'Save', popup: '', click: 'setAnswerText("view11")', css: '' }
                        ],
                        id: 'view11_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View11.prototype = proto_dhtmlx;

function View12() {
    this.id = "view12";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view12_question', label: 'What are your utility rates?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view12_answer', height: 150, inputWidth: 420, align: 'top', label: '', popup: '', css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'layout',
                        type: 'clean',
                        rows: [
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view12_layout1',
                                cols: [
                                    { view: 'label', label: 'Electric rate per KWH?', id: 'view12_lbl0', align: "left", width: 270 },
                                    { view: 'label', label: '$', id: 'view12_lbl2', align: 'right', width: 20 },
                                    { view: 'label', label: '0.121', id: 'view12_lbl_kwh', align: 'left' }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view12_layout2',
                                cols: [
                                    { view: 'button', label: '—', id: 'view12_btn1', click: 'decrease("view12_lbl_kwh","0.001", "n3", "0", "1")', type: 'round', width: 85, inputWidth: 70, align: "left" },
                                    { view: "slider", id: "view12_slider_kwh", label: "", labelWidth: 10, value: 86, name: "s1", max: 1000, min: 0, step: 1 },
                                    { view: 'button', label: '+', id: 'view12_btn2', click: 'increase("view12_lbl_kwh", "0.001", "n3", "0", "1")', type: 'round', width: 70, inputWidth: 70 }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view12_layout3',
                                cols: [
                                    { view: 'label', label: 'Gas rate per Therm?', popup: '', align: 'left', id: 'view12_lbl3', width: 270 },
                                    { view: 'label', label: '$', id: 'view12_lbl5', align: 'right', width: 20  },
                                    { view: 'label', label: '0.64', id: 'view12_lbl_ccf', align: 'left'}
                                    
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view12_layout4',
                                cols: [
                                    { view: 'button', label: '—', id: 'view12_btn3', click: 'decrease("view12_lbl_ccf", "0.01", "n2", "0", "3")', type: 'round', width: 85, inputWidth: 70, align: "left" },
                                    { view: "slider", id: "view12_slider_ccf", label: "", labelWidth: 10, value: 64, name: "s2", max: 300, min: 0, step: 1 },
                                    { view: 'button', label: '+', id: 'view12_btn4', click: 'increase("view12_lbl_ccf", "0.01", "n2", "0", "3")', type: 'round', width: 70, inputWidth: 70 }
                                ]
                            },
                            { view: 'button', label: 'Save', id: 'view12_btn5', click: 'view12.setAnswer()', align: 'center' }
                        ],
                        id: 'layout_5'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.showView = function() {
        var ratings = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(12).data);
        if (ratings.length != 0) {
            $$("view12_lbl_kwh").setValue(ratings.electric.toString());
            $$("view12_lbl_ccf").setValue(ratings.gas.toString());
        }
    };

    this.setAnswer = function() {
        var ratings = new Ratings();
        ratings.electric = $$("view12_lbl_kwh").getValue();
        ratings.gas = $$("view12_lbl_ccf").getValue();
        var text1 = "Electric rate: " + ratings.electric + " $/KWH";
        var text2 = "Gas rate: " + ratings.gas + " $/ccf";
        setAnswer(text1 + "\n" + text2, ratings);
    };
}

View12.prototype = proto_dhtmlx;

function View13() {
    this.id = "view13";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view13_question', label: 'On a scale of 1 to 5, how important is energy efficiency to you?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view13_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                height: 42,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'label', label: 'Very\nUnimportant', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Unimportant', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Neutral', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Important', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Very\nImportant', align: 'center', css: 'btn2line' },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', id: 'view13btn1', label: '1', css: 'btn2line', type: 'round', click: "setAnswer('Energy efficiency is very unimportant to&nbsp;me.')" },
                    { view: 'button', id: 'view13btn2', label: '2', css: 'btn2line', type: 'round', click: "setAnswer('Energy efficiency is unimportant to&nbsp;me.')" },
                    { view: 'button', id: 'view13btn3', label: '3', css: 'btn2line', type: 'round', click: "setAnswer('I do not feel strongly either&nbsp;way.')" },
                    { view: 'button', id: 'view13btn4', label: '4', css: 'btn2line', type: 'round', click: "setAnswer('Energy efficiency is important to&nbsp;me.')" },
                    { view: 'button', id: 'view13btn5', label: '5', css: 'btn2line', type: 'round', click: "setAnswer('Energy efficiency is very important to&nbsp;me.')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View13.prototype = proto_dhtmlx;

function View14() {
    this.id = "view14";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view14_question', label: 'On a scale of 1 to 5, how important is using environmentally friendly technology to you?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view14_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                height: 42,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'label', label: 'Very\nUnimportant', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Unimportant', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Neutral', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Important', align: 'center', css: 'btn2line' },
                    { view: 'label', label: 'Very\nImportant', align: 'center', css: 'btn2line' },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', id: 'view14btn1', label: '1', css: 'btn2line', type: 'round', click: "setAnswer('Using environmentally equipment is very unimportant to&nbsp;me. ')" },
                    { view: 'button', id: 'view14btn2', label: '2', css: 'btn2line', type: 'round', click: "setAnswer('Using environmentally equipment is unimportant to&nbsp;me.')" },
                    { view: 'button', id: 'view14btn3', label: '3', css: 'btn2line', type: 'round', click: "setAnswer('I am neutral on the use of environmentally friendly&nbsp;technology.')" },
                    { view: 'button', id: 'view14btn4', label: '4', css: 'btn2line', type: 'round', click: "setAnswer('Using environmentally equipment is important to&nbsp;me.')" },
                    { view: 'button', id: 'view14btn5', label: '5', css: 'btn2line', type: 'round', click: "setAnswer('Using environmentally equipment is very important to&nbsp;me.')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View14.prototype = proto_dhtmlx;

function View15() {
    this.id = "view15";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view15_question', label: 'How much have you budgeted for a new comfort system?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view15_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', id: 'view15btn1', label: '$ 4,000', css: 'btn2line', type: 'round', click: "setAnswer('$ 4,000')" },
                    { view: 'button', id: 'view15btn2', label: '$ 6,000', css: 'btn2line', type: 'round', click: "setAnswer('$ 6,000')" },
                    { view: 'button', id: 'view15btn3', label: '$ 8,000', css: 'btn2line', type: 'round', click: "setAnswer('$ 8,000')" },
                    { view: 'button', id: 'view15btn4', label: '$ 12,000', css: 'btn2line', type: 'round', click: "setAnswer('$ 12,000')" },
                    { view: 'button', id: 'view15btn5', label: '$ 20,000', css: 'btn2line', type: 'round', click: "setAnswer('$ 20,000')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View15.prototype = proto_dhtmlx;

function View16() {
    this.id = "view16";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view16_question', label: 'On a weekly basis how much could you afford?', align: 'center', height: 120, css: 'quest_label' },
            { view: 'label', id: 'view16_answer', label: '', align: 'top', height: 160, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', id: 'view16btn1', label: '$ 30', css: 'btn2line', type: 'round', click: "setAnswer('$ 30')" },
                    { view: 'button', id: 'view16btn2', label: '$ 35', css: 'btn2line', type: 'round', click: "setAnswer('$ 35')" },
                    { view: 'button', id: 'view16btn3', label: '$ 45', css: 'btn2line', type: 'round', click: "setAnswer('$ 45')" },
                    { view: 'button', id: 'view16btn4', label: '$ 65', css: 'btn2line', type: 'round', click: "setAnswer('$ 65')" },
                    { view: 'button', id: 'view16btn5', label: '$ 85', css: 'btn2line', type: 'round', click: "setAnswer('$ 85')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View16.prototype = proto_dhtmlx;

function View17() {
    this.id = "view17";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view17_question', label: 'What type of system were you interested in to cool or heat your home?', align: 'center', height: 80, css: 'quest_label' },
            { view: 'label', id: 'view17_answer', label: '', align: 'top', height: 200, css: 'fontSize32' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'layout',
                        type: 'clean',
                        rows: [
                            {
                                view: 'segmented',
                                options: [
                                    { label: 'Air Conditioner', key: '1', value: 'view_ac', id: 'view17_tab_1' },
                                    { label: 'Heat Pump', key: '2', value: 'view_hp', id: 'view17_tab_2' },
                                    { label: 'Package Unit', key: '3', value: 'view_pu', id: 'view17_tab_3' }
                                ],
                                id: 'view17_segm',
                                align: 'center',
                                multiview: 'true'
                            },
                            {
                                view: 'multiview',
                                type: 'clen',
                                cells: [
                                    {
                                        view: 'layout',
                                        type: 'clean',
                                        id: 'view_ac',
                                        rows: [
                                            { view: 'label', id: 'with1', label: 'with', align: 'center' },
                                            {
                                                view: 'layout',
                                                type: 'clean',
                                                id: 'view_ac2',
                                                cols: [
                                                    { view: 'button', id: 'view17btn1', label: 'Gas Furnace', css: 'btn2line', type: 'round', click: "setAnswer('Air Conditioner with Gas Furnace', 'ACGFV')" },
                                                    { view: 'button', id: 'view17btn2', label: 'Air Handler\nElectric Heat', css: 'btn2line', type: 'round', click: "setAnswer('Air Conditioner Air Handler/Electric Heat', 'ACAH1')" }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        view: 'layout',
                                        type: 'clean',
                                        id: 'view_hp',
                                        rows: [
                                            { view: 'label', id: 'with2', label: 'with', align: 'center' },
                                            {
                                                view: 'layout',
                                                type: 'clean',
                                                id: 'view_hp2',
                                                cols: [
                                                    { view: 'button', id: 'view17btn3', label: 'Gas Furnace', css: 'btn2line', type: 'round', click: "setAnswer('Heat Pump with Gas Furnace', 'HPGFV')" },
                                                    { view: 'button', id: 'view17btn4', label: 'Air Handler\nElectric Heat', css: 'btn2line', type: 'round', click: "setAnswer('Heat Pump with Air Handler/Electric Heat', 'HPAH1')" }
                                                ]
                                            }
                                        ]
                                    },
                                    {
                                        view: 'layout',
                                        type: 'clean',
                                        id: 'view_pu',
                                        rows: [
                                            { view: 'label', id: 'with3', label: 'with', align: 'center' },
                                            {
                                                view: 'layout',
                                                type: 'clean',
                                                id: 'view_pu2',
                                                cols: [
                                                    { view: 'button', id: 'view17btn5', label: 'Gas Heat', css: 'btn2line', type: 'round', click: "setAnswer('Package Unit with Gas Heat', 'PKGFV')" },
                                                    { view: 'button', id: 'view17btn6', label: 'Heat Pump\nElectric Heat', css: 'btn2line', type: 'round', click: "setAnswer('Package Unit with Heat Pump/Electric Heat', 'PKHE1')" },
                                                    { view: 'button', id: 'view17btn7', label: 'Electric\nHeat', css: 'btn2line', type: 'round', click: "setAnswer('Package Unit with Electric/Heat', 'PKAC1')" }
                                                ]
                                            }
                                        ]
                                    }
                                ],
                                id: 'view17_m'
                            }
                        ],
                        id: 'view17_l'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View17.prototype = proto_dhtmlx;

function View18() {
    this.id = "view18";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 74 },
            { view: 'label', id: 'view18_question', label: 'Requested Installation Date', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                align: 'bottom',
                height: 286,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    { view: 'calendar', id: 'view18_answer', date: new Date(), align: 'bottom', width: 435, height: 286, startOnMonday: 1, navigation: 1, skipEmptyWeeks: 1, calendarDateFormat: '%Y-%m-%d', calendarMonthHeader: '%F %Y', calendarDayHeader: '%d', calendarWeek: '%W', cellAutoHeight: true, weekHeader: 1, weekNumber: 0 },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 190 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view18_lblDate', label: '', align: 'center', height: 50, css: 'largetext' },
                            { view: 'button', id: 'view18_btn', label: 'Save', popup: '', click: 'view18.setAnswer()', css: '' }
                        ],
                        id: 'view18_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.showView = function () {
        if (listQuestions.dhtmlx().item(18).answer != '') {
            var rawdata = listQuestions.dhtmlx().item(18).data;
            if (rawdata != undefined) {
                var date = new Date();
                date.setTime(Date.parse(rawdata));
                $$("view18_answer").setValue(date);
                $$("view18_lblDate").setValue(formatdDate(date));
            }
        }
        else {
            $$("view18_answer").setValue(new Date());
            $$("view18_lblDate").setValue(formatdDate(new Date()));
        }
    };

    this.setAnswer = function() {
        var date = formatdDate($$("view18_answer").getValue());
        var data = $$("view18_answer").getValue();
        $$("view18_lblDate").setValue(date);
        setAnswer(date, data);
        $$("view18_answer").setValue(data);
    };
}

View18.prototype = proto_dhtmlx;

function View19() {
    this.id = "view19";
    this.lbl_loss = function () { return $$("view19_lbl_loss"); };
    this.slider_loss = function() { return $$("view19_slider_loss"); };
    this.lbl_gain = function () { return $$("view19_lbl_gain"); };
    this.slider_gain = function() { return $$("view19_slider_gain"); };
    
    this.view = { view: 'layout', type: 'clean', css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 160 },
            { view: 'label', id: 'view19_question', label: 'House Requirements', align: 'center', height: 75, css: 'quest_label' },
            { view: 'label', id: 'view19_answer', height: 175, inputWidth: 420, align: 'center', label: '', popup: '', css: 'quest_label' },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            { view: 'layout', type: 'clean', id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'layout', type: 'clean',
                        rows: [
                            { view: 'layout', type: 'clean', id: 'view19_layout1',
                                cols: [
                                    { view: 'label', label: 'Heating System Size', id: 'view19_lbl0', width: 250 },
                                    { view: 'label', label: '40,000', id: 'view19_lbl_loss', align: 'right', width: 90 },
                                    { view: 'label', label: 'BTU', id: 'view19_lbl2', css: 'smalltext', align: 'left'}
                                ]
                            },
                            { view: 'layout', type: 'clean', id: 'view19_layout2',
                                cols: [                                    
                                    { view: 'button', label: '—', id: 'view19_btn1', click: 'decrease("view19_lbl_loss", "5000", "n0", "40000", "120000")', type: 'round', width: 85, inputWidth: 70, align: "left" },
                                    { view: "slider", id: "view19_slider_loss", label: "", labelWidth: 10, value: 40000, name: "s1", max: 120000, min: 40000, step: 5000},
                                    { view: 'button', label: '+', id: 'view19_btn2', click: 'increase("view19_lbl_loss", "5000", "n0", "40000", "120000")', width: 70, inputWidth: 70, type: 'round' }
                                ]
                            },
                            { view: 'layout', type: 'clean', id: 'view19_layout3',
                                cols: [
                                    { view: 'label', label: 'Cooling System Size', popup: '', click: '', id: 'view19_lbl3', width: 250 },
                                    { view: 'label', label: '1.5', id: 'view19_lbl_gain', align: 'right', width: 45 },
                                    { view: 'label', label: 'Ton', id: 'view19_lbl5', css: 'smalltext', align: 'left'}
                                ]
                            },
                            { view: 'layout', type: 'clean', id: 'view19_layout4',
                                cols: [
                                    { view: 'button', label: '—', id: 'view19_btn3', click: 'decreaseGain("view19_lbl_gain", "0.5", "n1", "1.5", "5")', type: 'round', width: 85, inputWidth: 70, align: "left" },
                                    {view: 'slider', label: '', value: 15, min: 15, max: 50, _options_number: 7, labelWidth: 10,
                                        options: [
						                    { value: 15, label: '', id: 'view19_slider_option_2' },
						                    { value: 20, label: '', id: 'view19_slider_option_3' },
						                    { value: 25, label: '', id: 'view19_slider_option_4' },
						                    { value: 30, label: '', id: 'view19_slider_option_5' },
						                    { value: 35, label: '', id: 'view19_slider_option_6' },
						                    { value: 40, label: '', id: 'view19_slider_option_7' },
						                    { value: 50, label: '', id: 'view19_slider_option_8' }
					                    ], id: 'view19_slider_gain'
                                    },
                                    { view: 'button', label: '+', id: 'view19_btn4', click: 'increaseGain("view19_lbl_gain", "0.5", "n1", "1.5", "5")', type: 'round', width: 70, inputWidth: 70 }
                                 ]
                            },
                            { view: 'button', label: 'Save', id: 'view19_btn5', click: 'setAnswer19()', align: 'center' }
                        ], id: 'layout_5'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ], id: 'view19'
    };
}

function View20() {
    this.id = "view20";
    this.view = { view: 'layout', type: 'clean', css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_25', height: 60 },
            { view: 'label', id: 'view20_question', label: 'Choose System', align: 'center', height: 30, css: 'quest_label' },
            { view: 'layout', type: 'clean', id: 'layout_8', height: 310,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    { view: 'multiview', type: 'wide',
                        cells: [
                            { view: 'form', scroll: true, height: 310,
                                elements: [
                                    { view: 'label', label: 'SE', id: 'lbl_view20_1_1', align: 'center' },
                                    { view: 'label', label: 'Loading...', id: 'lbl_view20_1_2', align: 'center', height: '-1' },
                                    { view: 'label', label: '', id: 'lbl_view20_1_3', align: 'left', hidden:1 }
                                ], id: 'view20_1'
                            },
                            { view: 'form', scroll: true, height: 310,
                                elements: [
                                    { view: 'label', label: 'ME', id: 'lbl_view20_2_1', align: 'center' },
                                    { view: 'label', label: 'Loading...', id: 'lbl_view20_2_2', align: 'center', height: '-1' },
                                    { view: 'label', label: '', id: 'lbl_view20_2_3', align: 'left', hidden:1 }
                                ], id: 'view20_2'
                            },
                            { view: 'form', scroll: true, height: 310,
                                elements: [
                                    { view: 'label', label: 'LE', id: 'lbl_view20_3_1', align: 'center' },
                                    { view: 'label', label: 'Loading...', id: 'lbl_view20_3_2', align: 'center', height: '-1' },
                                    { view: 'label', label: '', id: 'lbl_view20_3_3', align: 'left', hidden:1 }
                                ], id: 'view20_3'
                            },
                            { view: 'form', scroll: true, height: 310,
                                elements: [
                                    { view: 'label', label: 'UE', id: 'lbl_view20_4_1', align: 'center' },
                                    { view: 'label', label: 'Loading...', id: 'lbl_view20_4_2', align: 'center', height: '-1' },
                                    { view: 'label', label: '', id: 'lbl_view20_4_3', align: 'left', hidden: 1 }
                                ], id: 'view20_4'
                            },
                            { view: 'form', scroll: true, height: 310,
                                elements: [
                                    { view: 'label', label: 'HE', id: 'lbl_view20_5_1', align: 'center' },
                                    { view: 'label', label: 'Loading...', id: 'lbl_view20_5_2', align: 'center', height: '-1' },
                                    { view: 'label', label: '', id: 'lbl_view20_5_3', align: 'left', hidden: 1 }
                                ], id: 'view20_5'
                            }
                        ], id: 'view20_multiview1'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 180 },
            { view: 'layout', type: 'clean', id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'form', scroll: false,
                        elements: [
                            { view: 'label', id: 'view20_answer', label: '', align: 'center', css: 'largetext' },
                            { view: 'segmented', css: 'view_lbl',
                                options: [
                                    { label: 'SE', key: 'view20_1', id: 'tab_1', value: 'view20_1', height: 64 },
                                    { label: 'ME', key: 'view20_2', id: 'tab_2', value: 'view20_2', height: 64 },
                                    { label: 'LE', key: 'view20_3', id: 'tab_3', value: 'view20_3', height: 64 },
                                    { label: 'UE', key: 'view20_4', id: 'tab_4', value: 'view20_4', height: 64 },
                                    { label: 'HE', key: 'view20_5', id: 'tab_5', value: 'view20_5', height: 64 }
                                ], id: 'view20_segm', multiview: 1, align: 'center'
                            },
                            { view: 'button', id: 'view20_btn', label: 'Save', popup: '', click: 'setAnswer25()', css: '' }
                        ], id: 'view20_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ], id: this.id
    };

    this.showView = function (showid) {
        $$("view20_" + showid).show();
        $$("view20_segm").setValue("view20_" + showid);
    };

    this.options = [
        { label: 'SE', key: 'view20_1', id: 'tab_1', value: 'view20_1', height: 64},
        { label: 'ME', key: 'view20_2', id: 'tab_2', value: 'view20_2', height: 64},
        { label: 'LE', key: 'view20_3', id: 'tab_3', value: 'view20_3', height: 64},
        { label: 'UE', key: 'view20_4', id: 'tab_4', value: 'view20_4', height: 64},
        { label: 'HE', key: 'view20_5', id: 'tab_5', value: 'view20_5', height: 64}
    ];

        this.ShowPackage = function (id) {
            $$("view20_segm").data.options.push(this.options[id]);
            $$("view20_segm").refresh();
        };

        this.HideAllPackages = function () {
            var len = $$("view20_segm").data.options.length;
            for (var i = 1; i <= len; i++) {
                $$("view20_segm").data.options = $$("view20_segm").data.options.slice(1);
            }
        };
}

function View21() {
    this.id = "view21";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_25', height: 100 },
            { view: 'label', id: 'view21_question', label: 'Utility Savings', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                height: 290,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: 'multiview',
                        type: 'wide',
                        cells: [
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view21_1',
                                cols: [
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 315,
                                        elements: [
                                            { view: 'text', label: 'SEER Rating for Existing System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt1', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'BTU Capacity for Existing System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt2', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'SEER Rating for Selected System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt3', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'BTU Capacity for NEW System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt4', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: "Customer's Electric Rate", labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt5', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'Average Cooling Hours (from DOE)', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_txt6', inputWidth: '0', readonly: 1 }
                                        ],
                                        id: 'view21_1_form_info'
                                    },
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 120,
                                        elements: [
                                        //{ view: 'input', value: '10', label: '', type: 'text', id: 'view21_cs_input1', labelWidth: '0' },
                                            { view: 'counter', name: '', label: '', step: 1, value: 10, min: '0', max: '100', labelPosition: 'right', labelAlign: 'left', id: 'view21_cs_input1' },
                                            { view: 'input', value: '', type: 'text', id: 'view21_cs_input2', labelWidth: '0' },
                                            //{ view: 'counter', name: '', label: '', step: 1, value: 95, min: '0', max: '100', labelPosition: 'right', labelAlign: 'left', id: 'view21_cs_input3' },
                                            { view: 'text', label: '15', type: 'text', id: 'view21_cs_input3', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: '54,000', type: 'text', id: 'view21_cs_input4', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: '0.12', type: 'text', id: 'view21_cs_input5', inputWidth: '0', readonly: 1 },
                                            { view: 'input', value: '1,800', type: 'text', id: 'view21_cs_input6', labelWidth: '0' }
                                        ],
                                        id: 'view21_1_form',
                                        rules: {
                                            view21_cs_input1: dhx.rules.isNotEmpty,
                                            view21_cs_input2: dhx.rules.isNotEmpty,
                                            view21_cs_input6: dhx.rules.isNotEmpty
                                        }
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view21_2',
                                cols: [
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 315,
                                        align: 'bottom',
                                        elements: [
                                            { view: 'text', label: 'Heat loss', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_2_txt1', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'AFUE Existing System %', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_2_txt2', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'AFUE New System %', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_2_txt3', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'Cost Per Therm', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_2_txt4', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'Average heating hours(from DOE)', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_2_txt5', inputWidth: '0', readonly: 1 }
                                        ],
                                        id: 'view21_2_form_info'
                                    },
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 120,
                                        elements: [
                                            { view: 'text', label: '14', type: 'text', id: 'view21_ghs_input1', inputWidth: '0', readonly: 1 },
                                            { view: 'counter', name: '', label: '', step: 1, value: 68, min: '0', max: '100', labelPosition: 'right', labelAlign: 'left', id: 'view21_ghs_input2' },
                                        //{ view: 'counter', name: '', label: '', step: 1, value: 95, min: '0', max: '100', labelPosition: 'right', labelAlign: 'left', id: 'view21_ghs_input3' },
                                            {view: 'text', label: '14', type: 'text', id: 'view21_ghs_input3', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: '0.12', type: 'text', id: 'view21_ghs_input4', inputWidth: '0', readonly: 1 },
                                            { view: 'input', value: '600', type: 'text', id: 'view21_ghs_input5', labelWidth: '0' }
                                        ],
                                        id: 'view21_2_form',
                                        rules: {
                                            view21_ghs_input5: dhx.rules.isNotEmpty
                                        }
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view21_3',
                                cols: [
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 335,
                                        elements: [
                                            { view: 'text', label: 'HSPF of existing system', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt1', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'BTU Capacity for Existing System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt2', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'HSPF of selected system', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt3', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'BTU Capacity for NEW System', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt4', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: "Customer's Electric Rate", labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt5', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: 'Average Cooling Hours (from DOE)', labelWidth: '320', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'view21_hp_txt6', inputWidth: '0', readonly: 1 }
                                        ],
                                        id: 'view21_3_form_info'
                                    },
                                    {
                                        view: 'form',
                                        scroll: false,
                                        height: 165,
                                        width: 100,
                                        elements: [
                                            { view: 'input', value: '10', label: '', type: 'text', id: 'view21_hp_input1', labelWidth: '0' },
                                            { view: 'input', value: '', type: 'text', id: 'view21_hp_input2', labelWidth: '0' },
                                            { view: 'input', value: '10', label: '', type: 'text', id: 'view21_hp_input3', labelWidth: '0' },
                                            { view: 'text', label: '54,000', type: 'text', id: 'view21_hp_input4', inputWidth: '0', readonly: 1 },
                                            { view: 'text', label: '0.12', type: 'text', id: 'view21_hp_input5', inputWidth: '0', readonly: 1 },
                                            { view: 'input', value: '1800', type: 'text', id: 'view21_hp_input6', labelWidth: '0' }
                                        ],
                                        id: 'view21_3_form',
                                        rules: {
                                            view21_hp_input1: dhx.rules.isNotEmpty,
                                            view21_hp_input2: dhx.rules.isNotEmpty,
                                            view21_hp_input3: dhx.rules.isNotEmpty,
                                            view21_hp_input6: dhx.rules.isNotEmpty
                                        }
                                    }
                                ]
                            }
                        ],
                        id: 'view21_multiview1'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 15 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [ 
                            { view: 'label', id: 'view21_lbl_cs', label: '', align: 'center', height: 37 },
                            { view: 'label', id: 'view21_lbl_ghs', label: '', align: 'center',  height: 37 },
                            { view: 'label', id: 'view21_lbl_hp', label: '', align: 'center',  height: 37 },
                            {
                                view: 'segmented',
                                options: [
                                    { label: 'Cooling Savings', key: 'view21_1', id: 'view21_tab1', value: 'view21_1', height: 64 },
                                    { label: 'Gas Heat Savings', key: 'view21_2', id: 'view21_tab2', value: 'view21_2', height: 64 },
                                    { label: 'Heat Pump Heating Savings', key: 'view21_3', id: 'view21_tab3', value: 'view21_3', height: 64 }
                                ],
                                id: 'view21_segm',
                                multiview: 1,
                                align: 'center'
                            },
                            { view: 'button', id: 'view21_btn_calc', label: 'Calculate', popup: '', click: 'setAnswer21()', css: '' },
                            { view: 'button', id: 'view21_btn', label: 'Save', popup: '', click: 'setAnswer21(true)', css: '' },
                            { view: 'label', id: 'view21_answer', label: '', align: 'center', css: 'answ_label' }
                        ],
                        id: 'view21_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.clearFields = function() {
        
    };
}

View21.prototype = proto_dhtmlx;

function View22() {
    this.id = "view22";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Cost to Keep Existing System', id: 'view22_question', align: 'center', height: 75, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'view21_l1',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: 'form',
                        scroll: false,
                        height: 235,
                        elements: [
                            { view: 'text', id: 'view22_txt1', inputWidth: '0', readonly: 1, labelWidth: '310', name: '', label: 'Remaining equipment life', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', align: 'center' },
                            { view: 'text', id: 'view22_txt2', inputWidth: '0', readonly: 1, labelWidth: '310', name: '', label: 'Utility savings per year', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', align: 'center' },
                            { view: 'text', id: 'view22_txt3', inputWidth: '0', readonly: 1, labelWidth: '310', name: '', label: 'Future repairs', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', align: 'center' },
                            { view: 'text', id: 'view22_txt4', inputWidth: '0', readonly: 1, labelWidth: '310', name: '', label: 'Sub Total', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', align: 'center' },
                            { view: 'text', id: 'view22_txt5', inputWidth: '0', readonly: 1, labelWidth: '310', name: '', label: 'Inflation over remaining equipment life', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', align: 'center' }
                        ],
                        id: 'view22_form_info'
                    },
                    {
                        view: 'form',
                        scroll: false,
                        height: 175,
                        width: 100,
                        elements: [
                            { view: 'input', label: '', placeholder: 'Years', type: 'text', id: 'view22_input1' },
                            { view: 'text', label: '', type: 'text', id: 'view22_input2', inputWidth: '0', readonly: 1 },
                            { view: 'input', label: '', placeholder: "$", type: 'text', id: 'view22_input3' },
                            { view: 'text', label: '', type: 'text', id: 'view22_input4', inputWidth: '0', readonly: 1 },
                            { view: 'input', label: '', placeholder: "%", type: 'text', id: 'view22_input5' }
                            
                        ],
                        id: 'view22_form',
                        rules: {
                            view22_input1: dhx.rules.isNotEmpty,
                            view22_input3: dhx.rules.isNotEmpty,
                            view22_input5: dhx.rules.isNotEmpty
                        }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'view22_l1',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'layout',
                        type: 'clean',
                        rows: [
                             {
                                 view: 'layout',
                                 type: 'clean',
                                 id: 'view22_l11',
                                 cols: [
                                    { view: 'button', label: 'Calculate SubTotal', popup: '', click: 'CalcSubtotal', css: '', id: 'view22_btn0' },
                                    { view: 'label', id: 'view22_lbl0', label: '', align: 'center', height: 50 }
                                ]
                             },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view22_l11',
                                cols: [
                                    { view: 'button', label: 'Cost to keep existing system', popup: '', click: 'SetAnswer22()', css: '', id: 'view22_btn1' },
                                    { view: 'label', id: 'view22_answer', label: '', align: 'center', height: 50 }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view22_l12',
                                cols: [
                                    { view: 'button', label: 'Save results', popup: '', click: 'SetAnswer22(true)', css: '', id: 'view22_btn2' }
                                                    //{ view: 'label', id: 'view22_answer', label: '', align: 'center', height: 50 }
                                ]
                            }
                        ]
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.clearFields = function () {
        $$("view22_input1").setValue("");
        $$("view22_input3").setValue("");
        $$("view22_input5").setValue("");
    };
}

View22.prototype = proto_dhtmlx;

function View23() {
    this.id = "view23";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_25', height: 85 },
            { view: 'label', id: 'view23_question', label: 'What other people have to say\nabout their systems', align: 'center', height: 60, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                height: 260,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: 'multiview',
                        type: 'wide',
                        cells: [
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view23_1',
                                height: 260,
                                cols: [
                                    {
                                        view: 'template',
                                        template: 'html->videoTemplate',
                                        id: 'view23_tmpl_1',
                                        datatype: "json"
                                        //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=SE'
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view23_2',
                                height: 260,
                                cols: [
                                    {
                                        view: 'template',
                                        template: 'html->videoTemplate',
                                        id: 'view23_tmpl_2',
                                        datatype: "json"
                                        //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=ME'
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view23_3',
                                height: 260,
                                cols: [
                                    {
                                        view: 'template',
                                        template: 'html->videoTemplate',
                                        id: 'view23_tmpl_3',
                                        datatype: "json"
                                        //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=LE'
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view23_4',
                                height: 260,
                                cols: [
                                    {
                                        view: 'template',
                                        template: 'html->videoTemplate',
                                        id: 'view23_tmpl_4',
                                        datatype: "json"
                                        //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=LE'
                                    }
                                ]
                            },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view23_5',
                                height: 260,
                                cols: [
                                    {
                                        view: 'template',
                                        template: 'html->videoTemplate',
                                        id: 'view23_tmpl_5',
                                        datatype: "json"
                                        //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=LE'
                                    }
                                ]
                            }
                        ],
                        id: 'view23_multiview1'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 170 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view23_answer', label: '', align: 'center' },
                            {
                                view: 'segmented',
                                css: 'view_lbl',
                                options: [
                                    { label: 'SE', key: 'view23_1', id: 'tab_1', value: 'view23_1', css: 'view_lbl' },
                                    { label: 'ME', key: 'view23_2', id: 'tab_2', value: 'view23_2', css: 'view_lbl' },
                                    { label: 'LE', key: 'view23_3', id: 'tab_3', value: 'view23_3', css: 'view_lbl' },
                                    { label: 'UE', key: 'view23_4', id: 'tab_4', value: 'view23_4', css: 'view_lbl' },
                                    { label: 'HE', key: 'view23_5', id: 'tab_5', value: 'view23_5', css: 'view_lbl' }
                                ],
                                id: 'view23_segm',
                                multiview: 1,
                                align: 'center'
                            },
                            { view: 'button', id: 'view23_btn', label: 'Acknowledge', popup: '', click: 'setAnswer("Acknowledged")', css: '' }
                            
                        ],
                        id: 'view23_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.options = [
        { label: 'SE', key: 'view23_1', id: 'tab_1', value: 'view23_1', css: 'view_lbl' },
        { label: 'ME', key: 'view23_2', id: 'tab_2', value: 'view23_2', css: 'view_lbl' },
        { label: 'LE', key: 'view23_3', id: 'tab_3', value: 'view23_3', css: 'view_lbl' },
        { label: 'UE', key: 'view23_4', id: 'tab_4', value: 'view23_4', css: 'view_lbl' },
        { label: 'HE', key: 'view23_5', id: 'tab_5', value: 'view23_5', css: 'view_lbl' }
    ];

    this.ShowPackage = function (id) {
        $$("view23_segm").data.options.push(this.options[id]);
        $$("view23_segm").refresh();
    };

    this.HideAllPackages = function () {
        for (var i = 1; i <= 5; ++i) {
            $$("view23_segm").data.options = $$("view23_segm").data.options.slice(1);
        }
    };

    this.loadPackagesInfo = function () {
        dhx.ajax().post("../../hvac_app/wizard/GetSelectedVideoUrls",
            null,
            {
                error: function() {
                    alert("Error in get video urls.");
                },
                success: view23.afterloadPackagesInfo
            });
    };

    this.afterloadPackagesInfo = function (data) {
        var obj = dhx.DataDriver.json.toObject(data);
        view23.HideAllPackages();
        for (i in obj) {
            switch (obj[i].sys) {
                case "SE":
                    {
                        $$("view23_tmpl_1").data = { url: obj[i].url };
                        view23.ShowPackage(0);
                        break;
                    }
                case "ME":
                    {
                        $$("view23_tmpl_2").data = { url: obj[i].url };
                        view23.ShowPackage(1);
                        break;
                    }
                case "LE":
                    {
                        $$("view23_tmpl_3").data = { url: obj[i].url };
                        view23.ShowPackage(2);
                        break;
                    }
                case "UE":
                    {
                        $$("view23_tmpl_4").data = { url: obj[i].url };
                        view23.ShowPackage(3);
                        break;
                    }
                case "HE":
                    {
                        $$("view23_tmpl_5").data = { url: obj[i].url };
                        view23.ShowPackage(4);
                        break;
                    }
                default:
            }
        }
    };

    this.showView = function() {
        var data20 = (new System()).toObject(listQuestions.dhtmlx().item(20).data);
        if (data20.length != undefined || data20.length == 0) {
            $$("view23_1").show();
            $$("view23_segm").setValue("view23_1");
        } else {
            $$("view23_" + data20.id.toString()).show();
            $$("view23_segm").setValue("view23_" + data20.id.toString());
        }
    };
}

View23.prototype = proto_dhtmlx;

function View24() {
    this.id = "view24";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 }, //
            {view: 'label', label: 'Reliable Installations', id: 'view24_question', align: 'center', height: 40, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'view24layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: 'layout',
                        type: 'clean',
                        id: 'view24_1',
                        height: 260,
                        cols: [
                            {
                                view: 'template',
                                template: 'html->videoTemplate',
                                id: 'view24_tmpl',
                                datatype: "json"
                                //url: '../../../HVAC_app/Wizard/GetVideoUrls?sysTypeText=LE'
                            }
                        ]
                    },
//                    {
//                        view: 'template',
//                        template: 'html->PictureTemplate',
//                        id: 'view24control_lbl_img1',
//                        datatype: "json",
//                        data: { urlText: "123" },
//                        height: 200
//                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 200 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view24_answer', label: '', align: 'center', height: 50 },
                            { view: 'button', label: 'Acknowledge', popup: '', click: 'setAnswer("Acknowledged")', css: '', id: 'view24_btn' }
                        ],
                        id: 'view24_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View24.prototype = proto_dhtmlx;

function View25() {
    this.id = "view25";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Who We Are', id: 'view25_question', align: 'center', height: 60, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'view25layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        id: "view25_scroll",
                        scroll: "y",
                        height: 245,
                        content:
                            {
                                view: 'label',
                                id: 'view25_lbl',
                                height: 'auto',
                                align: 'center',
                                css: 'answ_label',
                                label: 'Filled from your company.'
                            }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
//                    { view: 'view', id: 'temp_designer_view_36', width: 185 },
//                    {
//                        view: 'carousel',
//                        height: 200,
//                        panel: { },
//                        id: 'view25carousel_2',
//                        cols: [
//                            {
//                                view: 'template',
//                                template: 'html->PictureTemplate',
//                                id: 'view25control_lbl_img1',
//                                datatype: "json",
//                                data:{urlText:"123"}
//                            },
//                            {
//                                view: 'template',
//                                template: 'html->PictureTemplate',
//                                id: 'view25control_lbl_img2',
//                                datatype: "json",
//                                data: { urlText: "123" }
//                            },
//                            {
//                                view: 'template',
//                                template: 'html->PictureTemplate',
//                                id: 'view25control_lbl_img3',
//                                datatype: "json",
//                                data: { urlText: "123" }
//                            }
//                        ]
//                    },
//                    { view: 'view', id: 'temp_designer_view_31', width: 185 }
                ]
            }, 
            { view: 'view', id: 'temp_designer_view_24', height: 190 },
            { view: 'label', id: 'view25_answer', label: '', align: 'center', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'button', label: 'Acknowledge', popup: '', click: 'setAnswer("Acknowledged")', css: '', id: 'view25_btn' }
                        ],
                        id: 'view25_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View25.prototype = proto_dhtmlx;

function View26() {
    this.id = "view26";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Comfort Guarantee', id: 'view26_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'view26_scroll0',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        id: "view26_scroll",
                        scroll: "y",
                        height: 255,
                        content:
                            {
                                view: 'label',
                                id: 'view26_lbl',
                                height: 'auto',
                                align: 'left',
                                css: 'answ_label',
                                label: 'We <br>guarantee<br> that<br> at, design conditions, your new system will maintain a comfortable environment in your home for two full years from the date of installation. If the system fails to maintain a comfortable environment, we will repair the system and provide you with an additional one year HomeGuard at no additional charge.'
                            }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        align: 'center',
                        elements: [
                            { view: 'label', id: 'view26_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign26" style="text-align: center;"></div>', id: 'view26_tmpl', height: 110 },
                            { view: 'button', id: 'view26_btnClear', label: 'Clear', popup: '', click: 'clearSign(26)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view26_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(26)', css: '' },
                                    { view: 'button', id: 'view26_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(26, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view26_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View26.prototype = proto_guarantee;

function View27() {
    this.id = "view27";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Lemon Free Guarantee', id: 'view27_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y",
                        height: 255,
                        content: {
                            view: 'label',
                            id: 'view27_lbl',
                            height: 'auto',
                            label: 'If the compressor in your comfort system fails for any reason during the first five (5) years, instead of just replacing the compressor, we will replace the entire outdoor unit and not charge. If the heat exchanger in your new comfort system fails for any reason during the first five (5) years we will replace the entire furnace at no charge. Plus you will get a new Lemon Free Guarantee on the replacements.',
                            align: 'left',
                            css: 'answ_label'
                        }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view27_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign27" style="text-align: center;"></div>', id: 'view27_tmpl', height: 110 },
                            { view: 'button', id: 'view27_btnClear', label: 'Clear', popup: '', click: 'clearSign(27)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view27_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(27)', css: '' },
                                    { view: 'button', id: 'view27_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(27, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view27_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View27.prototype = proto_guarantee;

function View28() {
    this.id = "view28";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Never Undersold Guarantee', id: 'view28_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y",height: 255, 
                        content:
                            { view: 'label',
                                id: 'view28_lbl',
                                label: 'If you can find the exact same installation, using the same equipment, materials, workmanship, and guarantees for less we will match that price and give you a five (5) year HomeGuard.',
                                align: 'left',
                                height: 'auto',
                                css: 'answ_label' }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view28_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign28" style="text-align: center;"></div>', id: 'view28_tmpl', height: 110 },
                            { view: 'button', id: 'view28_btnClear', label: 'Clear', popup: '', click: 'clearSign(28)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view28_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(28)', css: '' },
                                    { view: 'button', id: 'view28_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(28, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view28_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View28.prototype = proto_guarantee;

function View29() {
    this.id = "view29";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Home Respect Guarantee', id: 'view29_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y",
                        height: 255, 
                        content:
                           { view: 'label',
                               id: 'view29_lbl',
                               label: 'We will wear shoe covers and use drop cloths while working in your home. We will not swear or use tobacco products while in your home. We will clean up the work area to your satisfaction before the job is done. If we fail to leave the work area clean to your satisfaction, we will re-clean the area and give you a three (3) year HomeGuard as an apology.',
                               align: 'left',
                               height: 'auto',
                               css: 'answ_label' }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view29_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign29" style="text-align: center;"></div>', id: 'view29_tmpl', height: 110 },
                            { view: 'button', id: 'view29_btnClear', label: 'Clear', popup: '', click: 'clearSign(29)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view29_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(29)', css: '' },
                                    { view: 'button', id: 'view29_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(29, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view29_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View29.prototype = proto_guarantee;

function View30() {
    this.id = "view30";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'Complete Satisfaction Guarantee', id: 'view30_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y",height: 255,
                        content:
                            { view: 'label',
                                id: 'view30_lbl',
                                label: 'If you are not completely satisfied with your new comfort system during the first year we will make it right. If for any reason we cannot make you satisfied with your new system we will remove your system and refund your entire investment.',
                                align: 'left',
                                height: 'auto',
                                css: 'answ_label' } 
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [                            
                            { view: 'label', id: 'view30_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign30" style="text-align: center;"></div>', id: 'view30_tmpl', height: 110 },
                            { view: 'button', id: 'view30_btnClear', label: 'Clear', popup: '', click: 'clearSign(30)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view30_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(30)', css: '' },
                                    { view: 'button', id: 'view30_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(30, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view30_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View30.prototype = proto_guarantee;

function View31() {
    this.id = "view31";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 60 },
            { view: 'label', label: '', id: 'view31_question0', align: 'center', height: 60, css: 'quest_label' },
            { view: 'label', label: 'This is the system you have selected:', id: 'view31_question', align: 'center', height: 60, css: 'quest_label', hidden: 1 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                height: 280,
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                                view: 'form',
                                scroll: true,
                                elements: [
                                    { view: 'label', label: 'You have selected the UE package<br>which includes:', id: 'lbl_view31', height: 70, align: 'center' },
                                //{ view: 'label', label: 'Package Specifications:', id: 'lbl_view31_4_22', align: 'left', css: "fontSize20" },
                                    { view: 'label', label: '', id: 'lbl_view31_info', align: 'center', height: 'auto', css: "fontSize20" }
                                ],
                                id: 'view31_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 200 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'layout',
                        type: 'clean',
                        rows: [
                            { view: 'label', id: 'view31_answer', label: '', align: 'center', height: 50 },
                            { view: 'button', label: 'Save', popup: '', click: 'setAnswer("Acknowledge")', css: '', id: 'view31_btn_Save' }
                        ],
                        id: 'layout_2'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };

    this.showView = function () {
        var data35 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(35).data);
        $$("lbl_view31").setValue("You have selected the " + data35.MainSystem.id + " package<br>which includes:");
        var descr = "";
        for (i in data35.Jobs) {
            descr += data35.Jobs[i].Description + "<br/>";
        }
        $$("lbl_view31_info").setValue(descr);
        //You have selected the UE package<br>which includes:
    };
}

View31.prototype = proto_dhtmlx;

function View32() {
    this.id = "view32";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 120 },
            { view: 'label', label: 'Your Authorization', id: 'view32_question', align: 'center', height: 60, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 135 },
                    {
                        view: 'layout',
                        type: 'clean',
                        height: 230,
                        rows: [
                            { view: 'template', template: '<div id="sign32" style="text-align: center;"></div>', id: 'view32_tmpl' },
                            { view: 'button', id: 'view32_btnClear', label: 'Clear', inputWidth: 100, align: 'center', popup: '', click: 'clearSign(32)', css: '' }
                        ],
                        id: 'layout_2'
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 135 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: true,
                        elements: [
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view32_layout1',
                                heigth: 160,
                                cols: [
                                    {
                                        view: 'form',
                                        scroll: 0,
                                        elements: [
                                            { view: 'label', label: 'Total Investment: ', id: 'view32_lbl1', align: 'right' },
                                            { view: 'label', label: 'Amount Per Week: ', id: 'view32_lbl2', align: 'right' }
                                        ],
                                        id: 'view32_from1'
                                    },
                                    {
                                        view: 'form',
                                        scroll: 0,
                                        elements: [
                                            { view: 'label', label: '', popup: '', click: '', id: 'view32_lbl3' },
                                            { view: 'label', label: '', popup: '', click: '', id: 'view32_lbl4' }
                                        ],
                                        id: 'view32_from2'
                                    }
                                ]
                            },
                            { view: 'label', id: 'view32_answer', label: '', align: 'center', height: 70 },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'view31_layout2',
                                cols: [
                                    { view: 'button', label: 'Finance Option 1', popup: '', css: '', id: 'view32_btn1', click: "openFinanceWindow(1)" },
                                    { view: 'button', label: 'Finance Option 2', popup: '', css: '', id: 'view32_btn2', click: "openFinanceWindow(2)" },
                                    { view: 'button', label: 'Finance Option 3', popup: '', css: '', id: 'view32_btn3', click: "openFinanceWindow(3)" }
                                ]
                            },
                            { view: 'button', id: 'view32_btn', label: 'Submit', popup: '', click: 'Submit()', css: '' }
                        ],
                        id: 'view32_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View32.prototype = proto_dhtmlx;

function View33() {
    this.id = "view33";
    this.view = {
        view: 'layout',
        type: 'wide',
        rows: [
            {
                view: 'layout',
                type: 'clean',
                rows: [
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', label: 'Contact Information', id: 'view33_lbl1', height: 50 }
                        ],
                        id: 'view33_form_1'
                    },
                    {
                        view: 'layout',
                        type: 'clean',
                        id: 'view33_layout_3',
                        cols: [
                            {
                                view: 'form',
                                scroll: 0,
                                elements: [
                                    { view: 'label', label: 'Email Address', id: 'view33_lbl2' }
                                ],
                                id: 'view33_form_2'
                            },
                            {
                                view: 'form',
                                scroll: false,
                                width: 518,
                                elements: [
                                    { view: 'input', label: '', popup: '', click: '', maxlength: '', disabled: false, placeholder: 'Enter Email Address', css: '', type: 'text', id: 'view33_input_email' }
                                ],
                                id: 'view33_form_3'
                            }
                        ]
                    },
                    {
                        view: 'layout',
                        type: 'clean',
                        id: 'view33_layout_3',
                        cols: 
                        [
                            {
                                view: 'richselect',
                                label: 'Status',
                                value: '2',
                                yCount: "3",
                                id: 'view33_richselect',
                                options: [
                                    { value: '2', label: "Choose status"},
                                    { value: '0', label: "Complete" },
                                    { value: '1', label: "Wait Estimate" }
                                    
                                ],
                                labelWidth: '0',
                                select: '2'
                            },
                            { view: 'button', label: 'Invoice', width:518,  id: 'view33_GenInvoice', click: "view33.GenInvoice()", type: 'form' }
                        ]
                    }
                ]
            },
            {
                view: 'layout',
                type: 'clean',
                rows: [
                    { view: 'label', label: 'Add Payment', id: 'view33_lbl3' },
                    {
                        view: 'layout',
                        type: 'clean',
                        id: 'layout_4',
                        cols: [
                            {
                                view: 'form',
                                scroll: 0,
                                elements: [
                                    { view: 'label', label: 'Payment Type', id: 'view33_lbl4' }
                                ],
                                id: 'view33_form_4'
                            },
                            {
                                view: 'form',
                                scroll: false,
                                elements: [
                                    {
                                        view: 'segmented',
                                        width: 580,
                                        options: [
                                             { label: 'Loading...', key: '1', value: '0', id: 'view33_segmented_tab_1' }
                                        ],
                                        id: 'view33_segmented_payment'
                                    }
                                ],
                                id: 'view33_form_5'
                            }
                        ]
                    },
                    {
                        view: 'layout',
                        type: 'clean',
                        id: 'layout_4',
                        cols: [
                            {
                                view: 'form',
                                scroll: 0,
                                width: 330,
                                elements: [
                                    { view: 'label', label: 'Amount', id: 'view33_lbl5' },
                                    { view: 'label', label: 'Authorization / Check Number', id: 'view33_lbl6' }
                                ],
                                id: 'view33_form_6'
                            },
                            {
                                view: 'form',
                                scroll: false,
                                elements: [
                                    { view: 'input', label: '', popup: '', click: '', maxlength: '', disabled: false, placeholder: 'Balance Amount: ', css: '', type: 'text', id: 'view33_input_amount' },
                                    { view: 'input', label: '', popup: '', click: '', maxlength: '', disabled: false, placeholder: 'Enter Authorization / Check #', css: '', type: 'text', id: 'view33_input_number' }
                                ],
                                rules: { view33_input_amount: dhx.rules.isNotEmpty },
                                id: 'view33_form_7'
                            }
                        ]
                    },
                    { view: 'button', label: 'Submit', id: 'view33_addPayment', click: "addPayment()", type: 'form' }
                ]
            },
            {
                view: 'layout',
                type: 'clean',
                rows: [
                    { view: 'label', label: 'Payments', id: 'control_label_7' },
                    {
                        view: 'dataview',
                        template: 'html->paymentItem',
                        select: false,
                        scroll: 'y',
                        type: { height: 35, width: 745, padding: 7, margin: 0 },
                        id: 'view33_payments',
                        datatype: 'json',
                        data: []
                    },
                    { view: 'label', label: '', id: 'view33_total_amount', width: 0, hidden: 1 },
                    { view: 'label', label: 'Finance', id: 'view33_question', width: 0, hidden: 1 },
                    { view: 'label', label: 'view33_answer', id: 'view33_answer', width: 0 }
                ]
            }
        ],
        id: this.id
    };

    this.GenInvoice = function () {
        var email = $$("view33_input_email").getValue();
        //        if (!validateEmail(email)) {
        //            dhx.notice("Email is not valid!");
        //            return null;
        //        }
        var datajs = $$("view33_payments").serialize();
        var totalAmount = GlobalizeToDouble($$("view33_total_amount").getValue());
        listQuestions.dhtmlx().update("33", { id: "33", question: "Payment", answer: "", data: JSON.stringify({ email: email, payments: datajs, total_amount: totalAmount }) });

        popupntfc.ChangeText("Creating invoice...");
        popupntfc.ShowPopup();
        dhx.ajax().post("../../hvac_app/wizard/GenerateInvoice", null, {
            error: function () {
                popupntfc.HidePopup();
                alert("Error in create invoice.");
            },
            success: view33.afterGeninvoice
        });
    };


    this.afterGeninvoice = function (ok) {
        var text = "";
        popupntfc.HidePopup();
        if (ok) {
            text = "Invoice generated";
            winCompleteJob.showBack();
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
    };

    this.loadPaymentTypes = function () {
            dhx.ajax().post("../../hvac_app/wizard/GetPaymentTypes", null, {
                error: function () {
                    alert("Error in get list of payment types");
                },
                success: view33.afterPost
            });
    };

    this.afterPost = function (posteddata) {
        $$("view33_segmented_payment").data.options = [];
        posteddata = dhx.DataDriver.json.toObject(posteddata);
        for (var i in posteddata) {
            var d = GlobalizeToDouble(i);
            $$("view33_segmented_payment").data.options[d] = {};
            $$("view33_segmented_payment").data.options[d].value = posteddata[d].Id;
            $$("view33_segmented_payment").data.options[d].label = posteddata[d].Name;
            $$("view33_segmented_payment").data.options[d].id = "view33_segm" + d;
            $$("view33_segmented_payment").data.options[d].key = d;

        }
        $$("view33_segmented_payment").refresh();
    };

    this.getTypeNamebyId = function(id) {
        for (var i in $$("view33_segmented_payment").data.options) {
            if ($$("view33_segmented_payment").data.options[i].value == id) {
                return $$("view33_segmented_payment").data.options[i].label;
            }
        }
    };

    this.loadData = function (jsondata) {
        popupntfc.HidePopup();
        if ($$("view33_payments").first() != undefined)
            $$("view33_payments").clearAll();
        if (listQuestions.dhtmlx().item(33).data) {
            var data33 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(33).data);
            var dt35 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(35).data);
            var grandTotal = GlobalizeToDouble(dt35.GrandTotal);
            for (var i in data33.payments) {
                $$("view33_payments").add(data33.payments[i]);
                grandTotal -= GlobalizeToDouble(data33.payments[i].payment);
            }

            $$("view33_total_amount").setValue(grandTotal);
            $$("view33_input_amount").data.placeholder = "Balance Amount: " + Globalize.format(Globalize.parseFloat($$("view33_total_amount").getValue().toString()), "c");
            $$("view33_input_amount").refresh();
            $$("view33_input_amount").setValue(Globalize.format(Globalize.parseFloat($$("view33_total_amount").getValue().toString()), "c"));
            $$("view33_input_email").setValue(data33.email);
        }
        else {
            var dt356 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(35).data);
            $$("view33_total_amount").setValue(dt356.GrandTotal);
            $$("view33_input_amount").data.placeholder = "Balance Amount: " + Globalize.format(Globalize.parseFloat($$("view33_total_amount").getValue().toString()), "c");
            $$("view33_input_amount").refresh();
            $$("view33_input_amount").setValue(Globalize.format(Globalize.parseFloat($$("view33_total_amount").getValue().toString()), "c"));

        }
        popupntfc.HidePopup();
    };

    this.AttachEvents = function () {
        $$("view33_richselect").attachEvent("onchange", function (newVal, oldVal) {
            if (newVal == 0) {
                $$("btnBack").show();
                $$("btnWait").hide();
            }
            
            if (newVal == 1) {
                $$("btnBack").hide();
                $$("btnWait").show();
            }
            
            if (newVal == 2) {
                $$("btnBack").hide();
                $$("btnWait").hide();
            }
        });
    };
}

View33.prototype = proto_dhtmlx;

function View34() {
    this.id = "view34";

    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 130 },
            { view: 'label', id: 'view34_question', label: 'Have you had your ductwork insulated for maximum efficiency and/or would you be interested in insulating to your ductwork to get the most from your energy dollar?', align: 'center', height: 140, css: 'quest_label' },
            { view: 'label', id: 'view34_answer', label: '', align: 'top', height: 140, css: "fontSize32" },
            { view: 'view', id: 'temp_designer_view_24', height: 50 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    { view: 'button', label: 'Have', id: 'view34btn1', css: 'btn2line', type: 'round', click: "setAnswer('We have already had our ductwork insulated for maximum&nbsp;efficiency.','Have')" },
                    { view: 'button', label: 'Interested', id: 'view34btn2', css: 'btn2line', type: 'round', click: "setAnswer('We would be interested in having our ductwork insulated to get the most from our energy&nbsp;dollar.','Interested')" },
                    { view: 'button', label: 'Not Interested', id: 'view34btn3', css: 'btn2line', type: 'round', click: "setAnswer('We are not interested in ductwork insulation at&nbsp;this&nbsp;time.','N/A')" },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View34.prototype = proto_dhtmlx;

function View35() {
    this.id = "view35";

    this.header = { count: "Qty", description: "Description", price: "", total_price: "" };

    this.view = {
        view: 'layout',
        type: 'wide',
        rows: [
            { view: 'label', label: 'List Items', id: 'view35_lbl0', align: 'center' },
            {
                view: 'layout',
                type: 'clean',
                rows: [
                    {
                        view: 'dataview',
                        template: 'html->priceListHeaderTemplate',
                        height: "auto",
                        select: false,
                        scroll: 'y',
                        type: {height: 'auto',width: 745,padding: 5,margin: 0},
                        id: 'view35_priceheader',
                        datatype: 'json',
                        data: this.header
                    },
                    {
                        view: 'dataview',
                        template: 'html->priceListTemplate',
                        height: 380,
                        select: false,
                        scroll: 'y',
                        type: { height: 'auto',
                            width: 745,
                            showMainRow: function (obj) {
                                if (obj.isMainPart || obj.isAccessory)
                                    return "display: none;";
                                return "";
                            },
                            showMainPartRow: function (obj) {
                                if (!obj.isMainPart)
                                    return "display: none;";
                                return "";
                            },
                            showAccessoryRow: function (obj) {
                                if (!obj.isAccessory)
                                    return "display: none;";
                                return "";  
                            },
                            padding: 5, 
                            margin: 0 },
                        id: 'view35_pricelist',
                        datatype: 'json',
                        data: []
                    }
                ]
            },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_3',
                cols: [
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', label: 'Sub Total', id: 'view35_lbl1' },
                            { view: 'label', label: 'Tax', id: 'view35_lbl2' },
                            { view: 'label', label: 'Total Investment', id: 'view35_lbl3' }
                        ],
                        id: 'view35_form1'
                    },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', label: '', id: 'view35_lbl_1', align: 'right', css: 'paddingRight' },
                            { view: 'label', label: '', id: 'view35_lbl_2', align: 'right', css: 'paddingRight' },
                            { view: 'label', label: '', id: 'view35_lbl_3', align: 'right', css: 'paddingRight' }
                        ],
                        id: 'view35_form2'
                    }
                ]
            },
            { view: 'button', label: 'Accept', popup: '', click: 'view35.setAnswer()', css: '', id: 'view35_btn' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_3',
                cols: [
                    { view: 'button', label: 'Addons', popup: '', click: 'view35.winShow()', css: '', id: 'view35_btn_access' },
                    { view: 'button', label: 'Tax rate', popup: '', click: 'view35.winTaxShow()', css: '', id: 'view35_btn_taxrate' }
                ]
            },
            { view: 'label', label: 'Price List', id: 'view35_question', width: 0, hidden: 1 },
            { view: 'label', label: 'view35_answer', id: 'view35_answer', width: 0, hidden: 1 },
            { view: 'label', label: '', id: 'view35_lbl_tax', align: 'right', hidden: 1 }
        ],
        id: this.id
    };

    this.AddAS = function (a, account) {
        var accessory = $$("view35_pricelist").item(a.code);
        if (a.code == "Package") {
            var system20 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(20).data.toString());
            var sys = system20.system.MainSystem;
            var mainitem = {
                Count: "1",
                Code: sys.Code,
                JobCode: sys.JobCode,
                Description: sys.Description,
                ResAccountCode: sys.ResAccountCode,
                Price: GlobalizeDoubleToString(sys.Price, "c"),
                TotalPrice: GlobalizeDoubleToString(sys.TotalPrice, "c"),
                Parts: sys.Parts
            };
            $$("view35_pricelist").add(mainitem, 0);
            var mainparts = sys.Parts;
            for (j in mainparts) {
                var item = {
                    //id - autogenerate
                    PartID: mainparts[j].PartID,
                    IdParant: mainitem.id,
                    isMainPart: true,
                    PartCode: mainparts[j].PartCode,
                    PartName: mainparts[j].PartName,
                    Qty: mainparts[j].Qty,
                    PartStdPrice: GlobalizeDoubleToString(mainparts[j].PartStdPrice, "c"),
                    TotalPrice: GlobalizeDoubleToString(GlobalizeToDouble(mainparts[j].PartStdPrice) * GlobalizeToDouble(mainparts[j].Qty), "c")
                };
                $$("view35_pricelist").add(item, 1+j);
            }
        } else {
            if (accessory != undefined) {
                accessory.Count = GlobalizeToDouble(account) + GlobalizeToDouble(accessory.Count);
                accessory.total_price = GlobalizeDoubleToString(GlobalizeToDouble(accessory.Count) * GlobalizeToDouble(accessory.Price), "c");
                $$("view35_pricelist").update(a.code, accessory);
            } else {
                accessory = {
                    Code: a.code,
                    JobCode: a.JobCode,
                    JobCodeId: a.JobCodeId,
                    Description: a.Description,
                    ResAccountCode: a.ResAccountCode,
                    Price: GlobalizeDoubleToString(a.Price, "c"),
                    Count: account,
                    total_price: GlobalizeDoubleToString(GlobalizeToDouble(account) * GlobalizeToDouble(a.Price), "c")
                };
                $$("view35_pricelist").add(accessory);
            }
        }
        this.UpdateTotalPrice();
    };

    this.removeItem = function (a) {
        var accessory = $$("view35_pricelist").item(a);
        if (accessory.Count > 1) {
            accessory.Count = GlobalizeToDouble(accessory.Count)-1;
            accessory.total_price = GlobalizeDoubleToString(GlobalizeToDouble(accessory.Count) * GlobalizeToDouble(accessory.Price), "c");
            $$("view35_pricelist").update(a, accessory);
        }
        else {
            $$("view35_pricelist").remove(a);
        }
        this.UpdateTotalPrice();
    };

    this.addPart = function(p) {
        var part = $$("view35_pricelist").item(p);

        part.Qty = GlobalizeToDouble(part.Qty) + 1;
        part.TotalPrice = GlobalizeDoubleToString(GlobalizeToDouble(part.Qty) * GlobalizeToDouble(part.PartStdPrice), "c");

        var main = $$("view35_pricelist").item(part.IdParant);
        for (i in main.Parts) {
            if (main.Parts[i].PartID == part.PartID) {
                main.Parts[i].Qty = part.Qty;
                main.Parts[i].TotalPrice = GlobalizeDoubleToString(GlobalizeToDouble(part.PartStdPrice) * GlobalizeToDouble(part.Qty), "c");
            }
        }
        $$("view35_pricelist").update(p, part);
        this.UpdateTotalPrice();

    };

    this.removePart = function (p) {
        var part = $$("view35_pricelist").item(p);
        if (part.Qty > 0) {

            part.Qty = GlobalizeToDouble(part.Qty) - 1;
            part.TotalPrice = GlobalizeDoubleToString(GlobalizeToDouble(part.Qty) * GlobalizeToDouble(part.PartStdPrice), "c");

            var main = $$("view35_pricelist").item(part.IdParant);
            var isDelete = true;
            for (i in main.Parts) {
                if (main.Parts[i].PartID == part.PartID) {
                    main.Parts[i].Qty = part.Qty;
                    main.Parts[i].Price = GlobalizeDoubleToString(part.PartStdPrice, "c");
                    main.Parts[i].TotalPrice = GlobalizeDoubleToString(GlobalizeToDouble(part.PartStdPrice) * GlobalizeToDouble(part.Qty), "c");
                }
                if (main.Parts[i].Qty > 0)
                    isDelete = false;
            }
            $$("view35_pricelist").update(p, part);

            if (isDelete) {
                for (i in main.Parts) {
                    $$("view35_pricelist").remove(main.Parts.id);
                }
                $$("view35_pricelist").remove(part.IdParant);
            }
            this.UpdateTotalPrice();
        }

    };

    this.changeTax = function (newTax) {
        $$("view35_lbl_tax").setValue(newTax);
        this.UpdateTotalPrice();
    };

    this.winShow = function () {
        winAS.dhtmlx().show();
        winAS.dhtmlx().setPosition(310, 110);
    };

    this.winTaxShow = function () {
        winTax.dhtmlx().show();
        winTax.dhtmlx().setPosition(310, 110);
    };

    this.showView = function () {
        $$("view35_lbl_tax").setValue(0);
        if (listQuestions.dhtmlx().item(35).data != undefined) {
            var data = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(35).data.toString());
            this.loadList(data);
        } else {
            var system20 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(20).data.toString());
            this.loadList(system20.system);
        }
        var data19 = (new SystemSize()).toObject(listQuestions.dhtmlx().item(19).data.toString());
        winAS.loadAccessories(data19.gain);
    };

    this.UpdateTotalPrice = function () {
        var pricelist = new PriceList();
        pricelist.Jobs = $$("view35_pricelist").serialize();
        this.loadList(pricelist);
    };

    this.Tax = { };

    this.loadList = function (selectedsys) {
        if ($$("view35_pricelist").first() != undefined) {
            $$("view35_pricelist").clearAll();
        }
        var totalamounttemp = 0;


        var ar = selectedsys.Jobs;
        for (i in ar) {
            if (ar[i].isMainPart == undefined) {
                if (ar[i].Parts != undefined) {
                    var mainitem = {
                        Count: "1",
                        Code: ar[i].Code,
                        JobCode: ar[i].JobCode,
                        Description: ar[i].Description,
                        ResAccountCode: ar[i].ResAccountCode,
                        Price: GlobalizeDoubleToString(ar[i].Price, "c"),
                        TotalPrice: GlobalizeDoubleToString(ar[i].TotalPrice, "c"),
                        Parts: ar[i].Parts
                    };
                    $$("view35_pricelist").add(mainitem);
                    var mainparts = ar[i].Parts;
                    for (j in mainparts) {
                        var item = {
                        //id - autogenerate
                            PartID: mainparts[j].PartID,
                            IdParant : mainitem.id,
                            isMainPart: true,
                            PartCode: mainparts[j].PartCode,
                            PartName: mainparts[j].PartName,
                            Qty: mainparts[j].Qty,
                            PartStdPrice: GlobalizeDoubleToString(mainparts[j].PartStdPrice, "c"),
                            TotalPrice: GlobalizeDoubleToString(GlobalizeToDouble(mainparts[j].PartStdPrice) * GlobalizeToDouble(mainparts[j].Qty), "c")
                        };
                        $$("view35_pricelist").add(item);
                        totalamounttemp += GlobalizeToDouble(item.TotalPrice);
                    }
                } else {

                    item = {
                        id: ar[i].Code,
                        isAccessory: true,
                        Code: ar[i].Code,
                        JobCode: ar[i].JobCode,
                        Description: ar[i].Description,
                        ResAccountCode:ar[i].ResAccountCode,
                        Count: ar[i].Count,
                        Price: GlobalizeDoubleToString(ar[i].Price, "c"),
                        TotalPrice: GlobalizeDoubleToString(GlobalizeToDouble(ar[i].Price) * GlobalizeToDouble(ar[i].Count), "c")
                    };
                    $$("view35_pricelist").add(item);
                    totalamounttemp += GlobalizeToDouble(item.TotalPrice);
                }
            }
        }
        $$("view35_lbl_1").setValue(GlobalizeDoubleToString(totalamounttemp, "c"));
        var tax = 0.05;
        if (selectedsys.TaxRate != undefined) {
            tax = GlobalizeToDouble(selectedsys.TaxRate);
            this.Tax = tax;
        }
        else {
            tax = GlobalizeToDouble($$("view35_lbl_tax").getValue());
        }
        $$("view35_lbl_tax").setValue(tax);
        $$("view35_lbl_2").setValue(GlobalizeDoubleToString(totalamounttemp * tax, "c"));
        $$("view35_lbl_3").setValue(GlobalizeDoubleToString(totalamounttemp * tax + totalamounttemp, "c"));
    };

    this.ChangeDesriptionOnMain = function(id, domOject) {
        $$("view35_pricelist").item(id).Description = domOject.value;
    };
    

    this.setAnswer = function () {
        var pricelist = new PriceList();
        var system20 = dhx.DataDriver.json.toObject(listQuestions.dhtmlx().item(20).data);
        var sys = system20.system.MainSystem;
        pricelist.MainSystem = sys;
        //return values to double
        pricelist.MainSystem.Price = GlobalizeToDouble(pricelist.MainSystem.Price);
        pricelist.MainSystem.TotalPrice = GlobalizeToDouble(pricelist.MainSystem.TotalPrice);

        var d = $$("view35_pricelist").serialize();
        var f = [];
        //return values to double
        for (var i in d) {
            //if 
            if (!d[i].isMainPart) {
                d[i].Price = GlobalizeToDouble(d[i].Price);
                f.push(d[i]);
            }
        }

        pricelist.Jobs = f;
        pricelist.TotalAmount = GlobalizeToDouble($$("view35_lbl_1").getValue());
        pricelist.Tax = GlobalizeToDouble($$("view35_lbl_2").getValue());
        pricelist.GrandTotal = GlobalizeToDouble($$("view35_lbl_3").getValue());
        pricelist.TaxRate = GlobalizeToDouble($$("view35_lbl_tax").getValue());

        //linked questions
        if (listQuestions.dhtmlx().item(32) != undefined) {
            listQuestions.dhtmlx().item(32).answer = "";
            listQuestions.dhtmlx().item(32).data = undefined;
        }

        setAnswer("Total Investment: " + GlobalizeDoubleToString(pricelist.GrandTotal, "c"), pricelist);
    };
}

View35.prototype = proto_dhtmlx;

function View36() {
    this.id = "view36";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'No Paperwork Promise', id: 'view36_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y", height: 255,
                        content:
                            { view: 'label',
                                id: 'view36_lbl',
                                label: 'We will fill out all rebate and registration forms to guarantee that you will receive them, if not; we will write you a check for the difference.',
                                align: 'left',
                                height: 'auto',
                                css: 'answ_label'
                            }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view36_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign36" style="text-align: center;"></div>', id: 'view36_tmpl', height: 110 },
                            { view: 'button', id: 'view36_btnClear', label: 'Clear', popup: '', click: 'clearSign(36)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view36_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(36)', css: '' },
                                    { view: 'button', id: 'view36_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(36, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view36_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View36.prototype = proto_guarantee;

function View37() {
    this.id = "view37";
    this.view = {
        view: 'layout',
        type: 'clean',
        css: "maninback",
        rows: [
            { view: 'view', id: 'temp_designer_view_24', height: 100 },
            { view: 'label', label: 'No Surprises Guarantee', id: 'view37_question', align: 'center', height: 50, css: 'quest_label' },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 155 },
                    {
                        view: "scrollview",
                        scroll: "y", height: 255,
                        content:
                            { view: 'label',
                                id: 'view37_lbl',
                                label: 'We ask lots of questions to make sure there are no surprises. We start work early so you can be to work on time.in Your price is locked in, if we missed something, we pay for it!',
                                align: 'left',
                                height: 'auto',
                                css: 'answ_label'
                            }
                    },
                    { view: 'view', id: 'temp_designer_view_36', width: 155 }
                ]
            },
            { view: 'view', id: 'temp_designer_view_24', height: 55 },
            {
                view: 'layout',
                type: 'clean',
                id: 'layout_8',
                cols: [
                    { view: 'view', id: 'temp_designer_view_36', width: 115 },
                    {
                        view: 'form',
                        scroll: false,
                        elements: [
                            { view: 'label', id: 'view37_answer', label: '', align: 'center', height: 42 },
                            { view: 'template', template: '<div id="sign37" style="text-align: center;"></div>', id: 'view37_tmpl', height: 110 },
                            { view: 'button', id: 'view37_btnClear', label: 'Clear', popup: '', click: 'clearSign(37)', css: '' },
                            {
                                view: 'layout',
                                type: 'clean',
                                id: 'layout_8',
                                cols: [
                                    { view: 'button', id: 'view37_btnAg', label: 'Important', popup: '', click: 'setAnswerSign(37)', css: '' },
                                    { view: 'button', id: 'view37_btnDag', label: 'Unimportant', popup: '', click: 'setAnswerSign(37, "Unimportant", false)', css: '' }
                                ]
                            }
                        ],
                        id: 'view37_form'
                    },
                    { view: 'view', id: 'temp_designer_view_31', width: 115 }
                ]
            }
        ],
        id: this.id
    };
}

View37.prototype = proto_guarantee;

function ListQuestions() {
    this.id = 'list_questions';
    this.proxy = new dhx.proxy({
            url: '../../hvac_app/wizard/getxmllist',
            storage: dhx.storage.local
        });
    this.view = { view: 'list', css: 'cust_list_item',
        type: { width: 'auto',
            height: 'auto',
            padding: 10,
            align: 'left',
            displayitem: function(obj) { return obj.answer == "" ? "not_answered" : "answered"; },
            template: '<div style="float:left; width:235px;"><div style="font-style:italic; font-size: 0.9em; width: 235px; white-space: pre-wrap;">#question#</div><div style="color:#707070; font-size: 0.9em; white-space: pre-wrap;">#answer#</div></div><div class="itemlist_img_{common.displayitem()}" ></div>'
        },
        select: true,
        scroll: true,
        multiview: true,
        id: this.id,
        align: 'left',
        waitMessage:true,
        //url: this.proxy,
        datatype: "json"
    };
    this.dhtmlx = function () { return $$(this.id); };

    this.Initialization = function () {
        dp = new dhx.DataProcessor({
            master: listQuestions.dhtmlx(),
            url: this.proxy
        });
        popup_controller.attachEvents();
        popup_settings.timer_total = $$("timer_counter").getValue();
        popup_settings.timer_total = 4000000;
        $$("slider").blockEvent();
        $$("btnNext").hide();
        $$("btnPrev").hide();
        $$("btnBack").hide();
        this.AttachEvents();
    };

    this.AttachEvents = function () {
        this.dhtmlx().attachEvent("onafterselect", "listselect");
        this.dhtmlx().attachEvent("onafterrender", "changeListItem");

        $$("view12_slider_kwh").attachEvent("onchange", function (ol) {
            var value = ol / 1000;
            $$("view12_lbl_kwh").setValue(Globalize.format(value, "n3"));
        });
        $$("view12_lbl_kwh").attachEvent("onchange", function (ne) {
            $$("view12_slider_kwh").blockEvent();
            $$("view12_slider_kwh").setValue(Math.ceil(Globalize.parseFloat(ne.toString()) * 1000));
            $$("view12_slider_kwh").unblockEvent();
        });
        $$("view12_slider_ccf").attachEvent("onchange", function (ol) {
            var value = ol / 100;
            $$("view12_lbl_ccf").setValue(Globalize.format(value, "n2"));
        });
        $$("view12_lbl_ccf").attachEvent("onchange", function (ne) {
            $$("view12_slider_ccf").blockEvent();
            $$("view12_slider_ccf").setValue(Math.ceil(Globalize.parseFloat(ne.toString()) * 100));
            $$("view12_slider_ccf").unblockEvent();
        });
        $$("view19_slider_loss").attachEvent("onchange", function (ol, ne) {
            $$("view19_lbl_loss").setValue(Globalize.format(ol, "n0"));
        });
        $$("view19_lbl_loss").attachEvent("onchange", function (ne, old) {
            $$("view19_slider_loss").blockEvent();
            $$("view19_slider_loss").setValue(Globalize.parseFloat(ne.toString()));
            $$("view19_slider_loss").unblockEvent();
        });
        $$("view19_slider_gain").attachEvent("onchange", function (ol) {
            var value = ol / 10;
            $$("view19_lbl_gain").setValue(Globalize.format(value, "n1"));
        });
        $$("view19_lbl_gain").attachEvent("onchange", function (ol) {
            $$("view19_slider_gain").blockEvent();
            $$("view19_slider_gain").setValue(Math.ceil(Globalize.parseFloat(ol.toString()) * 10));
            $$("view19_slider_gain").unblockEvent();
        });

        $$("view22_form").attachEvent("onchange", function (ne) {
            if ($$("view22_input1").getValue() != '' && $$("view22_input3").getValue() == '') {
                var cost = new CostSystem();
                cost.saving = Globalize.parseFloat($$("view22_input2").data.label);
                cost.remaing = Globalize.parseFloat($$("view22_input1").getValue());
                cost.repears = Globalize.parseFloat($$("view22_input3").getValue());
                $$("view22_input4").data.label = Globalize.format(cost.subtotal(), "c");
                $$("view22_input4").refresh();
            }
        });
        $$("view22_input3").attachEvent("onchange", function (ne, ol) {
            if ($$("view22_input1").getValue() != '' && $$("view22_input3").getValue() == '') {
                var cost = new CostSystem();
                cost.saving = Globalize.parseFloat($$("view22_input2").data.label);
                cost.remaing = Globalize.parseFloat($$("view22_input1").getValue());
                cost.repears = Globalize.parseFloat($$("view22_input3").getValue());
                $$("view22_input4").data.label = Globalize.format(cost.subtotal(), "c");
                $$("view22_input4").refresh();
            }
        });

        $$("view18_answer").attachEvent("onDateSelected", function (date) {
            $$("view18_lblDate").setValue(formatdDate(date));
        });

        view33.AttachEvents();
    };

    this.updateList = function () {
        dp._in_progress = {};
        dp.updatedRows = [];
        dp.ma = [];
        if ($$(this.id).first() != undefined)
            $$(this.id).clearAll();
        popupntfc.ChangeText("Loading job...");
        popupntfc.ShowPopup();
        $$(this.id).load(this.proxy, "json", afterLoad());
        
        timeLoadingUpdate = setInterval(function () {
            if (listQuestions.dhtmlx().first() != undefined) {
                clearInterval(timeLoadingUpdate);
                //listQuestions.dhtmlx().select(0);
                showView(0);
                listQuestions.dhtmlx().unselectAll();
                listQuestions.dhtmlx().select("0");
                popupntfc.HidePopup();
            }
        }, 100);
    };
}

function afterLoad() {
    //alert("loaded");
    //showView(0);
}


var popup_settings = new PopupSettings();
var popup_controller = new PopupController();

var winAS = new WindowAccessories();
var winTax = new WindowTaxRate();
var winFinOpt = new FinanceOptionWindow();
var winCompleteJob = new CompleteWindow();

var listQuestions = new ListQuestions();

var view0 = new View0();
var view1 = new View1();
var view2 = new View2();
var view3 = new View3();
var view5 = new View5();
var view6 = new View6();
var view7 = new View7();
var view8 = new View8();
var view9 = new View9();
var view10 = new View10();
var view11 = new View11();
var view12 = new View12();
var view13 = new View13();
var view14 = new View14();
var view15 = new View15();
var view16 = new View16();
var view17 = new View17();
var view18 = new View18();
var view19 = new View19();
var view20 = new View20();
var view21 = new View21();
var view22 = new View22();
var view23 = new View23();
var view24 = new View24();
var view25 = new View25();
var view26 = new View26();
var view27 = new View27();
var view28 = new View28();
var view29 = new View29();
var view30 = new View30();
var view31 = new View31();
var view32 = new View32();
var view33 = new View33();
var view34 = new View34();
var view35 = new View35();
var view36 = new View36();
var view37 = new View37();

var arraylistView = [view0.view];

var ui_HVAC_Questions = { view: 'layout', type: 'wide', id: 'app_layout',
    cols: [
        { view: 'layout', type: 'wide', width: 275,
            rows: [
                { view: 'toolbar', type: 'MainBar',
                    elements: [
                        { view: 'button', label: 'Exit', id: 'btn_logout', click: 'exitClick', inputWidth: '120', align: 'left' },
                        { view: 'button', label: 'Settings', id: 'control_button_8', popup: 'Menu1', inputWidth: '120', align: 'center' }
                    ], id: 'menu_toolbar'
                },
                listQuestions.view
            ], id: 'left_layout'
        },
        { view: 'layout', type: 'wide', width: 724,
            rows: [
                { view: 'toolbar', type: 'MainBar',
                    elements: [
                        { view: 'layout', type: 'wide',
                            rows: [
                                { view: 'button', label: 'Previous', id: 'btnPrev', inputWidth: '185', width: 200, type: 'prev', click: "click_btn_prev" },

                            ], id: 'layout_4', width: 200
                        },
                        { view: 'slider', label: '', labelWidth: '-20', value: 0, min: 0, max: 100, step: 1, _options_number: 100, id: 'slider', mode: 'minmax' },
                        { view: 'layout', type: 'wide',
                            rows: [
                                { view: 'button', label: 'Next', id: 'btnNext', type: 'next', inputWidth: '185', width: 200, align: 'right', click: "click_btn_next" },
                                { view: 'button', label: 'Complete Job', id: 'btnBack', type: 'next', inputWidth: '185', width: 200, align: 'right', click: "click_btn_back" },
                                { view: 'button', label: 'Wait Estimate', id: 'btnWait', type: 'next', inputWidth: '185', width: 200, align: 'right', click: "click_btn_wait" }
                            ], id: 'layout_4', width: 200
                        }
                    ], id: 'toolbar_2'
                },
                { view: 'multiview', type: 'wide',
                    cells: arraylistView, id: 'multiview_3'
                }
            ], id: 'layout_6'
        }
    ]
};
