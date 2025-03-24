
function filterTable() {
    var municipioFilter = document.getElementById('municipio').value.trim().toLowerCase();
    var cargoFilter = document.getElementById('cargo').value.trim().toLowerCase();
    var dataInicio = document.getElementById('dataInicio').value;
    
    var dataFim = document.getElementById('dataFim').value;      
    

    var table = document.getElementById('cadastrosTable');
    var rows = table.getElementsByTagName('tr');
    
    for (var i = 1; i < rows.length; i++) {
        var cells = rows[i].getElementsByTagName('td');
        
        var municipio = cells[2].textContent.trim().toLowerCase();
        var cargo = cells[3].textContent.trim().toLowerCase();
        var dataCadastroText = cells[4].textContent.trim();

        var showRow = true;

        if (municipioFilter && municipio.indexOf(municipioFilter) === -1) {
            showRow = false;
        }

        if (cargoFilter && cargo.indexOf(cargoFilter) === -1) {
            showRow = false;
        }

        if ((dataInicio || dataFim) && dataCadastroText) {
            var parts = dataCadastroText.split('/');
            if (parts.length === 3) {
                var dataCadastro = new Date(parts[2], parts[1] - 1, parts[0]);

                if (dataInicio) {
                    var inicioDate = new Date(dataInicio + "T00:00:00");
                    if (dataCadastro < inicioDate) {
                        showRow = false;
                    }
                }

                if (dataFim) {
                    var fimDate = new Date(dataFim + "T23:59:59");
                    if (dataCadastro > fimDate) {
                        showRow = false;
                    }
                }
            }
        }

        rows[i].style.display = showRow ? "" : "none";
    }
}

function resetFilters() {
    document.getElementById('municipio').value = "";
    document.getElementById('cargo').value = "";
    document.getElementById('dataInicio').value = "";
    document.getElementById('dataFim').value = "";
    filterTable();
}

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