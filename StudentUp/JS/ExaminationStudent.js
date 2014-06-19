var attestationSelect = document.querySelector("select[name='numberAttestation']"),
    subjectSelect = document.querySelector("select[name='subject']"),
    showButton = document.querySelector("input[type='button'][name='showMark']"),
    myAttestation = document.querySelector(".myAttestation");

function ShowAttestation() {
    var subject = new Array(),
        group = new Array(),
        option;
    if (subjectSelect.value != "-1")
        subject.push(Number(subjectSelect.value));
    else {
        option = subjectSelect.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            if (option[i].value != "-1")
                subject.push(Number(option[i].value));
    }

    $.post("/Search/AttestationInTheSubjectGroupsStudent", "userID=" + Number(getCookie("userID")) + "&numberAttestation=" + attestationSelect.value + "&subjectsID=" + subject, function (data) {
        myAttestation.innerHTML = data;
    });
}

showButton.addEventListener("click", ShowAttestation, false);