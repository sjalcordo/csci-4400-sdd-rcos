// AUTHOR: Amanda (ruana2@rpi.edu)
// DESC: Implements the display and functionality of the menu at
//       the top right corner of each page.

const menu = document.getElementById('menu');
const settingsClose = document.getElementById('settingsClose');
const settings = document.getElementById('settings');
const page = document.getElementById('container');
const pageButtons = page.querySelectorAll('button');

menu.addEventListener('click', () => 
{
    if (settings.style.display !== 'none') 
    {
        Close();
    }
    else Open();
});

settingsClose.addEventListener('click', () => 
{
    Close();
});

function Open() 
{
    settings.style.display = 'block';
    page.style.opacity = '0.2';
    pageButtons.forEach((button) => 
    {
        button.disabled = 'true';
    })
};

function Close() 
{
    settings.style.display = 'none';
    page.style.opacity = '1.0';
    pageButtons.forEach((button) => 
    {
        button.removeAttribute('disabled');
    })
};


const newGame = document.getElementById('newGame');
const homeScreen = document.getElementById('homeScreen');

newGame.addEventListener('click', () => 
{
    window.location.href = "/lobbyCode/index.html";
})

homeScreen.addEventListener('click', () => 
{
    window.location.href = "/index.html";
})