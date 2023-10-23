/// <reference path="jquery-1.9.1.intellisense.js" />
//Load Data in Table when documents is ready
$(document).ready(function () {
    loadData();
    
});

//Load Data function
function loadData() {
    $.ajax({
        url: "/HTRTicketPromotion/listTicketPromotion",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, ac) {
                html += '<tr>';
                html += '<td>' + ac.ID + '</td>';
                html += '<td>' + ac.GuestPID + '</td>';
                html += '<td>' + ac.GuestName + '</td>';
                html += '<td>' + ac.isPrinted + '</td>';
                html += '<td>' + ac.IssuedDate + '</td>';
                html += '<td>' + ac.IssuedTime + '</td>';
                html += '<td>' + ac.PrintedBy + '</td>';
                html += '<td>' + ac.PrintedQty + '</td>';
                html += '<td>' + ac.isReprinted + '</td>';              
                html += '<td>' + ac.isReprintedBy + '</td>';
                html += '<td>' + ac.NumberOfReprint + '</td>';
                //html += '<td>' + ac.isVoided + '</td>';
                //html += '<td>' + ac.VoidedBy + '</td>';
                //html += '<td>' + ac.VoidedDate + '</td>';
                html += '<td><a href="#" onclick="return Reprint(' + ac.ID + ')">Reprint</a>';
                //html += '<td><a href="#" onclick="return Reprint(' + ac.ID + ')">Reprint</a> | <a href="#" onclick="Void(' + ac.ID + ')">Void</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function enableItemYes(ItemPoints, PlayerID) {

    if (confirm('Are you sure you want to Enable this Item?')) {

        $.ajax({
            url: "/Logs/enableItemYes",
            type: "GET",
			data:{
			    ItemPoints: +ItemPoints,
			    PlayerID: +PlayerID
			},
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}

function reprintMiniBuffet() {
    $('#printBuffet').prop('disabled', true);
    return confirm('Are you sure you wish to reprint this ticket?');
}

//Insert Data Function 
function Add()
{
    //var res = validate();
    //if (res == false) {
    //    return false;
    //}    
    var acObj = {
        ID: $('#ID').val(),       
        GuestPID: $('#GuestPID').val(),
        GuestName: $('#GuestName').val(),
        isPrinted: $('#CallRecord').val(),
        PrintedDate: $('#ChatRecord').val(),
        PrintedQty: $('#MeetRecord').val(),
        PrintedBy: $('#GuestPID').val(),
        isReprinted: $('#GuestName').val(),
        ReprintedDate: $('#CallRecord').val(),
        isReprintedBy: $('#ChatRecord').val(),
        isVoided: $('#MeetRecord').val(),
        VoidedBy: $('#ChatRecord').val(),
        VoidedDate: $('#MeetRecord').val()
    };
    $.ajax({
        url: "/HTRTicketPromotion/Add",
        data: JSON.stringify(acObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {

            if (result == 1) {
                loadData();
                $('#myModal').modal('hide');
            }
            else {
                alert("This player has had ticket printed!");
            }                     
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//Function for getting the Data Based upon ID


//Function for voiding the ticket
function Void(ID) {

    //$('#CasinoKiosk_UserName').css('border-color', 'lightgrey');
    //$('#isActive').css('border-color', 'lightgrey');   
    //$('#UserEmailAddress').css('border-color', 'lightgrey');

    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/Void/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadData();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}
//Function for reprinting the ticket
function Reprint(ID) {

    //$('#CasinoKiosk_UserName').css('border-color', 'lightgrey');
    //$('#isActive').css('border-color', 'lightgrey');   
    //$('#UserEmailAddress').css('border-color', 'lightgrey');
    if (confirm('Are you sure you want to reprint this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/Reprint/" + ID,
            type: "GET",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadData();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}

function PrintGoldenHour(PlayerID) {
    if (confirm('Are you sure you want to reprint this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/PrintGoldenHour/" ,
            type: "GET",
            contentType: "application/json;charset=UTF-8",
			data:{
				PlayerID: +PlayerID
			},
            dataType: "json",
			
            success: function (result) {
               // loadData();
			  window.location.reload();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}

function pressPrint(playerID) {
    $('#printGolden').prop('disabled', true);
    window.open('/HTRTicketPromotion/PrintGoldenHour?PlayerID=' + playerID)
}

function hideInfor() {
    $('#formPatronInfo').hide();
}
function VoidGoldenHour(ID) {
    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/VoidGoldenHour/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}


function ReprintGoldenHour(ID) {
    if (confirm('Are you sure you want to reprint this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/ReprintGoldenHour/" + ID,
            type: "GET",
            contentType: "application/json;charset=UTF-8",
			data:{
				ID: +ID
			},
            dataType: "json",
            success: function (result) {
               // loadData();
			  window.location.reload();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}

//Add 20230809 Hantt start
function MiniBuffetRePrint(PlayerID) {
    if (confirm('Are you sure you want to reprint this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/MiniBuffetPrint/",
            type: "GET",
            contentType: "application/json;charset=UTF-8",
            data: {
                PlayerID: +PlayerID,
                PlayerName: PlayerName
            },
            dataType: "json",

            success: function (result) {
                // loadData();
                window.location.reload();

            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
        return false;
    }
}

function MiniBuffetPrint(playerID, playerName) {
    $('#printBuffet').prop('disabled', true);
    window.open('/HTRTicketPromotion/MiniBuffetPrint?PlayerID=' + playerID + "&PlayerName=" + playerName)
}

//Sunday promotion start
function SundayPromotionPrint(playerID, playerName) {
    $('#sundayPrintBtn').prop('disabled', true);
    window.open('/HTRTicketPromotion/PrintSundayPromotion?PlayerID=' + playerID + "&PlayerName=" + playerName)
}

function ReprintSundayPromotion() {
    $('#sundayPrintBtn').prop('disabled', true);
    return confirm('Are you sure you wish to reprint this ticket?');
}

function VoidSundayPromotion(ID) {
    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/VoidSundayPromotion/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();

            },
            error: function (errormessage) {
                alert('You do not have permission');
            }
        });
        return false;
    }
}
//Sunday promotion end

//Mid Autume start
function PrintMidAutume(playerID, playerName) {
    $('#midAutPrintBtn').prop('disabled', true);
    window.open('/HTRTicketPromotion/PrintMidAutumePromotion?PlayerID=' + playerID + "&PlayerName=" + playerName)
}

function ReprintMidAutume() {
    $('#midAutPrintBtn').prop('disabled', true);
    return confirm('Are you sure you wish to reprint this ticket?');
}

function VoidMidAutume(ID) {
    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/VoidMidAutumePromotion/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();

            },
            error: function (errormessage) {
                alert('You do not have permission');
            }
        });
        return false;
    }
}
//Mid Autume end

function hideInfo() {
    $('#formPatronInfoMiniBuffet').hide();
}
function VoidMiniBuffet(ID) {
    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/VoidMiniBuffet/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                window.location.reload();

            },
            error: function (errormessage) {
                alert('You do not have permission');
            }
        });
        return false;
    }
}

//Add 20230809 Hantt end



////Function for getting the PlayerName on PID
function Check() {

    //$('#CasinoKiosk_UserName').css('border-color', 'lightgrey');
    //$('#isActive').css('border-color', 'lightgrey');   
    //$('#UserEmailAddress').css('border-color', 'lightgrey');
    //$('#GuestPID').val("test");
    
        $.ajax({
            url: "/HTRTicketPromotion/GetNameByPID",
            type: "POST",
            data: JSON.stringify({
                PID: $("#GuestPID").val()
            }),
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {

                $('#GuestName').val(result);

                $('#myModal').modal('show');
                $('#btnAdd').show();
                $("#btnAdd").attr("disabled", false);
                $("#btnCheck").attr("disabled", false);
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
     
    return false;
}
//function for updating player's record
function Update() {
    //var res = validate();
    //if (res == false) {
    //    return false;
    //}
    var acObj = {
        ID: $('#ID').val(),       
        GuestPID: $('#GuestPID').val(),
        GuestName: $('#GuestName').val(),
        CallRecord: $('#CallRecord').val(),
        ChatRecord: $('#ChatRecord').val(),
        MeetRecord: $('#MeetRecord').val()
    };
    //alert($('#RoleId').val());
    $.ajax({
        url: "/ActivityTracking/Update",
        data: JSON.stringify(acObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            
            $('#myModal').modal('hide');           
            $('#ID').val();                     
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//function for deleting player's record
function Delele(ID) {
    var ans = confirm("Are you sure you want to delete this Record?");
    if (ans) {
        $.ajax({
            url: "/FOPatron/Delete/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadData();
                
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
}

//Function for clearing the textboxes
function clearTextBox() {
    
    
    $('#GuestPID').val("");
    $('#GuestName').val("");   

    $('#myModalLabel').text("Issue New Ticket")
    
    $("#btnAdd").attr("disabled", true);       
}
//Valdidation using jquery
function validate() {
    var isValid = true;
    //if ($('#UserID').val().trim() == "") {
    //    $('#UserID').css('border-color', 'Red');
    //    isValid = false;
    //}
    //else {
    //    $('#UserID').css('border-color', 'lightgrey');
    //}
    if ($('#FOPatron_UserName').val().trim() == "") {
        $('#FOPatron_UserName').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#FOPatron_UserName').css('border-color', 'lightgrey');
    }
    if ($('#isActive').val().trim() == "") {
        $('#isActive').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#isActive').css('border-color', 'lightgrey');
    }    
    return isValid;
}