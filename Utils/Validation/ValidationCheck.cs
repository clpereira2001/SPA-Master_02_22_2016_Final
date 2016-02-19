using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace QControls.Validation
{
    #region ValidationRule
    public class ValidationRule
    {
        public ValidationRule(ValidationType type, string fieldName, string errorMessage, IFieldValidationAttribute iFieldValidation)
        {
            Type = type;
            FieldName = fieldName;
            ErrorMessage = errorMessage;
            IFieldValidation = iFieldValidation;
        }
        public IFieldValidationAttribute IFieldValidation;
        public ValidationType Type = ValidationType.Required;
        public string FieldName = string.Empty;
        public string ErrorMessage = string.Empty;
    }
    #endregion

    public class ValidationCheck
    {
        #region GetErrors
        public static void CheckErrors(object model, System.Web.Mvc.ModelStateDictionary modelState, bool validateNotUpdateedFields)
        {
            Dictionary<string, List<ValidationRule>> errorRules = ValidationCheck.GetValidationRules(model.GetType());

            foreach (KeyValuePair<string, List<ValidationRule>> item in errorRules)
            {
                object obj = model.GetType().InvokeMember(item.Key, BindingFlags.GetProperty, null, model, null);

                foreach (ValidationRule rule in item.Value)
                {
                    if (!rule.IFieldValidation.Validate(obj))
                    {
                        if (modelState.ContainsKey(rule.FieldName) || validateNotUpdateedFields)
                            modelState.AddModelError(rule.FieldName, rule.IFieldValidation.GetErrorMessage(rule.FieldName));
                        continue;
                    }
                }
            }
        }
        public static void CheckErrors(object model, System.Web.Mvc.ModelStateDictionary modelState)
        {
            CheckErrors(model, modelState, false);
        }
        #endregion

        #region GetValidationRules
        public static Dictionary<string, List<ValidationRule>> GetValidationRules(Type modelType)
        {
            Dictionary<string, List<ValidationRule>> errorRules = new Dictionary<string, List<ValidationRule>>();

            PropertyInfo[] infos = modelType.GetProperties();
            CheckProperties(infos, errorRules);

            Type[] interfaces = modelType.GetInterfaces();
            foreach (Type type in interfaces)
            {
                if (!type.FullName.StartsWith("System."))
                {
                    PropertyInfo[] interfaceInfos = type.GetProperties();
                    CheckProperties(interfaceInfos, errorRules);
                }
            }

            return errorRules;
        }

        static void CheckProperties(PropertyInfo[] infos, Dictionary<string, List<ValidationRule>> errorRules)
        {
            foreach (PropertyInfo info in infos)
            {
                string title = string.Empty;
                object[] attrs_title = info.GetCustomAttributes(typeof(FieldTitleAttribute), true);

                if (attrs_title != null && attrs_title.Length > 0)
                {
                    title = ((FieldTitleAttribute)attrs_title[0]).FieldTitle;
                }

                object[] attrs = info.GetCustomAttributes(typeof(FieldValidationAttribute), true);
                foreach (IFieldValidationAttribute attr in attrs)
                {
                    if (attr != null)
                    {
                        ValidationRule rule = rule = new ValidationRule(attr.Type, info.Name, attr.GetErrorMessage(title), attr);

                        if (!errorRules.ContainsKey(rule.FieldName))
                        {
                            errorRules[rule.FieldName] = new List<ValidationRule>(); ;
                        }
                        errorRules[rule.FieldName].Add(rule);
                    }
                }
            }                        
        }

        #endregion

        #region Validation functions
        public static bool IsEmail(string s)
        {

            Regex regex = new Regex(RegularExpressions.Email);
            
            if (regex != null && !regex.IsMatch(s))
                return false;
            
            return true;
        }

        public static bool IsEmpty(object s)
        {
            if (s != null &&  s.ToString().Trim() != string.Empty)
                return false;

            return true;            
        }

        public static bool IsDate(string s)
        {
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(s, out date))
            {
                return true;
            }

            return false;
        }

        public static bool CheckMinValue(string s, int MinLength)
        {
            if (s != null && s.ToString().Length < MinLength && s.ToString().Length > 0)
                return false;

            return true;
        }

        public static bool CheckMaxValue(string s, int MaxLength)
        {
            if (s != null && s.ToString().Length > MaxLength && s.ToString().Length > 0)
                return false;

            return true;
        }

        public static bool CheckYear(object s)
        {
            if (s == null)
                return true;

            int year = 0;
            if (int.TryParse(Convert.ToString(s), out year) && year >= 1900 && year <= 2078)
                return true;

            return false;
        }

        public static bool CheckAlphanumeric(string s)
        {

            Regex regex = new Regex(RegularExpressions.AlphaNumeric);

            if (regex != null && !regex.IsMatch(s))
                return false;

            return true;
        }

        public static bool CheckAlpha(string s)
        {
            Regex regex = new Regex(RegularExpressions.Alpha);

            if (regex != null && !regex.IsMatch(s))
                return false;

            return true;
        }

        public static bool CheckName(string s)
        {
          Regex regex = new Regex(RegularExpressions.Name);
          return !(regex != null && !regex.IsMatch(s));
        }
        #endregion

        public static bool CheckFieldNumeric(string s)
        {
            Regex regex = new Regex(RegularExpressions.Numbers);
            if (regex != null && !String.IsNullOrEmpty(s) && !regex.IsMatch(s)) return false;
            return true;
        }

        public static bool CheckFieldAmericanPhone(string p)
        {
            Regex regex = new Regex(RegularExpressions.AmericanPhone);

            if (regex != null && !regex.IsMatch(p))
                return false;

            return true;
        }

        public static bool IsSpaced(string p)
        {
            return !    p.Contains(" ");
        }
    }
}