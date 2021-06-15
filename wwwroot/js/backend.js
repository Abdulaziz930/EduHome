$(document).ready(function () {
    //-- global search start --//

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
    //-- global search end --//

    //-- course search start --//

    let courseSearch;
    let allCourses = $("#course-list").html();
    let pagination = $("#paginationBox").html();

    $("#course-search-input").keyup(function () {
        courseSearch = $(this).val().trim();

        $("#course-list #course-card").remove();
        $("#paginationId").remove();

        if (courseSearch.length == 0) {
            $("#course-list").append(allCourses);
            $("#paginationBox").append(pagination);
            return;
        }

        $.ajax({
            url: `Course/Search?search=${courseSearch}`,
            type: 'GET',
            success: function (res) {
                $("#course-list").append(res);
            }
        })
    })
    //-- course search end --//

    //-- event search start --//

    let eventSearch;
    let allEvents = $("#event-list").html();
    let eventPagination = $("#eventPaginationBox").html();

    $("#event-search-input").keyup(function () {
        eventSearch = $(this).val().trim();

        $("#event-list #event-card").remove();
        $("#eventPaginationId").remove();
        console.log("1");
        if (eventSearch.length == 0) {
            $("#event-list").append(allEvents);
            $("#eventPaginationBox").append(eventPagination);
            return;
        }

        $.ajax({
            url: `Event/Search?search=${eventSearch}`,
            type: 'GET',
            success: function (res) {
                $("#event-list").append(res);
            }
        })
    })

    //-- event search end --//

})