var subjectSelect = document.querySelector("select[name='subject']"),
    groupSelect = document.querySelector("select[name='group']"),
    studentSelector = document.querySelector("select[name='student']");

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheSubject", { subjectID: subjectSelect.value }, function (data) {
        groupSelect.innerHTML = data;
        $.post("/Search/GetStudentsFromGroupOnTheSubject", { groupID: groupSelect.value, subjectID: subjectSelect.value }, function (data) {
            studentSelector.innerHTML = data;
        });
    });
});

subjectSelect.addEventListener("change", function () {
    $.post("/Search/GetGroupsOnTheSubject", { subjectID: subjectSelect.value }, function (data) {
        groupSelect.innerHTML = data;
        $.post("/Search/GetStudentsFromGroupOnTheSubject", { groupID: groupSelect.value, subjectID: subjectSelect.value }, function (data) {
            studentSelector.innerHTML = data;
        });
    });
}, false);

groupSelect.addEventListener("change", function () {
    $.post("/Search/GetStudentsFromGroupOnTheSubject", { groupID: groupSelect.value, subjectID: subjectSelect.value }, function (data) {
        studentSelector.innerHTML = data;
    });
}, false);