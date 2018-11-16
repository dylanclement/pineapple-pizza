
const player = document.getElementById('player');
const canvas = document.getElementById('canvas');
const context = canvas.getContext('2d');
const captureButton = document.getElementById('capture');

const constraints = {
    video: {width: {min: 1280}, height: {min: 720}}
};

captureButton.addEventListener('click', () => {
    // Draw the video frame to the canvas.
    context.drawImage(player, 0, 0, 1280, 720);
    image = canvas.toDataURL("image/png");
    image = image.replace('data:image/png;base64,', '');

    // Stop streaming the webcam
    //player.srcObject.getTracks()[0].enabled = false;

    // Set button
    $('#capture').removeClass('btn-primary').addClass('btn-secondary').html("Processing...")

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
            $('#capture').removeClass('btn-secondary').addClass('btn-primary').html('Go!')
            // Display results modal
            $('#myModal').modal()

            var resultText = document.getElementById('result-text')
            var resultMessage = document.getElementById('result-message')
            var resultConfidence = document.getElementById('result-confidence')

            if (msg.statusCode == 200) {
                resultText.innerHTML = "Success"
                resultText.style = "color:green"
                resultConfidence.innerHTML = "Confidence: " + msg.matchConfidence
                resultMessage.innerHTML = ""
            } else {
                resultText.innerHTML = "Error"
                resultText.style = "color:red"
            }
            if (msg.message !== null) {
                resultMessage.innerHTML = msg.message
            }
            //player.srcObject.getTracks()[0].enabled = true;
            console.log('Image saved successfully !')
        }
    });
});

// Attach the video stream to the video element and autoplay.
navigator.mediaDevices.getUserMedia(constraints)
.then((stream) => {
    player.srcObject = stream;
});