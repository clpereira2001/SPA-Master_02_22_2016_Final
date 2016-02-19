using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

using Vauction.Configuration;
using QControls.Validation;

namespace Vauction.Models
{
  [Serializable]
  partial class OuterSubscription : IOuterSubscription
  {
    public string EmailConfirm { get; set; }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);
      if (String.IsNullOrEmpty(Email))
          modelState.AddModelError("Email", "<br /><br /><br /><br />Please enter your email address");
      
      if (!ValidationCheck.IsEmpty(this.Email) && !ProjectConfig.DataProvider.UserRepository.ValidateOuterSubscriptionEmail(this.Email, ID))
          modelState.AddModelError("Email", "<br /><br /><br /><br />This email already present in system");

      if (this.Email != this.EmailConfirm)
          modelState.AddModelError("Email", "<br /><br /><br /><br />Email and confirmation email should be match.");
    }
  }
}