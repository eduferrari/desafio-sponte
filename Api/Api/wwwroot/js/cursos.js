const uri = '/api/cursos';
let todos = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => swal("Ops!", 'Não foi possível carregar os alunos cadastrados.', 'error'));
}

function addItem() {

    const itens = {
        Nome: document.getElementById('txtAddNome').value.trim(),
        Duracao: parseInt(document.getElementById('txtAddDuracao').value.trim(), 10),
        DataLimiteMatricula: document.getElementById('txtAddDataLimiteMatricula').value.trim(),
        Custo: currencyFormat(document.getElementById('txtAddCusto').value.trim()),
        DisciplinasAssociadas: document.getElementById('txtAddDisciplinasAssociadas').value.trim()
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
            swal("Parabéns!", "Curso adicionado com sucesso", "success");
            getItems();
        }, error: function (error) {
            swal("Ops!", "Não foi possível adicionar o curso!", "error");
        }
    });
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE'
    })
        .then(() => getItems())
        .then(function (text) {
            swal("Parabéns!", "Curso deletado com sucesso", "success");
        })
        .catch(function (error) {
            swal("Ops!", "Não foi possível deletar o curso!", "error");
        });
}

function displayEditForm(id) {
    const item = todos.find(item => item.cursoId === id);

    document.getElementById('hdCursoID').value = item.cursoId;
    document.getElementById('txtUpdNome').value = item.nome;
    document.getElementById('txtUpdDuracao').value = item.duracao;
    document.getElementById('txtUpdDataLimiteMatricula').value = item.dataLimiteMatricula;
    document.getElementById('txtUpdCusto').value = currencyFormat(item.custo);
    document.getElementById('txtUpdDisciplinasAssociadas').value = item.disciplinasAssociadas;
    document.getElementById('editForm').style.display = 'block';
    document.getElementById('insertForm').style.display = 'none';
}

function updateItem() {

    const itens = {
        CursoID: parseInt(document.getElementById('hdCursoID').value.trim(), 10),
        Nome: document.getElementById('txtUpdNome').value.trim(),
        Duracao: parseInt(document.getElementById('txtUpdDuracao').value.trim(), 10),
        DataLimiteMatricula: document.getElementById('txtUpdDataLimiteMatricula').value.trim(),
        Custo: currencyFormat(document.getElementById('txtUpdCusto').value.trim()),
        DisciplinasAssociadas: document.getElementById('txtUpdDisciplinasAssociadas').value.trim()
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
            swal("Parabéns!", "Curso alterado com sucesso", "success");
            getItems();
        }, error: function (error) {
            swal("Ops!", "Não foi possível alterar o curso!", "error");
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
    const name = (itemCount === 1) ? 'curso' : 'cursos';
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
        editButton.setAttribute('onclick', `displayEditForm(${item.cursoId})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Excluir';
        deleteButton.classList.add('btn', 'btn-danger', 'btn-sm', 'btn-block');
        deleteButton.setAttribute('onclick', `deleteItem(${item.cursoId})`);
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let txtNome = document.createTextNode(item.nome);
        td1.appendChild(txtNome);

        let td2 = tr.insertCell(1);
        let txtCusto = document.createTextNode('R$ ' + currencyFormat(item.custo));
        td2.appendChild(txtCusto);

        let td3 = tr.insertCell(2);
        let txtDataLimiteMatricula = document.createTextNode(item.dataLimiteMatricula);
        td3.appendChild(txtDataLimiteMatricula);

        let td4 = tr.insertCell(3);
        let txtDuracao = document.createTextNode(item.duracao + 'H');
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