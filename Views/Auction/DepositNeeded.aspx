<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PreviewBid>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HTMLhead" runat="server">
    <title>Deposit needed - <%=Consts.CompanyTitleName %></title>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="center_content">
        <form method="post" action="<%=Consts.ProtocolSitePort %>/Account/PayDeposit">
            <%=Html.AntiForgeryToken() %>

            <div role="alert" class="alert alert-danger">
                You need to place a deposit of <%=Model.TotalDeposit.GetCurrency()%>
            </div>
            <p>
                As per our terms and conditions, customers that reach your level of bidding are required to leave a <%=Model.DepositNeed.GetCurrency() %>  good faith auction deposit. This policy helps insure the integrity of the bidding on the site, enabling users to bid with confidence, knowing that other users are not placing fictitious bids.
      Auction deposits are fully refundable if you do not win any items, and are automatically refunded upon settlement of the auction items that you have won.

            </p>

            <%if (Model.PreviousDeposit > 0)
              { %>
            <p>
                You currently have a deposit of <%=Model.PreviousDeposit.GetCurrency() %>
                <br />
                Please place an additional deposit of <strong><%=Model.DepositNeed.GetCurrency() %></strong> in order to place your bid
          Auction deposits are fully refundable if you do not win any items, and are automatically refunded upon settlement of the auction items that you have won.
      
            </p>
            <%} %>
            <p>
                If you have any questions or problems please let us know via email or call customer support at 1-800-735-2719 Monday through Friday 8:00AM-4:30PM ET
        
            </p>



            <p>
                <button type="submit" id="submitDeposit" class="btn btn-primary btn-lg text-uppercase margin-767-top-sm"><span>Submit Deposit</span></button>

                <button type="button" id="cancelBid" style="" class="btn btn-primary btn-lg text-uppercase margin-767-top-sm" onclick="window.location='<%= Url.Action ("AuctionDetail", "Auction", new { controller = "Auction", action = "AuctionDetail", id = Model.LinkParams.ID, evnt=Model.LinkParams.EventUrl, cat=Model.LinkParams.CategoryUrl, lot=Model.LinkParams.LotTitleUrl })%>'">
                    <span>Cancel Bid</span>
                </button>

            </p>
        </form>
    </div>
</asp:Content>
