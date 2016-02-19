<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">

  <script src="<%=this.ResolveUrl("~/public/scripts/jquery-1.4.1.min.js") %>" type="text/javascript"></script>
  
   <style type="text/css">
      #largeImg {
	      border: solid 1px #ccc;
	      width: 680px;	      
	      padding: 5px;
      }
      .thumbs img {
	      border: solid 1px #ccc;
	      width: 120px;	      
	      padding: 4px;
      }
      .thumbs img:hover {
	      border-color: #FF9900;
      }      
      
      #blankImg
      {
        border: solid 0px #ccc;	      
      }
      
      #blankImg:hover
      {
        border: solid 0px #ccc;	      
      }
      
      .btns
      {
      	
      	
      }
      .btns img {
	      border: solid 1px white;
	      padding-top:0px;
      	padding-bottom:0px;
      	margin-top:px;
      	margin-bottom:0px;
      	height:16px;      	
      }
      .btns img:hover {
	      border-color: #CCC;
      }
     
      #fileUpload
        {
            position: absolute;
        }
        .customFile
        {
            width: 200px;
            margin-left: -70px;
            //margin-right:-50px;
            //width:100px;      
            cursor: default;
            height: 21px;
            z-index: 2;
            filter: alpha(opacity: 0);
            opacity: 0;
            top:5px;
            //top:28px;
        }
        .fakeButton
        {
            position: absolute;
            z-index: 1;
            width: 120px;
            height: 21px;
            margin-left:10px; 
            //margin-left:-120px;            
            top:10px;
            background: url('/public/images/Add.png') no-repeat right top;               
            float: left;
            //top:28px;
        }
       
        .blocker
        {
            position: absolute;
            z-index: 3;
            width: 170px;
            margin-left: -70px;   
            //margin-right:-50px;
            //width:55px;                        
            height: 21px;            
            top:5px;
            background: url('/public/images/transparent.gif');     
            //top:28px;              
        }      
        #activeBrowseButton
        {
            background: url('/public/images/Add.png') no-repeat right top;
            display: none;
        }
        
   </style>
   
   <title></title>
