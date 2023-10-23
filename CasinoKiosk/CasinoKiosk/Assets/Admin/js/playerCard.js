$(document).ready(function () {
    loadData();
    $("#fCardID").focus();

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
function submitForm(e) {
    e.preventDefault();
    $.ajax({
        url: "/PlayerCard/RegisterPlayer",
        type: 'POST',
        data: {
            fcardId: +$("#fCardID").val(),
            patronId: $("#patronID").val(),
            isVisitor: $("#chkVisitor").prop("checked")
        },
        dataType: "json",
        success: function (result) {
            loadData();
            if (+result == -1) {
                const inp = document.getElementById('patronID');
                inp.setCustomValidity("Patron ID is duplicated!");
                inp.reportValidity();
            } else if (+result == -2) {
                const inp = document.getElementById('fCardID');
                inp.setCustomValidity("Card ID is duplicated!");
                inp.reportValidity();
            }else {
                $("#fCardID").val('');
                $("#patronID").val('');
            }

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    })

}

function loadData() {

    $.ajax({
        url: "/PlayerCard/getInitData",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (data) {

            var table = $('#datatable-json').DataTable({
                data: data,
                columns: [
                  { "data": "FCardID" },
                  { "data": "PID" },
                  { "data": "IsVisitor"},
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
            });

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
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
            if (result.Remark == "Visitor") {
                $("#mChkVisitor").prop("checked", true);
            } else {
                $("#mChkVisitor").prop("checked", false);
            }
            $('#mCardInfo').modal("show");

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    })

}

function UpdateCardInfoByID() {
    $.ajax({
        url: "/PlayerCard/UpdateCardInfoByID",
        type: 'GET',
        data: {
            Id: +$('#mID').val(),
            fCardID: $("#mCardID").val(),
            isVisitor: $("#mChkVisitor").prop("checked")
        },
        dataType: "json",
        success: function (result) {
            $('#mCardInfo').modal("hide");
            loadData();

        },
        error: function (errormessage) {
            alert(errormessage.responseText);
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
            loadData();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    })
}