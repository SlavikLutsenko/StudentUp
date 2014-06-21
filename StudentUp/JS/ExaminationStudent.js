var attestationSelect = document.querySelector("select[name='numberAttestation']"),
    subjectSelectAttestation = document.querySelector("select[name='subjectAttestation']"),
    showButtonAttestation = document.querySelector("input[type='button'][name='showAttestation']"),
    myAttestation = document.querySelector(".myAttestation");

function ShowAttestation() {
    var subject = new Array(),
        group = new Array(),
        option;
    if (subjectSelectAttestation.value != "-1")
        subject.push(Number(subjectSelectAttestation.value));
    else {
        option = subjectSelectAttestation.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            if (option[i].value != "-1")
                subject.push(Number(option[i].value));
    }

    $.post("/Search/AttestationInTheSubjectGroupsStudent", "userID=" + Number(getCookie("userID")) + "&numberAttestation=" + attestationSelect.value + "&subjectsID=" + subject, function (data) {
        myAttestation.innerHTML = data;
    });
}

showButtonAttestation.addEventListener("click", ShowAttestation, false);

/*===============================================*/

var typeSession = document.querySelector("select[name='typeSession']"),
    subjectSelectSession = document.querySelector("select[name='subjectSession']"),
    showButtonSession = document.querySelector("input[type='button'][name='showSession']"),
    mySession = document.querySelector(".mySession");

function ShowSession() {
    var subject = new Array(),
        option;
    if (subjectSelectSession.value != "-1")
        subject.push(Number(subjectSelectSession.value));
    else {
        option = subjectSelectSession.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            if (option[i].value != "-1")
                subject.push(Number(option[i].value));
    }
    $.post("/Search/SessionResultStudent", "userID=" + Number(getCookie("userID")) + "&typeSession=" + typeSession.value + "&subjectsID=" + subject, function (data) {
        mySession.innerHTML = "<h3>" + subjectSelectSession.selectedOptions[0].innerHTML + "</h3>" + data;
    });
}

showButtonSession.addEventListener("click", ShowSession, false);