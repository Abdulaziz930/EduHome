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

    //-- teacher search start --//

    let teacherSearch;
    let allTeachers = $("#teacher-list").html();
    let teacherPagination = $("#teacherPaginationBox").html();

    $("#teacher-search-input").keyup(function () {
        teacherSearch = $(this).val().trim();

        $("#teacher-list #teacher-card").remove();
        $("#teacherPaginationId").remove();

        if (teacherSearch.length == 0) {
            $("#teacher-list").append(allTeachers);
            $("#teacherPaginationBox").append(teacherPagination);
            return;
        }

        $.ajax({
            url: `Teacher/Search?search=${teacherSearch}`,
            type: 'GET',
            success: function (res) {
                $("#teacher-list").append(res);
            }
        })
    })

    //-- teacher search end --//

    //-- blog search start --//

    let blogSearch;
    let allBlogs = $("#blog-list").html();
    let blogPagination = $("#blogPaginationBox").html();

    $("#blog-search-input").keyup(function () {
        blogSearch = $(this).val().trim();

        $("#blog-list #blog-card").remove();
        $("#blogPaginationId").remove();

        if (blogSearch.length == 0) {
            $("#blog-list").append(allBlogs);
            $("#blogPaginationBox").append(blogPagination);
            return;
        }

        $.ajax({
            url: `Blog/Search?search=${blogSearch}`,
            type: 'GET',
            success: function (res) {
                $("#blog-list").append(res);
            }
        })
    })
    //-- blog search end --//

    //-- subscribe start --//

    let subscribeInput;

    $("#Subscribe-Button").click(function () {

        $("#response-subscribe").empty();

        if (isAuthenticated == false) {
            subscribeInput = $("#Email-Subscribe").val().trim();
            if (subscribeInput.length == 0)
                return;

            $.ajax({
                url: `Home/Subscribe?email=${subscribeInput}`,
                type: 'GET',
                success: function (res) {
                    $("#response-subscribe").append(res);
                }
            })
        } else {
            $.ajax({
                url: "Home/Subscribe?email=",
                type: 'GET',
                success: function (res) {
                    $("#response-subscribe").append(res);
                }
            })
        }

    })

    //-- subscribe end --//

})