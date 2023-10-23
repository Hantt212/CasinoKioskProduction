<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DailyTickets.aspx.cs" Inherits="CasinoKiosk.Assets.Reports.DailyTickets" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.2.1.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>

    <script type="text/javascript">
        function doPrint() {
            var prtContent = document.getElementById('<%= ReportViewerDailyBonus.ClientID %>');
                prtContent.border = 0; //set no border here

                var WinPrint = window.open('', '', 'left=100,top=100,width=800,height=1000,toolbar=0,scrollbars=1,status=0,resizable=1');
                var is_chrome = Boolean(WinPrint.chrome);
                var isPrinting = false;
                WinPrint.document.write(prtContent.innerHTML);
                WinPrint.document.close();

                if (is_chrome) {
                    WinPrint.onload = function () { // wait until all resources loaded 
                        isPrinting = true;
                        WinPrint.focus();
                        WinPrint.print();
                        WinPrint.close();
                        isPrinting = false;
                    };
                    setTimeout(function () { if (!isPrinting) { WinPrint.print(); WinPrint.close(); } }, 300);
                }
                else {
                    WinPrint.document.close(); // necessary for IE >= 10
                    WinPrint.focus();
                    WinPrint.print();
                    WinPrint.close();
                }

                return true;
            }
    </script>


</head>
<body>

    <form id="form1" runat="server">
        <div style="width: 731px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>            
            <asp:Button ID="btnShow" runat="server" OnClick="btnShow_Click" Text="Show Report" Width="110px" Font-Size="12pt" />
            <asp:Button ID="btnPrint" runat="server" OnClientClick="doPrint();" Text="Print Report" Width="110px" Font-Size="12pt" ClientIDMode="Static" />
            <br />
            <br />
            <asp:CheckBox ID="chkGaming" runat="server" Text="Gaming"/>
            &nbsp;           
            <asp:CheckBox ID="chkFaB" runat="server" Text="F&B"/>
            <br />
            <rsweb:ReportViewer ID="ReportViewerDailyBonus" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1200px" Height="1200px" SizeToReportContent="True" ShowPrintButton="True" Font-Bold="True">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>