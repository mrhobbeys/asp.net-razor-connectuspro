function LocationScreen() {
    this.id = "p_view_5";
    this.view = {
        view: 'layout',
        type: 'clean',
        rows: [
            { view: 'textarea', label: 'Textarea', id: 'loc_lbl_Address', height: 120 },
            {
                view: 'layout',
                type: 'clean',
                id: 'loc_layout_3',
                cols: [
                    {
                        view: 'form',
                        scroll: true, 
                        css: 'writeBackground',
                        elements: [
                            {
                                view: 'richselect',
                                label: 'Status',
                                value: '0',
                                yCount: "6",
                                id: 'loc_richselect',
                                options: [
                                    { value: 0, label: "None" },
                                    { value: 5, label: "Traveling" },
                                    { value: 6, label: "Arrived" }
                                ],
                                labelWidth: '70',
                                select: 1
                                
                            }
                        ],
                        id: 'loc_form_2'
                    },
                    {
                        view: 'form',
                        scroll: true,
                        elements: [
                            { view: 'button', label: 'Show Job Location', id: 'loc_control_button_4', type: 'round', click: 'locScreen.ShowJobLocation' },
                            { view: 'button', label: 'Show Current Location', id: 'loc_control_button_5', type: 'round', click: 'locScreen.ShowCurrentLocation' },
                            { view: 'button', label: 'Route to Job', id: 'loc_control_button_6', type: 'round', click: 'locScreen.RouteToJob' }
                        ],
                        id: 'loc_form_3'
                    },
                    {
                        view: 'form',
                        scroll: 0,
                        elements: [
                            { view: 'button', label: 'Show Office Location', id: 'loc_control_button_7', type: 'round', click: 'locScreen.ShowOfficeLocation' },
                            { view: 'button', label: 'Large Map', id: 'loc_control_button_8', type: 'round' },
                            { view: 'button', label: 'Route to Office', id: 'loc_control_button_9', type: 'round', click: 'locScreen.RouteToOffice' }
                        ],
                        id: 'loc_form_4'
                    }
                ]
            },
            {
                view: 'googlemap',
                zoom: 16,
                center: { x: 29.71664, y: -95.284715 },
                mapType: 'ROADMAP',
                id: 'loc_map_2'
            }
        ],
        id: "p_view_5"
    };

    this.LoadStatuses = function () {
        dhx.ajax().post("../../hvac_app/wizard/GetJobStatusesForTablet", null, {
            error: function () {
                alert("Error in get job statuses");
            },
            success: locScreen.AfterLoadStatuses
        });
    };

    this.AfterLoadStatuses = function (data) {
        $$("loc_richselect").data.data.length = 0;
        $$("loc_richselect").data.data = dhx.DataDriver.json.toObject(data);
        $$("loc_richselect").refresh();
    };

    this.AttachEvents = function () {
        $$("loc_richselect").attachEvent("onchange", function (newVal, oldVal) {
            if (newVal != oldVal) {
                dhx.ajax().sync().post("../../hvac_app/wizard/ChangeJobStatus", { statusid: newVal }, {
                    error: function () {
                        alert("Error in get list of accessories");
                    },
                    success: null
                });
            }
        });
    };

    this.gmap = function () { return $$("loc_map_2").map; };

    this.ShowJobLocation = function () {
        locScreen.gmap().setCenter(locScreen.SetJobLocation());
    };

    this.SetJobLocation = function () {
        var gmap = $$("loc_map_2").map;
        if (gmap.jobAddressMarker != undefined)
            gmap.jobAddressMarker.setMap(null);
        var id = datastory.get("id_job");
        var point = job_list.dhtmlx().item(id).point;
        var myLatlng = new google.maps.LatLng(point.y, point.x);
        gmap.jobAddressMarker = new google.maps.Marker({
            position: myLatlng,
            map: gmap
        });
        return myLatlng;
    };

    this.ShowOfficeLocation = function() {
        locScreen.gmap().setCenter(locScreen.SetOfficeLocation());
    };

    this.SetOfficeLocation = function () {
        var gmap = $$("loc_map_2").map;
        if (gmap.officeAddressMarker != undefined)
            gmap.officeAddressMarker.setMap(null);
        var id = datastory.get("id_job");
        var franchisePoint = job_list.dhtmlx().item(id).franchisePoint;
        var myLatlng = new google.maps.LatLng(franchisePoint.y, franchisePoint.x);
        gmap.officeAddressMarker = new google.maps.Marker({
            position: myLatlng,
            map: gmap
        });
        return myLatlng;
    };

    this.ShowCurrentLocation = function() {
        if (navigator.geolocation !== null) {
            var options = { enableHighAccuracy: false, timeout: 60000, maximumAge: 0 };
            navigator.geolocation.getCurrentPosition(locScreen.geolocationSuccess, locScreen.geolocationError, options);
        } else {
            dhx.notice({ delay: 1500, message: "Location Services not enabled" });
        }
    };

    this.geolocationSuccess = function (position) {
            var gmap = $$("loc_map_2").map;
            if (gmap.currentMarker != undefined)
                gmap.currentMarker.setMap(null);
            var myLatlng = new google.maps.LatLng(position.coords.latitude, position.coords.longitude);
            gmap.currentMarker = new google.maps.Marker({
                position: myLatlng,
                map: gmap
            });
            gmap.setCenter(myLatlng);
    };

    this.geolocationError = function() {
        dhx.notice({ delay: 1500, message: "Could not retrieve location" });
    };

    this.RouteToJob = function () {
        if (locScreen.gmap().jobAddressMarker == undefined)
            locScreen.SetJobLocation();
        if (locScreen.gmap().currentMarker == undefined)
            locScreen.ShowCurrentLocation();
        if (locScreen.gmap().currentMarker == undefined)
            dhx.notice({ delay: 1500, message: "Could not create route to job" });
        else {
            var request = {
                origin: locScreen.gmap().currentMarker.position,
                destination: locScreen.gmap().jobAddressMarker.position,
                travelMode: google.maps.TravelMode.DRIVING
            };
            directionsService.route(request, function(response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                }
                else {
                    dhx.notice({ delay: 1500, message: "Could not create route to job" });
                }
            });
        }
    };
    this.RouteToOffice = function () {
        if (locScreen.gmap().officeAddressMarker == undefined)
            locScreen.SetOfficeLocation();
        if (locScreen.gmap().currentMarker == undefined)
            locScreen.ShowCurrentLocation();
        if (locScreen.gmap().currentMarker == undefined)
            dhx.notice({ delay: 1500, message: "Could not create route to office" });
        else {
            var request = {
                origin: locScreen.gmap().currentMarker.position,
                destination: locScreen.gmap().officeAddressMarker.position,
                travelMode: google.maps.TravelMode.DRIVING
            };
            directionsService.route(request, function(response, status) {
                if (status == google.maps.DirectionsStatus.OK) {
                    directionsDisplay.setDirections(response);
                }
                else {
                    dhx.notice({ delay: 1500, message: "Could not create route to office" }); 
                }
            });
        }
    };
}

