var searchSelect = document.querySelector("select[name='searchType']"),
    subjectSelect = document.querySelector("select[name='subject']"),
    groupSelect = document.querySelector("select[name='group']"),
    showButton = document.querySelector("input[type='button'][name='showMark']"),
    myStudent = document.querySelector(".myStudent");

function ShowAttestation() {
    var subject = new Array(),
        group = new Array(),
        searchType = new Array();
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
    $.post("/Search/SearchStudent", "searchType=" + searchSelect.value + "&subjectsID=" + subject + "&groupsID=" + group, function (data) {
        myStudent.innerHTML = data;
    });
}

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
        groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
    });
});

searchSelect.addEventListener("change", function() {
    switch (this.value) {
        case "1":
        case "2":
            subjectSelect.style.display = "block";
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
            break;
        case "3":
            subjectSelect.style.display = "none";
            $.post("/Search/GetGroupsOnTheLecturer", { userID: Number(getCookie("userID")) }, function (data) {
                groupSelect.innerHTML = "<option value='-1'>Усі</option>" + data;
            });
            break;
    }
}, false);

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