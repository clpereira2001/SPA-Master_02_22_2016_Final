using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using QControls.Validation;
using System.Text;

namespace Vauction.Utils.Html
{
    public static class HtmlExtensions
    {
        #region HtmlHelper extensions

        public static string ValidationMessageArea(this HtmlHelper htmlHelper, string name)
        {
            ModelStateDictionary state = htmlHelper.ViewData.ModelState;
            string errorMessage = string.Empty;
            if (state != null && state[name] != null && state[name].Errors != null && state[name].Errors.Count > 0)
            {
                errorMessage = state[name].Errors[0].ErrorMessage;
            }

            Dictionary<string, string> d = new Dictionary<string, string>();

            if (errorMessage == String.Empty)
                return "<span id=\"vm_" + name + "\" class=\"field-validation-error\" id=\"\"></span>";

            return "<span id=\"vm_" + name + "\" class=\"field-validation-error show\" id=\"\">" + errorMessage + "</span>";
        }

        public static string SubmitWithClientValidation(this HtmlHelper htmlHelper, string value, string classname, string icon = "")
        {
            string out_str = string.Empty;

            if (htmlHelper.ViewData.Model != null)
            {
                out_str += GetClientErrorData(htmlHelper.ViewData.Model.GetType());
                int intIndex = out_str.IndexOf("SetInputsMaxLength(errorRules);");
                if (intIndex != -1)
                {
                    out_str += "<button class=\"" + classname + "\" type=\"submit\" id=\"btSubmit\" onclick=\"return SubmitFormWithClientValidation( this)\">";
                }
                else
                {
                    out_str += "<button class=\"" + classname + "\" type=\"submit\" id=\"btSubmit\" onclick=\"return SubmitFormWithClientValidation(errorRules, this)\">";
                }
                out_str += icon;
                out_str += value;
                out_str += "</button>";
            }
            else
            {
                out_str += "<button type=\"submit\"  class=\"" + classname + "\"><span>" + value + "</span></button>";
            }

            return out_str;
        }

        public static string SubmitWithClientValidation(this HtmlHelper htmlHelper, string value)
        {
            string out_str = string.Empty;

            if (htmlHelper.ViewData.Model != null)
            {
                out_str += GetClientErrorData(htmlHelper.ViewData.Model.GetType());


                out_str += "<button class=\"cssbutton small white\" type=\"submit\" id=\"btSubmit\" onclick=\"return SubmitFormWithClientValidation( this)\">";
                out_str += "    <span>" + value + "</span>";
                out_str += "</button>";
            }
            else
            {
                out_str += "<button type=\"submit\"  class=\"cssbutton submit_payment small white\"><span>" + value + "</span></button>";
            }

            return out_str;
        }

        public static string GetClientErrorData(Type modelType)
        {
            string out_str = "<script language=\"javascript\" src=\"../../Scripts/jquery-1.11.0.min.js;../../public/scripts/validation.js\" type=\"text/javascript\">" + Environment.NewLine;

            out_str += "var errorRules = new Hashtable();" + Environment.NewLine + Environment.NewLine;

            Dictionary<string, List<ValidationRule>> errorRules = ValidationCheck.GetValidationRules(modelType);

            foreach (KeyValuePair<string, List<ValidationRule>> item in errorRules)
            {
                out_str += "var fieldErrorRules = new Hashtable();" + Environment.NewLine;

                foreach (ValidationRule rule in item.Value)
                {
                    string errorMessage = rule.ErrorMessage;

                    //out_str += "fieldErrorRules.put(\"" + rule.Type.ToString().ToLower() + "\", \"" + errorMessage + "\");" + Environment.NewLine;
                    out_str += "fieldErrorRules.put(\"" + rule.Type.ToString().ToLower() + "\", new Array(\"" + errorMessage + "\"" + rule.IFieldValidation.GetAdditionalParams() + "));" + Environment.NewLine;
                }

                out_str += "errorRules.put(\"" + item.Key + "\", fieldErrorRules);" + Environment.NewLine + Environment.NewLine;

            }
            if (errorRules.Count > 0)
            {
                out_str += "SetInputsMaxLength(errorRules);" + Environment.NewLine;
            }
            else

            { out_str += "SetInputsMaxLength();" + Environment.NewLine; }
            out_str += "</script>" + Environment.NewLine;
            return out_str;
        }

        #endregion

        public class PrintInfo
        {
            public string Header { get; set; }
            public bool Total { get; set; }
        }