function backTojobList() {
    $$("p_view_3").show();
    $$("p_view_3").unselectAll();
    $$("loc_btn_back").hide();
    $$("loc_control_button_2").hide();
}

function backToSetupScreen() {
    $$("p_view_4").show();
    $$("p_view_back").hide();
}

function launchApp() {
    $$("app_layout").show();
    listQuestions.updateList();
    finance_options_controller.LoadData();
    guaranteetexts_controller.LoadGuaranteeTexts(guaranteetexts_controller.afterGetGuaranteeTexts);
    save_images.LoadData(save_images.afterLoadForApp);

    view22.clearFields();
    view23.loadPackagesInfo();
    
    $$("btnBack").hide();
}

function JobList() {
    this.id = 'p_view_3',
    this.url = '../../hvac_app/wizard/getjobs';
    this.proxy = new dhx.proxy({
            url: this.url,
            storage: dhx.storage.local
        });
    this.view = {
        view: 'list', css: "p_logo",
        type: {
            width: 'auto',
            height: 22,
            margin: 0,
            padding: 10,
            template: '<div class="menu_item">#Address#</div><div class="corner" ></div>'
        },
        scroll: true,
        select: true,
        id: this.id,
        datatype: 'json',
        data: [
            { "id": "1", "Address": "Demo Address 1" }
        ]
    };

    this.dhtmlx = function() {
        return $$(this.id);
    };

    this.updateList = function () {
        if (this.dhtmlx().first() != undefined) {
            this.dhtmlx().clearAll();
        }
        popupntfc.ChangeText("Loading jobs...");
        //popupntfc.ShowPopup();
        this.dhtmlx().load(this.proxy, "json",function () {
            popupntfc.HidePopup();
        });
    };
}

function JobListController() {
    this.Initialization = function () {
        dp2 = new dhx.DataProcessor({
            master: $$('p_view_3'),
            url: job_list.proxy
        });

        directionsDisplay.setMap(locScreen.gmap());
        $$("loc_lbl_Address").blockEvent();
        this.AttachEvents();

        locScreen.AttachEvents();
    };

    this.AttachEvents = function () {
        job_list.dhtmlx().attachEvent("onitemclick", function (id) {
            $$("p_title").setValue("Location");
            datastory.put("id_job", id);
            job_list.dhtmlx().unselectAll();
            //TODO here put constructor for location screen
            $$("loc_lbl_Address").setValue(job_list.dhtmlx().item(id).Address);
            $$("loc_richselect").blockEvent();
            $$("loc_richselect").setValue(job_list.dhtmlx().item(id).travelingStatusId);
            $$("loc_richselect").unblockEvent();
            $$("loc_btn_back").show();
            $$("loc_control_button_2").show();
            $$("p_view_5").show();
        });
        job_list.dhtmlx().unselectAll();

    };
}

