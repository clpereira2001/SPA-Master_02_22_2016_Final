using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.DynamicData;
using System.Web.Mvc;
using Vauction.Utils.Validation;

namespace Vauction.Models
{
  [Serializable]
  [MetadataType(typeof(AuctionListingMetData))]
  public partial class AuctionListing : IAuctionListing
  {
    public class AuctionListingMetData
    {      
      [DisplayName("Category")]
      [Range(1, 10000000.00, ErrorMessage = "Category is required")]
      public Int64 Category_ID { get; set; }

      [Required(ErrorMessage = "Title is required")]
      public string Title { get; set; }
            
      [Required(ErrorMessage = "Quantity is required")]      
      [RegularExpression(MetaDataValidation.IntegerValueExpression, ErrorMessage=MetaDataValidation.IntegerValueExpressionMessage)]      
      public Int32 Quantity { get; set; }

      [Required(ErrorMessage = "Price is required")]
      [Range(1, 10000000.00, ErrorMessage = MetaDataValidation.ValueMustBeGreaterZeroMessage)]
      [RegularExpression(MetaDataValidation.DecimalValueExpression, ErrorMessage = MetaDataValidation.DecimalValueExpressionMessage)]
      public decimal Price { get; set; }

      [Required(ErrorMessage = "Reserve is required")]
      [Range(1, 10000000.00, ErrorMessage = MetaDataValidation.ValueMustBeGreaterZeroMessage)]
      [RegularExpression(MetaDataValidation.DecimalValueExpression, ErrorMessage = MetaDataValidation.DecimalValueExpressionMessage)]
      public decimal Reserve { get; set; }

      [Required(ErrorMessage = "Cost is required")]
      [Range(1, 10000000.00, ErrorMessage = MetaDataValidation.ValueMustBeGreaterZeroMessage)]
      [RegularExpression(MetaDataValidation.DecimalValueExpression, ErrorMessage = MetaDataValidation.DecimalValueExpressionMessage)]
      public decimal Cost { get; set; }

      [Required(ErrorMessage = "Shipping is required"), Range(0, 10000000.00, ErrorMessage = MetaDataValidation.ValueMustBePositiveMessage)]
      [RegularExpression(MetaDataValidation.DecimalValueExpression, ErrorMessage = MetaDataValidation.DecimalValueExpressionMessage)]
      public decimal Shipping { get; set; }

      [Required]
      [StringLength(50)]
      public string Location { get; set; }

      [StringLength(50)]
      public string InternalID { get; set; }
    }
  }
}