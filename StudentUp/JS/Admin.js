var lecturerSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='lecturer']"),
    subjectSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='subjectID']"),
    groupSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='group']"),
    studentSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='students']"),

    groupSelect_selectElder = document.querySelector("#selectElder select[name='group']"),
    elderSelect_selectElder = document.querySelector("#selectElder select[name='elder']"),

    groupSelect_Mail = document.querySelector("#b-mailParents select[name='groupID']"),
    studentSelect_Mail = document.querySelector("#b-mailParents select[name='studentID']");

$(document).ready(function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelect_SubjectForStudent.value }, function (data) {
        subjectSelect_SubjectForStudent.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_SubjectForStudent.value }, function (data) {
        studentSelect_SubjectForStudent.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_selectElder.value }, function (data) {
        elderSelect_selectElder.innerHTML = data;
    });
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_Mail.value }, function (data) {
        studentSelect_Mail.innerHTML = data;
    });
});

lecturerSelect_SubjectForStudent.addEventListener("change", function () {
    $.post("/Search/GetSubjectsLecturer", { lecturerID: lecturerSelect_SubjectForStudent.value }, function (data) {
        subjectSelect_SubjectForStudent.innerHTML = data;
    });
}, false);

groupSelect_SubjectForStudent.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_SubjectForStudent.value }, function (data) {
        studentSelect_SubjectForStudent.innerHTML = data;
    });
}, false);

groupSelect_selectElder.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_selectElder.value }, function (data) {
        elderSelect_selectElder.innerHTML = data;
    });
}, false);


document.querySelector("input[type='button'][name='attestation1']").addEventListener('click', function () {
    $.post("/Files/Attestation", { numberAttestation: 1 }, function (fileName) {
        window.location = '/Files/Download?fileName=' + fileName;
    });
}, false);

document.querySelector("input[type='button'][name='attestation2']").addEventListener('click', function () {
    $.post("/Files/Attestation", { numberAttestation: 2 }, function (fileName) {
        window.location = '/Files/Download?fileName=' + fileName;
    });
}, false);

/*****************************************************************/

groupSelect_Mail.addEventListener("change", function () {
    $.post("/Search/GetGroupOfStudents", { groupID: groupSelect_Mail.value }, function (data) {
        studentSelect_Mail.innerHTML = data;
        $.post("/Search/GetContactParentStudent", { studentID: studentSelect_Mail.value }, function (data) {
            document.querySelector("#b-mailParents input[type='email'][name='emailParents']").value = data;
        });
    });
}, false);

studentSelect_Mail.addEventListener("change",function() {
    $.post("/Search/GetContactParentStudent", { studentID: studentSelect_Mail.value }, function (data) {
        document.querySelector("#b-mailParents input[type='email'][name='emailParents']").value = data;
    });
},false)