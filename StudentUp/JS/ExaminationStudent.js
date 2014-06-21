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