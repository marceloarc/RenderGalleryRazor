function showChangeImageButton() {
    document.getElementById('changeImageButton').style.display = 'block';
}

function hideChangeImageButton() {
    document.getElementById('changeImageButton').style.display = 'none';
}

function openFileSelector() {
    document.getElementById('avatarInput').click();
}

function previewImage(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            document.getElementById('userAvatar').src = e.target.result;
        };

        reader.readAsDataURL(input.files[0]);
    }
}

function atualizarDetalhes() {
    // Adicione aqui a lógica para atualizar os detalhes da conta, incluindo o novo avatar.
}