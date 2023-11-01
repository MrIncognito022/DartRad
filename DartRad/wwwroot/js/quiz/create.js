$(document).ready(function () {
    $('#addQuestionButton').click(function () {
        $.ajax({
            url: '../Quiz/QuesionPartial',
            type: 'GET',
            success: function (partialView) {
                let container = `<div class="partial-inner-container row align-items-center border p-2 m-1">
                                ${partialView}
                                <div class="col-1">
                                <button onclick="RemoveContainer(this)" type="button" class="btn btn-danger remove-question-button">&times;</button>

</div>
                                </div>`;
                $('#questionsContainer').append(container);
            },
            error: function () {
                console.log('Failed to load the question editor.');
            }
        });
    });

   
});

function RemoveContainer(button) {
    var questionText = $(button).closest('.partial-inner-container').find('[name="QuestionText"]').val();

    if (questionText && questionText.trim() !== '') {
        Swal.fire({
            title: 'Confirmation',
            text: 'Are you sure you want to remove this question? Any unsaved changes will be lost.',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#3085d6',
            cancelButtonColor: '#d33',
            confirmButtonText: 'Yes',
            cancelButtonText: 'No'
        }).then(function (result) {
            if (result.isConfirmed) {
                $(button).closest('.partial-inner-container').remove();
            }
        });
    } else {
        $(button).closest('.partial-inner-container').remove();
    }
}