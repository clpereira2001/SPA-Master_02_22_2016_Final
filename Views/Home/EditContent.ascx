<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<EditContentParams>" %>
<%@ Import Namespace="Vauction.Utils.Autorization" %>

<%SessionUser cuser = AppHelper.CurrentUser; 
  if (cuser!=null && cuser.IsAdminType){%>
  <span id="spEditContent" class="editcontent_link" style="margin-left:20px; cursor:pointer; color:navy"><u>Edit Content</u></span>
  <div id="dvEditPanel" style="display:none">
  <span id="spCancelContent" style="margin-left:20px; cursor:pointer; color:navy"><u>Cancel</u></span> 
  <span id="spPreviewContent" style="margin-left:20px; cursor:pointer; color:navy"><u>Preview</u></span>
  <span id="spSaveContent" style="margin-left:20px; cursor:pointer; color:navy"><u>Save Content</u></span>  
  </div>
  <br />
    <%string sContent = "";
      using (System.IO.StreamReader sr = new System.IO.StreamReader(Server.MapPath("~/Views/Home/" + Model.File + ".ascx")))
      {
          string line;
          int knt = 0;
          while ((line = sr.ReadLine()) != null)
          {
              if (knt > 0)
                  sContent += line.Trim();
              knt++;
          }
          sContent = sContent.Trim();
      }
      
      %>
  <textarea id="PageContent" rows="15" cols="100" style="display:none"><%=sContent %></textarea>
  <br />

  <script type="text/javascript">
    $(document).ready(function () {
      $("#spEditContent").click(function () {
        $("#dvEditPanel, #PageContent").show();
        $("#spPreviewContent").show();
        $("#spEditContent,#dvContent").hide();
        $('#PageContent').attr('value', $("#dvContent").html());
        tinyMCE.init({
          mode: "exact",
          elements: "PageContent",
          theme: "modern",
          plugins: "pagebreak,layer,table,save,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,template,anchor,charmap,hr,image,link,emoticons,code,textcolor",
          fontsize_formats: "8pt 9pt 10pt 11pt 12pt 14pt 16pt 18pt 20pt 22pt 26pt 36pt",
          toolbar: "undo redo pastetext | bold,italic,underline,strikethrough | formatselect | styleselect | fontselect | fontsizeselect",
          theme_advanced_toolbar_location: "top",
          theme_advanced_toolbar_align: "left",
          theme_advanced_statusbar_location: "bottom",
          theme_advanced_resizing: false,
          extended_valid_elements: "style[*]",
          width: "<%=Model.Width %>",
          height: "<%=Model.Height %>"
        });
      });
      $("#spCancelContent").click(function () {
        $("#spEditContent, #dvContent").show();
        if (tinyMCE.get('PageContent')!=null)
          tinyMCE.get('PageContent').remove();
        $("#dvEditPanel, #PageContent").hide();
      });
      $("#spPreviewContent").click(function () {
        $("#dvContent").html(tinyMCE.get('PageContent').getContent());
        //if (tinyMCE.get('PageContent') != null)
        //  tinyMCE.get('PageContent').remove();
        $("#spPreviewContent,#PageContent").hide();
        $("#spEditContent,#dvContent").show();
      });
      $("#spSaveContent").click(function () {
        var _content = (tinyMCE.get('PageContent') != null) ? tinyMCE.get('PageContent').getContent() : $("#dvContent").html();
        $("#dvContent").html(_content);
        $("#spCancelContent").click();
        $.post('/Home/EditContent', { file: "<%=Model.File %>", content: _content }, function (data) {
          if (data.Status != null && data.Status == "ERROR") {
            MessageBox("Update content", data.Message, null, "error");
          }
        }, 'json');
      });
    });
  </script>  
<%} %>


<% Html.RenderPartial(Model.File);%>
