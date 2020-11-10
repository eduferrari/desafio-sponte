const uri = '/api/alunos';
let todos = [];

function getItems() {
    fetch(uri)
        .then(response => response.json())
        .then(data => _displayItems(data))
        .catch(error => console.error('Não foi possível carregar os alunos cadastrados.', error));
}

function addItem() {

    var data = new FormData();
    data.append("Nome", document.getElementById('txtAddNome').value.trim());
    data.append("Cpf", document.getElementById('txtAddCpf').value.trim());
    data.append("Email", document.getElementById('txtAddEmail').value.trim());
    data.append("DataNascimento", document.getElementById('txtAddDataNascimento').value.trim());
    data.append("files", $("#flAddFoto")[0].files[0]);

    $.ajax({
        async: true,
        type: 'POST',
        url: uri,
        contentType: false,
        processData: false,
        mimeType: "multipart/form-data",
        data: data,
        success: function () {
            swal("Parabéns!", "Curso adicionado com sucesso!", "success");
            getItems();
        }, error: function () {
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
            swal("Parabéns!", "Aluno deletado com sucesso", "success");
        })
        .catch(function (error) {
            swal("Ops!", "Não foi possível deletar o aluno!", "error");
        });
}

function displayEditForm(id) {
    const item = todos.find(item => item.alunoId === id);

    document.getElementById('hdAlunoID').value = item.alunoId;
    document.getElementById('txtUpdNome').value = item.nome;
    document.getElementById('txtUpdCpf').value = item.cpf;
    document.getElementById('txtUpdEmail').value = item.email;
    document.getElementById('txtUpdDataNascimento').value = item.dataNascimento;
    document.getElementById('txtUpdFoto').setAttribute('src', item.foto !== '' ? item.foto : '/media/avatar/no-image.jpg');
    document.getElementById('editForm').style.display = 'block';
    document.getElementById('insertForm').style.display = 'none';
}

function updateItem() {

    const itens = {
        Nome: document.getElementById('txtAddNome').value.trim(),
        Cpf: document.getElementById('txtAddCpf').value.trim(),
        Email: document.getElementById('txtAddEmail').value.trim(),
        DataNascimento: document.getElementById('txtAddDataNascimento').value.trim()
    };

    var files = document.getElementById('flAddFoto')[0].files;
    if (files.length > 0) {
        itens.append('arquivo', files[0]);
    }

    $.ajax({
        dataType: 'json',
        type: 'PUT',
        url: uri,
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify(itens),
        success: function (data) {
            swal("Parabéns!", "Aluno alterado com sucesso", "success");
            getItems();
        }, error: function (error) {
            swal("Ops!", "Não foi possível alterar o aluno!", "error");
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
    const name = (itemCount === 1) ? 'aluno' : 'alunos';
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
        editButton.setAttribute('onclick', `displayEditForm(${item.alunoId})`);

        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Excluir';
        deleteButton.classList.add('btn', 'btn-danger', 'btn-sm', 'btn-block');
        deleteButton.setAttribute('onclick', `deleteItem(${item.alunoId})`);
        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        let txtNome = document.createTextNode(item.nome);
        td1.appendChild(txtNome);

        let td2 = tr.insertCell(1);
        let txtEmail = document.createTextNode(item.email);
        td2.appendChild(txtEmail);

        let td3 = tr.insertCell(2);
        let txtCpf = document.createTextNode(item.cpf);
        td3.appendChild(txtCpf);

        let td4 = tr.insertCell(3);
        td4.appendChild(editButton);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });
    todos = data;
}