        //hasTotal: 0 - No Total
        //1- Total string
        //2- Total string with item count
        public static string GetHtmlTableString(object data,
            Dictionary<string, PrintInfo> printData, int hasTotal)
        {
            IEnumerable enumData = data as IEnumerable;
            StringBuilder result = null;
            Dictionary<string, decimal> total = new Dictionary<string, decimal>();
            object first = null;
            int count = 0;
            if (enumData != null)
            {
                bool header = false;
                result = new StringBuilder("<table>\r\n");
                foreach (object item in enumData)
                {
                    if (!header)
                    {
                        first = item;
                        result.Append("\t<tr>\r\n");
                        foreach (PropertyInfo info in item.GetType().GetProperties())
                        {
                            if (printData.ContainsKey(info.Name))
                            {
                                result.AppendFormat("\t\t<th>{0}</th>\r\n", printData[info.Name].Header);
                                if (printData[info.Name].Total)
                                    total.Add(info.Name, 0);
                            }
                        }
                        result.Append("\t</tr>\r\n");
                        header = true;
                    }

                    result.Append("\t<tr>\r\n");
                    foreach (PropertyInfo info in item.GetType().GetProperties())
                    {
                        if (printData.ContainsKey(info.Name))
                        {
                            object val = info.GetValue(item, null);
                            result.AppendFormat("\t\t<td>{0}</td>\r\n", val);
                            if (printData[info.Name].Total)
                                total[info.Name] += Convert.ToDecimal(val);
                        }
                    }
                    result.Append("\t</tr>\r\n");
                    count++;
                }
                if (hasTotal > 0 && first != null)
                {
                    result.Append("\t<tr>\r\n");
                    bool fst = false;
                    foreach (PropertyInfo info in first.GetType().GetProperties())
                    {
                        if (!fst)
                        {
                            if (hasTotal == 1)
                                result.AppendFormat("\t\t<td><b>Total</b></td>\r\n");
                            else
                                result.AppendFormat("\t\t<td><b>Total for {0} items</b></td>\r\n", count);
                            fst = true;
                        }
                        else
                        {
                            if (total.ContainsKey(info.Name))
                                result.AppendFormat("\t\t<td><b>{0}</b></td>\r\n", total[info.Name]);
                            else
                                result.AppendFormat("\t\t<td></td>\r\n");
                        }
                    }

                    result.Append("\t</tr>\r\n");
                }
                result.Append("</table>");
            }
            return result == null ? null : result.ToString();
        }

        //hasTotal: 0 - No Total
        //1- Total string
        //2- Total string with item count
        public static string GetHtmlTableString(object data,
            Dictionary<string, int> printData, int hasTotal)
        {
            IEnumerable enumData = data as IEnumerable;
            StringBuilder result = null;
            Dictionary<string, decimal> total = new Dictionary<string, decimal>();
            object first = null;
            int count = 0;
            if (enumData != null)
            {
                bool header = false;
                result = new StringBuilder("<div class=\"print-table\"><table>\r\n");
                foreach (object item in enumData)
                {
                    if (!header)
                    {
                        first = item;
                        result.Append("\t<tr>\r\n");
                        foreach (PropertyInfo info in item.GetType().GetProperties())
                        {
                            if (info.Name[0] != '_')
                            {
                                result.AppendFormat("\t\t<th>{0}</th>\r\n", info.Name);
                                if (printData.ContainsKey(info.Name))
                                    total.Add(info.Name, 0);
                            }
                        }
                        result.Append("\t</tr>\r\n");
                        header = true;
                    }

                    result.Append("\t<tr>\r\n");
                    foreach (PropertyInfo info in item.GetType().GetProperties())
                    {
                        if (info.Name[0] != '_')
                        {
                            object val = info.GetValue(item, null);
                            if (info.PropertyType == typeof(decimal) ||
                                info.PropertyType == typeof(Nullable<Decimal>))
                            {
                                result.AppendFormat("\t\t<td>${0}</td>\r\n", Convert.ToDecimal(val).ToString("0.00"));
                            }
                            else
                            {
                                result.AppendFormat("\t\t<td>{0}</td>\r\n", val);
                            }
                            if (printData.ContainsKey(info.Name))
                                total[info.Name] += Convert.ToDecimal(val);
                        }
                    }
                    result.Append("\t</tr>\r\n");
                    count++;
                }
                if (hasTotal > 0 && first != null)
                {
                    result.Append("\t<tr>\r\n");
                    bool fst = false;
                    foreach (PropertyInfo info in first.GetType().GetProperties())
                    {
                        if (info.Name[0] != '_')
                        {
                            if (!fst)
                            {
                                if (hasTotal == 2)
                                    result.AppendFormat("\t\t<td><b>Total</b></td>\r\n");
                                else
                                    result.AppendFormat("\t\t<td><b>Total for {0} items</b></td>\r\n", count);
                                fst = true;
                            }
                            else
                            {
                                if (total.ContainsKey(info.Name))
                                {
                                    if (info.PropertyType == typeof(decimal) ||
                                      info.PropertyType == typeof(Nullable<Decimal>))
                                    {
                                        result.AppendFormat("\t\t<td><b>${0}</b></td>\r\n", total[info.Name].ToString("0.00"));
                                    }
                                    else
                                    {
                                        result.AppendFormat("\t\t<td><b>{0}</b></td>\r\n", Convert.ToInt32(total[info.Name]));
                                    }
                                }
                                else
                                    result.AppendFormat("\t\t<td></td>\r\n");
                            }
                        }
                    }

                    result.Append("\t</tr>\r\n");
                }
                result.Append("</table></div>");
                result.Append("<script type=\"text/javascript\">window.print()</script>");
            }
            return result == null ? null : result.ToString();
        }
    }
}