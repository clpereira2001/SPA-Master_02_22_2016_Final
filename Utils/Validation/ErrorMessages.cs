
namespace QControls.Validation
{
    public class ErrorMessages
    {
        public const string Required = @"'{0}' is required ";
        public const string Invalid = @"'{0}' is invalid";
        public const string InvalidEmail = @"'{0}' is invalid.";
        public const string MinLength = @"'{0}' must be at least {1} characters(s)";
        public const string MaxLength = @"'{0}' must be maximum {1} characters(s)";
        public const string YearWrong = @"Year must be 1900 - 2078.";
        public const string AlphaWrong = @"Field should contain letters only";
        public const string NumericWrong = @"Field should contain digits only";
        public const string AlphaNumericWrong = @"Field should contain alphanumeric symbols only";

        public const string FieldNonSpacedWrong = @"Field cannot contain spaces";
        public const string AmericanPhoneWrong = @"Field should be like: XXX-XXX-XXXX or XXX-XXX-XXXX EXTXXXX";

        public static string GetRequired(string field) { return string.Format(Required, field); }
        public static string GetInvalid(string field) { return string.Format(Invalid, field); }
        public static string GetInvalidEmail(string field) { return string.Format(InvalidEmail, field); }
        public static string GetMinLength(string field, int length) { return string.Format(MinLength, field, length); }
        public static string GetMaxLength(string field, int length) { return string.Format(MaxLength, field, length); }
        public static string GetYearWrong(string field) { return string.Format(YearWrong, field); }


        public static string GetAlphanumericWrong(string title)
        {
            return string.Format(AlphaNumericWrong, title);
        }

        public static string GetAlphaWrong(string title)
        {
            return string.Format(AlphaWrong, title);
        }

        public static string GetFieldNumericWrong(string title)
        {
            return string.Format(NumericWrong, title);
        }

        public static string GetFieldAmericanPhoneWrong(string title)
        {
            return string.Format(AmericanPhoneWrong, title);
        }

        public static string GetFieldNonSpaced(string title)
        {
            return string.Format(FieldNonSpacedWrong, title);
        }
    }
}