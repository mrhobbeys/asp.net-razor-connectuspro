using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Plumber.Models
{
    public class OfferModel
    {

        public int OfferId { get; set; }

        public string Title { get; set; }

        public string SubTitle { get; set; }

        public string Content { get; set; }

        public List<OfferModel> GetOffers()
        {
            return new List<OfferModel>(){
                new OfferModel(){ OfferId = 1,
                    Title = "<strong>$100 Off Heating &amp; Air Conditioning System Replacement</strong>",
                    SubTitle = "The Long Summer Can Wear Out Your System",
                    Content = @"<p>
                                    If the hot Texas summer has taken its toll on your air conditioning system, call
                                    <strong>1-800-PLUMBER</strong> and receive $100 off any new home comfort replacement
                                    system.
                                </p>
                                <p>
                                    Not only will your house be more comfortable, but today&rsquo;s newer systems can
                                    save you hundreds on your utility bills.
                                </p>
                                <p>
                                    Ask your technician for more information or call us at <strong>1-800-PLUMBER</strong>
                                    today to book your appointment.
                                </p>"},                    
                new OfferModel(){ OfferId = 2,
                    Title = "Water Heater Special",
                    SubTitle = "$100.00 Trade In!",
                    Content = @"<p>
                                    Now is the time to replace that aging water heater. <strong>1-800-PLUMBER</strong>
                                    is now offering a <strong>$100.00</strong> trade in for your old water heater!
                                </p>
                                <p>
                                    Hot water usage always increases in the fall and winter. That old heater will fail
                                    you at the worst possible time. A cold shower is bad, but flooded carpeting is worse.
                                </p>
                                <p>
                                    Take advantage of this offer today and receive a <strong>$100.00 trade in</strong>
                                    on your old water heater. Plus, we offer a 6 year warranty on the new heater and
                                    2 years on parts and labor.
                                </p>
                                <p>
                                    Call today or schedule your service call on line. <strong>1-800-PLUMBER</strong>
                                    your water heater specialist!
                                </p>"},
                new OfferModel(){ OfferId = 3,
                    Title = "800-BIO Drain Maintenance",
                    SubTitle = "Blockages Stay Away",
                    Content = @"<p>
                                    <strong>800-BIO</strong> will help eliminate drain problems in your home or business!
                                    By removing the fats, oils, and grease you will remove the major cause of a blockage
                                    in your drainage system. It even has the <strong>EPA Seal of Approval</strong>,
                                    so it's safe for you and your pipes!
                                </p>
                                <p>
                                    Ask our technician about <strong>800-BIO</strong> on his next visit or call us today
                                    at <strong>1-800-PLUMBER</strong>.</p>"}
            };
        }

    }
}