var directionsDisplay;
var directionsService = new google.maps.DirectionsService();

dhx.ready(function () {
    if (!String.prototype.trim) {
        String.prototype.trim = function () {
            return jQuery.trim(this);
        };
    }

    directionsDisplay = new google.maps.DirectionsRenderer();

    dhx.ui.fullScreen();
    window.scrollTo(0, 1);

    arraylist = [view0];
    //    arraylist = [view0, view1, view2, view3, view5,
    //        view6, view7, view8, view9, view34, view10, view11,
    //        view12, view13, view14, view15, view16, view17,
    //        view18, view19, view20, view21, view22, view23,
    //        view24, view25, view26, view27, view28, view29,
    //         view36, view37, view30, view31, view35, view32, view33];

    arraylistView = [view0.view];
    //    arraylistView = [view0.view, view1.view, view2.view, view3.view, view5.view,
    //        view6.view, view7.view, view8.view, view9.view, view34.view, view10.view, view11.view,
    //        view12.view, view13.view, view14.view, view15.view, view16.view, view17.view,
    //        view18.view, view19.view, view20.view, view21.view, view22.view, view23.view,
    //        view24.view, view25.view, view26.view, view27.view, view28.view, view29.view,
    //        view36.view, view37.view, view30.view, view31.view, view35.view, view32.view, view33.view];

    datastory = dhx.storage.cookie;

    dhx.ajax().sync().post("../../hvac_app/hvacconfig/GetListOfQuestion", null, {
        error: function (text, xml, XmlHttpRequest) {
            alert("Error in loading list of questions");
        },
        success: afterGetListOfQuestions
    });

   
});
    
function afterGetListOfQuestions(data) {
    var dt = dhx.DataDriver.json.toObject(data);
    if (dt.length > 0) {
        var arrayListString = "arraylist = [";
        var arrayListViewString = "arraylistView = [";
        for (var i = 0; i < dt.length - 1; i++) {
            arrayListString += "view" + dt[i].Id + ", ";
            arrayListViewString += "view" + dt[i].Id + ".view, ";
        }
        arrayListString += "view" + dt[dt.length - 1].Id + " ];";
        arrayListViewString += "view" + dt[dt.length - 1].Id + ".view ]; ";
        eval(arrayListString);
        eval(arrayListViewString);
    }
    dhx.ui(popupntfc.view).hide();
    dhx.ui(popup_settings.view).hide();
    dhx.ui(winAS.view).hide();
    dhx.ui(winTax.view).hide();
    dhx.ui(winFinOpt.view).hide();
    dhx.ui(winCompleteJob.view).hide();

    ui_HVAC_Questions.cols[1].rows[1].cells = arraylistView;
    dhx.ui({ id: 'app', view: 'layout', height: 758, width: 1024,
        rows: [
                { view: 'multiview', type: 'wide',
                    cells: [
                        ui_HVAC_Login,
                        ui_HVAC_Preparation,
                        ui_HVAC_Questions
                    ], id: 'Main_MultiView'
                }
            ]
    });
    save_logo.loadLogoImage();
    
    if (dt.length > 0) {
        leftMenu.Initialization();
        joblist_controller.Initialization();
        setup_screen_controller.Initialization();

        //hvac questions part
        listQuestions.Initialization();
    }		
    dhx.ajax().post("../../hvac_app/account/islogged", null, islogged);

    
}