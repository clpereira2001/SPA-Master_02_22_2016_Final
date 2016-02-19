using QControls.Utils.Validation;
using Relatives.Models.CustomBinders;
using System.Web.Mvc;
using QControls.Validation;
using System;
using System.Globalization;
namespace Vauction.Models
{
  [ModelBinder(typeof(CustomItemBinder))]
  public class CreditCardInfo : IValidateModel
  {
    [FieldTitle("Card Number")]
    [FieldRequired]
    [FieldNonSpaced]
    [FieldCheckNumeric]
    [FieldCheckMaxLength(16)]
    [FieldCheckMinLength(13)]    
    public string CardNumber { get; set; }

    [FieldCheckNumeric]
    [FieldRequired]    
    public int ExpirationMonth { get; set; }

    [FieldCheckNumeric]
    [FieldRequired]        
    public int ExpirationYear { get; set; }

    [FieldTitle("Card Secure Code")]
    [FieldRequired]
    [FieldNonSpaced]
    [FieldCheckNumeric]
    [FieldCheckMaxLength(4)]
    [FieldCheckMinLength(3)]
    public string CardCode { get; set; }

    [FieldTitle("Address 1")]
    [FieldRequired]
    public string Address1 { get; set; }    
    
    public string Address2 { get; set; }

    [FieldTitle("City")]
    [FieldRequired]
    public string City { get; set; }

    [FieldTitle("Zip")]
    [FieldRequired]      
    public string Zip { get; set; }

    [FieldTitle("State")]
    [FieldRequired]
    public string State { get; set; }
    
    [FieldTitle("Country")]
    [FieldRequired]
    public long? Country { get; set; }

    [FieldTitle("CountryTitle")]    
    public string CountryTitle { get; set; }

    #region IValidateModel
    public void Validate(ModelStateDictionary modelState) {       
      if (DateTime.Now.CompareTo(new DateTime(ExpirationYear, ExpirationMonth, DateTime.DaysInMonth(ExpirationYear, ExpirationMonth))) > 0)
      {
        modelState.AddModelError("ExpirationYear", "The card's date has expired.");
      }
    }
    #endregion
  }
}