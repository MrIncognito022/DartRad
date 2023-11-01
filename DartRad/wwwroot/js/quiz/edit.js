$(document).ready(() => {
    var disabled = $('#disableInputs').val();
    if (disabled) {
        $('input,select,textarea').attr('disabled', disabled);
    }
});