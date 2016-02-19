<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<ConsignNowForm>" %>

<asp:Content ID="Content2" ContentPlaceHolderID="HTMLhead" runat="server">
    <% Html.Script("MicrosoftAjax.js"); %>
    <% Html.Script("MicrosoftMvcAjax.js"); %>
    <% Html.Script("MicrosoftMvcValidation.js"); %>
    <% Html.Script("jquery.uploadify.js"); %>
    <% Html.Script("general.js"); %>
    <%Html.StyleUrl("/public/css/custom-gray/jquery-ui-1.7.2.css");
      Html.Script("/public/scripts/jquery-ui-1.7.2.custom.min.js");
      Html.StyleUrl("/public/css/jquery.motionCaptcha.0.2.css");%>
    <title>Consign Now - <%=Consts.CompanyTitleName %> </title>
    <style type="text/css">
        .message_list {
            list-style: none;
            margin: 0;
            padding: 0;
            width: 100%;
        }

            .message_list li {
                padding: 0;
                margin: 0;
            }

        .message_head {
            background-color: #002868;
            color: #FFFFFF;
            padding: 5px;
            cursor: default;
            font-weight: bold;
            width: 100%;
        }

        .message_body {
            padding: 0 0 5px 0;
            cursor: default;
            margin: 0;
        }

        .dialog_form {
            padding: 0;
        }

        .images {
            list-style-type: none;
            margin: 0;
            padding: 0;
        }

            .images li {
                margin: 3px 3px 3px 0;
                padding: 0;
                float: left;
                text-align: center;
                width: 120px;
                height: 115px;
                background: #f9f9f9;
                border: 1px solid #cdcdcd;
            }

                .images li img.img {
                    max-width: 110px;
                    max-height: 110px;
                    padding: 2px;
                }

                .images li img.remove {
                    border: 1px solid #cdcdcd;
                    background-color: #f2f2f2;
                    cursor: pointer;
                    float: right;
                    margin: 2px;
                }

        .delObject {
            position: absolute;
            top: 0;
            right: 0;
            margin: -5px -5px 0 0;
            display: block;
            background: url("/public/images/dellSprite.png") no-repeat 0 -21px;
            width: 20px;
            height: 20px;
        }

            .delObject:hover {
                background-position: 0 0;
            }

        .field-validation-error {
            color: red;
        }

        .mb0 {
            margin-bottom: 0;
        }
    </style>
    <% Html.EnableClientValidation(); %>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="col-md-9 col-sm-8">
        <div class="center_content left">
            <% if (ViewData["Error"] != null)
               { %>
            <div style="padding: 5px; color: #800000; border: dotted 1px #800000; margin: 5px; font-family: Arial, Helvetica, sans-serif; font-size: 14px;">
                <%:ViewData["Error"] %>
            </div>
            <% } %>

            <% using (Html.BeginForm("ConsignNow", "Consignor", FormMethod.Post, new { id = "ConsignNowForm" }))
               {
    %>
            <%: Html.ValidationSummary(true) %>
            <%: Html.HiddenFor(model => model.ID) %>

<%--            <h1 class="title">Consign Now</h1>--%>
            <h4>Interested in Consigning?</h4>
            <p>
                If you are thinking of consigning to an upcoming FARS Auction please fill out the brief form below to begin the consignment process or you can call the Consignor Hotline at 1-800-735-2719
   
            </p>
            <h4>Tell Us About Your Items</h4>
            <p>
                Use the box below to tell us about your item(s):
   
            </p>
            <%: Html.TextAreaFor(model=>model.Description, new {@rows="15",@cols="80", @style="width:100%;margin-top:-10px;", @tabindex = "2" })%>
            <%: Html.ValidationMessageFor(model => model.Description) %>
            <p style="margin-top: 10px;">
                How did you acquire the items (inherited, purchased gallery/auction, etc.)?<br />
                <%: Html.TextBoxFor(model => model.Acquire, new { @style = "width:100%", @tabindex = "3" }) %>
            </p>
            <p class="mb0">
                For the fastest and most accurate response, please provide at least one image per item.
   
            </p>
            <div id="form_iupload" title="Batch image upload" class="dialog_form">
                <input type="file" id="biuFile" />
                Please close this form after uploading the images.
     
                <button type="button" style="width: 115px!important;" class="cssbutton small white closeUploadForm"><span>CLOSE</span></button>
            </div>
            <button type="button" style="width: 175px!important;" class="btn btn-primary text-uppercase impUpload"><span>UPLOAD IMAGE(S)</span></button>
            <div>
                <ul class="images">
                </ul>
            </div>
            <div style="clear: both;"></div>
            <div style="margin: 10px 0;">
                <p class="mb0">
                    Email:<em style="color: brown">*</em>
                    <%: Html.TextBoxFor(model => model.Email, new { @style = "width:80%;float:right", @tabindex = "4" }) %>
                </p>
                <%: Html.ValidationMessageFor(model => model.Email) %>
            </div>

            <div style="margin: 10px 0;">
                <p class="mb0">
                    First Name:<em style="color: brown">*</em>
                    <%: Html.TextBoxFor(model => model.FirstName, new { @style = "width:80%;float:right", @tabindex = "5" }) %>
                </p>
                <%: Html.ValidationMessageFor(model => model.FirstName) %>
            </div>

            <div style="margin: 10px 0;">
                <p class="mb0">
                    Last Name:<em style="color: brown">*</em>
                    <%: Html.TextBoxFor(model => model.LastName, new { @style = "width:80%;float:right", @tabindex = "6" }) %>
                </p>
                <%: Html.ValidationMessageFor(model => model.LastName) %>
            </div>

            <p>
                Primary Phone
           
                <%: Html.TextBoxFor(model => model.Phone, new { @style = "width:80%;float:right", @tabindex = "7" }) %>
                <br />
                <%: Html.ValidationMessageFor(model => model.Phone) %>
            </p>

            <%--<p>--%>
                <div class="checkbox alt-check margin-bottom-lg">
                    <label class="text-uppercase text-weight-700">
                    <%: Html.CheckBoxFor(model => model.Finance) %><span class="check-icon"></span> I am an attorney, trust officer, or financial advisor.
                    </label>
                </div>
            <%--</p>--%>

            <%--<p>--%>
            <div class="checkbox alt-check margin-bottom-lg">
                <label class="text-uppercase text-weight-700">
                <%: Html.CheckBoxFor(model => model.Subscribe) %><span class="check-icon"></span> Please include me in FARS free e-mail announcements.
                </label>
            </div>
   
            <%--</p>--%>

            <p>
                Photo(s) are highly recommended for the most accurate evaluations. Due to the high volume of requests we receive, it may take us a few days to get back to you.
   
            </p>
            <div style="height: 35px;">
                <img src='<%:Url.Action("Show", "Captcha") %>' alt="" style="border: 1px solid #ccc" />
            </div>
            <div style="margin: 10px 0;">
                <p class="mb0">
                    Enter the characters from the image:<em style="color: brown">*</em>
                    <%: Html.TextBoxFor(model => model.CaptchaValue, new { @style = "width:50%;float:right", @tabindex = "8" }) %>
                </p>
                <%: Html.ValidationMessageFor(model => model.CaptchaValue) %>
            </div>

            <div style="width: 100%; text-align: center;">
                <input style="width: 115px!important;" class="btn btn-primary text-uppercase" type="submit" value="SUBMIT" />
            </div>
            <% } %>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="cphScriptBottom" runat="server">
    <script type="text/javascript" src="/public/scripts/jquery.motionCaptcha.0.2.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $(".impUpload").click(function () {
                $('#form_iupload').dialog('open');
            });

            $(".closeUploadForm").click(function () {
                $('#form_iupload').dialog('close');
            });

            $("#form_iupload").dialog({
                bgiframe: true,
                autoOpen: false,
                height: 300,
                width: 370,
                modal: true,
                gridview: true,
                close: function () { imageRefresh(); }
            });

            $('#biuFile').fileUpload({
                'uploader': '/public/uploader.swf',
                'script': '/Consignor/ConsignNowUploadImage',
                'scriptData': { 'tmpID': '<%=Model.ID %>' },
        'cancelImg': '/public/cancel.png',
        'multi': 'true',
        'auto': 'false',
        'fileDesc': 'Image',
        'fileExt': '*.jpg;*.jpeg;*.png;',
        'method': 'POST',
        'queueSizeLimit': '5'
      });

        imageRefresh();

        function imageRefresh() {
            $(".images li").remove();
            $.post('<%=Url.Action("GetConsignNowImages", "Consignor")%>', { tmpID: '<%=Model.ID%>' }, function (data) {
            $.each(data, function (index, value) {
                imageAdd(value.Title, value.Description);
            });
        }, 'json');
    }

        function imageAdd(fileName, filePath) {
            $(".images").append('<li class="ui-state-default" style="position: relative;" rel="' + fileName + '">' +
              '<a class="delObject remove" rel="' + fileName + '" title="Delete Image"></a>' +
              '<div class="clear"></div>' +
              '<img class="img" alt="" src="' + filePath + '" />' +
              '</li>');
            $(" .remove[rel='" + fileName + "']").click(function () {
                reviewImageRemove(fileName);
            });
        }

        function reviewImageRemove(fileName) {
            ConfirmBox("DELETING IMAGE", "Do you really want to delete this image?", function () {
                $.post('<%=Url.Action("ConsignNowDeleteImage", "Consignor")%>', { tmpID: '<%=Model.ID%>', fileName: fileName }, function (data) {
                if (data.Type == 0) {
                    $(".images li[rel='" + fileName + "']").remove();
                }
            }, 'json');
        });
      }
    });
  </script>
</asp:Content>
