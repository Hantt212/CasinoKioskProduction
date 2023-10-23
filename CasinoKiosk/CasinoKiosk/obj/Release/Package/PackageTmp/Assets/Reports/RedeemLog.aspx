<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="RedeemLog.aspx.cs" Inherits="CasinoKiosk.Assets.Reports.RedeemLog1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="https://code.jquery.com/jquery-3.2.1.js"></script>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap-theme.min.css" />
    <script src="https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js"></script>
    <script src="Scripts/moment.js"></script>
    <script src="Scripts/bootstrap-datetimepicker.js"></script>
    <link href="css/pikaday.css" rel="stylesheet" />
    <link href="css/site.css" rel="stylesheet" />
    <link href="css/theme.css" rel="stylesheet" />
    <link href="css/triangle.css" rel="stylesheet" />
    <script src="pikaday.js" type="text/javascript"></script>
    <script src="moment.js" type="text/javascript"></script>
</head>
<body>

    <form id="form1" runat="server">

        <div style="width: 731px">
            <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
            <br />
            <asp:Label ID="Label1" runat="server" Text="From:  " Font-Size="12pt"></asp:Label>
            &nbsp;&nbsp;&nbsp;&nbsp;
        <asp:TextBox ID="txtFromDate" runat="server" Width="240px" Font-Size="Medium"></asp:TextBox>
            <script type="text/javascript">
                var pickerFromDate = new Pikaday(
        {
            field: document.getElementById('txtFromDate'),
            format: 'D MMM YYYY',
            firstDay: 1,
            defaultDate: new Date(),
            minDate: new Date(2019, 12, 31),
            maxDate: new Date(2030, 12, 31),
            yearRange: [2000, 2030],
            numberOfMonths: 1,
            theme: 'dark-theme'
        });


            </script>
            <asp:Label ID="Label2" runat="server" Text="To:  " Font-Size="12pt"></asp:Label>

           

        <asp:TextBox ID="txtToDate" runat="server" Width="240px" Font-Size="Medium"></asp:TextBox>
            
        <script type="text/javascript">
            var pickerToDate = new Pikaday(
    {

        field: document.getElementById('txtToDate'),
        format: 'D MMM YYYY',
        firstDay: 1,
        defaultDate: new Date(),
        minDate: new Date(2019, 12, 31),

        maxDate: new Date(2030, 12, 31),
        yearRange: [2000, 2030],
        numberOfMonths: 1,
        theme: 'dark-theme',

    });


        </script>
            <br />
            <asp:DropDownList runat="server" ID="ddlReports" Width="180px" Font-Size="12pt" Height="30px">
                <asp:ListItem Text="Items Redemption" Value="1" Selected="True"></asp:ListItem>
                <asp:ListItem Text="Daily Bonus" Value="2"></asp:ListItem>
                <asp:ListItem Text="Weekly Bonus" Value="3"></asp:ListItem>
                <asp:ListItem Text="Friday Bonus" Value="4"></asp:ListItem>
                <asp:ListItem Text="Points Redemption" Value="5"></asp:ListItem>
                <%--Add 20230526 Hantt start--%>
                <asp:ListItem Text="FO Patron Log" Value="6"></asp:ListItem>
                <%--Add 20230526 Hantt end--%>                 
            </asp:DropDownList>
            
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;

            <asp:Button ID="btnShow" runat="server" OnClick="btnShow_Click" Text="Show Report" Width="110px" Font-Size="12pt" />


            <br />
            <br />

            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Font-Names="Verdana" Font-Size="8pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="1200px" Height="1200px" SizeToReportContent="True">
            </rsweb:ReportViewer>



        </div>




    </form>
</body>
</html>

