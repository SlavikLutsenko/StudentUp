var subjectSelect = document.querySelector("select[name='subjectID']");

$(document).ready(function () {
    $.post("/Search/GetGroupsOnTheSubject", { subjectID: subjectSelect.value }, function (data) {
        document.querySelector("select[name='groupID']").innerHTML = data;
    });
});

subjectSelect.addEventListener("change", function () {
    $.post("/Search/GetGroupsOnTheSubject", { subjectID: subjectSelect.value }, function (data) {
        document.querySelector("select[name='groupID']").innerHTML = data;
    });
}, false); 