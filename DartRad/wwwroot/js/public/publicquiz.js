var quizId = 0;
$(document).ready(() => {
    quizId = $('#quizId').val();
    $('#quizForm').parsley({
        triggerAfterFailure: 'input change'
    });

});

$('#btnSubmit').click(() => {


    var isValid = $('#quizForm').parsley().validate();

    if (!isValid) {
        console.log('fail');
        return;
    }

    // Create an array to hold the answers
    let answers = [];

    // Get the selected answers for multiple-choice questions
    $('input[type="radio"]:checked').each(function () {
        let questionId = $(this).attr('name').replace('ans_', '');
        let selectedAnswerId = $(this).val();
        let answer = {
            questionId: questionId,
            selectedAnswerId: selectedAnswerId
        };
        answers.push(answer);
    });

    // Get the written answers for open-ended questions
    $('input[type="text"]').each(function () {
        let questionId = $(this).attr('name').replace('ans_', '');
        let writtenAnswer = $(this).val();
        let answer = {
            questionId: questionId,
            writtenAnswer: writtenAnswer
        };
        answers.push(answer);
    });

    // Prepare the data to be sent to the server
    let data = {
        quizId: quizId,
        answers: answers
    };
    $.ajax({
        url: '/Quiz/' + quizId,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {
            console.log(response);
            if (!response.success) {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
            else {
                location.href = `/quiz/${quizId}/result` 
            }
        },
        error: function (xhr, status, error) {
            debugger;
            var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Failed to submit the quiz.';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage
            });
        }

    });
});