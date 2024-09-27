$(document).ready(function () {
    loadPromotionList();
   
    if (+$('#promotionCombo').val() >= 0) {
        loadPromotionLog();
    }
});

var array;
$('.typeahead').typeahead({
    source: function (query, process) {
        return $.post("/HTRTicketPromotion/GetPIDByPromotion", { PromotionID: $('#proID').val() }, function (data) {
            array = data
            var playerIdArr = data.map(function (player) {
                return player.PlayerID;
            });
            data = playerIdArr;
            return process(data);
        });
    },
    updater: function (item) {
        // do what you want with the item here
        var playerInfo = array.find(function (player) {
            return player.PlayerID === item;
        });

        //Get current playerName in Patron
        $.ajax({
            url: "/HTRTicketPromotion/GetNameByPID",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            data: {
                PID: +playerInfo.PlayerID
            },
            dataType: "json",
            success: function (result) {
                $('#newPidName').val(result);
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });

        $('#pid').val(playerInfo.PlayerID);
        $('#curPidName').val(playerInfo.PlayerName);
        return item;
    }
})


//Load Data function
function loadPromotionList() {
    $.ajax({
        url: "/HTRTicketPromotion/GetHTRPromotionList",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            //Get Promotion for table
            var html = '';
            var comboHtml = '';
            var proId = 0;

            var table = $('#proTbl').DataTable({
                data: result,
                columns: [
                  {
                      "data": "PromotionId",
                      render: function (data, type, row, meta) {
                          proId = data;
                          return proId;
                      }
                  },
                  {
                      "data": "PromotionName",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              var html = '';
                              html += '<a href="#" onclick="openPromotionMd(' + proId + ')" >' + data + '</a>';
                              data = html;
                          }
                          return data;
                      }
                  },
                  { "data": "PromotionContent" },
                  { "data": "Condition" },
                  { "data": "IsLuckyDate" },
                  { "data": "CreatedBy" },
                  { "data": "CreatedTime" },
                  { "data": "UpdatedBy" },
                  {
                      "data": "PromotionId",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              var html = '';
                              html += '<a href="#" onclick="openPIDModifyMd(' + proId + ')" ><i class="fas fa-fw fa-pen"></i></a>';
                              data = html;
                          }
                          return data;
                      }
                  }
                ],
                responsive: true,
                destroy: true,
            });

            $('#proTbl').DataTable({
                "bDestroy": true,
                "order": [[0, "desc"]],
                "dom": 'lBfrtip',
                "buttons": [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ]
            });

            $.each(result, function (index,item) {
                comboHtml += ` <option value="` + item.PromotionId + `" id="` + item.PromotionId + `">` + item.PromotionName + `</option>`;
            });
            $('#promotionCombo').html(comboHtml);
            
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function openPromotionMd(PromtionID) {
    clearPromotionMd();
    if (+PromtionID == 0) {
        $('#proTitle').html('Add Promotion');
    } else {
        $('#proTitle').html('Promotion Information');
        $.ajax({
            url: "/HTRTicketPromotion/GetPromotionById",
            type: "GET",
            contentType: "application/json;charset=utf-8",
            data: {
                PromotionId: +PromtionID
            },
            dataType: "json",
            success: function (result) {
                $('#proID').val(result.PromotionId),
                $('#proName').val(result.PromotionName),
                $('#proCondition').val(result.Condition),
                $('#isActive').prop("checked", result.IsActived),
                $('#proContent').val(result.Content),
                $('#isLuckyDate').prop("checked", result.IsLuckyDate)
            },
            error: function (errormessage) {
                alert(errormessage.responseText);
            }
        });
    }
    $("#proRegisModal").modal("show");
}

function openPIDModifyMd(PromtionID) {
    clearPidModifyMd();
    $('#proID').val(PromtionID),
    $("#pidModifyModal").modal("show");
}

