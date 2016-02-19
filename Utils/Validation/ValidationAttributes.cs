using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace QControls.Validation
{
    public enum ValidationType
    {
        Required = 1,
        Email = 2,
        MinLength = 3,
        MaxLength = 4,
        Year = 5,
        Alpha = 6,
        Alphanumeric=7,
        Numeric = 8,
        AmericanPhone = 9,
        FieldNonSpaced = 10
    }

    #region IFieldValidationAttribute
    public interface IFieldValidationAttribute
    {
        bool Validate(object value);
        string GetErrorMessage(string title);
        string GetAdditionalParams();
        ValidationType Type { get; }
    }
    #endregion

    #region FieldTitleAttribute
    public class FieldTitleAttribute : Attribute
    {
        protected string fieldTitleStr = string.Empty;

        public string FieldTitle
        {
            get { return fieldTitleStr; }
        }

        public FieldTitleAttribute(string title)
        {
            fieldTitleStr = title;
        }

    }
    #endregion

    #region FieldValidationAttribute
    public class FieldValidationAttribute : Attribute, IFieldValidationAttribute
    {     
        public ValidationType type;

        public ValidationType Type
        {
            get { return type; }
        }

        virtual public bool Validate(object value)
        {
            return true;
        }

        virtual public string GetErrorMessage(string title)
        {
            return string.Empty;
        }

        virtual public string GetAdditionalParams()
        {
            return string.Empty;
        }        

    }
    #endregion

    #region FieldRequiredAttribute
    public class FieldRequiredAttribute : FieldValidationAttribute
    {
        public FieldRequiredAttribute()
        {
            type = ValidationType.Required;            
        }
        
        public override bool Validate(object value)
        {
            return !ValidationCheck.IsEmpty(value);
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetRequired(title);
        }
    }
    #endregion

    #region FieldNonSpacedAttribute
    public class FieldNonSpacedAttribute : FieldValidationAttribute
    {
        public FieldNonSpacedAttribute()
        {
            type = ValidationType.FieldNonSpaced;            
        }
        
        public override bool Validate(object value)
        {
            return ValidationCheck.IsSpaced(value == null ? string.Empty : value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetFieldNonSpaced(title);
        }
    }
    #endregion

    #region FieldCheckEmailAttribute
    public class FieldCheckEmailAttribute : FieldValidationAttribute
    {
        public FieldCheckEmailAttribute()
        {
            type = ValidationType.Email;            
        }

        public override bool Validate(object value)
        {
            return ValidationCheck.IsEmail(value == null ? string.Empty : value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetInvalidEmail(title);
        }
    }
    #endregion

    #region FieldCheckMinLengthAttribute
    public class FieldCheckMinLengthAttribute : FieldValidationAttribute
    {
        int MinLength = 0;

        public FieldCheckMinLengthAttribute(int minLength)
        {
            type = ValidationType.MinLength;            
            MinLength = minLength;
        }

        public override bool Validate(object value)
        {
            return ValidationCheck.CheckMinValue(value == null ? string.Empty : value.ToString(), MinLength);
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetMinLength(title, MinLength);
        }

    }
    #endregion

    #region FieldCheckMaxLengthAttribute
    public class FieldCheckMaxLengthAttribute : FieldValidationAttribute
    {
        int MaxLength = 0;

        public FieldCheckMaxLengthAttribute(int maxLength)
        {
            type = ValidationType.MaxLength;
            MaxLength = maxLength;
        }

        public override bool Validate(object value)
        {
            return ValidationCheck.CheckMaxValue(value == null ? string.Empty : value.ToString(), MaxLength);
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetMaxLength(title, MaxLength);            
        }

        public override string GetAdditionalParams()
        {
            return  "," + MaxLength.ToString();
        }
    }
    #endregion

    #region FieldCheckYearAttribute
    public class FieldCheckYearAttribute : FieldValidationAttribute
    {
        public FieldCheckYearAttribute()
        {            

        }

        public override bool Validate(object value)
        {
            return ValidationCheck.CheckYear(value == null ? string.Empty :value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetYearWrong(title);
        }
    }
    #endregion

    #region FieldCheckAlpha
    public class FieldCheckAlphaAttribute : FieldValidationAttribute
    {
        public FieldCheckAlphaAttribute()
        {

        }

        public override bool Validate(object value)
        {
          return ValidationCheck.CheckAlpha(value == null ? string.Empty : value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetAlphaWrong(title);
        }
    }
    #endregion

    #region FieldCheckAlpha
    public class FieldCheckNameAttribute : FieldValidationAttribute
    {
      public FieldCheckNameAttribute()
      {

      }

      public override bool Validate(object value)
      {
        return ValidationCheck.CheckName(value == null ? string.Empty : value.ToString());
      }

      public override string GetErrorMessage(string title)
      {
        return ErrorMessages.GetAlphaWrong(title);
      }
    }
    #endregion

    #region FieldCheckAlphanumeric
    public class FieldCheckAlphanumericAttribute : FieldValidationAttribute
    {
        public FieldCheckAlphanumericAttribute()
        {

        }

        public override bool Validate(object value)
        {
          return ValidationCheck.CheckAlphanumeric(value == null ? string.Empty : value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetAlphanumericWrong(title);
        }
    }
    #endregion

    #region FieldCheckNumeric
    public class FieldCheckNumericAttribute : FieldValidationAttribute
    {
        public FieldCheckNumericAttribute()
        {

        }

        public override bool Validate(object value)
        {
            if (value == null) return true;
            return ValidationCheck.CheckFieldNumeric(value == null ? string.Empty : value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetFieldNumericWrong(title);
        }
    }
    #endregion

    #region FieldCheckAmericanPhone
    public class FieldCheckAmericanPhoneAttribute : FieldValidationAttribute
    {
        public FieldCheckAmericanPhoneAttribute()
        {

        }

        public override bool Validate(object value)
        {
            if (value == null || Convert.ToString(value).Length<1) return true;
            return ValidationCheck.CheckFieldAmericanPhone(value.ToString());
        }

        public override string GetErrorMessage(string title)
        {
            return ErrorMessages.GetFieldAmericanPhoneWrong(title);
        }
    }
    #endregion
}