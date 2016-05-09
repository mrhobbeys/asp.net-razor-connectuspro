function ProtoDHTMLX() {
    this.id;
    this.dhtmlx = function () {
        return $$(this.id);
    };    

    this.idNum = function() {
        return this.id.replace("view", "");
    };
}
var proto_dhtmlx = new ProtoDHTMLX();

function GuaranteeScreen() {
    this.id;
    this.getGuaranteeText = function () {
        if ($$(this.id + "_lbl") != undefined) {
            return $$(this.id + "_lbl").config.label;
        }
    };
    this.setGuaranteeText = function (text) {
        if ($$(this.id + "_lbl") != undefined) {
            $$(this.id + "_lbl").config.label = text;
            $$(this.id + "_lbl").refresh();
        }
    };
}

GuaranteeScreen.prototype = proto_dhtmlx;

var proto_guarantee = new GuaranteeScreen();

var p_list_items = [
    { id: "1", text: "Home" },
    { id: "2", text: "Send/Receive" },
    { id: "3", text: "Job List" },
    { id: "4", text: "Setup" }
];

var source_list;
var datastory;
var timeLoadingUpdate;

function SystemSize() {
    this.loss;
    this.gain;
    this.toString = function () {
        return "{ gain: " + Globalize.parseFloat(this.gain.toString()) + ", loss: " + Globalize.parseFloat(this.loss.toString()) + "}";
    };
    this.toObject = function(text) {
        if (text != undefined) {
            return dhx.DataDriver.json.toObject(text);
        }
        var size = new SystemSize();
        size.loss = 40000;
        size.gain = 1.5;
        return size;
    };
}

function Ratings() {
    this.electric;
    this.gas;
    this.toString = function () {
        return JSON.stringify(this);
        //return "{ electric: " + Globalize.parseFloat(this.electric) + ", gas: " + Globalize.parseFloat(this.gas) + "}";
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function System() {
    this.name;
    this.seer_rating;
    this.id;
    this.system;
    this.toString = function () {
        return JSON.stringify(this).replace(/<br\/>/g, "\\n"); 
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function Savings(cs,ghs,hp) {
    this.cs = cs;
    this.ghs = ghs;
    this.hp = hp;
    this.Summury = function() {
        return Globalize.parseFloat(this.cs.calculated_value) + Globalize.parseFloat(this.ghs.calculated_value) + Globalize.parseFloat(this.hp.calculated_value);
    };
    this.toString = function () {
        return JSON.stringify(this);
        //return "{ cs: " + this.cs.toString() + ", ghs: " + this.ghs.toString() + ", hp: " + this.hp.toString() + "}";
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function CoolingSavings() {
    this.seer_rating_exist;
    this.btu_capacity_exist;
    this.average_cooling_hours;
    this.calculated_value;
    this.toString = function () {
        return JSON.stringify(this);
        //return "{ seer_rating_exist: " + this.seer_rating_exist + ", btu_capacity_exist: " + this.btu_capacity_exist + ", average_cooling_hours: " + this.average_cooling_hours + ", calculated_value: " + this.calculated_value + "}";
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function GasHeatSavings() {
    this.afue_exist;
    this.afue_selected;
    this.average_heating_hours;
    this.calculated_value;
    this.toString = function () {
        return JSON.stringify(this);
        //return "{ afue_exist: " + this.afue_exist + ", afue_selected: " + this.afue_selected + ", average_heating_hours: " + this.average_heating_hours + ", calculated_value: " + this.calculated_value + "}";
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function HeatPumpHeatingSavings() {
    this.hspf_exist;
    this.btu_capacity_exist;
    this.hspf_selected;
    this.average_cooling_hours;
    this.calculated_value;
    this.toString = function () {
        return JSON.stringify(this);
        //return "{ hspf_exist: " + this.hspf_exist + ", btu_capacity_exist: " + this.btu_capacity_exist + ", hspf_selected: " + this.hspf_selected + ", average_cooling_hours: " + this.average_cooling_hours + ", calculated_value: " + this.calculated_value + "}";
    };
    this.toObject = function (text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function CostSystem() {
    this.remaing = 0;
    this.saving = 0;
    this.repears = 0;
    this.inflation = 0;
    this.subtotal = function() {
        return this.saving * this.remaing + this.repears;
    };
    this.total_value = 0;
    this.CalculateTotalValue = function() {
        this.total_value = this.subtotal() * (1 + this.inflation);
        return;
    };

    this.toString = function() {
        return JSON.stringify(this);
        //return "{ saving: " + this.saving + ", remaing: " + this.remaing + ", repears: " + this.repears + ", inflation: " + this.inflation + ", total_value: " + this.total_value + "}";
    };
    this.toObject = function(text) {
        return dhx.DataDriver.json.toObject(text);
    };
}

function PriceList() {
    this.Jobs = [];
    this.totalAmount;
    this.tax;
    this.grandTotal;

    this.toString = function () {
        return JSON.stringify(this);
    };
}
