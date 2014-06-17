var lecturerSelector_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='lecturer']"),
    subjectSelector_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='subjectID']"),
    groupSelector_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='group']"),
    studentSelector_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='students']"),

    groupSelector_selectElder = document.querySelector("#selectElder select[name='group']"),
    elderSelector_selectElder = document.querySelector("#selectElder select[name='elder']");

$(document).ready(function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelector_SubjectForStudent.value }, function (data) {
        subjectSelector_SubjectForStudent.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector_SubjectForStudent.value }, function (data) {
        studentSelector_SubjectForStudent.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector_selectElder.value }, function (data) {
        elderSelector_selectElder.innerHTML = data;
    });
});

lecturerSelector_SubjectForStudent.addEventListener("change", function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelector_SubjectForStudent.value }, function (data) {
        subjectSelector_SubjectForStudent.innerHTML = data;
    });
}, false);

groupSelector_SubjectForStudent.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector_SubjectForStudent.value }, function (data) {
        studentSelector_SubjectForStudent.innerHTML = data;
    });
}, false);

groupSelector_selectElder.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelector_selectElder.value }, function (data) {
        elderSelector_selectElder.innerHTML = data;
    });
}, false);