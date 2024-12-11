// AUTHORS: Amanda (ruana2@rpi.edu) and Nneoma (anaemn@rpi.edu)
// DESC: Handle user input of the lobby code and check whether code is valid

// Connect to the server
const socket = io();

// inputs is equivalent to entire code, each input is one character of the code
const inputs = document.querySelectorAll('#codeForm input');
inputs.forEach((input, index) =>
{
    input.addEventListener('input', () => HandleInput(input, index));
    input.addEventListener('keydown', (event) => HandleKeyDown(event, input));
});

// move to next input after value is entered in current input
function HandleInput(input, index)
{
    if (input.value.length === 1)
    {
        let nextInput = input.nextElementSibling;
        if (nextInput)
        {
            nextInput.focus();
        }
    }
}

function HandleKeyDown(event, input)
{
    // allow the user to use arrow keys to move between inputs to modify code
    if (event.key === "ArrowRight")
    {
        let nextInput = input.nextElementSibling;
        if (nextInput)
        {
            nextInput.focus();
        }
    }
    else if (event.key === "ArrowLeft")
    {
        let previousInput = input.previousElementSibling;
        if (previousInput)
        {
            previousInput.focus();
        }
    }
    // if current input is empty and backspace/delete is pressed, delete the value of the prev input
    else if (event.key === "Backspace")
    {
        if (input.value !== "")
        {
            input.value = "";
        }
        else
        {
            let previousInput = input.previousElementSibling;
            if (previousInput)
            {
                previousInput.focus();
                previousInput.value = "";
            }
        }
    }
    else if (event.key === "Enter")
    {
        const codeInputs = document.querySelectorAll('.code');
        let codeValues = "";

        codeInputs.forEach(input =>
        {
            codeValues += input.value; 
        });

        socket.emit('join-lobby', codeValues.toLowerCase());
        codeInputs.forEach(input => input.value = '');
    }
}

// Listen for code submission - button
document.getElementById('codeForm').addEventListener('submit', (event) =>
{
     event.preventDefault(); 

     const codeInputs = document.querySelectorAll('.code');
     let codeValues = "";

     codeInputs.forEach(input =>
     {
         codeValues += input.value; 
     });

     socket.emit('join-lobby', codeValues.toLowerCase());
     codeInputs.forEach(input => input.value = '');
 });

// Listen for the 'lobbyConnection' event from the server
socket.on('join-lobby-success', function()
{
    window.location.href = "/profileCreation/profile_creation.html";
});

// Listen for the 'lobbyConnection' event from the server
socket.on('join-lobby-fail-dne', function()
{
    //When lobby doesn't exist prints that the lobby doesn't exist
    document.getElementById('response').textContent = "Lobby does not exist. Please try again.";
});


 