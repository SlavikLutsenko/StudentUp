var subjectSelect = document.querySelector("select[name='subject']"),
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
    subject += ")";
    $.post("/Search/ShowMarkStudent", { userID: Number(getCookie("userID")), subjectsID: subject }, function (data) {
        myMark.innerHTML = data;
    });
}

showButton.addEventListener("click", ShowMark, false);

ShowMark();