function SetupScreen() {
    this.id = "p_view_4";
    this.view = { view: 'layout', type: 'clean', 
        rows: [
            { view: 'layout', type: 'clean', id: 'p_view_4_layout1', 
                cols: [
                    { view: 'form', scroll: 0,
                        elements: [
                            { view: 'label', label: 'CompanyCodeID', popup: '', click: '', id: 'p_view4_lblFrID' }
                        ], id: 'p_view_4_form_1'
                    },
                    { view: 'form', scroll: true,
                        elements: [
                            { view: 'label', label: '51', id: 'p_view4_lblFrIDInfo', align: 'right' },
                            { view: 'label', label: '11', id: 'p_view4_lblTaIDInfo', align: 'right' }
                        ], id: 'p_view_4_form_2'
                    }
                ]
            },
            { view: 'label', label: '', popup: '', click: '', id: 'control_label_6' },
            { view: 'label', label: '', popup: '', click: '', id: 'control_label_7' }           
        ], id: this.id
    };
    
    this.setup_list_dhtmlx = function() {
        return $$("setup_list");
    };
}

function SetupScreenController() {
    this.Initialization = function () {
        //selected_systems_controller.AttachEvents();
        //this.AttachEvents();
    };
    
    this.AttachEvents = function () {
        setup_screen.setup_list_dhtmlx().attachEvent("onafterselect", function (id) {
            switch (id) {
                case "1":
                    {
                        save_selected_systems.dhtmlx().show();
                        break;
                    }
                case "2":
                    {
                        save_video_urls.dhtmlx().show();
                        break;
                    }
                case "3":
                    {
                        save_finance_options.dhtmlx().show();
                        break;
                    }
                case "4":
                    {
                        save_texts_guarantee.dhtmlx().show();
                        break;
                    }
                case "5":
                    {
                        save_images.dhtmlx().show();
                        break;
                    }
                case "6":
                    {
                        save_logo.dhtmlx().show();
                    }
            }
            $$("p_view_back").show();
            setup_screen.setup_list_dhtmlx().unselectAll();
        });
    };

    this.LoadData = function () {
        dhx.ajax().get("../../hvac_app/wizard/GetVideoUrls?systypetext=ALL", null, save_video_urls.SetData);
    };
}

function SaveLogoUrl() {
    this.id = "p_view_11";

    this.view = {
        view: 'form', css: "p_logo",
        scroll: false,
        elements: [
            { view: 'text', label: 'Enter Url for Logo Image: ', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'txtlogoUrl', labelWidth: '180', placeholder: 'Enter url' },
            { view: 'button', label: 'Save', id: 'p_view_11_send', type: 'form', click: 'save_logo.saveUrl()' }
        ],
        id: this.id
    };

    this.saveUrl = function () {
        dhx.ajax().sync().post("../../hvac_app/wizard/SetLogo",
           { url: $$("txtlogoUrl").getValue() }, save_logo.afterSetLogo);
    };

    this.afterSetLogo = function () {
        for (var i = 0; i < document.styleSheets.length; ++i) {
            if (document.styleSheets[i].href != null)
                if (document.styleSheets[i].href.indexOf("site.css") != -1)
                    document.styleSheets[i].cssRules[3].style.backgroundImage = "url('" + $$("txtlogoUrl").getValue() + "')";
        }
    };

    this.loadLogoImage = function() {
        dhx.ajax().post("../../hvac_app/wizard/GetLogo",
            null, save_logo.afterGetLogo);
    };

    this.afterGetLogo = function (data) {
        var logo = dhx.DataDriver.json.toObject(data).url;
        $$("txtlogoUrl").setValue(logo);
        save_logo.afterSetLogo();
    };
}

SaveLogoUrl.prototype = proto_dhtmlx;

