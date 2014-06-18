var subjectSelect = document.querySelector("select[name='subject']"),
    groupSelect = document.querySelector("select[name='group']"),
    studentSelector = document.querySelector("select[name='student']");

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
        groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
        $.post("/Search/GetStudentsOnTheLecturer", { userID: Number(getCookie("userID")), subjectID: -1 }, function (data) {
            studentSelector.innerHTML = "<option value='-1'>Усі</option>" + data;
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