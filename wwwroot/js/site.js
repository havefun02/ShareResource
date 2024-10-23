$(document).ready(function () {


    $("#logoutForm").on("submit", function (event) {
        event.preventDefault();

        $.ajax({
            type: "POST",
            url: "/api/v1/auths/logout",
            success: function (response) {
                if (response.redirectUrl) {
                    window.location.href = response.redirectUrl;
                }
            },
            error: function () {
                alert("Logout failed. Please try again.");
            }
        });
    });
});