function SaveVideoUrls() {
    this.id = "p_view_6";
    this.view = { view: 'form', scroll: false,
        elements: [
            { view: 'text', label: 'YouTube Code of Video for SE:', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'UrlVideoSE', labelWidth: '255', placeholder: 'Enter shot code of youtube video' },
            { view: 'text', label: 'YouTube Code of Video for ME:', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'UrlVideoME', labelWidth: '255', placeholder: 'Enter shot code of youtube video' },
            { view: 'text', label: 'YouTube Code of Video for LE:', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'UrlVideoLE', labelWidth: '255', placeholder: 'Enter shot code of youtube video' },
            { view: 'text', label: 'YouTube Code of Video for UE:', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'UrlVideoUE', labelWidth: '255', placeholder: 'Enter shot code of youtube video' },
            { view: 'text', label: 'YouTube Code of Video for HE:', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'UrlVideoHE', labelWidth: '255', placeholder: 'Enter shot code of youtube video' },
            { view: 'button', label: 'Save', id: 'p_view_6_send', type: 'form', click: 'SaveUrls' }
        ], id: this.id
    };

    this.SaveUrls = function() {
        dhx.ajax().post("../../hvac_app/wizard/changevideourls", { se: $$("UrlVideoSE").getValue(), me: $$("UrlVideoME").getValue(), le: $$("UrlVideoLE").getValue(), ue:$$("UrlVideoUE").getValue(), he:$$("UrlVideoHE").getValue()}, save_video_urls.afterChangeUrls);
    };

    this.SetData = function (obj) {
        var data = dhx.DataDriver.json.toObject(obj);
        for (i in data) {
            switch (data[i].sys) {
                case "SE":
                    {
                        $$("UrlVideoSE").setValue(data[i].url);
                        break;
                    }
                case "ME":
                    {
                        $$("UrlVideoME").setValue(data[i].url);
                        break;
                    }
                case "LE":
                    {
                        $$("UrlVideoLE").setValue(data[i].url);
                        break;
                    }
                case "HE":
                    {
                        $$("UrlVideoHE").setValue(data[i].url);
                        break;
                    }
                case "UE":
                    {
                        $$("UrlVideoUE").setValue(data[i].url);
                        break;
                    }
                default:
            }
        }

    };

    this.afterChangeUrls = function () {
        dhx.notice({ delay: 750, message: "Saved" });
    };
}
SaveVideoUrls.prototype = proto_dhtmlx;

function SaveFinanceOptions() {
    this.id = "p_view_7";
    this.view = {
        view: 'layout',
        type: 'clean',
        rows: [
            {
                view: 'form',
                scroll: 1,
                elements: [
                    { view: 'label', label: 'Finance Options for SE', id: 'se_lbl' },
                    { view: 'text', label: 'Option #1', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'se_txt_op1' },
                    { view: 'text', label: 'Option #2', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'se_txt_op2' },
                    { view: 'text', label: 'Option #3', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'se_txt_op3' },
					{ view: 'label', label: 'Finance Options for ME', id: 'me_lbl' },
					{ view: 'text', label: 'Option #1', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'me_txt_op1' },
					{ view: 'text', label: 'Option #2', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'me_txt_op2' },
					{ view: 'text', label: 'Option #3', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'me_txt_op3' },
					{ view: 'label', label: 'Finance Options for LE', id: 'le_lbl' },
					{ view: 'text', label: 'Option #1', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'le_txt_op1' },
					{ view: 'text', label: 'Option #2', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'le_txt_op2' },
					{ view: 'text', label: 'Option #3', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'le_txt_op3' },
                	{ view: 'label', label: 'Finance Options for UE', id: 'ue_lbl' },
					{ view: 'text', label: 'Option #1', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'ue_txt_op1' },
					{ view: 'text', label: 'Option #2', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'ue_txt_op2' },
					{ view: 'text', label: 'Option #3', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'ue_txt_op3' },
                    { view: 'label', label: 'Finance Options for HE', id: 'he_lbl' },
					{ view: 'text', label: 'Option #1', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'he_txt_op1' },
					{ view: 'text', label: 'Option #2', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'he_txt_op2' },
					{ view: 'text', label: 'Option #3', placeholder: 'Enter option', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'he_txt_op3' }
				], id: 'p_view7_form'
            },
            { view: 'button', label: 'Save', id: 'p_view_7_send', type: 'form', click: 'SaveOptions' }

        ],
        id: this.id
    };

    this.SaveOptions = function () {
        dhx.ajax().post("../../hvac_app/wizard/changefinanceoptions",
            $$("p_view7_form").getValues(), save_finance_options.afterchangeOptions);
    };

    this.afterchangeOptions = function() {
        dhx.notice({ delay: 750, message: "Saved" });
    };
}

SaveFinanceOptions.prototype = proto_dhtmlx;

