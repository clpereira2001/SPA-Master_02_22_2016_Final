using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using QControls.Validation;

namespace Vauction.Models
{
  [Serializable]
  [MetadataType(typeof(ConsignNowForm))]
  public class ConsignNowForm
  {
    public string ID { get; set; }

    public string Description { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [RegularExpression(RegularExpressions.Email, ErrorMessage = "Email is invalid.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    public string LastName { get; set; }

    public string Acquire { get; set; }
    public string Phone { get; set; }
    public bool Finance { get; set; }
    public bool Subscribe { get; set; }
    public List<string> FileLinks { get; set; }
    public List<string> Attachments { get; set; }
    public string CaptchaValue { get; set; }

    public ConsignNowForm()
    {
      FileLinks = new List<string>();
      Attachments = new List<string>();
    }
  }
}