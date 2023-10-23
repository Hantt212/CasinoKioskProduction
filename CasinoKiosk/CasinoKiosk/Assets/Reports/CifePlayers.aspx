<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CifePlayers.aspx.cs" Inherits="CasinoKiosk.Assets.Reports.CifePlayers" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.2.1.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>



    <script src="../Scripts/jquery-ui-1.11.4/jquery-ui.min.js"></script>
    <link href="../Scripts/jquery-ui-1.11.4/jquery-ui.min.css" rel="stylesheet" />

     <%--<script  type="text/javascript">
        $(document).ready(function () {
            var isChrome = /Chrome/.test(navigator.userAgent) && /Google Inc/.test(navigator.vendor);
            initDatePickers();
            Sys.WebForms.PageRequest
         r.getInstance().add_endRequest(applicationInitHandler);

            function applicationInitHandler() {
                initDatePickers();
            }

            function initDatePickers() {
                if (isChrome) {
                    $('[id*=ParameterTable] td span:contains("Date")')
	                .each(function () {
	                    var td = $(this).parent().next();
	                    $('input', td).datepicker({
	                        format: 'd/m/Y'
	                    });
	                });
                }
            }
            
        });
        // end Fix Chrome
    </script>--%>
    </head>
    <body>
        
        <form id="form1" runat="server">
        <div style="width: 731px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>            
            
            
            
            <br />
            <rsweb:ReportViewer ID="ReportViewerPlayers" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1200px" Height="1200px" SizeToReportContent="True" ShowPrintButton="True" Font-Bold="True">
            </rsweb:ReportViewer>
        </div>
    </form>




        </body>
    </html>