</head>
<body>  
  <% List<string> Lst = ViewData["Images"] as List<string>;
     Int64 id = Convert.ToInt64(ViewData["AuctionID"]);
     string path = AppHelper.GetSiteUrl("/public/AuctionImages/"+(string.Format("{0}/{1}/{2}", id / 1000000, id / 1000, id)) + "/");
  string completeURL; %>        
  <div id="dvMain">
  <table style="table-layout:fixed;" cellpadding="1" cellspacing="1" >
    <colgroup>
      <col style="width:132px" /><col style="width:132px" /><col style="width:132px" /><col style="width:132px" /><col style="width:132px" />
    </colgroup>     
   <tr class="btns">
      <%for (int i = 0; i < Lst.Count; i++)
        {%>               
        <td style="padding-right:0px;">
          <div style='float:left; width:50px'>
            <img src="/public/images/Delete.png" alt="Remove image" onclick="SubmitFrm('#btnSubmitRemove<%=i %>')" />
          </div>            
          <div style="float:left;width:20px">
            <%if (i > 0)
              { %><img src='/public/images/Arrow_Left.png' onclick="MoveImage(true, '#btnSubmitRemove<%=i %>', <%=i %>)" /><%}
              else
              {%>&nbsp;<%} %>
          </div>
          <div style="float:left; width:20px">    
            <% if (i + 1 != Lst.Count)
               { %><img src='/public/images/Arrow_Right.png' onclick="MoveImage(false, '#btnSubmitRemove<%=i %>', <%=i %>)" /><%}
               else
               {%>&nbsp;<%} %>
          </div>
          <% using (Html.BeginForm("ImageEditedUpload", "Consignor", FormMethod.Post, new { enctype = "multipart/form-data" })){%>
            <input type="hidden" name="img" value="<%=Lst[i] %>" />
            <input type="hidden" id="direction<%=i %>" name="direction" value="none" />            
            <input type="submit" id="btnSubmitRemove<%=i %>" style="display:none" />
          <%}%>
        </td>
      <%} %>
      <% if (Lst.Count + 1 < 6)
         { %>
         <td style="text-align:right; padding-right:0px; height:10px; z-index: 10; width:132px;"> &nbsp;
          <% using (Html.BeginForm("ImageEditedUpload", "Consignor", FormMethod.Post, new { enctype = "multipart/form-data" }))
             {%>
            <div id="wrapper">
              <input type="file" id="fileUpload" name="fileUpload" size="1" />                 
            </div>
            <input type="submit" id="btnSubmit" style="display:none" />
          <%}%>
         </td>
         <%for (int i = Lst.Count + 1; i < 5; i++)
          { %>
            <td style='width:132px;'>&nbsp;</td>
          <%} %>                
      <%} %>
    </tr>
    <tr class="thumbs">
      <%for (int i = 0; i < Lst.Count; i++)
       {
         completeURL = path + Html.Encode(Lst[i]); %>
         <td style="padding:0px; text-align:center;width:132px;vertical-align:top;"><a href="<%=completeURL%>" title="Image <%=(i+1).ToString() %>"><img src="<%=completeURL %>" /></a></td>
      <%} %>
      <% if (Lst.Count + 1 < 6)
         { %>
         <td style="padding:0px; text-align:center;width:132px;border: solid 1px #ccc;vertical-align:middle; height:140px;">
          <img alt="Upload image" id="blankImg" src="/public/images/UploadBlank.png" />
         </td>
      <%} %>
       <%for (int i = Lst.Count + 1; i < 5; i++)
          { %>
            <td style='width:132px'>&nbsp;</td>
          <%} %>  
    </tr>
    <% if (Lst.Count > 0)
       {
         completeURL = path + Html.Encode(Lst[0]); %>
     <tr>
      <td colspan="5">&nbsp;</td>
     </tr>
     <tr>
      <td colspan="5" style="text-align:center;">
        <img id="largeImg" src="<%=completeURL %>" alt="<%=Lst[0] %>" style="width:700px" />
      </td>
    </tr>
    <%} %>
  </table>
  </div>
  
<script type="text/javascript">
  $(document).ready(function() {
    $(".thumbs a").click(function() {
      var largePath = $(this).attr("href");
      var largeAlt = $(this).attr("title");
      $("#largeImg").attr({ src: largePath, alt: largeAlt });

      $("#ifrm", top.document).attr("height", parseInt($("#dvMain").height()) + 20);

      return false;
    });

    if (parseInt($("#ifrm", top.document).attr("height")) < parseInt($("#dvMain").height()))
      $("#ifrm", top.document).attr("height", parseInt($("#dvMain").height()) + 20)
  });

      function SubmitFrm(btn) {
        $(btn).click();
      }

      function MoveImage(IsLeft, btn, num) {
        $("#direction"+num).attr("value", IsLeft);        
        $(btn).click();
      }

      window.onload = WindowOnLoad;
      var fileInput = document.getElementById('fileUpload');      
      var activeButton = document.createElement('div');
      var bb = document.createElement('div');
      var bl = document.createElement('div');
      function WindowOnLoad() {
        var wrap = document.getElementById('wrapper');        
        activeButton.setAttribute('id', 'activeBrowseButton');
        fileInput.value = '';
        fileInput.onchange = HandleChanges;
        fileInput.onmouseover = MakeActive;
        fileInput.onmouseout = UnMakeActive;
        fileInput.className = 'customFile';
        bl.className = 'blocker';
        bb.className = 'fakeButton';
        activeButton.className = 'fakeButton';
        wrap.appendChild(bb);
        wrap.appendChild(bl);
        wrap.appendChild(activeButton);        
      };
      
      function HandleChanges() {
        $("#btnSubmit").click();
      };
      
      function MakeActive() {
        activeButton.style.display = 'block';
      };
      function UnMakeActive() {
        activeButton.style.display = 'none';
      };
  </script>
</body>
</html>