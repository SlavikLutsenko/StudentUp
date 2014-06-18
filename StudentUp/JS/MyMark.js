var subjectSelect = document.querySelector("select[name='subject']"),
    groupSelect = document.querySelector("select[name='group']"),
    studentSelector = document.querySelector("select[name='student']"),
    showButton = document.querySelector("input[type='button'][name='showMark']"),
    myMark = document.querySelector(".myMark");

function ShowMark() {
    var subject, group, student, option;
    subject = group = student = "(";
    if (subjectSelect.value != "-1")
        subject += subjectSelect.value;
    else {
        option = subjectSelect.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++) 
            subject += (option[i].value == "-1" ? "" : option[i].value + (i != end - 1 ? "," : ""));
    }
    if (groupSelect.value != "-1")
        group += groupSelect.value;
    else {
        option = groupSelect.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            group += (option[i].value == "-1" ? "" : option[i].value + (i != end - 1 ? "," : ""));
    }
    if (studentSelector.value != "-1")
        student += studentSelector.value;
    else {
        option = studentSelector.querySelectorAll("option");
        for (var i = 0, end = option.length; i < end; i++)
            student += (option[i].value == "-1" ? "" : option[i].value + (i != end - 1 ? "," : ""));
    }
    subject += ")";
    group += ")";
    student += ")";
    $.post("/Search/ShowMark", {userID: Number(getCookie("userID")), subjectsID: subject, groupsID: group, studentsID: student}, function (data) {
        myMark.innerHTML = data;
    });
}

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
        groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
        $.post("/Search/GetStudentsOnTheLecturer", { userID: Number(getCookie("userID")), subjectID: -1 }, function (data) {
            studentSelector.innerHTML = "<option value='-1'>Усі</option>" + data;
            ShowMark();
        });
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
        if (groupSelect.value == -1) {
            url = "/Search/GetStudentsOnTheLecturer";
            params = { userID: Number(getCookie("userID")), subjectID: subjectSelect.value };
        } else {
            url = "/Search/GetStudentsFromGroupOnTheSubject";
            params = { groupID: groupSelect.value, subjectID: subjectSelect.value };
        }
        $.post(url, params, function (data) {
            studentSelector.innerHTML = "<option value='-1'>Усі</option>" + data;
        });
    });
}, false);

groupSelect.addEventListener("change", function () {
    var url, params;
    if (groupSelect.value == -1) {
        url = "/Search/GetStudentsOnTheLecturer";
        params = { userID: Number(getCookie("userID")), subjectID: subjectSelect.value };
    } else {
        url = "/Search/GetStudentsFromGroupOnTheSubject";
        params = { groupID: groupSelect.value, subjectID: subjectSelect.value };
    }
    $.post(url, params, function (data) {
        studentSelector.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
}, false);

showButton.addEventListener("click", ShowMark, false);