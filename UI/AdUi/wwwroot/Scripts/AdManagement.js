var adManagement = {}

adManagement.getSessionStorage = (key) => {
    return sessionStorage.getItem(key);
}

adManagement.setSessionStorage = (key, data) => {
    return sessionStorage.setItem(key, data);
}

adManagement.scrollToBottom = (id) => {
    var div = document.getElementById(id);
    if (div && div.scrollHeight && div.clientHeight) {
        div.scrollTop = div.scrollHeight - div.clientHeight;
    }
}

adManagement.playNotification = () => {
    document.getElementById('notification').play();
}