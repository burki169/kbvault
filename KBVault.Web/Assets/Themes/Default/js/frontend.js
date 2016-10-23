
function BindLikeArticleButtons() {
    $(".btn-like-action").click(function () {
        var actionUrl = $(this).attr("data-action");
        var articleId = $(this).attr("data-id");
        var currentObj = $(this);
        $.ajax({
            url: actionUrl,
            type: 'POST',
            data: "{ 'articleId':'" + articleId + "' }",
            dataType: "json",
            contentType: "application/json; charset=utf-8",
            activeControl: currentObj,
            article: articleId
        }).done(function (data) {
            if (data.Successful) {
                var currentObj = $(this)[0].activeControl;
                var articleId = $(this)[0].article;
                currentObj.fadeOut(400, function () { $(this).remove(); });
                $.cookie('article-'+articleId, 'liked', { expires: 7000, path: '/' });
            }
            else {
                alert(data.ErrorMessage);
            }
        });
    });
}

function RemoveLikeButtonForLikedArticles() {
    $(".btn-like-action").each(function () {
        var articleId = $(this).attr("data-id");
        var cookieVal = $.cookie("article-" + articleId);
        if (cookieVal == "liked") {
            $(this).remove();
        }
    });
}

function log(message) {
    console.log(message);
}

function BindAutoComplete() {
    $(".search-control").bind("keydown", function (event) {
        if (event.keyCode === $.ui.keyCode.TAB &&
        $(this).autocomplete("instance").menu.active) {
            event.preventDefault();
        }
    }).autocomplete({
        source: function (request, response) {
            $.ajax({
                type: "POST",
                url: KB_SearchUrl,
                data: "{ 'id':'" + request.term + "' }",
                dataType: "json",
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    response($.map(data.Data, function (item) {
                        return {
                            label: item.ArticleTitle,
                            value: item.ArticleTitle,
                            id: item.ArticleId
                        }
                    }));
                }
            });
        },         
        minLength: 2,
        select: function (event, ui) {
            $("#ArticleId").val(ui.item.id);
            $("#SearchForm").submit();
             /*log(ui.item ?
             "Selected: " + ui.item.value + " aka " + ui.item.id :
             "Nothing selected, input was " + this.value);*/
         }
     });
}

function BindSearchLoadMore() {
    $("#BtnLoadMore").click(function () {
        var actionUrl = $(this).attr("data-href");
        var currentPage = $("#CurrentPage").val();
        var searchKeyword = $("#SearchKeyword").val();
        var currentButton = $(this);
        $.ajax({
            url: actionUrl,
            type: 'POST',
        data: "{ 'CurrentPage':'" + currentPage + "','SearchKeyword':'"+searchKeyword+"' }",
        dataType: "json",
        contentType: "application/json; charset=utf-8",
        activeControl: currentButton,
        //article: articleId
    }).done(function (data) {
        if (data.Successful && data.Data.Results.length > 0) {
            var results = data.Data.Results;
            $.each(results, function (key, value) {
                var htmlText = " <div class=\"col-lg-12\">";
                htmlText += "<a href='/home/detail/" + value.ArticleSefName + "'>" + value.ArticleTitle + "</a>";
                htmlText += "</div>";
                $("#SearchResultContainer").append(htmlText);                
            });
            $("#CurrentPage").val(data.Data.CurrentPage);
        }
        else if (data.Successful && data.Data.Results.length == 0) {
            $(this)[0].activeControl.html("end of results...");
            $(this)[0].activeControl.fadeOut(1500);
        }
        else {
            alert(data.ErrorMessage);
        }
    });
});
}

$(function () {
    "use strict";
    RemoveLikeButtonForLikedArticles();
    BindLikeArticleButtons();
    BindAutoComplete();
    BindSearchLoadMore();
});