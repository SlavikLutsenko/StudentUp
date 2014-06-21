var lecturerSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='lecturer']"),
    subjectSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='subjectID']"),
    groupSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='group']"),
    studentSelect_SubjectForStudent = document.querySelector("#setSubjectForStudent select[name='students']"),

    groupSelect_selectElder = document.querySelector("#selectElder select[name='group']"),
    elderSelect_selectElder = document.querySelector("#selectElder select[name='elder']");

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


document.querySelector("input[type='button'][name='attestation1']").addEventListener('click', function() {
    //$.ajax({
    //    type: 'POST',
    //    url: "/Files/Attestation1", 
    //    contentType: 'application/json; charset=utf-8',
    //    dataType: 'json',
    //    success: function (returnValue) {
    //        alert(returnValue);
    //        window.location = '/Files/Download?file=' + returnValue;
    //    },
    //    error: function(e) {
    //        alert("error");
    //    }
    //});
    $.post("/Files/Attestation1", function (fileName) {
         window.location = '/Files/Download?fileName=' + fileName;
    });
}, false);

document.querySelector("input[type='button'][name='attestation2']").addEventListener('click', function() {
    alert(2);
}, false);

document.querySelector("input[type='button'][name='session']").addEventListener('click', function() {
    alert(3);
}, false);