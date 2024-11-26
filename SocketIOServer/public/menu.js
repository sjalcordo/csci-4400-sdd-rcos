const menu = document.getElementById('menu');
const settingsClose = document.getElementById('settingsClose');
const settings = document.getElementById('settings');
const page = document.getElementById('container');
const pageButtons = page.querySelectorAll('button');
menu.addEventListener('click', () => {
    if (settings.style.display !== 'none') {
        close();
    }
    else open();
});
settingsClose.addEventListener('click', () => {
    close();
});

function open() {
    settings.style.display = 'block';
    page.style.opacity = '0.2';
    pageButtons.forEach((button) => {
        button.disabled = 'true';
    })
};
function close() {
    settings.style.display = 'none';
    page.style.opacity = '1.0';
    pageButtons.forEach((button) => {
        button.removeAttribute('disabled');
    })
};

const newGame = document.getElementById('newGame');
const homeScreen = document.getElementById('homeScreen');
newGame.addEventListener('click', () => {
    window.location.href = "/lobbyCode/index.html";
})
homeScreen.addEventListener('click', () => {
    window.location.href = "/index.html";
})