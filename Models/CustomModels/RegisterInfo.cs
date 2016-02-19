using System;
using System.Text;
using QControls.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  [Serializable]
  public class RegisterInfo
  {
    public Int64 ID = 0;

    [FieldTitle("User ID")]
    [FieldRequired]
    [FieldNonSpaced]
    [FieldCheckMaxLength(20)]
    public string Login { get; set; }

    [FieldTitle("Date Of Birth")]
    public DateTime DateOfBirth { get; set; }

    [FieldTitle("E-mail")]
    [FieldRequired]
    [FieldCheckEmail]
    [FieldCheckMaxLength(255)]
    public string Email { get; set; }

    [FieldTitle("Confirm E-mail")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    [FieldCheckEmail]
    public string ConfirmEmail { get; set; }

    [FieldTitle("Password")]
    [FieldRequired]
    [FieldCheckMinLength(6)]
    [FieldCheckMaxLength(20)]
    public string Password { get; set; }

    [FieldTitle("Confirm passowrd")]
    [FieldRequired]
    [FieldCheckMaxLength(20)]
    public string ConfirmPassword { get; set; }

    public string ConfirmCode { get; set; }

    [FieldCheckMaxLength(255)]
    public string Reference { get; set; }

    [FieldTitle("First Name")]
    [FieldRequired]
    [FieldCheckName]
    [FieldCheckMaxLength(50)]
    public string BillingFirstName { get; set; }

    [FieldTitle("Middle Name")]
    [FieldCheckName]
    [FieldCheckMaxLength(2)]    
    public string BillingMIName { get; set; }

    [FieldTitle("Last Name")]
    [FieldRequired]
    [FieldCheckName]
    [FieldCheckMaxLength(50)]
    public string BillingLastName { get; set; }

    [FieldTitle("Address(1)")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string BillingAddress1 { get; set; }
    [FieldCheckMaxLength(255)]
    public string BillingAddress2 { get; set; }

    [FieldTitle("City/Town")]
    [FieldCheckMaxLength(255)]
    [FieldRequired]
    public string BillingCity { get; set; }

    [FieldTitle("State")]
    [FieldRequired]
    [FieldCheckMaxLength(20)]
    public string BillingState { get; set; }

    [FieldTitle("Zip")]
    [FieldRequired]    
    [FieldCheckMaxLength(10)]
    public string BillingZip { get; set; }

    #region BillingPhone
    [FieldTitle("Billing Phone")]
    [FieldRequired]
    public string BillingPhone { get; set; }
    #endregion

    [FieldTitle("Billing Work Phone")]
    public string BillingWorkPhone { get; set; }
    
    [FieldCheckMaxLength(20)]
    public string BillingFax { get; set; }

    [FieldTitle("Shipping First Name")]
    [FieldRequired]
    [FieldCheckName]
    [FieldCheckMaxLength(50)]
    public string ShippingFirstName { get; set; }

    [FieldTitle("Shipping Middle Name")]
    [FieldCheckName]
    [FieldCheckMaxLength(2)]
    public string ShippingMIName { get; set; }

    [FieldTitle("Shipping Last Name")]
    [FieldRequired]
    [FieldCheckName]
    [FieldCheckMaxLength(50)]
    public string ShippingLastName { get; set; }

    [FieldTitle("Shipping Address(1)")]
    [FieldRequired()]
    [FieldCheckMaxLength(255)]
    public string ShippingAddress1 { get; set; }
    [FieldCheckMaxLength(255)]
    public string ShippingAddress2 { get; set; }

    [FieldTitle("City/Town")]
    [FieldRequired]
    [FieldCheckMaxLength(255)]
    public string ShippingCity { get; set; }

    [FieldCheckMaxLength(20)]
    public string ShippingFax { get; set; }

    [FieldTitle("State")]
    [FieldRequired()]
    [FieldCheckMaxLength(20)]
    public string ShippingState { get; set; }

    [FieldTitle("Shipping Zip")]
    [FieldRequired()]    
    [FieldCheckMaxLength(10)]
    public string ShippingZip { get; set; }

    [FieldTitle("Shipping Phone")]
    [FieldRequired]
    public string ShippingPhone { get; set; }

    [FieldTitle("Shipping Work Phone")]
    public string ShippingWorkPhone { get; set; }

    [FieldTitle("Shipping Country")]
    [FieldRequired]
    public long ShippingCountry { get; set; }

    [FieldTitle("Country")]
    [FieldRequired]
    public long BillingCountry { get; set; }

    public bool RecieveWeeklySpecials { get; set; }
    public bool RecieveNewsUpdates { get; set; }
    public bool BillingLikeShipping { get; set; }

    [FieldTitle("Agree")]
    [FieldRequired]
    public bool Agree { get; set; }

    [FieldTitle("Modifyed")]
    public bool IsModifyed { get; set; }

    [FieldTitle("ConsignorShip")]
    public bool IsConsignorShip { get; set; }

    [FieldTitle("NotPasswordReset")]
    public bool NotPasswordReset { get; set; }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);

      //check Login
      if (!ValidationCheck.IsEmpty(this.Login) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateLogin(this.Login, ID))
      {
        modelState.AddModelError("Login", "This login already present in system");
      }

      //check Email
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.Config.DataProvider.GetInstance().UserRepository.ValidateEmail(this.Email, ID))
      {
        modelState.AddModelError("Email", "This email already present in system");
      }

      if (!String.Equals(this.Email, this.ConfirmEmail, StringComparison.Ordinal))
      {
        modelState.AddModelError("Email", "The Email and confirmation Email do not match.");
        modelState.AddModelError("ConfirmEmail", "");
      }

      if (!String.Equals(this.Password, this.ConfirmPassword, StringComparison.Ordinal))
      {
        modelState.AddModelError("Password", "The password and confirmation password do not match.");
        modelState.AddModelError("ConfirmPassword", "");
      }

      if (!ProjectConfig.Config.DataProvider.GetInstance().UserRepository.CheckChangePassword(this.ID, this.Password))
      {
        modelState.AddModelError("Password", "Need to change and confirmation the password.");
        modelState.AddModelError("ConfirmPassword", "");
      }

      if (!Agree) modelState.AddModelError("Agree", "YOU MUST CHECK THE BOX BELOW");
      
      StringBuilder sb = new StringBuilder(BillingPhone);
      sb.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("x", "");
      BillingPhone = sb.ToString();

      sb = new StringBuilder(BillingWorkPhone);
      sb.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("x", "");
      BillingWorkPhone = sb.ToString();

      sb = new StringBuilder(ShippingPhone);
      sb.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("x", "");
      ShippingPhone = sb.ToString();

      sb = new StringBuilder(ShippingWorkPhone);
      sb.Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Replace(" ", "").Replace("x", "");
      ShippingWorkPhone = sb.ToString();
      
      if (String.IsNullOrWhiteSpace(BillingPhone))
        modelState.AddModelError("BillingPhone", "The Phone number is required");

      if (String.IsNullOrWhiteSpace(ShippingPhone) && !BillingLikeShipping)
        modelState.AddModelError("ShippingPhone", "The Phone number is required");
      
      //if ((modelState.ContainsKey("BillingPhone1") && modelState["BillingPhone1"].Errors.Count > 0) || (modelState.ContainsKey("BillingPhone2") && modelState["BillingPhone2"].Errors.Count > 0))
      //  modelState.AddModelError("BillingPhone", "Phone should contain digits only");
      //if ((modelState.ContainsKey("BillingWorkPhone1") && modelState["BillingWorkPhone1"].Errors.Count > 0) || (modelState.ContainsKey("BillingWorkPhone2") && modelState["BillingWorkPhone2"].Errors.Count > 0))
      //  modelState.AddModelError("BillingWorkPhone", "Work Phone should contain digits only");

      //if ((modelState.ContainsKey("ShippingPhone1") && modelState["ShippingPhone1"].Errors.Count > 0))
      //  modelState.AddModelError("ShippingPhone", "Phone should contain digits only");
      //if ((modelState.ContainsKey("ShippingWorkPhone1") && modelState["ShippingWorkPhone1"].Errors.Count > 0) || (modelState.ContainsKey("ShippingWorkPhone2") && modelState["ShippingWorkPhone2"].Errors.Count > 0))
      //  modelState.AddModelError("ShippingWorkPhone", "Work Phone should contain digits only");


    }
  }
}