function FinanceOptionsController() {
    this.data;
    this.LoadData = function() {
        dhx.ajax().post("../../hvac_app/wizard/GetFinanceOptions",
            null, this.afterLoadedOptions);
    };
    this.afterLoadedOptions = function (loadeddata) {
        finance_options_controller.data = dhx.DataDriver.json.toObject(loadeddata);
        for (i in finance_options_controller.data) {
            if (finance_options_controller.data[i].sys == "LE") {
                $$("le_txt_op1").setValue(finance_options_controller.data[i].FinanceOption1);
                $$("le_txt_op2").setValue(finance_options_controller.data[i].FinanceOption2);
                $$("le_txt_op3").setValue(finance_options_controller.data[i].FinanceOption3);
            }
            if (finance_options_controller.data[i].sys == "ME") {
                $$("me_txt_op1").setValue(finance_options_controller.data[i].FinanceOption1);
                $$("me_txt_op2").setValue(finance_options_controller.data[i].FinanceOption2);
                $$("me_txt_op3").setValue(finance_options_controller.data[i].FinanceOption3);
            }
            if (finance_options_controller.data[i].sys == "SE") {
                $$("se_txt_op1").setValue(finance_options_controller.data[i].FinanceOption1);
                $$("se_txt_op2").setValue(finance_options_controller.data[i].FinanceOption2);
                $$("se_txt_op3").setValue(finance_options_controller.data[i].FinanceOption3);
            }
            if (finance_options_controller.data[i].sys == "UE") {
                $$("ue_txt_op1").setValue(finance_options_controller.data[i].FinanceOption1);
                $$("ue_txt_op2").setValue(finance_options_controller.data[i].FinanceOption2);
                $$("ue_txt_op3").setValue(finance_options_controller.data[i].FinanceOption3);
            }
            if (finance_options_controller.data[i].sys == "HE") {
                $$("he_txt_op1").setValue(finance_options_controller.data[i].FinanceOption1);
                $$("he_txt_op2").setValue(finance_options_controller.data[i].FinanceOption2);
                $$("he_txt_op3").setValue(finance_options_controller.data[i].FinanceOption3);
            }
        }
    };
}

function SetupSystems() {
    this.id = "p_view_8";

    this.urlForNonSelected = '../../hvac_app/wizard/getforchoosesystem';
    this.urlForSelected = '../../hvac_app/wizard/GetSelectedSystems';

    this.view = {
        view: 'layout',
        type: 'clean',
        rows: [
            {
                view: 'layout',
                type: 'wide',
                id: 'layout_3',
                cols: [
                    { view: 'view', id: 'temp_designer_view_9' },
                    {
                        view: 'layout',
                        type: 'wide',
                        rows: [
                            { view: 'label', label: 'Not Selected', align: 'center', id: 'lblNotSelected' },
                            {
                                view: 'list',
                                type: {
                                    width: 'auto',
                                    height: 22,
                                    margin: 0,
                                    padding: 10,
                                    template: '<div class="menu_item">#text#</div><div class="corner"></div>'
                                },
                                scroll: true,
                                select: true,
                                id: 'p_view8_list_to_choose',
                                datatype: 'json',
                                data: []
                            }]
                    },
                    {
                        view: 'layout',
                        type: 'wide',
                        rows: [
                            { view: 'label', label: 'Selected', align:'center', id: 'lblSelected' },
                            {
                                view: 'list',
                                type: {
                                    width: 'auto',
                                    height: 22,
                                    margin: 0,
                                    padding: 10,
                                    template: '<div class="corner_back">&nbsp</div><div class="menu_item">#text#</div>'
                                },
                                scroll: true,
                                select: true,
                                id: 'p_view8_selected_list',
                                datatype: 'json',
                                data:[]
                                //url: '../../hvac_app/wizard/GetSelectedSystems'
                            }
                        ]
                    },
                    { view: 'view', id: 'temp_designer_view_11' }
                ]
            },
            { view: 'button', label: 'Save', popup: '', click: 'save_selected_systems.saveSelected', css: '', id: 'p_view_8_btnSave' },
            { view: 'view', id: 'temp_designer_view_4' }
        ],
        id: this.id
    };


    this.oldlist = function() { return $$("p_view8_list_to_choose"); };
    this.newlist = function () { return $$("p_view8_selected_list"); };

    this.saveSelected = function () {
        var newlist = $$("p_view8_selected_list");
        var systems = newlist.serialize();
        var forsend=new Array();
        for (var i in systems) {
            forsend[i] = systems[i].text;
        }
        dhx.ajax().sync().post("../../hvac_app/wizard/SetSystems",
            forsend, save_selected_systems.afterSetSystems);
    };

    this.afterSetSystems = function () {
        dhx.notice({ delay: 750, message: "Saved" });
    };

    this.MoveFromOldToNew = function (id) {
        var oldList = $$("p_view8_list_to_choose");
        var newlist = $$("p_view8_selected_list");
        var item = oldList.item(id);
        newlist.blockEvent();
        oldList.blockEvent();
        oldList.remove(id);
        newlist.add(item);
        oldList.unselectAll();
        newlist.unblockEvent();
        oldList.unblockEvent();
    };

    this.MoveFromNewToOld = function(id) {
        var oldList = $$("p_view8_list_to_choose");
        var newlist = $$("p_view8_selected_list");
        var item = newlist.item(id);
        newlist.remove(id);
        oldList.blockEvent();
        oldList.add(item);
        newlist.unselectAll();
        oldList.unblockEvent();
    };
}

SetupSystems.prototype = proto_dhtmlx;

