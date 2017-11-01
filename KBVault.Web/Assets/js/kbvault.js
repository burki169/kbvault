/*!
 * Author: 2B Proje Evi
 * Date: 2 Jun 2014
 * Description:
 * This file contains common event handlers for admin tasks
 !**/

function BindCategoryIconPreview() {
    $("#Icon")
        .keyup(function() {
            var val = $(this).val();
            $("#categoryIconPreview").attr("class", "fa fa-2x fa-" + val);
        });
}

//
// Remove Attachment
//
function BindRemoveAttachmentEvents() {
    
    $(".remove-attachment").click(function () {
        var actionUrl = $(this).attr("data-url");
        var confirmMessage = $(this).attr("data-confirm-message");
        var parentSpan = $(this).parent("span").parent("span");
        if (!confirm(confirmMessage))
            return;
        $.ajax({
            url: actionUrl,
            type: 'POST',
            parentControl: parentSpan
        }).done(function (data) {
            if (data.Successful) {
                var parent = $(this)[0].parentControl;
                $(parent).fadeOut(400, function () { $(this).remove(); });
            }
            else {
                alert(data.ErrorMessage);
            }
        });
    });
}

//
// Remove User
//

function BindRemoveUserEvents() {
    $(".remove-user-action").click(function () {
        var actionUrl = $(this).attr("data-action-url");
        var parentTableRow = $(this).parent("td").parent("tr");
        var confirmMessage = $(this).attr("data-confirm-message");
        if (!confirm(confirmMessage))
            return;
        $.ajax({
            url: actionUrl,
            type: 'POST',
            parentControl:parentTableRow
        }).done(function (data) {
            if (data.Successful) {
                var parentCtrl = $(this)[0].parentControl;
                parentCtrl.fadeOut(500, function () { $(this).remove(); });
            }
            else
                alert(data.ErrorMessage);
        });
    });
}

//
// Remove article
//

function BindRemoveArticleEvents() {
    
    $(".remove-article").click(function () {
        var actionUrl = $(this).attr("data-action-url");
        var confirmMessage = $(this).attr("data-confirm-message");
        var parentTableRow = $(this).parent("td").parent("tr");
        if (!confirm(confirmMessage))
            return;
        $.ajax({
            url: actionUrl,
            type: 'POST',
            parentControl: parentTableRow,
        }).done(function (data) {
            if (data.Successful) {
                var parent = $(this)[0].parentControl;
                $(parent).fadeOut(400, function () { $(this).remove(); });
            }
            else {
                alert(data.ErrorMessage);
            }
        });
    });
}

function BindRemoveTagEvents() {
    $(".remove-tag").click(function () {
        var parentDivControl = $(this).parent("span").parent("div");
        var actionUrl = $(this).attr("data-action-url");
        var confirmMessage = $(this).attr("data-confirm-message");
        if (!confirm(confirmMessage))
            return;
        $.ajax({
            url: actionUrl,
            type:'POST',
            parentControl : parentDivControl
        }).done( function(data){
            if (data.Successful) {
                var parentControl = $(this)[0].parentControl;
                parentControl.fadeOut(500, function () { $(this).remove(); });
            }
            else {
                alert(data.ErrorMessage);
            }
        });
    });
}

function BindRestoreEvent() {
    $(".btn-restore").click(function () {
        var id = $(this).attr("data-order");
        var backupFile = $(this).attr("data-file");
        var actionUrl = $(this).attr("data-url");
        $("#restore-progress-bar-" + id).show();
        $(this).hide();
        $.ajax({
            url: actionUrl,
            type: 'POST',
            Id: id,
            data: { file: backupFile }
        }).done(function (data) {
            var id = this.Id;
            if (data.Successful) {
                alert(data.ErrorMessage);
                document.location.reload();
            }
            else
                alert(data.ErrorMessage);
            $("#restore-progress-bar-"+id).hide();
            $("#restore-"+id).show();
        }).fail(function (data) {
            var id = this.Id;
            $("#restore-progress-bar-" + id).hide();
            $("#restore-" + id).show();
            alert(data.statusText);
        });
    });
}

function BindBackupEvent() {
    $("#create-backup").click(function () {
        $("#backup-progress-bar").show();
        $("#create-backup").hide();
        var actionUrl = $(this).attr("data-action-url");        
        $.ajax({
            url: actionUrl,
            type: 'POST'
        }).done(function (data) {
            if (data.Successful) {
                alert("Backup successful");
                document.location.reload();
            }
            else
                alert("Backup failed");
            $("#backup-progress-bar").hide();
            $("#create-backup").show();
        }).fail(function (data) {
            alert(data.statusText);
            $("#backup-progress-bar").hide();
            $("#create-backup").show();
        });;
    });
}

$(function () {
    "use strict";
    BindRestoreEvent();
    BindRemoveAttachmentEvents();
    BindRemoveArticleEvents();
    BindRemoveUserEvents();
    BindRemoveTagEvents();
    BindBackupEvent();
    BindCategoryIconPreview();
    //
    // Editable mode
    //
    $.fn.editable.defaults.mode = 'popup';
    $(".editable").each(function (i, e) {
        var PostEditUrl = $(e).attr("data-url");
        $(e).editable({
            url: PostEditUrl,
            send: 'always'
        });
    });
    
    //
    // Tag it
    //
    $(".tag-it").each(function (i, e) {
        var tagSourceUrl = $(e).attr("tagsourceurl");
        $(e).tagit({
            minLength: 3,
            tagSource: function (request, response) {
                $.ajax({
                    type: "POST",
                    url: tagSourceUrl,
                    data: "{ 'term':'" + request.term + "' }",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data.Successful) {
                            response($.map(data.Data, function (v, k) {
                                return {
                                    label: v,
                                    value: v
                                }
                            }));
                        }                        
                    }
                });//ajax
            }
        }); //tagit
    });//each

    $('[data-toggle="tooltip"]').tooltip(); 
});