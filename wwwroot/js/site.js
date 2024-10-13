
$(document).ready(function () {
    $('#uploadForm').on("submit", function (event) {
        event.preventDefault(); 
        const formData = new FormData(this); 
        function updateStatus(type, message) {
            const statusDiv = $('#status');

            statusDiv
                .removeClass('d-none alert-success alert-danger') 
                .addClass(`alert-${type}`) 
                .text(message) 
                .fadeIn(); 

            setTimeout(() => statusDiv.fadeOut(), 5000);
        }
        $.ajax({
            url: 'http://localhost:3000/Resource/Upload', // Target URL
            type: 'POST', // HTTP method
            data: formData, // Data to send
            processData: false, // Prevent jQuery from converting the data into query string
            contentType: false, // Prevent jQuery from setting content-type header
            enctype: 'multipart/form-data', // Ensure enctype is correct

            success: function (response) {
                $('#gallery').append(response)
                updateStatus('success',`Created successfully`);
                $('#uploadForm')[0].reset();
            },
            error: function (xhr, status, error) {
                updateStatus('danger', `Failed to add`);
                $('#uploadForm')[0].reset();
            }
        });
    });
});