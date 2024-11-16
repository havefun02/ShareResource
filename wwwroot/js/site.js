$(document).on('click', '.clickable-items', function () {
    function redirectToItem(imgId) {
        const url = '/resources/' + imgId; // Use string concatenation
        window.location.href = url;
    }
        const imgId = $(this).data('imgid');
redirectToItem(imgId);
    });

$(document).on('click', '.btn-like-toggle', function (e) {
    e.stopPropagation(); // Prevent card click from triggering
    const button = $('.btn-like-toggle');
    const imgId = $(this).data('imgid');
    const userId = $(this).data("userid");

    const isLiked = $(this).hasClass('liked');
    if (isLiked) {
        if (updateState(false, userId, imgId) != 0) {
            $(this).html('<i class="fas fa-thumbs-up"></i> Like');
            $(this).toggleClass('liked');
            const likeCountElement = $(this).siblings('.like-count');
            let currentCount = parseInt(likeCountElement.text());
            likeCountElement.text(currentCount - 1); // Increment the like count by 1
        }

    } else {
        if (updateState(true, userId, imgId) != 0) {
            $(this).html('<i class="fas fa-thumbs-down"></i> Unlike');
            $(this).toggleClass('liked');
            const likeCountElement = $(this).siblings('.like-count');
            let currentCount = parseInt(likeCountElement.text());
            likeCountElement.text(currentCount + 1); // Increment the like count by 1

        }

    }
});
function updateState(state, userId, imgId) {

    let form = { userId: userId, state: state, resourceId: imgId }
    data = JSON.stringify(form);
$.ajax({
    url: `/api/v1/resources/like`,
type: 'POST',
    contentType: "application/json",
    data: data,
success: function(response) {
    return 1
            },
error: function(error) {
    console.error('Error liking image:', error);
    return 0
            }
        });
    }
