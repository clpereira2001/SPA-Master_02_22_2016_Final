<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PersonalShopperItem>" %>

<%@ Import Namespace="Vauction.Models.CustomModels" %>
<%@ Import Namespace="Vauction.Utils.Html" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Edit Personal Shopper - <%=Consts.CompanyTitleName %></title>
    <% Html.Script("hashtable.js"); %>
    <% Html.Script("validation.js"); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <%
        List<SelectListItem> iscur = new List<SelectListItem>();
        iscur.Add(new SelectListItem { Text = "Active", Value = "True", Selected = true });
        iscur.Add(new SelectListItem { Text = "Not Active", Value = "False" });

        List<SelectListItem> ishtml = new List<SelectListItem>();
        ishtml.Add(new SelectListItem { Text = "Text", Value = "False" });
        ishtml.Add(new SelectListItem { Text = "HTML", Value = "True", Selected = true });

        List<SelectListItem> list = new List<SelectListItem>();
        list.Add(new SelectListItem { Text = "Select", Value = "", Selected = true });
        foreach (IdTitle c in ViewData["category"] as List<IdTitle>)
            list.Add(new SelectListItem { Text = c.Title, Value = c.ID.ToString() });
    %>
    <div class="col-md-9 col-sm-8">
        <div id="registration" class="control">
            <div class="row">
                <div class="col-sm-12">
                    <p class="text-primary text-weight-900 text-lg">Edit Personal Shopper</p>
                </div>
            </div>

            <% using (Html.BeginForm())
               { %>
            <%=Html.AntiForgeryToken() %>
            <%=Html.Hidden("psi_id", Model.ID) %>
            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="Keyword1" class="text-uppercase hidden-xs">
                            Keyword 1</label>
                        <%: Html.TextBox("Keyword1", Model.Keyword1, new { @size = "40", @maxlength = "60",@placeholder="Keyword #1",@class="form-control" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlCategory1" class="text-uppercase hidden-xs">Category 1</label>

                        <%=Html.DropDownList("ddlCategory1", list, new {@class="form-control"})%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="Keyword2" class="text-uppercase hidden-xs">
                            Keyword 2</label>
                        <%: Html.TextBox("Keyword2", Model.Keyword2, new { @size = "40", @maxlength = "60",@placeholder="Keyword #2",@class="form-control" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlCategory2" class="text-uppercase hidden-xs">Category 2</label>

                        <%=Html.DropDownList("ddlCategory2", list, new {@class="form-control"})%>
                    </div>
                </div>
            </div>



            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="Keyword3" class="text-uppercase hidden-xs">
                            Keyword 3</label>
                        <%: Html.TextBox("Keyword3", Model.Keyword3, new { @size = "40", @maxlength = "60",@placeholder="Keyword #3",@class="form-control" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlCategory3" class="text-uppercase hidden-xs">Category 3</label>

                        <%=Html.DropDownList("ddlCategory3", list, new {@class="form-control"})%>
                    </div>
                </div>
            </div>


            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="Keyword4" class="text-uppercase hidden-xs">
                            Keyword 4</label>
                        <%: Html.TextBox("Keyword4", Model.Keyword4, new { @size = "40", @maxlength = "60",@placeholder="Keyword #4",@class="form-control" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlCategory4" class="text-uppercase hidden-xs">Category 4</label>

                        <%=Html.DropDownList("ddlCategory4", list, new {@class="form-control"})%>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="Keyword5" class="text-uppercase hidden-xs">
                            Keyword 5</label>
                        <%: Html.TextBox("Keyword5", Model.Keyword5, new { @size = "40", @maxlength = "60",@placeholder="Keyword #5",@class="form-control" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlCategory5" class="text-uppercase hidden-xs">Category 5</label>

                        <%=Html.DropDownList("ddlCategory5", list, new {@class="form-control"})%>
                    </div>
                </div>
            </div>




            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlIsHTML" class="text-uppercase">Email text format</label>

                        <%=Html.DropDownList("ddlIsHTML", ishtml, new {@class="form-control",@PlaceHolder="HTML" })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="ddlIsCur" class="text-uppercase">Status</label>
                        <%=Html.DropDownList("ddlIsCur", iscur, new { @class="form-control",@PlaceHolder="Currently" })%>
                    </div>
                </div>
            </div>


            <div class="row">
                <div class="col-sm-6">
                    <div class="form-group">
                        <label for="PersonalDate" class="text-uppercase">Expiration date</label>
                        <%= Html.TextBox("PersonalDate", (Model.DateExpires.CompareTo(DateTime.MinValue) == 0) ? DateTime.Now.AddDays(7).ToShortDateString() : Model.DateExpires.ToShortDateString(), new {@class="form-control",@PlaceHolder="Expiration date"  })%>
                    </div>
                </div>
                <div class="col-sm-6">
                    <label for="" class="text-uppercase">&nbsp;</label>
                    <br />
                    <%= Html.SubmitWithClientValidation("Save","text-uppercase btn btn-danger col-sm-12 text-weight-600")%>
                    <%--<a href="#" class="text-uppercase btn btn-danger col-sm-12 text-weight-600">SAVE</a>--%>
                </div>
            </div>




            <%} %>
            <div class="back_link">
                <%=Html.ActionLink("Return to My Account Page", "MyAccount", new {controller="Account", action="MyAccount"}) %>
            </div>

        </div>
    </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $("#ddlIsCur").val('<%= Model.IsActive %>');
            $("#ddlCategory1").val(parseInt('<%= Model.Category_ID1 %>'));
            $("#ddlCategory2").val(parseInt('<%= Model.Category_ID2 %>'));
            $("#ddlCategory3").val(parseInt('<%= Model.Category_ID3 %>'));
            $("#ddlCategory4").val(parseInt('<%= Model.Category_ID4 %>'));
            $("#ddlCategory5").val(parseInt('<%= Model.Category_ID5 %>'));
            $("#ddlIsHTML").val('<%= Model.IsHTML %>');
        });
  </script>
</asp:Content>
