﻿var config = {
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
    document.getElementById("login").classList.remove("show");
    document.getElementById("login").classList.add("hide");
    document.getElementById("api_profile_create").classList.remove("hide")
    document.getElementById("api_profile_create").classList.add("show");
    document.getElementById("api_profile_delete").classList.remove("hide")
    document.getElementById("api_profile_delete").classList.add("show");
    document.getElementById("api_profile_add_weblink").classList.remove("hide")
    document.getElementById("api_profile_add_weblink").classList.add("show");
    document.getElementById("api_profile_delete_weblink").classList.remove("hide")
    document.getElementById("api_profile_delete_weblink").classList.add("show");
    document.getElementById("api_orcid").classList.remove("hide")
    document.getElementById("api_orcid").classList.add("show");
    document.getElementById("logout").classList.remove("hide")
    document.getElementById("logout").classList.add("show");
}

function uiLoggedOut() {
    document.getElementById("login").classList.remove("hide");
    document.getElementById("login").classList.add("show");
    document.getElementById("api_profile_create").classList.remove("show")
    document.getElementById("api_profile_create").classList.add("hide");
    document.getElementById("api_profile_delete").classList.remove("show")
    document.getElementById("api_profile_delete").classList.add("hide");
    document.getElementById("api_profile_add_weblink").classList.remove("show")
    document.getElementById("api_profile_add_weblink").classList.add("hide");
    document.getElementById("api_profile_delete_weblink").classList.remove("show")
    document.getElementById("api_profile_delete_weblink").classList.add("hide");
    document.getElementById("api_orcid").classList.remove("show")
    document.getElementById("api_orcid").classList.add("hide");
    document.getElementById("logout").classList.remove("show")
    document.getElementById("logout").classList.add("hide");
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