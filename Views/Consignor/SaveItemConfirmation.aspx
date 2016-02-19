<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Auction>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>
<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
  <title>Saving item confirmation - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
  <div class="center_content">
<%--    <h1 class="title">Saving item confirmation</h1>--%>
    <%SessionUser cuser = AppHelper.CurrentUser; 
      if (Model == null)
       { %>
      Sorry, an error occurred while processing your request. Please try to add/edit the auction item again. <br class="clear" />      
    <%}
       else
       {%>
    <table>
    <tr>
      <td style="width:200px" class="bordered" ><b>Lot#</b></td><td><%=Model.Lot%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Title:</td><td><%=Model.Title%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Status:</td><td><%=((Consts.AuctionStatus) Model.Status).ToString()%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Start:</td><td><%=Model.StartDate%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">End:</td><td><%=Model.EndDate%></td>
    </tr>
    <tr>
      <td><b>Listing Time:</b>  (DATE IN)</td><td><%=Model.NotifiedOn%></td>  
    </tr>
    <tr>
      <td colspan="2"><hr /></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Event:</td><td><%=(Model.Event!=null)?Model.Event.Title : "----"%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Category:</td><td><%=Model.FullCategory%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Taxable:</td><td><%=(Model.IsTaxable) ? "yes" : "no"%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Opening Bid:</td><td><%=Model.Price.GetCurrency()%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Reserve Price:</td><td><%=Model.Reserve.GetCurrency()%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Cost:</td><td><%=Model.Cost.GetCurrency()%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Location:</td><td><%=Model.Location%></td>
    </tr>
   <tr>
      <td style="font-weight:bold">Featured:</td><td><%=(Model.IsFeatured) ? "yes" : "no"%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Bold:</td><td><%=(Model.IsBold) ? "yes" : "no"%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Internal Id#:</td><td><%=Model.InternalID%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Shipping:</td><td><%=Model.Shipping.GetCurrency()%></td>
    </tr>
    <tr>
      <td style="font-weight:bold">Charge Multiple Shipping:</td><td><%=(Model.IsMultipleShipping) ? "yes" : "no"%></td>
    </tr>
     <tr>
      <td style="font-weight:bold">Consignor Shipped:</td><td><%=(Model.IsConsignorShip) ? "yes" : "no"%></td>
    </tr>
    <tr>
      <td colspan="2"><hr /></td>
    </tr>
    <tr>
      <td colspan="2"><b>Description:</b><br /><%=Model.Description%></td>
    </tr>
  </table>
  <br />  
    
    <% if (cuser.IsSellerType){%>
      <button type="button" class="btn btn-primary text-uppercase btn-lg" style="//width:100px" onclick="window.location='<%=Url.Action("AddAuction", "Consignor", new {id = Model.Event.ID})%>'">
        <span>Add more</span>
      </button>  
      <button type="button" class="btn btn-info text-uppercase btn-lg" style="//width:65px" onclick="window.location='<%=Url.Action("ModifyAuction", "Consignor", new {id = Model.ID})%>'">
        <span>Edit</span>
      </button> 
      <button type="button" class="btn btn-danger pull-right text-uppercase btn-lg" style="//width:65px; margin-left:2px;" onclick="window.location='<%=Url.Action("Index", "Consignor")%>'">
        <span>Cancel</span>
      </button>  
    <% } else {
        byte edttype = Convert.ToByte(ViewData["edittype"]); %>
       <button type="button" class="btn btn-info text-uppercase btn-lg" onclick="window.location='<%=Url.Action("ModifyAuction", "Consignor", new {id = Model.ID, edittype=edttype})%>'">
        <span>Edit</span>
      </button> 
      <button type="button" class="btn btn-info text-uppercase btn-lg" onclick="window.location='<%=Url.Action("AuctionDetail", edttype==1?"Auction":"Preview", new {id = Model.ID})%>'">
        <span>Back to item</span>
      </button>  
    <% }
  }%>

  <% if (cuser.IsSellerType)
     {%>
        <br class="clear" />
        <div class="back_link">
          <%=Html.ActionLink("Consigned Items", "EditAuction", new {controller = "Consignor", action = "EditAuction"})%>
          &nbsp;&nbsp;|&nbsp;&nbsp;
          <%=Html.ActionLink("Return to Seller Tool page", "Index", new {controller = "Consignor", action = "Index"})%>
        </div>
  <% }%>
  </div>
</asp:Content>