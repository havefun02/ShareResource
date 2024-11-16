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
    const isDisabled = button.prop('disabled');
    if (isDisabled) return;
const imgId = $(this).data('imgid');
const userId = $(this).data("userid");

const isLiked = $(this).hasClass('liked');

$(this).toggleClass('liked');

    if (isLiked) {
        updateState(false, userId, imgId)

            $(this).html('<i class="fas fa-thumbs-up"></i> Like');

        } else {
        updateState(true, userId, imgId) 

            $(this).html('<i class="fas fa-thumbs-down"></i> Unlike');

        }

const likeCountElement = $(this).siblings('.like-count');
            
            
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
    likeCountElement.text(response.newLikeCount);
            },
error: function(error) {
    console.error('Error liking image:', error);
            }
        });
    }
