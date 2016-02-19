<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
	<title>Free Email Registration Confirmed - <%=Consts.CompanyTitleName %></title>
</asp:Content>

<asp:Content ID="aboutContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="control congratutation">
	    <h2>Registration confirmed</h2>
	    <div class="blue_box">
            <p>
                Thank you. Your subscription was confirmed successfully.
            </p>
        </div>
    </div>    
</asp:Content>
