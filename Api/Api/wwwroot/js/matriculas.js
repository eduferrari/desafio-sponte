const uri = '/api/matriculas';
let todos = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => swal("Ops!", 'Não foi possível carregar as matrículas!', 'error'));

    getAlunos('slAddAluno');
    getCursos('slAddCursos');
}

function getAlunos(input) {

    let addAlunos = document.getElementById(input);
    addAlunos.length = 0;

    let defaultOption = document.createElement('option');
    defaultOption.text = 'Selecione o aluno';
    defaultOption.value = '';

    addAlunos.add(defaultOption);
    addAlunos.selectedIndex = 0;

    const url = '/api/alunos';

    fetch(url)
        .then(
            function (response) {
                if (response.status === 200) {
                    response.json().then(function (data) {
                        let option;

                        for (let i = 0; i < data.length; i++) {
                            option = document.createElement('option');
                            option.text = data[i].nome;
                            option.value = data[i].alunoId;
                            addAlunos.add(option);
                        }
                    });
                } else {
                    swal("Ops!", 'Não foi possível carregar a lista de alunos!', 'error')
                }
            }
        )
        .catch(function (err) {
            swal("Ops!", 'Não foi possível carregar a lista de alunos!', 'error')
        });
}

function getCursos(input) {

    let addCursos = document.getElementById(input);

    addCursos.length = 0;

    let defaultOption = document.createElement('option');
    defaultOption.text = 'Selecione os cursos';
    defaultOption.value = '';

    addCursos.add(defaultOption);
    addCursos.selectedIndex = 0;

    const url = '/api/cursos';

    fetch(url)
        .then(
            function (response) {
                if (response.status === 200) {
                    response.json().then(function (data) {
                        let option;

                        for (let i = 0; i < data.length; i++) {
                            option = document.createElement('option');
                            option.text = data[i].nome + ' > R$ ' + currencyFormat(data[i].custo);
                            option.value = data[i].cursoId;
                            addCursos.add(option);
                        }
                    });
                } else {
                    swal("Ops!", 'Não foi possível carregar a lista de cursos!', 'error')
                }
            }
        )
        .catch(function (err) {
            swal("Ops!", 'Não foi possível carregar a lista de cursos!', 'error')
        });
}

function addItem() {

    const itens = {
        AlunoId: parseInt(document.getElementById('slAddAluno').value.trim(), 10),
        Cursos: $('#slAddCursos').val().toString(),
        ValorTotal: 0,
        Data: document.getElementById('txtAddData').value.trim()
    };

    $.ajax({
        dataType: 'json',
        type: 'POST',
        url: uri,
        data: JSON.stringify(itens),
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        success: function (data) {
            swal("Parabéns!", "Matrícula efetuada com sucesso", "success");
            getItems();
        }, error: function (error) {
            swal("Ops!", "Não foi possível efetuar sua matrícula!", "error");
        }
    });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getItems())
        .then(function (text) {
            swal("Parabéns!", "Matrícula cancelada com sucesso!", "success");
        })
        .catch(function (error) {
            swal("Ops!", "Não foi possível cancelar sua matrícula!", "error");
        });
}

function displayEditForm(id) {

    getAlunos('slUpdAluno');
    getCursos('slUpdCursos');

    const item = todos.find(item => item.matriculaId === id);

    document.getElementById('hdMatriculaID').value = item.matriculaId;
    document.getElementById('txtUpdData').value = item.data;
    document.getElementById('txtUpdValorTotal').value = currencyFormat(item.valorTotal);
    document.getElementById('editForm').style.display = 'block';
    document.getElementById('insertForm').style.display = 'none';

    setTimeout(function () {
        document.getElementById('slUpdAluno').value = item.alunoId;

        $.each(item.cursos.split(","), function (i, e) {
            $("#slUpdCursos option[value='" + e + "']").prop("selected", true);
        });

    }, 1500);
}

function updateItem() {

    const itens = {
        MatriculaId: parseInt(document.getElementById('hdMatriculaID').value.trim(), 10),
        AlunoId: parseInt(document.getElementById('slUpdAluno').value.trim(), 10),
        Cursos: $('#slUpdCursos').val().toString(),
        ValorTotal: 0,
        Data: document.getElementById('txtUpdData').value.trim()
    };

    $.ajax({
        dataType: 'json',
        type: 'PUT',
        url: uri,
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(itens),
        success: function (data) {
            swal("Parabéns!", "Matrícula alterada com sucesso!", "success");
            getItems();
        }, error: function (error) {
            swal("Ops!", "Não foi possível alterar sua matrículo!", "error");
        }
    });

    closeInput();
    return false;
}

function closeInput() {
    document.getElementById('insertForm').style.display = 'block';
    document.getElementById('editForm').style.display = 'none';
}

function _displayCount(itemCount) {
    const name = (itemCount === 1) ? 'matrícula' : 'matrículas';
    document.getElementById('counter').innerText = `${itemCount} ${name}`;
}

function _displayItems(data) {
    const tBody = document.getElementById('list');
    tBody.innerHTML = '';
    _displayCount(data.length);
    const button = document.createElement('button');
    data.forEach(item => {
        let editButton = button.cloneNode(false);
        editButton.innerText = 'Editar';
        editButton.classList.add('text-center', 'btn', 'btn-warning', 'btn-sm', 'btn-block');
        editButton.setAttribute('onclick', `displayEditForm(${item.matriculaId})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Cancelar';
        deleteButton.classList.add('btn', 'btn-danger', 'btn-sm', 'btn-block');
        deleteButton.setAttribute('onclick', `deleteItem(${item.matriculaId})`);
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let txtMatricula = document.createTextNode(item.matriculaId);
        td1.appendChild(txtMatricula);

        let td2 = tr.insertCell(1);
        let txtAluno = document.createTextNode(item.alunoNome);
        td2.appendChild(txtAluno);

        let td3 = tr.insertCell(2);
        let txtData = document.createTextNode(item.data);
        td3.appendChild(txtData);

        let td4 = tr.insertCell(3);
        let txtDuracao = document.createTextNode('R$ ' + currencyFormat(item.valorTotal));
        td4.appendChild(txtDuracao);

        let td5 = tr.insertCell(4);
        td5.appendChild(editButton);

        let td6 = tr.insertCell(5);
        td6.appendChild(deleteButton);
    });
    todos = data;
}

function currencyFormat(num) {
    return num.toLocaleString('pt-br', { minimumFractionDigits: 2 });
}