function SelectSystemController() {
    this.AttachEvents = function () {
        var oldList = $$("p_view8_list_to_choose");
        var newlist = $$("p_view8_selected_list");
        oldList.attachEvent("onafterselect", save_selected_systems.MoveFromOldToNew);
        newlist.attachEvent("onafterselect", save_selected_systems.MoveFromNewToOld);
    };

    this.LoadSystems = function () {
        if (save_selected_systems.oldlist().first() != undefined) {
            save_selected_systems.oldlist().clearAll();
        }
        if (save_selected_systems.newlist().first() != undefined) {
            save_selected_systems.newlist().clearAll();
        }
        save_selected_systems.oldlist().load(save_selected_systems.urlForNonSelected);
        save_selected_systems.newlist().load(save_selected_systems.urlForSelected);
    };
}

function SaveTextScreens() {
    this.id = "p_view9";

    this.view =
        {
            view: 'layout',
            type: 'clean',
            rows: [
                { view: 'label', label: 'Please, select Guarantee to change text', popup: '', click: '', id: 'p_view9_lblHeadre', align: 'center' },
                {
                    view: "scrollview",
                    id: "garantee_scroll",
                    scroll: "x",
                    content:
                        {
                            view: 'segmented',
                            options: [
                                { label: 'Comfort Guarantee', value: 'p_view9_v1', id: 'p_view9_v1' },
                                { label: 'Lemon Free', value: 'p_view9_v2', id: 'p_view9_v2' },
                                { label: 'Never Undersold', value: 'p_view9_v3', id: 'p_view9_v3' },
                                { label: 'Home Respect', value: 'p_view9_v4', id: 'p_view9_v4' },
                                { label: 'Complete Satisfaction', value: 'p_view9_v5', id: 'p_view9_v5' },
                                { label: 'No Paperwork Promise', value: 'p_view9_v6', id: 'p_view9_v6' },
                                { label: 'No Surprises', value: 'p_view9_v7', id: 'p_view9_v7' }
                            ],
                            id: 'p_view9_segmented',
                            multiview: 1,
                            width: 1010,
                            align: 'center'
                        }
                },
            //                {
            //                    view: 'segmented',
            //                    options: [
            //                        { label: 'Comfort Guarantee', value: 'p_view9_v1', id: 'p_view9_v1' },
            //                        { label: 'Lemon Free', value: 'p_view9_v2', id: 'p_view9_v2' },
            //                        { label: 'Never Undersold', value: 'p_view9_v3', id: 'p_view9_v3' },
            //                        { label: 'Home Respect', value: 'p_view9_v4', id: 'p_view9_v4' },
            //                        { label: 'Complete Satisfaction', value: 'p_view9_v5', id: 'p_view9_v5' },
            //                        { label: 'No Paperwork Promise', value: 'p_view9_v6', id: 'p_view9_v6' },
            //                        { label: 'No Surprises', value: 'p_view9_v7', id: 'p_view9_v7' }
            //                    ],
            //                    id: 'p_view9_segmented',
            //                    multiview: 1,
            //                    align: 'center'
            //                },
                {
                    view: 'multiview',
                    type: 'clean',
                    cells: [
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v1_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v1_txt")', css: '', id: 'p_view9_v1_btnSave' }
                            ],
                            id: 'p_view9_v1'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v2_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v2_txt")', css: '', id: 'p_view9_v2_btnSave' }
                            ],
                            id: 'p_view9_v2'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v3_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v3_txt")', css: '', id: 'p_view9_v3_btnSave' }
                            ],
                            id: 'p_view9_v3'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v4_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v4_txt")', css: '', id: 'p_view9_v4_btnSave' }
                            ],
                            id: 'p_view9_v4'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v5_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v5_txt")', css: '', id: 'p_view9_v5_btnSave' }
                            ],
                            id: 'p_view9_v5'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v6_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v6_txt")', css: '', id: 'p_view9_v6_btnSave' }
                            ],
                            id: 'p_view9_v6'
                        },
                        {
                            view: 'form',
                            scroll: false,
                            elements: [
                                { view: 'textarea', label: 'Enter Guarantee Text', id: 'p_view9_v7_txt', inputHeight: '285', height: 300 },
                                { view: 'button', label: 'Save', popup: '', click: 'guaranteetexts_controller.saveGuarantee("p_view9_v7_txt")', css: '', id: 'p_view9_v7_btnSave' }
                            ],
                            id: 'p_view9_v7'
                        }
                    ],
                    id: 'p_view9_multiview'
                }
            ],
            id: this.id
        };
}

SaveTextScreens.prototype = proto_dhtmlx;

