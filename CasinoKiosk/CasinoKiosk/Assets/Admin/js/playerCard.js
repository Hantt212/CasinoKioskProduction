$(document).ready(function () {
    loadData('');

    const inputElement = document.getElementById('query');
    inputElement.addEventListener('focusout', function (event) {
        var queryVal = $('#query').val();
        loadData(queryVal);
    });

    $("#fCardID").focus();

    //prevent auto tab
    $(document).keypress(
        function (event) {
          if (event.which == '13') {
              event.preventDefault();
          }
  });

});





function clickRegister() {
    document.getElementById('patronID').setCustomValidity('');
    document.getElementById('fCardID').setCustomValidity('');
}
function changeFCardID()
{
    var cardVal = +$('#fCardID').val();
    $('#fCardID').val(cardVal);
}

function changeIsVisitor(item) {
    if (item.checked == false) {
        $("#lblPassportID").css("display", "none");
        $("#passportID").css("display", "none");
    } else {
        $("#lblPassportID").css("display", "block");
        $("#passportID").css("display", "block");
    }
}


function submitForm(e) {
    e.preventDefault();
    var isVisitor = $("#chkVisitor").prop("checked");
    var passport = "";
    if (isVisitor == true) {
        passport = $("#passportID").val();
    }

    $.ajax({
        url: "/PlayerCard/RegisterPlayer",
        type: 'POST',
        data: {
            fcardId: $("#fCardID").val(),
            patronId: $("#patronID").val(),
            passportId: passport,
            isVisitor: isVisitor
        },
        dataType: "json",
        success: function (result) {
            
            if (+result == -3){
                $("#mTimeOut").modal("show");
            } else {
                loadData();
                if (+result == -1) {
                    const inp = document.getElementById('patronID');
                    inp.setCustomValidity("Patron ID is duplicated!");
                    inp.reportValidity();
                } else if (+result == -2) {
                    const inp = document.getElementById('fCardID');
                    inp.setCustomValidity("Card ID is duplicated!");
                    inp.reportValidity();
                } else {
                    $("#fCardID").val('');
                    $("#patronID").val('');
                    $("#passportID").val('');
                }
            }
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    })

}

function loadData(param) {
    $.ajax({
        url: "/PlayerCard/getInitData",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        data: { query: param },
        dataType: "json",
        success: function (data) {

            var table = $('#datatable-json').DataTable({
                data: data,
                searching: false,
                lengthChange: false,
                columns: [

                  { "data": "FCardID" },
                  { "data": "PID" },
                  { "data": "PassportID" },
                  { "data": "IsVisitor" },
                  { "data": "IsActive" },
                  { "data": "DateInserted" },
                  { "data": "DateUpdated" },
                  { "data": "UpdatedBy" },
                  {
                      "data": "ID",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              data = '<a href="#" onclick="return GetCardInfoByID(' + data + ')">Edit</a> | <a href="#" onclick="DelCardInfoByID(' + data + ')">Delete</a>';
                          }
                          return data;
                      }
                  },

                ],
                responsive: true,
                destroy: true,
                order: [[8, 'desc']]

            });

        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    });

   
}

function GetCardInfoByID(Id) {
    $.ajax({
        url: "/PlayerCard/GetCardInfoByID",
        type: 'GET',
        data: {
            Id: +Id
        },
        dataType: "json",
        success: function (result) {
            $('#mID').val(result.ID);
            $("#mPID").val(result.PID);
            $("#mCardID").val(result.FCardID);
            $("#mPassportID").val(result.PassportID);
            if (result.Remark == "Visitor") {
                $("#mChkVisitor").prop("checked", true);
                $("#mPassportDiv").css("display", "block");
            } else {
                $("#mChkVisitor").prop("checked", false);
                $("#mPassportDiv").css("display", "none");
            }
            $('#mCardInfo').modal("show");

        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    })

}


function changeVisitorModel(item) {
    if (item.checked == false) {
        $("#mPassportDiv").css("display", "none");
    } else {
        $("#mPassportDiv").css("display", "block");
    }
}

function changeMFCardID() {
    var cardVal = +$('#mCardID').val();
    $('#mCardID').val(cardVal);
}

function UpdateCardInfoByID() {
    var isVisitor = $("#mChkVisitor").prop("checked");
    var passport = "";
    if (isVisitor == true) {
        passport = $("#mPassportID").val();
    }

    $.ajax({
        url: "/PlayerCard/UpdateCardInfoByID",
        type: 'GET',
        data: {
            Id: +$('#mID').val(),
            fCardID: +$("#mCardID").val() + "",
            passport:passport,
            isVisitor: isVisitor
        },
        dataType: "json",
        success: function (result) {
            
            $('#mCardInfo').modal("hide");
            if (+result == -3) {
                $("#mTimeOut").modal("show");
            } else {
                loadData('');
            }
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    })
}

function DelCardInfoByID(Id) {
    $("#mCardIdDel").val(Id)
    $('#mConfirmDel').modal("show");
}

function ExecDelCardInfoByID() {
    $.ajax({
        url: "/PlayerCard/DelCardInfoByID",
        type: 'GET',
        data: {
            Id: +$("#mCardIdDel").val()
        },
        dataType: "json",
        success: function (result) {
            $('#mConfirmDel').modal("hide");
            loadData('');
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    })
}