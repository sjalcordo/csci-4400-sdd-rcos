// inputs is equivalent to entire code, each input is one character of the code
const inputs = document.querySelectorAll('#codeForm input');
inputs.forEach((input, index) => {
    input.addEventListener('input', () => handleInput(input, index));
    input.addEventListener('keydown', (event) => handleKeyDown(event, input));
});

// move to next input after value is entered in current input
function handleInput(input, index) {
    if (input.value.length === 1) {
        let nextInput = input.nextElementSibling;
        if (nextInput) {
            nextInput.focus();
        }
    }
}

function handleKeyDown(event, input) {
    // allow the user to use arrow keys to move between inputs to modify code
    if (event.key === "ArrowRight") {
        let nextInput = input.nextElementSibling;
        if (nextInput) {
            nextInput.focus();
        }
    }
    else if (event.key === "ArrowLeft") {
        let previousInput = input.previousElementSibling;
        if (previousInput) {
            previousInput.focus();
        }
    }
    // if current input is empty and backspace/delete is pressed, delete the value of the prev input
    else if (event.key === "Backspace") {
        if (input.value !== "") {
            input.value = "";
        }
        else {
            let previousInput = input.previousElementSibling;
            if (previousInput) {
                previousInput.focus();
                previousInput.value = "";
            }
        }
    }
}