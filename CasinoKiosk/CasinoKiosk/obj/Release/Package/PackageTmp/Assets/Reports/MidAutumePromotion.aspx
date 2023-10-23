<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MidAutumePromotion.aspx.cs" Inherits="CasinoKiosk.Assets.Reports.MidAutumePromotion" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.2.1.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <style>
        @media print { 
            #MidAutumeReport {
               width: 800px; max-height: 1000px; 
            } 
        }

        /*table{
            border-width:thin;
            border-style:solid;
        }*/
    </style>
    <script type="text/javascript">
        function doPrint() {
             var prtContent = document.getElementById('<%= ReportMidAutume.ClientID %>');
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
<body style="text-align: center">

    <form id="form1" runat="server" style="display:inline-block">
        <div style="margin-top: 20px; margin-bottom: 20px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>            
           <asp:Button CssClass="btn btn-primary mt-3 " ID="btnShow" runat="server" OnClick="btnShow_Click" Style="text-align:center" Text="Show Report" />

           <%-- <input type="submit" name="btnShow" value="Show Report" id="btnShow" class="btn btn-primary mt-3" style="float: right" />--%>
            <asp:Button ID="btnPrint" CssClass="btn btn-warning mt-3"  runat="server" OnClientClick="doPrint();"  Style="text-align:center; margin-left: 10px" Text="Print Report"  />
            <br />
            
            
            
        </div>
        <div>
            <rsweb:ReportViewer ID="ReportMidAutume" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1200px" Height="1200px" BorderStyle="Solid" BorderWidth="1px" SizeToReportContent="True" ShowPrintButton="True" Font-Bold="True">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>