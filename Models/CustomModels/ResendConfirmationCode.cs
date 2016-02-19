﻿using System;
using System.Reflection;
using System.Collections.Generic;
using System.Web.Mvc;
using QControls.Validation;
using Vauction.Configuration;

namespace Vauction.Models
{
  public class ResendConfirmationCode
  {
    [FieldTitle("E-mail")]
    [FieldRequired]
    [FieldCheckEmail]
    [FieldCheckMaxLength(80)]
    public string Email { get; set; }

    public void Validate(System.Web.Mvc.ModelStateDictionary modelState)
    {
      ValidationCheck.CheckErrors(this, modelState);

      if (!ValidationCheck.IsEmpty(this.Email) && ValidationCheck.IsEmail(this.Email))
      {
        IUser user = ProjectConfig.DataProvider.UserRepository.GetUserByEmail(this.Email, false);
        if (user == null)
        {
          modelState.AddModelError("Email", "Sorry, the e-mail address entered was not found.  Please try again.");
        }
        else if (user.IsConfirmed)
        {
          modelState.AddModelError("Email", "Sorry, the e-mail address already confirmed.");
        }
      }
    }
  }
}