function GuaranteeTextsController() {
    this.LoadGuaranteeTexts = function(func) {
        dhx.ajax().post("../../hvac_app/wizard/GetGuaranteeTexts",
            null, func);
    };

    this.afterGetGuaranteeForSetup = function (data) {
        var dt = dhx.DataDriver.json.toObject(data);
        $$("p_view9_v1_txt").setValue(dt.cg);
        $$("p_view9_v2_txt").setValue(dt.lfg);
        $$("p_view9_v3_txt").setValue(dt.nug);
        $$("p_view9_v4_txt").setValue(dt.hrg);
        $$("p_view9_v5_txt").setValue(dt.csg);
        $$("p_view9_v6_txt").setValue(dt.npp);
        $$("p_view9_v7_txt").setValue(dt.ns);
    };

    this.afterGetGuaranteeTexts = function (data) {
        var dt = dhx.DataDriver.json.toObject(data);
        //26 - Comfort Guarantee (cg)
        //27 - Lemon Free Guarantee (lfg)
        //28 - Never Undersold Guarantee (nug)
        //29 - Home Respect Guarantee (hrg)
        //30 - Complete Satisfaction Guarantee (csg)
            view26.setGuaranteeText(dt.cg);
            view27.setGuaranteeText(dt.lfg);
            view28.setGuaranteeText(dt.nug);
            view29.setGuaranteeText(dt.hrg);
            view30.setGuaranteeText(dt.csg);
            view36.setGuaranteeText(dt.npp);
            view37.setGuaranteeText(dt.ns);
    };

    this.saveGuarantee = function (idtextarea) {
        var idtext = 0;
        switch (idtextarea) {
            case "p_view9_v1_txt":
                {
                    idtext = 26;
                    break;
                }
            case "p_view9_v2_txt":
                {
                    idtext = 27;
                    break;
                }
            case "p_view9_v3_txt":
                {
                    idtext = 28;
                    break;
                }
            case "p_view9_v4_txt":
                {
                    idtext = 29;
                    break;
                }
            case "p_view9_v5_txt":
                {
                    idtext = 30;
                    break;
                }
            case "p_view9_v6_txt":
                {
                    idtext = 36;
                    break;
                }
            case "p_view9_v7_txt":
                {
                    idtext = 37;
                    break;
                }
            default:
        }
        dhx.ajax().sync().post("../../hvac_app/wizard/SetGuaranteeTexts",
            {id: idtext, text: $$(idtextarea).getValue()}, save_texts_guarantee.afterSetSystems);
    };

    this.afterSetSystems = function() {
        dhx.notice({ delay: 2000, message: "Saved" });
    };
}

function SaveImages() {
    this.id = "p_view10";

    this.view = {
        view: 'layout',
        type: 'wide',
        rows: [
            {
                view: 'form',
                scroll: 0,
                elements: [
                    { view: 'label', label: 'Reliable Installations screen', id: 'p_view10_lblRI' },
                    { view: 'text', label: 'YouTube Code of Video ', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'p_view10_txtRI', placeholder: 'Enter shot code of youtube video' },
                    { view: 'button', label: 'Save', id: 'p_view10_btnSaveRI', type: 'round', click: 'save_images.SaveData("ri")' }
                ],
                id: 'p_view10_formRI'
            },
            {
                view: 'form',
                scroll: 0,
                elements: [
                    { view: 'label', label: 'Who We Are screen', id: 'p_view10_lblWWA' },
                    { view: 'text', label: 'Url #1', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'p_view10_txtWWA1' },
                    { view: 'text', label: 'Url #2', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'p_view10_txtWWA2' },
                    { view: 'text', label: 'Url #3', value: '', labelPosition: 'left', labelAlign: 'left', type: 'text', id: 'p_view10_txtWWA3' },
                    { view: 'button', label: 'Save', id: 'p_view10_btnSaveWWA', type: 'round', click: 'save_images.SaveData("wwa")' }
                ],
                id: 'p_view10_formWWA'
            }
        ],
        id: this.id
    };

    this.LoadData = function (func) {
        dhx.ajax().post("../../hvac_app/wizard/GetImagesUrl",
            null, func);
    };

    this.afterLoadToSetup = function (data) {
        var dt = dhx.DataDriver.json.toObject(data);
        $$("p_view10_txtRI").setValue(dt.ri.Urls[0]);
        for (var i in dt.wwa.Urls) {
            var iy = i;
            $$("p_view10_txtWWA" + (++iy)).setValue(dt.wwa.Urls[i]);
        }
    };

    this.afterLoadForApp = function (data) {
        var dt = dhx.DataDriver.json.toObject(data);
        $$("view24_tmpl").data = { url: dt.ri.Urls[0] };
        $$("view24_tmpl").refresh();
        $$("view25_lbl").setValue(dt.wwa.Urls[0]);
        $$("view25_lbl").refresh();
//        $$("view25control_lbl_img2").data = { urlText: dt.wwa.Urls[1] };
//        $$("view25control_lbl_img2").refresh();
//        $$("view25control_lbl_img3").data = { urlText: dt.wwa.Urls[2] };
//        $$("view25control_lbl_img3").refresh();
    };

    this.SaveData = function (param) {
        var data;
        if (param == "ri") {
            data = $$("p_view10_formRI").getValues();
            
        }
        if (param == "wwa") {
            data = $$("p_view10_formWWA").getValues();
        }
        dhx.ajax().post("../../hvac_app/wizard/SetImageUrls?form=" + param,
            data, save_images.afterSave);
    };

    this.afterSave = function () {
        dhx.notice({ delay: 750, message: "Saved" });
    };
}

