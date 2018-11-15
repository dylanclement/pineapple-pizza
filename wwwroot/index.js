const player = document.getElementById('player');
const canvas = document.getElementById('canvas');
const context = canvas.getContext('2d');
const captureButton = document.getElementById('capture');

const constraints = {
video: true,
};

captureButton.addEventListener('click', () => {
// Draw the video frame to the canvas.
context.drawImage(player, 0, 0, canvas.width, canvas.height);
image = canvas.toDataURL("image/png");
image = image.replace('data:image/png;base64,', '');

$.ajax({
        type: 'POST',
        url: 'api/values',

        data: image,
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        error: function(msg) {
            if (msg.status != 200) {
                alert('Error saving image. ' + JSON.stringify(msg));
            } else {
                alert('Success!');
            }
        },
        success: function(msg) {
            alert('Image saved successfully !');
        }
    });
});

// Attach the video stream to the video element and autoplay.
navigator.mediaDevices.getUserMedia(constraints)
.then((stream) => {
    player.srcObject = stream;
});