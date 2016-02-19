<%@ Control Language="C#" AutoEventWireup="true" Inherits="System.Web.Mvc.ViewUserControl<Vauction.Models.CustomModels.AuctionFilterParams>" %>
<%@ Import Namespace="Vauction.Utils.Html" %>
<% using (Html.BeginForm("SearchByAuctionID", "Home", FormMethod.Get))
   {%>
    <div class="row">
        <div class="col-sm-12">
            <p class="text-primary text-weight-900 text-lg">Lot#</p>
        </div>
    </div>

    <div class="col-sm-6">
        <div class="row">
            <div class="form-group">
                <label for="lot">Enter Lot#</label>
                <div class="input-group full-width">                   
                    <%=Html.TextBox("LotNo", Model.LotNo, new { @class = "form-control" })%>
                    <span class="input-group-btn">                        
                        <% =Html.SubmitWithClientValidation("View Lot","btn btn-primary btn-square")%>
                    </span>
                </div>
            </div>
        </div>
    </div>


    <div class="row">
        <div class="col-sm-12">
            <p class="text-primary text-weight-900 text-lg">Search by item information</p>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-6">
            <div class="form-group">
                <label for="pf" class="text-uppercase">Price from</label>                
                <%=Html.TextBox("FromPrice", Model.FromPrice,new { @class = "form-control",placeholder="FromPrice"})%>   
            </div>
        </div>
        <div class="col-sm-6">
            <div class="form-group">
                <label for="pt" class="text-uppercase">Price to</label>               
                 <%=Html.TextBox("ToPrice", Model.ToPrice,new { @class = "form-control",placeholder="ToPrice"})%> 
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-12">
            <div class="form-group">
                <label for="Title" class="text-uppercase">Title</label>               
                 <%=Html.TextBox("Title", Model.Title,new { @class = "form-control",placeholder="Title" })%> 
                
            </div>
        </div>
    </div>

    <div class="row">
        <div class="col-sm-12">
            <div class="form-group">
                <label for="Description" class="text-uppercase">Description</label>
                <!--<input type="text" class="form-control" id="Description" placeholder="Description">-->
                <%=Html.TextBox("Description",Model.Description,new { @class = "form-control",placeholder="Description" })%>               
            </div>
        </div>
    </div>


    <div class="checkbox alt-check margin-bottom-lg">
        <label class="text-weight-700">
            <input type="checkbox" checked="">
            <span class="check-icon"></span>
            Include Closed Auctions (up to 30 days).
        </label>
    </div>

    <div class="row margin-top-lg">
        <div class="col-sm-12">            
            <%= Html.SubmitWithClientValidation("Search","hvr-wobble-bottom text-uppercase text-weight-600 btn btn-danger btn-lg padding-horizontal-xl")%>
        </div>
    </div>


<%}%>