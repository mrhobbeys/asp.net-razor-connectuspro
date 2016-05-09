var ui_HVAC_Login = {
    view: 'layout',
    type: 'clean',
    css: "p_logo",
    rows: [
        { view: 'label', label: 'version 1.069', id: 'lblVersion', height: 300 },
        {
            view: 'layout',
            type: 'clean',
            id: 'layout_4',
            cols: [
                { view: 'view', id: 'temp_designer_view_9' },
                {
                    view: 'form',
                    scroll: 0,
                    elements: [
                        { view: 'input', label: '', placeholder: 'Enter Login', type: 'text', id: 'input_login' },
                        { view: 'input', label: '', placeholder: 'Enter Password', type: 'password', id: 'input_password' },
                        { view: 'checkbox', label: 'Remember me', value: 1, labelPosition: 'right', id: 'input_rem' }
                    ],
                    id: 'login_form'
                },
                { view: 'view', id: 'temp_designer_view_13' }
            ]
        },
        { view: 'button', label: 'Login', id: 'btn_login', type: 'form', click: 'login()', inputWidth: '200', align: 'center' },
        { view: 'view', id: 'temp_designer_view_14' }
    ],
    id: 'view_login'
};

function PopupNotification() {
    this.id = "notificationPopup";
    this.view = {
        view: "window",
        id: this.id,
        headHeight: 0,
        header: { view: 'view', id: 'temp_designer_view_24', height: 1 },
        body: {
            view: 'label',
            label: 'Complete Job',
            popup: '',
            click: '',
            id: 'tnfcLabel'
        },
        left: 500,
        modal: true,
        position: 'center'
    };

    this.ChangeText = function (newtext) {
        $$("tnfcLabel").setValue(newtext);
    };

    this.ShowPopup = function () {
        $$(this.id).show();
    };

    this.HidePopup = function () {
        $$(this.id).hide();
    };
};
PopupNotification.prototype = proto_dhtmlx;

var popupntfc = new PopupNotification();

function login() {
    //dhx.notice({ delay: 750, message: "Checking ..." });
    popupntfc.ChangeText("Checking...");
    popupntfc.ShowPopup();
    var formData = $$('login_form').getValues();
    dhx.ajax().get("../../hvac_app/account/login", formData, { error: function (text, xml, XmlHttpRequest) {
                            popupntfc.HidePopup();
                          alert("Error in requst to server when app try to login");
                        }, success:afterCall});
}

function logout() {
    dhx.confirm({
            title: "Logout",
            message: "Do you really want to exit?",
            callback: ConfirmClickLogout
        });
}

function ConfirmClickLogout(ok){
    if (!ok)
        return null;
   // $$("preparation_app").reconstruct();
    $$("preparation_app").show();
    $$("p_list_menu").select($$("p_list_menu").first());
    job_list.dhtmlx().unselectAll();
    $$("p_view_1").show();
    dhx.ajax().post("../../hvac_app/account/logout", null, afterLogout);
}

function afterLogout() {
    $$("view_login").show();
    $$("input_login").setValue("");
    $$("input_password").setValue("");
    $$("input_rem").setValue("1");
    clearInterval(gps_interval);
}

function islogged(text) {
    var data = dhx.DataDriver.json.toObject(text);
    if (data.result) {
        var frID = datastory.get("franchise_id"); 
        $$("p_view4_lblFrIDInfo").setValue(frID);
        $$("preparation_app").show();
        LoadSetupData();

        return true;

    }
    return false;
}

function afterCall(text, json) {
    popupntfc.HidePopup();
    var data = dhx.DataDriver.json.toObject(text, json);
    if (data.result == "success") {
        window.location.reload();
        //----- after login
//        var frID = datastory.get("franchise_id");
//        $$("p_view4_lblFrIDInfo").setValue(frID);
//        $$("preparation_app").show();
//        LoadSetupData();
        //----- after login
    } else {
        dhx.alert("The login and password are incorrect!");
    }
}

var gps_interval;

function LoadSetupData() {
    getPosition();
    locScreen.LoadStatuses();
    view33.loadPaymentTypes();
    gps_interval = setInterval(getPosition, 60000);
    save_logo.loadLogoImage();
    job_list.updateList();
    //winAS.loadAccessories();
    winTax.loadTaxRates();
    selected_systems_controller.LoadSystems();
    setup_screen_controller.LoadData();
    finance_options_controller.LoadData();
    guaranteetexts_controller.LoadGuaranteeTexts(guaranteetexts_controller.afterGetGuaranteeForSetup);
    save_images.LoadData(save_images.afterLoadToSetup);
}

var options = { enableHighAccuracy: false, timeout: 60000, maximumAge: 0 };

function getPosition() {
    if (navigator.geolocation !== null) {
        navigator.geolocation.getCurrentPosition(geolocationSuccess, geolocationError, options);
    }
    else {
        dhx.notice({ delay: 750, message: "Location Services not enabled" });
    }

}

function geolocationSuccess(position) {
    var time = new Date();
    $.ajax({
        type: "post",
        url: "../../hvac_app/wizard/savegpspoint",
        data: { Lat: position.coords.latitude, Lng: position.coords.longitude, TimeRecord: time.toGMTString() },
        success: function (data) {
            if (!data.result) {
                dhx.notice({ delay: 750, message: "Service not work" });
            }

        }
    });
}

function geolocationError() {
    dhx.notice({ delay: 750, message: "Location Services not enabled" });
}
