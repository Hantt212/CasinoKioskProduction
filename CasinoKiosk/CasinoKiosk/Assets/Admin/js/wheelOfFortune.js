$(document).ready(function () {
    loadData();
    loadPrizeInfo();
});


function loadData() {

    $.ajax({
        url: "/WheelOfFortune/getInitData",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (data) {
            var table = $('#wofLogTbl').DataTable({
                data: data,
                columns: [
                  { "data": "ID" },
                  { "data": "PlayerID" },
                  { "data": "PlayerName" },
                  { "data": "Prize" },
                  { "data": "DateInserted" },
                ],
                responsive: true,
                destroy: true,
                order: [[4, 'desc']],
               
                
            });
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    });
}


function loadPrizeInfo() {
    $.ajax({
        url: "/WheelOfFortune/getInitPrizeInfo",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (data) {

            var table = $('#wofInfoTbl').DataTable({
                data: data,
                columns: [
                  { "data": "ID" },
                  { "data": "DisplayName" },
                  { "data": "PositionID" },
                  { "data": "OriginalQty" },
                  { "data": "RemainingQty" },
                  {
                       "data": "ID",
                       render: function (data, type, row, meta) {
                           if (type === "display") {
                               data = '<a href="#" onclick="return GetPrizeInfoByID(' + data + ')">Edit</a> | <a href="#" onclick="ChangePosition(' + data + ')">Change Position</a>';
                           }
                           return data;
                       }
                   },
                ],
                responsive: true,
                destroy: true,
                pageLength: 25,
               // order: [[2, 'desc']]
            });
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    });
}


function GetPrizeInfoByID(Id) {
    $.ajax({
        url: "/WheelOfFortune/getPrizeInfoByID",
        type: 'GET',
        data: {
            Id: +Id
        },
        dataType: "json",
        success: function (result) {
            $('#mID').val(result.ID);
            $("#mPrize").val(result.DisplayName);
            $("#mTotalQty").val(result.OriginalQty);
            $("#mRemainQty").val(result.RemainingQty);
            $("#mPosition").val(result.PositionID);
            if (result.isSelected == false) {
                $("#mSelectedLbl").show();
                $("#mSelected").css("display", "block");
            } else {
                $("#mSelectedLbl").hide();
                $("#mSelected").css("display", "none");
            }
            $('#mPrizeInfo').modal("show");

        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    })
}

function UpdatePrizeInfoByID(e) {
    e.preventDefault();

    var isSelected = $("#mSelected").prop("checked");
    var totalQty = +$("#mTotalQty").val();
    var remainQty = +$("#mRemainQty").val();
    if (totalQty < remainQty) {
        const inp = document.getElementById('mTotalQty');
        inp.setCustomValidity("Error: Total quantity cannot be less than the remaining quantity.");
        inp.reportValidity();
    }else{
        $.ajax({
            url: "/WheelOfFortune/UpdatePrizeInfoByID",
            type: 'GET',
            data: {
                Id: +$('#mID').val(),
                PrizeName: $("#mPrize").val(),
                TotalQty: totalQty,
                IsSelected: isSelected
            },
            dataType: "json",
            success: function (result) {

                $('#mPrizeInfo').modal("hide");
                if (+result == -3) {
                    $("#mTimeOut").modal("show");
                } else {
                    loadPrizeInfo();
                }
            },
            error: function (errormessage) {
                $("#mTimeOut").modal("show");
            }
        })
    }
    
}

function onClickPrizeById() {
    const inp = document.getElementById('mTotalQty');
    inp.setCustomValidity("");
}

function ChangePosition() {
    $.ajax({
        url: "/WheelOfFortune/getInitPrizeInfo",
        type: 'GET',
        data: {},
        dataType: "json",
        success: function (result) {
            var html = ``;
            $.each(result, function(key, info) {
                html += `<div class="form-group form-inline">`
                html += `  <label for="mPrize` + info.ID + `" class="font-weight-bold form-control col-md-8">` + info.DisplayName + `</label>`
                html += `  <input type="number" class="form-control ml-3 col-md-2 mPrizeNumber" value="` + info.PositionID + `" id="prize` + info.ID+ `" />`
                html += `</div>`
            });

            $("#mPrizeDiv").html(html);
            $("#mPositionInfo").modal("show");
        },
        error: function (errormessage) {
            $("#mTimeOut").modal("show");
        }
    });
}

function updPosition(e) {
    e.preventDefault();
    var element = document.getElementsByClassName('mPrizeNumber');
    var length = element.length;
    var arrPos = [];
    var arrPize = [];
    $.each(element, function (key, item) {
        let posVal = +item.value;
        if (0 < posVal && posVal  <= length) {
            if (arrPos.indexOf(posVal) < 0) {
                arrPos.push(posVal);
                arrPize.push({ ID: +item.id.replace("prize", ""), PositionID: posVal })
            } else {
                const inp = document.getElementById(item.id);
                inp.setCustomValidity("Duplicated.");
                return inp.reportValidity();
            }
            
        } else {
            const inp = document.getElementById(item.id);
            inp.setCustomValidity("Invalid.");
            return inp.reportValidity();
        }
    });

    if (arrPize.length == length) {
        $.ajax({
            type: 'POST',
            url: '/WheelOfFortune/updPosition/?nocache=' + Math.random(),
            contentType: "application/json;charset=utf-8",
            data: JSON.stringify(arrPize),
            dataType: "json",
            success: function (data) {
                $("#mPositionInfo").modal("hide");
                if (+data == -3) {
                    $("#mTimeOut").modal("show");
                } else {
                    loadPrizeInfo();
                }
            },
            error: function (message) {
                $("#mTimeOut").modal("show");
            }
        });
    }
}
function onClickPosition() {
    var element = document.getElementsByClassName('mPrizeNumber');
    $.each(element, function (key, item) {
        const inp = document.getElementById(item.id);
        inp.setCustomValidity("");
    });
}