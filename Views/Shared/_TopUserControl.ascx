<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl" %>

<%  string strContentPageHeader1 = Model.ToString();

    var name = Html.ViewContext.View;
    var currentController = ViewContext.RouteData.Values["controller"] as string;
    var currentAction = ViewContext.RouteData.Values["action"] as string;
    var strData = "Hello, <strong>" + AppHelper.CurrentUser + "</strong>!";

    Vauction.Models.Enums.Field field = Vauction.Models.Enums.FieldConfiguration.GetFieldData(currentController, currentAction);
    var strContentPageHeader = "";
    var strAction = "";
    if (field != null)
    {
        if (field.LabelText == null && field.LabelText == "")
        {
            strContentPageHeader = "Index";
            strAction = "Index";
        }
        else
        {
            strContentPageHeader = field.LabelText;
            if (field.ActionKey == null && field.ActionKey == "")
            {
                strAction = strContentPageHeader;
            }
            else { strAction = field.ActionKey; }
        }
    }
    else
    { strContentPageHeader = currentController;
    strAction = currentController;
    }
  //  Vauction.Models.Enums.AppController.ContentPageHeader strContentPageHeader = Vauction.Models.Enums.AppController.GetEnumValue<Vauction.Models.Enums.AppController.ContentPageHeader>(currentAction);
    
%>
<div class="container margin-top-xs">
    <div class="row">
        <div class="col-sm-6">
            <div class="row">
                <ol class="breadcrumb">
                    <li><a href="/Home/Index">Home</a></li>
                    <li class="active"><%= strAction %> </li>
                </ol>
            </div>
        </div>
        <div class="col-sm-6 hidden-xs">
            <ul class="list-unstyled list-inline pull-right select-top">
                <li>
                    <a href="#">
                        <span class="fa-stack fa-lg">
                            <i class="fa fa-circle fa-stack-2x"></i>
                            <i class="fa fa-bookmark-o fa-stack-1x fa-inverse"></i>
                        </span>
                    </a>
                </li>
                <li>
                    <a href="#">
                        <span class="fa-stack fa-lg">
                            <i class="fa fa-circle fa-stack-2x"></i>
                            <i class="fa fa-envelope fa-stack-1x fa-inverse"></i>
                        </span>
                    </a>
                </li>
                <li>
                    <a href="#">
                        <span class="fa-stack fa-lg">
                            <i class="fa fa-circle fa-stack-2x"></i>
                            <i class="fa fa-print fa-stack-1x fa-inverse"></i>
                        </span>
                    </a>
                </li>
            </ul>
        </div>
    </div>
</div>
<div class="container margin-bottom-xl">
    <div class="row">
        <div class="col-sm-12">
            <p class="text-primary text-weight-900 text-lg border-bottom border-weight-xs border-primary">
                <%=       strContentPageHeader  %>
                <a href="#" class="btn btn-link pull-right margin-top-xs text-muted margin-767-top-none">Back to Item <i class="fa fa-caret-square-o-right"></i></a>
            </p>
        </div>
    </div>
</div>