function openPromotionLogMd(logId) {
    clearPromotionLogMd();
    $.ajax({
        url: "/HTRTicketPromotion/GetPromotionLogById",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        data: {
            logID: +logId
        },
        dataType: "json",
        success: function (result) {
            $('#proLogID').val(logId);
            $('#proLogTicketNo').val(result.ID);
            $('#proLogPlayerName').val(result.PlayerName);
           
            $("#proLogModal").modal("show");
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function savePromotionLog() {
    $.ajax({
        url: "/HTRTicketPromotion/SavePromotionLog",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({
            LogID: +$('#proLogID').val(),
            PlayerName: $('#proLogPlayerName').val()
        }),
        dataType: "json",
        success: function (result) {
            $("#proLogModal").modal("hide");
            loadPromotionLog();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function savePromotion() {
    $.ajax({
        url: "/HTRTicketPromotion/SaveHTRPromotion",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        data: JSON.stringify({
            PromotionId: +$('#proID').val(),
            PromotionName: $('#proName').val(),
            Condition: $('#proCondition').val(),
            IsActived: $('#isActive').prop("checked"),
            PromotionContent: $('#proContent').val(),
            IsDisplayLuckyDate: $('#isLuckyDate').prop("checked")
        }),
        dataType: "json",
        success: function (result) {
            $("#proRegisModal").modal("hide");
            loadPromotionList();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function savePIDModify() {
    $.ajax({
        url: "/HTRTicketPromotion/SavePIDModify",
        type: "POST",
        contentType: "application/json;charset=utf-8",
        data:   JSON.stringify({
            proID: +$('#proID').val(),
            curPidName: $('#curPidName').val(),
            newPidName: $('#newPidName').val()
        }),
        dataType: "json",
        success: function (result) {
            $("#pidModifyModal").modal("hide");
            loadPromotionList();
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

function PrintHTRPromotion(playerID, promotionID) {
    $('#proPrintBtn').prop('disabled', true);
    window.open('/HTRTicketPromotion/PrintHTRPromotion?PlayerID=' + playerID + "&PromotionID=" + promotionID)
}

function VoidHTRPromotion(ID) {
    if (confirm('Are you sure you want to void this ticket?')) {

        $.ajax({
            url: "/HTRTicketPromotion/VoidHTRPromotion/" + ID,
            type: "POST",
            contentType: "application/json;charset=UTF-8",
            dataType: "json",
            success: function (result) {
                loadPromotionLog();
            },
            error: function (errormessage) {
                alert('You do not have permission');
            }
        });
        return false;
    }
}

function ReprintHTRPromotion() {
    $('#proPrintBtn').prop('disabled', true);
    return confirm('Are you sure you wish to reprint this ticket?');
}


/* ===================================DETAIL PROMOTION===================================*/
function loadPromotionLog() {
    $.ajax({
        url: "/HTRTicketPromotion/GetHTRPromotionLog",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data:{
            PromotionID: +$('#promotionCombo').val()
        },
        success: function (result) {

            //Get data for Log table
            var html = '';
            var logId;
            var table = $('#logTbl').DataTable({
                data: result,
                columns: [
                  {
                      "data": "ID",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              logId = data;
                          }
                          return logId;
                      }
                  },
                  { "data": "PlayerID" },
                  {
                      "data": "PlayerName",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              var html = '';
                              if ($('#promotionCombo').val() == "4") {
                                  html += '<a href="#" onclick="openPromotionLogMd(' + logId + ')" >' + data + '</a>';
                              }else{
                                  html += data;
                              }
                              
                              data = html;
                          }
                          return data;
                      }
                  },
                  { "data": "PrintedDate" },
                  { "data": "PrintedBy" },
                  {
                      "data": "isVoided",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              var html = '';
                              if (!data) {
                                  html += '<a href="/HTRTicketPromotion/ReprintHTRPromotion?ID=' + logId + '" onclick="ReprintHTRPromotion()" target="_blank">Reprint</a> ';
                              }
                              data = html;
                          }
                          return data;
                      }
                  },
                  { "data": "ReprintedBy" },
                  { "data": "ReprintedDate" },
                  {
                      "data": "ID",
                      render: function (data, type, row, meta) {
                          if (type === "display") {
                              var html = '';
                              html += '<a href="#" onclick="VoidHTRPromotion(' + data + ')">Void</a>';
                              data = html;
                          }
                          return data;
                      }
                  },
                  { "data": "VoidedBy" },
                  { "data": "VoidedDate" },
                ],
                responsive: true,
                destroy: true,
            });

            $('#logTbl').DataTable({
                "bDestroy": true,
                "order": [[0, "desc"]],
                "dom": 'lBfrtip',
                "buttons": [
                    'copy', 'csv', 'excel', 'pdf', 'print'
                ]
            });
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}


function searchPromotionByPatron() {
    event.preventDefault();
    $.ajax({
        url: "/HTRTicketPromotion/SearchPromotionByPatron",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        data: {
            PromotionID: +$('#promotionCombo').val(),
            PatronID: +$('#patronID').val()
        },
        success: function (info) {
                $('#notFounfPD').html("");

                //Get data for Log table
                var html = '';
                html += `<form action="" method="post" id="formMidAutume">`
                html += `	<div class="card col-md-6">`
                html += `		<div class="card-body">`
                html += `			<h5 class="card-title">Patron Information</h5>`
                html += ``
                html += `			<div class="row g-3">`
                html += `				`
                html += `				<div class="col-md-3">`
                html += `					<div class="form-floating">`
                html += `						<label for="floatingName">Player ID</label>`
                html += `						<input type="text" class="form-control" value="` + info.PlayerID + `" readonly>`
                html += `					</div>`
                html += `				</div>`
                html += `				<div class="col-md-9">`
                html += `					<div class="form-floating">`
                html += `						<label for="floatingName">Player Name</label>`
                html += `						<input type="text" class="form-control" value="` + info.PlayerName + `"  readonly>`
                html += `					</div>`
                html += `				</div>`
                html += `			   `
                html += `				<div class="col-md-12">`
                if (+info.Quantity == 1) {
                    html += `						<button type="button" id="proPrintBtn" class="btn btn-primary mt-3" style="float: right" onclick="PrintHTRPromotion(` + info.PlayerID + `, ` + info.PromotionId + `)">Print</button>`
                } else {
                    html += `						<button type="button" id="proPrintBtn" class="btn btn-primary mt-3 disabled" style="float: right" >Print</button>`
                }


                html += `				</div>`
                html += `			</div>`
                html += ``
                html += `		</div>`
                html += `	</div>`
                html += `</form>`

                $('#patronInfo').html(html);
            
        },
        error: function (errormessage) {
            $('#notFounfPD').html("Warning! No available Information");
        }
    });
}

function changePromotion() {
    $('#patronInfo').html('');
    $('#notFounfPD').html('');
    loadPromotionLog();
}

function clearPromotionMd() {
    $('#proID').val(''),
    $('#proName').val(''),
    $('#proCondition').val(''),
    $('#isActive').prop("checked", false),
    $('#proContent').val(''),
    $('#proStartDate').val(''),
    $('#proEndDate').val('')
}

function clearPromotionLogMd() {
   $('#proLogID').val('');
   $('#proLogPlayerName').val('');
   $('#proLogTicketNo').val('');
}

function clearPidModifyMd() {
    $('#pid').val('');
    $('#curPidName').val('');
    $('#newPidName').val('');
}


