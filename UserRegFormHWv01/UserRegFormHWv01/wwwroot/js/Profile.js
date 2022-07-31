﻿// script for /Auth/Profile

document.addEventListener('DOMContentLoaded', () => {
    const userRealName = document.getElementById("userRealName");
    userRealName.onclick = editableClick;
    userRealName.onblur = e => {
        e.target.removeAttribute("contenteditable");
        if (e.target.savedValue != e.target.innerText) {
            fetch("/Auth/ChangeRealName?NewName=" + e.target.innerText)
                .then(r => r.text())
                .then(t => {
                    if (t == "incorrect" || t == "Forbidden") {
                        e.target.innerText = e.target.savedValue;
                    }
                })
        }
    };
    userRealName.onkeydown = editableKeyDown;

    const userEmail = document.getElementById("userEmail");
    userEmail.onclick = editableClick;
    userEmail.onblur = e => {
        e.target.removeAttribute("contenteditable");
        if (e.target.savedValue != e.target.innerText) {
            fetch("/Auth/ChangeEmail?NewEmail=" + e.target.innerText)
                .then(r => r.text())
                .then(t => {
                    if (t == "incorrect" || t == "Forbidden") {
                        e.target.innerText = e.target.savedValue;
                    }
                })
        }
    };
    userEmail.onkeydown = editableKeyDown;

    const userLogin = document.getElementById("userLogin");
    if (!userLogin) throw "userLogin not found in DOM";
    userLogin.onclick = editableClick;
    userLogin.onblur = userLoginBlur;
    userLogin.onkeydown = editableKeyDown;


    const userOldPassword = document.getElementById("userOldPassword");
    if (!userOldPassword) throw "userСurrentPassword not found in DOM";

    const userNewPassword = document.getElementById("userNewPassword");
    if (!userNewPassword) throw "userNewPassword not found in DOM";

    const changePassword = document.getElementById("changePassword");
    if (!changePassword) throw "changePassword not found in DOM";
    changePassword.onclick = e => {
        if (userOldPassword.value != null && userNewPassword.value != null) {
            fetch("/Auth/ChangePassword", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ NewPassword: userNewPassword.value, OldPassword: userOldPassword.value })
            })
                .then(r => r.json())
                .then(console.log);
        }
    };


    const userNewAvatar = document.getElementById("userNewAvatar");
    if (!userNewAvatar) throw "userNewAvatar not found in DOM";
    userNewAvatar.onchange = avatarChange;
});

function avatarChange(e) {
    if (e.target.files.length > 0) {
        const formData = new FormData();
        formData.append("userAvatar", e.target.files[0]);
        fetch("/Auth/ChangeAvatar", {
            method: "POST",
            body: formData
        }).then(r => r.json())
            .then(console.log);
            //.then(j => {
            //    if (j.status == "Ok") {
            //        document.getElementById("userLogo").src = "/img/" + j.message;
            //    }
            //    else {
            //        alert(j.message);
            //    }
            //});
    }
}

function editableClick(e) {
    e.target.setAttribute("contenteditable", true);
    e.target.savedValue = e.target.innerText;
}

function userLoginBlur(e) {
    e.target.removeAttribute("contenteditable");
    if (e.target.savedValue != e.target.innerText) {
        fetch("/Auth/ChangeLogin", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(e.target.innerText)
        })
            .then(r => r.json())
            .then(console.log);
    }
}

function editableKeyDown(e) {
    if (e.key == "Enter") {
        e.preventDefault();
        e.target.blur();
    }
}