SaveImages.prototype = proto_dhtmlx;

function LeftMenuController() {
    this.Initialization = function () {
        this.AttachEvents();
    };

    this.AttachEvents = function () {
	$$("p_list_menu").select($$("p_list_menu").first());
        $$("p_list_menu").attachEvent("onafterselect", function (id) {
            $$("p_title").setValue($$("p_list_menu").item(id).text);
            $$("p_view_" + id).show(); //this is we here for. MULTIVIEW will be switched to the cell with form view
            $$("loc_btn_back").hide();
            $$("p_view_back").hide();
            $$("loc_control_button_2").hide();
            return true;
        });
        $$("p_list_menu").attachEvent("onitemclick", function(id) {
            $$("p_title").setValue($$("p_list_menu").item(id).text);
            $$("p_view_" + id).show(); //this is we here for. MULTIVIEW will be switched to the cell with form view
            $$("loc_btn_back").hide();
            $$("p_view_back").hide();
            $$("loc_control_button_2").hide();
            return true;
        });
    };
}

var locScreen = new LocationScreen();

var leftMenu = new LeftMenuController();

var job_list = new JobList();
var joblist_controller = new JobListController();

var setup_screen = new SetupScreen();
var setup_screen_controller = new SetupScreenController();

var save_video_urls = new SaveVideoUrls();

var save_finance_options = new SaveFinanceOptions();
var finance_options_controller = new FinanceOptionsController();

var save_selected_systems = new SetupSystems();
var selected_systems_controller = new SelectSystemController();

var save_texts_guarantee = new SaveTextScreens();
var guaranteetexts_controller = new GuaranteeTextsController();

var save_logo = new SaveLogoUrl();

var save_images = new SaveImages();


function SaveUrls() {
    save_video_urls.SaveUrls();
}

function SaveOptions() {
    save_finance_options.SaveOptions();
}

var ui_HVAC_Preparation = { view: 'layout', type: 'wide', id: 'preparation_app',
    cols: [
        { view: 'layout', type: 'wide', width: 275,
            rows: [
                { view: 'toolbar', type: 'MainBar',
                    elements: [
                        { view: 'button', label: 'Logout', id: 'p_btn_logout', click: 'logout', inputWidth: '120', align: 'left' }
                    ], id: 'p_menu_toolbar'
                },
                { view: 'list', css: 'cust_list_item',
                    type: { width: 'auto',
                        height: 'auto',
                        padding: 10,
                        align: 'left',
                        template: '<div class="menu_item">#text#</div><div class="itemlist_img_not_answered" ></div>'
                    },
                    select: true,
                    multiview: true,
                    id: 'p_list_menu',
                    align: 'left',
                    data: [
    { id: "1", text: "Home" },
    { id: "2", text: "Send/Receive" },
    { id: "3", text: "Job List" },
    { id: "4", text: "Setup" }
],
                    datatype: "json"
                }
            ], id: 'left_layout'
        },
        { view: 'layout', type: 'wide', width: 724,
            rows: [
                { view: 'toolbar', type: 'MainBar',
                    elements: [
                        { view: 'layout', type: 'clean',
                            rows: [
                                { view: 'button', label: 'Back', id: 'p_view_back', type: 'prev', click: 'backToSetupScreen()', hidden: 1 },
                                { view: 'button', label: 'Back to Job List', id: 'loc_btn_back', type: 'prev', click: 'backTojobList()', hidden: 1 }
                            ], id: 'layout_4', width: 200
                        },
                        { view: 'label', id: 'p_title', label: '', align: 'center', height: 42 },
                        { view: 'layout', type: 'clean',
                            rows: [
                               { view: 'button', label: 'Launch Application', id: 'loc_control_button_2', type: 'next', click: 'launchApp', hidden:1 }
                            ], id: 'layout_4', width: 200
                        }
                    ], id: 'p_toolbar_2'
                },
                {  view: 'multiview', type: 'wide', id: "p_multiView2",
                    cells: [
                        { view: 'view', id: 'p_view_1', css: "p_logo" },
						{ view: 'button', 
						    id: 'p_view_2',
						    label: 'Update Job List',
						    type: 'round',
						    inputWidth: '185',
						    width: 200,
						    align: 'center',
						    css: "p_logo",
						    click: "job_list.updateList()"
						},
						job_list.view,
                        locScreen.view, 
                        setup_screen.view,
                        save_logo.view,
                        save_video_urls.view,
                        save_finance_options.view,
                        save_selected_systems.view,
                        save_texts_guarantee.view,
                        save_images.view
                    ]}
            ]
        }
    ]
};
