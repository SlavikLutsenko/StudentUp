var lecturerSelector = document.querySelector("#setSubjectForStudent select[name='lecturer']"),
    subjectSelector = document.querySelector("#setSubjectForStudent select[name='subjectID']"),
    groupSelector = document.querySelector("#setSubjectForStudent select[name='group']"),
    studentSelector = document.querySelector("#setSubjectForStudent select[name='students']");

$(document).ready(function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelector.value }, function (data) {
        subjectSelector.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector.value }, function (data) {
        studentSelector.innerHTML = data;
    });
});

lecturerSelector.addEventListener("change", function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelector.value }, function (data) {
        subjectSelector.innerHTML = data;
    });
}, false);

groupSelector.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector.value }, function (data) {
        studentSelector.innerHTML = data;
    });
}, false);