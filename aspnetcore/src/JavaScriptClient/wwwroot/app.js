var config = {
    authority: "https://localhost:5001",
    client_id: "js",
    redirect_uri: "https://localhost:5003/callback.html",
    response_type: "code",
    scope: "openid profile api1",
    post_logout_redirect_uri: "https://localhost:5003/index.html",
};
var mgr = new Oidc.UserManager(config);

function log() {
    document.getElementById('results').innerText = '';

    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
        document.getElementById('results').innerHTML += msg + '\r\n';
    });
}

var api = function (url) {
    mgr.getUser().then(function (user) {
        var xhr = new XMLHttpRequest();
        xhr.open("GET", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

var api_post = function (url) {
    mgr.getUser().then(function (user) {
        var xhr = new XMLHttpRequest();
        xhr.open("POST", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

var api_delete = function (url) {
    mgr.getUser().then(function (user) {
        var xhr = new XMLHttpRequest();
        xhr.open("DELETE", url);
        xhr.onload = function () {
            log(xhr.status, JSON.parse(xhr.responseText));
        }
        xhr.setRequestHeader("Authorization", "Bearer " + user.access_token);
        xhr.send();
    });
}

document.getElementById("login").addEventListener("click", login, false);
//document.getElementById("api_identity").addEventListener("click", api("https://localhost:6001/identity"), false);
//document.getElementById("api_orcid").addEventListener("click", api("https://localhost:6001/orcid"), false);
document.getElementById("logout").addEventListener("click", logout, false);


function uiLoggedIn() {
    var loginBtn = document.getElementById("login");
    loginBtn.classList.add("hide");
    loginBtn.classList.remove("show");

    var apiIdentityBtn = document.getElementById("api_identity");
    apiIdentityBtn.classList.add("show");
    apiIdentityBtn.classList.remove("hide");

    var apiOrcidBtn = document.getElementById("api_orcid");
    apiOrcidBtn.classList.add("show");
    apiOrcidBtn.classList.remove("hide");

    var logoutBtn = document.getElementById("logout");
    logoutBtn.classList.add("show");
    logoutBtn.classList.remove("hide");
}

function uiLoggedOut() {
    var loginBtn = document.getElementById("login");
    loginBtn.classList.add("show");
    loginBtn.classList.remove("hide");

    var apiIdentityBtn = document.getElementById("api_identity");
    apiIdentityBtn.classList.add("hide");
    apiIdentityBtn.classList.remove("show");

    var apiOrcidBtn = document.getElementById("api_orcid");
    apiOrcidBtn.classList.add("hide");
    apiOrcidBtn.classList.remove("show");

    var logoutBtn = document.getElementById("logout");
    logoutBtn.classList.add("hide");
    logoutBtn.classList.remove("show");
}

mgr.getUser().then(function (user) {
    if (user) {
        log("User logged in", user.profile);
        uiLoggedIn();
    }
    else {
        log("User not logged in");
        uiLoggedOut();
    }
});

function login() {
    mgr.signinRedirect();
}

function logout() {
    mgr.signoutRedirect();
}