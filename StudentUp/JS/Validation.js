var verificationElement = document.querySelectorAll("input[data-verification]"),
    messageBox = document.querySelector(".b-message");

for (var i = 0, end = verificationElement.length; i < end; i++) {
    verificationElement[i].Verification = function () {
        var flags = this.getAttribute("data-verification").split(" ");
        for (var j = 0, endFlags = flags.length; j < endFlags; j++)
            switch (flags[j]) {
                case "notEmpty":
                    if (this.value === "") return "notEmpty";
                    break;
                case "email":
                    if (!(regEmail).test(this.value)) return "email";
                    break;
                case "password":
                    if (this.value.length < 6) return "password";
                    break;
                case "replacePassword":
                    if (document.querySelector("input[data-verification*='password']").value != this.value) return "replacePassword";
                    break;
            }
        return "OK";
    }

    verificationElement[i].ShowError = function () {
        var message = this.Verification();
        if (message != "OK") {
            if (messageBox.querySelector("p[data-element='" + this.getAttribute("name") + "']") == null) {
                var errorMessage = document.createElement("p");
                errorMessage.className = "error";
                errorMessage.setAttribute("data-element", this.getAttribute("name"));
                switch (this.Verification()) {
                    case "notEmpty":
                        errorMessage.innerHTML = "Вы не заполнили поле \"" + this.getAttribute("placeholder") + "\"";
                        break;
                    case "email":
                        errorMessage.innerHTML = "Вы ввели не правильные Email";
                        break;
                    case "password":
                        errorMessage.innerHTML = "Пароль должен быть длинее чем 6 символов";
                        break;
                    case "replacePassword":
                        errorMessage.innerHTML = "Пароли должны совпадать";
                        break;
                }
                messageBox.appendChild(errorMessage);
            }
            return false;
        }
        return true;
    }
}

function RemoveError(errorEl) {
    if (errorEl != null)
        messageBox.removeChild(errorEl);
    if (messageBox.querySelectorAll("p[data-element]").length == 0) {
        SubmitError(document.querySelector("form"), false);
    }
}

function Verification() {
    var result = true,
        errorMessages = messageBox.querySelectorAll("p");
    for (var i = 0, end = errorMessages.length; i < end; i++)
        messageBox.removeChild(errorMessages[i]);
    for (var i = 0, end = verificationElement.length; i < end; i++)
        if (verificationElement[i].Verification() != "OK") {
            verificationElement[i].ShowError();
            result = false;
        }
    return result;
}

function SubmitError(form, type) {
    var submitError = form.querySelector("input[type='submit']");
    if (type)
        submitError.disabled = true;
    else
        submitError.disabled = false;
}