using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Vauction.Utils;

namespace Vauction.Models.Enums
{
    public static class FieldConfiguration
    {
        public static Field GetFieldData(string ControllerKey, string ActionKey)
        {
            List<Field> data = new List<Field>()
            {
                new Field() { ControllerKey = "Account", ActionKey="LogOn",  LabelText = "Login" },
                new Field() { ControllerKey = "Account", ActionKey="Register",  LabelText = "Registration" },
                 new Field() { ControllerKey = "Home", ActionKey="Product",  LabelText = "Consigner Services" },
                   new Field() { ControllerKey = "Home", ActionKey="AdvancedSearch",  LabelText = "Advanced Search" },
                  new Field() { ControllerKey = "Home", ActionKey="Terms",  LabelText = "Terms &amp; Conditions" },
                  new Field() { ControllerKey = "Home", ActionKey="Privacy",  LabelText = "Privacy" },
                new Field() { ControllerKey = "Home", ActionKey="FAQs",  LabelText = "Frequently Asked Questions" },
                new Field() { ControllerKey = "Consignor", ActionKey="ConsignNow",  LabelText = "Consign Now" },
                new Field() { ControllerKey = "Consignor", ActionKey="ConsignNowSuccess",  LabelText = "Thank you for your consignment inquiry" },
                new Field() { ControllerKey = "Home", ActionKey="SiteMap",  LabelText = "Site Map" },
                new Field() { ControllerKey = "Home", ActionKey="ResourceCenter",  LabelText = "Resource Center" },
                new Field() { ControllerKey = "Home", ActionKey="ContactUs",  LabelText = "Contact Us" },
                 new Field() { ControllerKey = "Event", ActionKey="Index",  LabelText = "Event" },
                   new Field() { ControllerKey = "Home", ActionKey="ContactUs",  LabelText = "Contact Us" },
                     new Field() { ControllerKey = "Home", ActionKey="FreeEmailAlertsRegister",  LabelText = "Register for free email alerts" },
                      new Field() { ControllerKey = "Account", ActionKey="Profile",  LabelText = "Personal Information" },
                      new Field() { ControllerKey = "Account", ActionKey="MyAccount",  LabelText = "My Account" },
                      new Field() { ControllerKey = "Account", ActionKey="PastAuction",  LabelText = "My Account" },
                       new Field() { ControllerKey = "Account", ActionKey="EditPersonalShopper",  LabelText = "My Account" },
                        new Field() { ControllerKey = "Account", ActionKey="WatchBid",  LabelText = "My Account" },
                        new Field() { ControllerKey = "Auction", ActionKey="SuccessfulBid",  LabelText = "Congratulations" + ((AppHelper.CurrentUser == null) ? "" : ", " + AppHelper.CurrentUser.Login + "!") },
                        new Field() { ControllerKey = "Auction", ActionKey="OutBid",  LabelText = "Out Bid" },
                      
                       new Field() { ControllerKey = "Account", ActionKey="PayDepositFailed",  LabelText = "Pay Deposit Failed" },
                        new Field() { ControllerKey = "Auction", ActionKey="DealOfTheWeek",  LabelText = "Deal of the Week" },
              
                
            };

            return data.FirstOrDefault(n => n.ControllerKey == ControllerKey && n.ActionKey == ActionKey);
        }
    }
}