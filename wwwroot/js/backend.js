$(document).ready(function () {
    //global search
    let globalSearch;

    $("#global-search-input").keyup(function () {
        globalSearch = $(this).val().trim();

        $("#global-result li").remove();

        if (globalSearch.length == 0)
            return;

        $.ajax({
            url: `Home/Search?search=${globalSearch}`,
            type: 'GET',
            success: function (res) {
                $("#global-result").append(res);
            }
        })
    })
})