
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
    document.getElementById('results').style.display = 'none'

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
            // Display results
            resultDiv = document.getElementById('results')
            resultDiv.style.display = ''

            if (msg.statusCode == 200) {
                var resultText = document.getElementById('result-text')
                resultText.innerHTML = "Result: Success"
                resultText.style = "color:green"
                document.getElementById('result-confidence').innerHTML = "Confidence: " + msg.matchConfidence
            } else {
                var resultText = document.getElementById('result-text')
                resultText.innerHTML = "Result: Sorry"
                resultText.style = "color:red"
            }
            if (msg.message !== null) {
                document.getElementById('result-message').innerHTML = "Message: " + msg.message
            }
            console.log('Image saved successfully !')
        }
    });
});

// Attach the video stream to the video element and autoplay.
navigator.mediaDevices.getUserMedia(constraints)
.then((stream) => {
    player.srcObject = stream;
});