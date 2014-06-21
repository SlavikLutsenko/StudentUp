var attestationSelect = document.querySelector("select[name='numberAttestation']"),
    subjectSelectAttestation = document.querySelector("select[name='subjectAttestation']"),
    groupSelectAttestation = document.querySelector("select[name='groupAttestation']"),
    showButtonAttestation = document.querySelector("input[type='button'][name='showAttestation']"),
    myAttestation = document.querySelector(".myAttestation");

var setAttestationButton;

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
    if (groupSelectAttestation.value != "-1")
        group.push(Number(groupSelectAttestation.value));
    else {
        option = groupSelectAttestation.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            if (option[i].value != "-1")
                group.push(Number(option[i].value));
    }
    $.post("/Search/AttestationInTheSubjectGroupsLecturer", "numberAttestation=" + attestationSelect.value + "&subjectsID=" + subject + "&groupsID=" + group, function (data) {
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
        groupSelectAttestation.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
});

subjectSelectAttestation.addEventListener("change", function () {
    var url, params;
    if (subjectSelectAttestation.value == -1) {
        url = "/Search/GetGroupsOnTheLecturer";
        params = { userID: Number(getCookie("userID")) };
    } else {
        url = "/Search/GetGroupsOnTheSubject";
        params = { subjectID: subjectSelectAttestation.value };
    }
    $.post(url, params, function (data) {
        groupSelectAttestation.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
}, false);

showButtonAttestation.addEventListener("click", ShowAttestation, false);


/*=================================================*/

var typeSession = document.querySelector("select[name='typeSession']"),
    subjectSelectSession = document.querySelector("select[name='subjectSession']"),
    groupSelectSession = document.querySelector("select[name='groupSession']"),
    showButtonSession = document.querySelector("input[type='button'][name='showSession']"),
    mySession = document.querySelector(".mySession");

function ShowSession() {
    $.post("/Search/SessionResultLecturer", { subjectID: subjectSelectSession.value, groupID: groupSelectSession.value, typeSession: typeSession.value }, function (data) {
        mySession.innerHTML = "<h3>" + subjectSelectSession.selectedOptions[0].innerHTML + ":" + groupSelectSession.selectedOptions[0].innerHTML + "</h3>" + data;
        var button = mySession.querySelector("input[type='button'][name='showSetSession']") || mySession.querySelector("input[type='button'][name='createExam']");
        if (button.getAttribute("name") == "showSetSession")
            button.addEventListener('click', function() {
                $.post("/Search/ShowSetSession", { subjectID: subjectSelectSession.value, groupID: groupSelectSession.value, typeSession: typeSession.value }, function (data) {
                    mySession.innerHTML = data;
                    mySession.querySelector("input[type='button'][name='setSession']").addEventListener("click", function() {
                        var students = new Array(),
                            marks = new Array();
                        var studentResult = mySession.querySelectorAll(".studentResult");
                        for (var i = 0; i < studentResult.length; i++) {
                            students.push(studentResult[i].querySelector("input[name='studentID']").value);
                            marks.push(studentResult[i].querySelector("input[name='mark']").value);
                        }
                        $.post("/SetSession", "subjectID=" + subjectSelectSession.value + "&studentsID=" + students + "&marks=" + marks + "&typeSession=" + typeSession.value, function (data) {
                            alert(data);
                            ShowSession();
                        });
                    }, false);
                });
            }, false);
        else 
            button.addEventListener("click", function() {
                $.post("/Files/Session", { subjectID: subjectSelectSession.value, groupID: groupSelectSession.value }, function (fileName) {
                    if (fileName == null) alert("Цій групі ще не витавили сесію");
                    else window.location = '/Files/Download?fileName=' + fileName;
                });
            }, false);
    });
}

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
        groupSelectSession.innerHTML = data;
    });
});

subjectSelectSession.addEventListener("change", function () {
    $.post("/Search/GetGroupsOnTheSubject", { subjectID: subjectSelectSession.value }, function (data) {
        groupSelectSession.innerHTML = data;
    });
}, false);

showButtonSession.addEventListener("click", ShowSession, false);




