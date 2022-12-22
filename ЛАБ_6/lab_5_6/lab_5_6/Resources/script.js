function send() {
    let request = new XMLHttpRequest();
    request.open("POST", '/' + textInput.value);
    request.onload = () => {
	    emailsOutput.innerHTML = request.responseText;
    }

    request.send();
}

sendTextButton.addEventListener('click', send);