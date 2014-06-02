function CenteredAll() {
    var bAll = document.querySelector(".b-all");
    bAll.style.marginTop = -bAll.clientHeight / 2 + "px";
}

var userForm = document.querySelector(".b-all form"),
    inputElement = userForm.querySelectorAll("input[type='text'], input[type='password']");


userForm.addEventListener("submit", function (e) {
    if (Verification() == false) {
        e.preventDefault();
        SubmitError(userForm, true);
        CenteredAll();
    }
}, false);

userForm.addEventListener("blur", function (e) {
    if (e.target.Verification() != "OK") {
        RemoveError(document.querySelector(".b-all .b-message p[data-element='" + e.target.getAttribute("name") + "']"));
        e.target.ShowError();
        SubmitError(userForm, true);
    }
    else
        RemoveError(document.querySelector(".b-all .b-message p[data-element='" + e.target.getAttribute("name") + "']"));
    CenteredAll();
}, true);