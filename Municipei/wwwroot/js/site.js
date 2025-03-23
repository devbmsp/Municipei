$(document).ready(function () {
    $('#Cpf').mask('000.000.000-00');
    $('#Phone').mask('(00) 00000-0000');
});
document.getElementById("generateCode").addEventListener("click", function (event) {
    event.preventDefault();
    var email = document.getElementById("regEmail").value;
    if (email.trim() !== "") {
        document.getElementById("authEmail").value = email;
        document.getElementById("registerForm").style.display = "none";
        document.getElementById("authForm").style.display = "block";
    } else {
        alert("Preencha o campo de email!");
    }
});
$(function () {
    $('#Cpf').mask('000.000.000-00');
    $('#Phone').mask('(00) 00000-0000');
});

document.getElementById("generateCode").onclick = () => {
    const email = $('#regEmail').val().trim();
    if (!email) return alert('Preencha o campo de email!');

    $('#hiddenEmail, #authEmail').val(email);
    $('#registerForm').hide();
    $('#authForm').show();
};

document.getElementById("sendCodeBtn").onclick = async () => {
    const email = $('#hiddenEmail').val();
    const token = $('input[name="__RequestVerificationToken"]').val();

    const res = await fetch('@Url.Action("Authorization","Home")', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/x-www-form-urlencoded',
            'RequestVerificationToken': token
        },
        body: new URLSearchParams({ email })
    });

    if (!res.ok) {
        const err = await res.text();
        return alert(err);
    }

    $('#userEmail').text(email);
    $('#authForm').hide();
    $('#codeContainer').show();
};