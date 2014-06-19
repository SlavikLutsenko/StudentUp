var attestationSelect = document.querySelector("select[name='numberAttestation']"),
    subjectSelect = document.querySelector("select[name='subject']"),
    groupSelect = document.querySelector("select[name='group']"),
    showButton = document.querySelector("input[type='button'][name='showMark']"),
    myAttestation = document.querySelector(".myAttestation");

var setAttestationButton;

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
    if (groupSelect.value != "-1")
        group.push(Number(groupSelect.value));
    else {
        option = groupSelect.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            if (option[i].value != "-1")
                group.push(Number(option[i].value));
    }
    $.post("/Search/AttestationInTheSubjectGroupsLecturer", "numberAttestation="+attestationSelect.value+"&subjectsID="+subject+"&groupsID="+group, function (data) {
        myAttestation.innerHTML = data;
        setAttestationButton = document.querySelectorAll("input[type='button'][name='setAttestation']");
        for (var i = 0; i < setAttestationButton.length; i++) {
            setAttestationButton[i].addEventListener("click", function (e) {
                var form = e.target.parentNode,
                    subject = form.querySelector("input[name='subject']").value,
                    group = form.querySelector("input[name='group']").value,
                    minMark = form.querySelector("input[name='minMark']").value;
                $.post("/SetAttestation", { numberAttestation: Number(attestationSelect.value), subjectID: subject, groupID: group, minMark: minMark }, function (data) {
                    alert(data);
                    ShowAttestation();
                });
            }, false);
        }
    });
}

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
        groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
});

subjectSelect.addEventListener("change", function () {
    var url, params;
    if (subjectSelect.value == -1) {
        url = "/Search/GetGroupsOnTheLecturer";
        params = { userID: Number(getCookie("userID")) };
    } else {
        url = "/Search/GetGroupsOnTheSubject";
        params = { subjectID: subjectSelect.value };
    }
    $.post(url, params, function (data) {
        groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
}, false);

showButton.addEventListener("click", ShowAttestation, false);