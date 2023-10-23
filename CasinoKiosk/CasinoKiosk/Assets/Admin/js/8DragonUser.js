﻿/// <reference path="jquery-1.9.1.intellisense.js" />
//Load Data in Table when documents is ready
$(document).ready(function () {
    loadData();
    
});

//Load Data function
function loadData() {
    $.ajax({
        url: "/POS8Dragon/selectListPOS8DragonUser",
        type: "GET",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            var html = '';
            $.each(result, function (key, user) {
                html += '<tr>';
                //html += '<td>' + player.ID + '</td>';
                //html += '<td>' + user.UserID + '</td>';
                html += '<td>' + user.Username + '</td>';
                html += '<td>' + user.Password + '</td>';
                html += '<td>' + user.isActived + '</td>';               
                html += '<td><a href="#" onclick="return GetbyID(' + user.ID + ')">Edit</a> | <a href="#" onclick="Delele(' + user.ID + ')">Delete</a></td>';
                html += '</tr>';
            });
            $('.tbody').html(html);
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//Insert Data Function 
function Add()
{
    var res = validate();
    if (res == false) {
        return false;
    }    
    var userObj = {       
       ID: $('#ID').val(),
       Username: $('#UserName').val(),
       Password: $('#Password').val(),
       isActived: $('#isActive').prop('checked') == true ? "true" : "false",       
    };
    $.ajax({
        url: "/POS8Dragon/Add",
        data: JSON.stringify(userObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
           
            $('#myModal').modal('hide');
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
}

//Function for getting the Data Based upon ID
function GetbyID(ID) {
    
    //$('#CasinoKiosk_UserName').css('border-color', 'lightgrey');
    //$('#isActive').css('border-color', 'lightgrey');   
    //$('#UserEmailAddress').css('border-color', 'lightgrey');
    

    $.ajax({
        url: "/POS8Dragon/GetbyID/" + ID,
        type: "GET",
        contentType: "application/json;charset=UTF-8",
        dataType: "json",
        success: function (result) {
            $('#ID').val(result.ID);
            $('#UserName').val(result.Username);
            $('#Password').val(result.Password);
            $('#isActive').prop('checked', result.isActived);           
            
            //alert($('#RoleName').val(result.RoleId));
            
            //$("#RoleName").append(new Option("option text", "value"));

            $('#myModal').modal('show');
            $('#btnUpdate').show();
            $('#myModalLabel').text("Update Currency Converter User")
            $('#btnAdd').hide();           
        },
        error: function (errormessage) {
            alert(errormessage.responseText);
        }
    });
    return false;
}

//function for updating player's record
function Update() {
    var res = validate();
    if (res == false) {
        return false;
    }
    var userObj = {
        ID: $('#ID').val(),
        Username: $('#UserName').val(),
        Password: $('#Password').val(),
        isActived: $('#isActive').prop('checked') == true ? "true" : "false",
        
    };
    //alert($('#RoleId').val());
    $.ajax({
        url: "/POS8Dragon/Update",
        data: JSON.stringify(userObj),
        type: "POST",
        contentType: "application/json;charset=utf-8",
        dataType: "json",
        success: function (result) {
            loadData();
            
            $('#myModal').modal('hide');           
            $('#ID').val();
            $('#UserName').val();
            $('#Password').val();
            $('#isActive').val();          
           
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
            url: "/POS8Dragon/Delete/" + ID,
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
    
    $('#UserName').val("");
    $('#Password').val("");
    $('#isActive').prop('checked', 0);

    $('#myModalLabel').text("Add POS 8Dragoon User")

    $('#btnUpdate').hide();
    $('#btnAdd').show();

    $('#UserName').css('border-color', 'lightgrey');
    $('#isActive').css('border-color', 'lightgrey');
    
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
    if ($('#UserName').val().trim() == "") {
        $('#UserName').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#UserName').css('border-color', 'lightgrey');
    }
    if ($('#Password').val().trim() == "") {
        $('#Password').css('border-color', 'Red');
        isValid = false;
    }
    else {
        $('#Password').css('border-color', 'lightgrey');
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