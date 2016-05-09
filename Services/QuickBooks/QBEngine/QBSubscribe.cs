using System;
using System.Collections.Generic;
using System.Text;
using QBFC10Lib;
namespace QBEngine
{
    public class QBSubscribe
    {
        // AppID and AppName sent to QuickBooks
public const  string cAppName = "Prism Pay";
static string message;
public const  string cAppID = "";
// SubscriberID should be a unique identifier for this application
//  the value should be obtained by calling guidgen
//Const cSubscriberID = "{7F6CDF64-B862-4d1f-877F-E8E341F1FBA4}"

const  string cSubscriberID = "{7F6CDF64-B862-4d1f-877F-E8E341F1FBC3}";
// Information about the Callback Application: "QBFCEventsCallback"
const string cCallbackAppName = "Prism Pay";
// ProgID is the ProjectName.ClassName

const string cCallbackProgID = "WrapperCOMEXE.MyWrapper";

public static string Subscribe()
{
	QBSessionManager sessMgr = new QBSessionManager();

	//
	// For a subscription request we only need an OpenConnection, no session...
	sessMgr.OpenConnection(cAppID, cAppName);

	//
	// Create a Subscription request
	
    Array array = sessMgr.QBXMLVersionsForSubscription;
    short mjrVersion = (short)double.Parse(array.GetValue(array.Length - 1).ToString());
    short mnrVersion = 0;//double.Parse(array.GetValue(0).ToString());

    ISubscriptionMsgSetRequest subRq = default(ISubscriptionMsgSetRequest);
	subRq = sessMgr.CreateSubscriptionMsgSetRequest(mjrVersion, mnrVersion);

	//
	// Add a UIExtension subscription to our request
	IUIExtensionSubscriptionAdd subAdd = default(IUIExtensionSubscriptionAdd);
	subAdd = subRq.AppendUIExtensionSubscriptionAddRq();

	//
	// set up the subscription request with the required information, we're adding to
	// the file menu in this case, and just for fun, we're making it a cascading menu
	subAdd.SubscriberID.SetValue(cSubscriberID);
	subAdd.COMCallbackInfo.AppName.SetValue(cAppName);
	subAdd.COMCallbackInfo.ORProgCLSID.ProgID.SetValue(cCallbackProgID);
	subAdd.MenuExtensionSubscription.AddToMenu.SetValue(ENAddToMenu.atmBanking);

	//
	// For the cascade fun, we're just going to add items to the cascade menu...
	IMenuItem subMenu = default(IMenuItem);

	subMenu = subAdd.MenuExtensionSubscription.ORMenuSubmenu.Submenu.MenuItemList.Append();
	//
	// this is the text that the user will see in QuickBooks:
	subMenu.MenuText.SetValue("Setup");
	//
	// this is the tag we'll get in our event handler to know which menu item was
	// selected:
	subMenu.EventTag.SetValue("Setup");

	subMenu = subAdd.MenuExtensionSubscription.ORMenuSubmenu.Submenu.MenuItemList.Append();
	//
	// this is the text that the user will see in QuickBooks:
	subMenu.MenuText.SetValue("Process Payment");
	//
	// this is the tag we'll get in our event handler to know which menu item was
	// selected:
	subMenu.EventTag.SetValue("Process Payment");


    subMenu = subAdd.MenuExtensionSubscription.ORMenuSubmenu.Submenu.MenuItemList.Append();
    //
    // this is the text that the user will see in QuickBooks:
    subMenu.MenuText.SetValue("Recurring Payments");
    //
    // this is the tag we'll get in our event handler to know which menu item was
    // selected:
    subMenu.EventTag.SetValue("Recurring Payments");

    subMenu = subAdd.MenuExtensionSubscription.ORMenuSubmenu.Submenu.MenuItemList.Append();
    //
    // this is the text that the user will see in QuickBooks:
    subMenu.MenuText.SetValue("Gateway Sync");
    //
    // this is the tag we'll get in our event handler to know which menu item was
    // selected:
    subMenu.EventTag.SetValue("Gateway Sync");


    //subMenu = subAdd.MenuExtensionSubscription.ORMenuSubmenu.Submenu.MenuItemList.Append();
    ////
    //// this is the text that the user will see in QuickBooks:
    //subMenu.MenuText.SetValue("Transaction Logs");
    ////
    //// this is the tag we'll get in our event handler to know which menu item was
    //// selected:
    //subMenu.EventTag.SetValue("Transaction Logs");

	//
	// Send the request and get the response, since we're sending only one request there
	// will be only one response in the response list
	ISubscriptionMsgSetResponse subRs = default(ISubscriptionMsgSetResponse);
	subRs = sessMgr.DoSubscriptionRequests(subRq);
	IResponse resp = default(IResponse);

	//
	// Check the response and display an appropriate message to the user.
	resp = subRs.ResponseList.GetAt(0);
	if ((resp.StatusCode == 0)) {
        
		//Interaction.MsgBox("Successfully added Prism Pay Plugin to QuickBooks Customer  menu, restart QuickBooks to see results");
	} else {
	//	Interaction.MsgBox("Could not add to QuickBooks menu: " + resp.StatusMessage);
	}

    message = resp.StatusMessage;
	sessMgr.CloseConnection();
	sessMgr = null;


    return message;
	//' Subscribe to events...

	//' Create the outer subscription request XML "envelope"
	//Dim inputXMLDoc As New XmlDocument()
	//inputXMLDoc.AppendChild(inputXMLDoc.CreateXmlDeclaration("1.0", Nothing, Nothing))
	//inputXMLDoc.AppendChild(inputXMLDoc.CreateProcessingInstruction("qbxml", "version=""3.0"""))
	//Dim qbXML As XmlElement = inputXMLDoc.CreateElement("QBXML")
	//inputXMLDoc.AppendChild(qbXML)
	//Dim SubReq As XmlElement = inputXMLDoc.CreateElement("QBXMLSubscriptionMsgsRq")
	//qbXML.AppendChild(SubReq)

	//' Now create the Data Event subscription to find out about customer changes
	//' Note that we will get notifications indirectly if the customer balance changes, etc.
	//' when an invoice is created...
	//Dim DataSubReq As XmlElement = inputXMLDoc.CreateElement("UIExtensionSubscriptionAddRq")
	//SubReq.AppendChild(DataSubReq)
	//Dim DataSubAdd As XmlElement
	//DataSubAdd = inputXMLDoc.CreateElement("UIExtensionSubscriptionAdd")
	//DataSubReq.AppendChild(DataSubAdd)

	//' Note that our SubscriberID is the same as the DataEventApp uses for
	//' the OwnerID of data extensions.  this isn't necessary but it is convenient...
	//AddSimpleElement(inputXMLDoc, DataSubAdd, "SubscriberID", cSubscriberID)

	//' Set up the COMCallback information
	//Dim COMCallback As XmlElement
	//COMCallback = inputXMLDoc.CreateElement("COMCallbackInfo")
	//DataSubAdd.AppendChild(COMCallback)
	//AddSimpleElement(inputXMLDoc, COMCallback, "AppName", cCallbackAppName)

	//' We supply the ProgID, this is the most convenient way to supply callback
	//' information for a COM class written in visual basic which does it's best to
	//' hide information like CLSIDs from the programmer.
	//AddSimpleElement(inputXMLDoc, COMCallback, "ProgID", cCallbackProgID)

	//'
	//' We set to DeliverAlways just to show how we can have a fine granularity of
	//' whether to process events or not by implementing an application-specific event
	//' queue.  This could easily be changed to DeliverOnlyIfRunning and then as soon
	//' as the QBDataEventApp creates an instance of the QBDateEventHandler class to
	//' start tracking events QuickBooks will start delivering the events...
	//'
	//Dim AddMenu As XmlElement
	//AddMenu = inputXMLDoc.CreateElement("MenuExtensionSubscription")
	//DataSubAdd.AppendChild(AddMenu)
	//AddSimpleElement(inputXMLDoc, AddMenu, "AddToMenu", "File")
	//Dim MenuItem As XmlElement
	//MenuItem = inputXMLDoc.CreateElement("MenuItem")
	//AddMenu.AppendChild(MenuItem)
	//AddSimpleElement(inputXMLDoc, MenuItem, "MenuText", "TestDOTNETMenuItem")
	//AddSimpleElement(inputXMLDoc, MenuItem, "EventTag", "TestDOTNETMenuItem")

	//'Finally, send the subscription request to QuickBooks
	//Dim subXML As String
	//subXML = inputXMLDoc.OuterXml
	//MsgBox(subXML)


	//Dim RP As Interop.QBXMLRP2.RequestProcessor2
	//Dim connectionOpened As Boolean = False
	//Try
	//    ' Get the RequestProcessor and open a connection.
	//    RP = New Interop.QBXMLRP2.RequestProcessor2()
	//    RP.OpenConnection(cAppID, cAppName)
	//    connectionOpened = True

	//    Dim resp As String
	//    resp = RP.ProcessSubscription(subXML)

	//    ' We'll just show the response.  Note that if we try to subscribe again we'll get
	//    ' an error.  We're not doing anything else with the response...
	//    MsgBox(Prompt:=resp, Title:="Subscribe Complete")
	//Catch
	//    MessageBox.Show("Error", Err.Description)

	//Finally
	//    If connectionOpened Then
	//        RP.CloseConnection()
	//    End If
	//End Try

}


public static string Unsubscribe()
{
	QBSessionManager sessMgr = default(QBSessionManager);
	sessMgr = new QBSessionManager();
	// Again, we're dealing with subscriptions, which are independent of the company
	// so there is no need to BeginSession, just open the connection.
	sessMgr.OpenConnection("", cAppName);

	// Set up the SubscriptionDel request
    Array array = sessMgr.QBXMLVersionsForSubscription;
    short mjrVersion = (short)double.Parse(array.GetValue(array.Length - 1).ToString());
    short mnrVersion = 0;//double.Parse(array.GetValue(0).ToString());
    

	ISubscriptionMsgSetRequest submsg;
	submsg = sessMgr.CreateSubscriptionMsgSetRequest(mjrVersion, mnrVersion);
	ISubscriptionDel uiextend = default(ISubscriptionDel);
	uiextend = submsg.AppendSubscriptionDelRq();
	uiextend.SubscriberID.SetValue(cSubscriberID);
	uiextend.SubscriptionType.SetValue(ENSubscriptionType.stUIExtension);


	// Send the request
	ISubscriptionMsgSetResponse subresp = default(ISubscriptionMsgSetResponse);
	subresp = sessMgr.DoSubscriptionRequests(submsg);
	IResponse resp = default(IResponse);
	resp = subresp.ResponseList.GetAt(0);

	// Check the result and display an appropriate message to the user
	if ((resp.StatusCode == 0)) {
		//Interaction.MsgBox("Successfully removed from QuickBooks File menu, restart QuickBooks to see results");
	} else {
		//Interaction.MsgBox("Could not remove from QuickBooks menu: " + resp.StatusMessage);
	}
    message = resp.StatusMessage;
	// Close the connection with QuickBooks, we didn't Begin a session so there is
	// no need to EndSession.
	sessMgr.CloseConnection();
	sessMgr = null;

    return message;

}
    }
}
