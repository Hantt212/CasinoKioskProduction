/// <reference path="jquery-1.9.1.intellisense.js" />
//Load Data in Table when documents is ready
$(document).ready(function () {
  
});

//Load Data function
function getDetail(auth) {
    number  = 0;
    $.ajax({
        url: "/HTRTicketPromotion/DetailMarket",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        data :{
            authorizer: auth
        },
        dataType: "json",
        success: function (result) {
            var html = '';
            $('#detailMarTbl').DataTable().destroy();
            $.each(result, function (key, market) {
                number++;
                html += '<tr>';
                html += '<td>' + number + '</td>';
                
                html += '<td>' + market.Authorizer + '</td>';
                html += '<td>' + convertDateFromCsToJs(market.Status) + '</td>';
                html += '<td>' + market.PlayerID + '</td>'; 
                html += '<td>' + market.PlayerName + '</td>';
                
                html += '</tr>';
            });
            $('#detailMarTbd').html(html);

           

            $('#detailMarTbl').DataTable({
                "bDestroy": true,
                "order": [[0, "asc"]],
                "dom": 'Bfrtip',
                "buttons": [
                     'copy', 'csv', 'excel', 'pdf', 'print'
                ]
            });

            $('#detailMarketModal').modal('show');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function convertDateFromCsToJs(value) {
    if (value) {
        var pattern = /Date\(([^)]+)\)/;
        var results = pattern.exec(value);
        var dt = new Date(parseFloat(results[1]));
        return dt.getDate() + "/" + (dt.getMonth() + 1) + "/" + dt.getFullYear();
    } else {
        var date = new Date();
        return date.getDate() + "/" + (date.getMonth() + 1) + "/" + date.getFullYear();
    }

}

