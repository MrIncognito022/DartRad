let quizId = $('#h_quizId').val();
let isApproved = $('#h_quizApproved').val() == 'value';

const QuestionTypes = {
    MultipleChoice: "Multiple Choice",
    TrueFalse: "True False",
    ShortAnswer: "Short Answer",
    Matching: "Matching"
}

$(document).ready(function () {
    // Call the loadQuestions function to fetch and display the questions on page load
    loadQuestions(quizId);
});
var questionsArr = [];
// Function to fetch questions data and append to the container
function loadQuestions(quizId) {
    $.ajax({
        url: '../GetQuestions',
        type: 'GET',
        data: { id: quizId },
        success: function (questions) {
            questionsArr = questions.data;
            // Clear the existing questions container
            $('#questionsContainer').empty();

            // Append each question to the container
            questions.data.forEach(function (question) {
                let answerHtml = '';

                let hasAnswers = true;
                let isCorrectAnsAvailable = false;

                let answerArr = [];
                if (question.questionType === QuestionTypes.MultipleChoice) {
                  
                    answerArr = question.answerMultipleChoices;
                    if (answerArr.length == 0) {
                        hasAnswers = false;
                    }
                }
                else if (question.questionType == QuestionTypes.ShortAnswer) {
                   
                    answerArr = question.answerShortAnswer;
                    if (answerArr.length == 0) {
                        hasAnswers = false;
                    }
                }
                else if (question.questionType == QuestionTypes.Matching) {
                    isCorrectAnsAvailable = true; // all answers are true
                    answerArr = question.answerMatching;
                    if (answerArr.length == 0) {
                        hasAnswers = false;
                    }
                }

                if (hasAnswers == true) {
                    answerArr.forEach(answer => {

                        let answerButtons = `  <div>
                                                     <button class="btn btn-sm btn-secondary" onclick="editAnswer(${question.id}, ${answer.id})"><i class="fa fa-pencil"></i></button>
                                                     <button class="btn btn-sm btn-danger" onclick="deleteAnswer(${question.id}, ${answer.id})"><i class="fa fa-trash"></i> </button>               
                                               </div>`;
                        if (question.questionType == QuestionTypes.Matching) {
                            answerHtml += `
                             <li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <!-- Split again in 2 divs one for left side and one for right -->
                                    <div class="row" style="width:80%;margin-left:5px;" >
                                        <div class="col-5 left-answer">
                                            ${answer.leftSide}
                                        </div>
                                        <div class="col-1 right-arrow">
                                         <i class="fa fa-long-arrow-right fa-2x"></i>
                                        </div>
                                        <div class="col-5 right-answer">
                                            ${answer.rightSide}
                                        </div>
                                    </div>

                                     ${!isApproved ? answerButtons : ''}
                                </div>
                                
                            </li>`;
                       
                        }
                        else {
                            answerHtml += `
                            <li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <div>${answer.answerText}
                                ${answer.isCorrect ? `<span class="badge bg-success">Correct</span>` : ''}</div>

                                ${!isApproved ? answerButtons : ''}
                                </div>
                                
                            </li>`;
                        }
                       
                        if (answer.isCorrect) {
                            isCorrectAnsAvailable = true;
                        }
                    });
                }
                       
                let addAnswerButton = `<button title="Add New Answer" class="btn btn-sm btn-primary" onclick="addAnswer(${question.id})"><i class="fa fa-plus"></i> Add </button>`;

                let errorText = 'No Correct Answer Exists';

                //if (question.questionType == QuestionTypes.Matching) {
                //    errorText = "No Answer Exists";
                //}

                let noCorrectAnswerError = `<span class="text-danger error-text"><i class="fa fa-exclamation-triangle"></i> ${errorText}</span>`;

                let cardBody = `<div class="card-body" >
                            <div class="d-flex justify-content-between mt-2 mb-2 mx-3">
                                <div>
                                    <div class="fw-bold fs-5 card-subtitle mb-2 mr-1" style="display:inline"> Answers </div>
                                    ${ (hasAnswers && isCorrectAnsAvailable) ? '' : noCorrectAnswerError}
                                    </div>

                                ${!isApproved ? addAnswerButton : ''}
                            </div>
                            <ol class="answer-list" id="answer_div_${question.id}">
                                ${answerHtml}
                            </ol>
                        </div>`;

                if (question.questionType == QuestionTypes.TrueFalse) {
                   
                    cardBody = '';
                }

                let questionButtons = `  <button title="Edit" onclick="editQuestion(${question.id})" class="btn btn-sm btn-secondary"><i class="fa fa-pencil"></i></button>
                                    <button title="Delete" onclick="deleteQuestion(${question.id})" class="btn btn-sm btn-danger"><i class="fa fa-trash"></i></a>
                              `;

                const questionHtml = `
                    <div class="card mt-2">
                        <div class="card-header">
                            <div class="d-flex flex-row justify-content-between">
                                <div>
                                    <h5 class="card-text mb-0">${question.questionText}</h5>
                                    <small class="card-subtitle mb-2 text-muted">${question.questionType}</small>
                                </div>
                                <div>

                                    ${!isApproved ? questionButtons : ''}
                                </div>
                            </div>
                        </div>
                        ${cardBody}
                    </div>`;

                $('#questionsContainer').append(questionHtml);
            });
        },
        error: function () {
            console.log('Failed to load questions.');
        }
    });
}

function deleteQuestion(questionId) {
    var quizId = $('#h_quizId').val();
    // Show a confirmation dialog
    swal.fire({
        title: 'Delete Question',
        text: 'Are you sure you want to delete this question?\nThis will delete all associated answers',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Delete',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {
            // Send a delete request to the server
            $.ajax({
                url: `../DeleteQuestion`,
                data: {
                    quizId: quizId,
                    questionId: questionId
                },
                type: 'DELETE',
                success: function (response) {
                    // Check the response and show a success dialog
                    if (response.success) {
                        swal.fire({
                            title: 'Success',
                            text: 'Question deleted successfully',
                            icon: 'success',
                            timer: 3000, // Autohide after 3 seconds
                            showConfirmButton: true
                        });

                        // Refresh the questions list
                        loadQuestions(quizId);
                    } else {
                        swal.fire({
                            title: 'Error',
                            text: response.message,
                            icon: 'error'
                        });
                    }
                },
                error: function (xhr, status, error) {
                    var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Failed to create the question.';
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: errorMessage
                    });
                }
            });
        }
    });
}

$('#btnCreateQuestion').click(function () {
    $('#questionId').val('');
    $('#questionType').val('0');
    $('#questionText').val('');

    $('#questionModalLabel').text("Add Question");
    $('#questionModal').modal('show');
});

$('#saveQuestionBtn').click(function () {
    var questionId = $('#questionId').val();
    // determine the operation based on questionId
    if (questionId) {
        updateQuestion();
    }
    else {
        addQuestion();
    }
});

function addQuestion() {
    // Get the question data from the modal inputs
    var questionType = $('#questionType').val();
    var questionText = $('#questionText').val();

    // Create a data object to send in the AJAX request
    var data = {
        questionType: questionType,
        questionText: questionText
    };

    // Send the AJAX request to the server to create the question
    $.ajax({
        url: '../CreateQuestion?quizId=' + quizId,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                // Close the modal
                $('#questionModal').modal('hide');
                swal.fire({
                    title: "Question added successfully!",
                    icon: "success",
                    timer: 3000, // Duration in milliseconds (3 seconds)
                    buttons: false, // Hide the "OK" button
                }).then(function () {
                    // Refresh the question list or perform any other actions
                    // Clear the modal inputs
                    $('#questionType').val('0');
                    $('#questionText').val('');
                   
                    // Refresh the questions list
                    loadQuestions(quizId);

                    
                });

            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }

        },
        error: function (xhr, status, error) {
            var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Failed to create the question.';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage
            });
        }
    });
}

function editQuestion(questionId) {
    // Retrieve question data via AJAX
    $.ajax({
        url: '../GetQuestion',
        type: 'GET',
        data: { questionId: questionId, quizId: quizId },
        success: function (response) {
            if (response.success) {
               
                // Populate the modal fields with the existing question data
                $('#questionId').val(response.data.id);
                $('#questionText').val(response.data.questionText);
                $('#questionType').val(response.data.questionType);
                $('#questionModalLabel').text("Edit Question");
                // Open the question modal
                $('#questionModal').modal('show');
            }
            else {

            }
           
        },
        error: function () {
            
            Swal.fire({
                icon: 'error',
                title: 'Failed to retrieve question data.',
                text: response.message
            });
        }
    });
}

function updateQuestion() {
    var questionId = $('#questionId').val();
    var questionText = $('#questionText').val();
    var questionType = $('#questionType').val();

    // Prepare the data to be sent in the AJAX request
    var data = {
        id: questionId,
        questionText: questionText,
        questionType: questionType
    };

    // Send an AJAX request to update the question
    $.ajax({
        url: '../UpdateQuestion?quizid=' + quizId,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {
            if (response.success) {
                // Close the question modal
                $('#questionModal').modal('hide');
                // Display a success message
                Swal.fire({
                    icon: 'success',
                    title: 'Question Updated',

                    buttons: false,
                    timer: 3000
                });
                // Refresh the question list
                loadQuestions(quizId);
            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
           
        },
        error: function () {
            // Display an error message
            Swal.fire({
                icon: 'error',
                title: 'Failed to Update Question',
                text: 'An error occurred while updating the question.',
                timer: 3000
            });
        }
    });
}

function addAnswer(questionId) {
    var questionById = questionsArr.find(x => x.id == questionId);

    if (questionById.questionType == QuestionTypes.Matching) {
        // hide regular fields and diplay fields relevant to this type
        $('#divRegularAnswer').hide();
        $('#divMatchingAnswers').show();
    }
    else {

        $('#divRegularAnswer').show();
        $('#divMatchingAnswers').hide();
    }

    $('#isCorrectContainer').show();
    // Clear the input fields
    $('#answerModal input[name="questionId"]').val(questionId);
    $('#answerText').val('');
    $('#isCorrect').prop('checked', false);

    // Display the addAnswerModal
    $('#answerModal').modal('show');

    $('#answerModalLabel').text("Add Answer");

    // check question type
    var q = questionsArr.find(x => x.id == questionId);
    if (q.questionType == "Short Answer") {
        // hide checkbox
        $('#isCorrectContainer').hide();
    }
   
}

function editAnswer(questionId, answerId) {
    $('#answerModal input[name="questionId"]').val(questionId);
    $('#isCorrectContainer').show();

    var questionById = questionsArr.find(x => x.id == questionId);

    if (questionById.questionType == QuestionTypes.Matching) {
        // hide regular fields and diplay fields relevant to this type
        $('#divRegularAnswer').hide();
        $('#divMatchingAnswers').show();
    }
    else {

        $('#divRegularAnswer').show();
        $('#divMatchingAnswers').hide();
    }

    $.ajax({
        url: '../GetAnswerById',
        type: 'GET',
        data: { answerId: answerId, questionId: questionId, quizId: quizId },
        success: function (response) {
            if (response.success) {
                // Populate the answer modal with the retrieved data
                $('#answerId').val(response.data.id);
                $('#answerText').val(response.data.answerText);
                $('#isCorrect').prop('checked', response.data.isCorrect);

                $('#answerModalLabel').text("Edit Answer");

                // check question type
                var q = questionsArr.find(x => x.id == questionId);
                if (q.questionType == QuestionTypes.ShortAnswer) {
                    // hide checkbox
                    $('#isCorrectContainer').hide();
                }
                else if (q.questionType == QuestionTypes.Matching) {
                    $('#leftSide').val(response.data.leftSide);
                    $('#rightSide').val(response.data.rightSide);
                }
                // Show the answer modal
                $('#answerModal').modal('show');
            } else {
                // Handle the error case
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Failed to fetch answer.';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage
            });
        }
    });
}

function saveAnswer() {

    var answerId = $('#answerId').val();

    if (answerId) {
        updateAnswer();
    }
    else {
        createAnswer();
    }

   
}

function createAnswer() {
    // Retrieve the questionId from the hidden field
    var questionId = $('#answerModal input[name="questionId"]').val();

    let data = {
        answer: {
            answerText: $('#answerText').val(),
            isCorrect: $('#isCorrect').prop('checked')
         }
    };

    var questionById = questionsArr.find(x => x.id == questionId);
    if (questionById.questionType == QuestionTypes.Matching) {
        // use a different input fields
        data = {
            matchingAnswer : {
                leftSide: $('#leftSide').val(),
                rightSide: $('#rightSide').val()
            }
        }
    }

    // AJAX call to the AddAnswer endpoint
    $.ajax({
        url: `../AddAnswer?quizId=${quizId}&questionId=${questionId}`,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {


            if (response.success) {
                swal.fire({
                    title: "Answer added successfully!",
                    icon: "success",
                    timer: 3000, // Duration in milliseconds (3 seconds)
                    buttons: false, // Hide the "OK" button
                }).then(function () {
                    // Refresh the question list or perform any other actions
                    // Clear the modal inputs
                    $('#answerText').val('');
                    $('#isCorrect').prop('checked', false);

                    // Refresh the questions list
                    loadQuestions(quizId);

                    // Close the modal
                    $('#answerModal').modal('hide');
                });

            }
            else {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: response.message
                });
            }
        },
        error: function () {
            console.log('Failed to add the answer. Please try again.');
        }
    });
}

function updateAnswer() {
    // Get the answer text and isCorrect value from the modal inputs
    const answerText = $('#answerText').val();
    const isCorrect = $('#isCorrect').prop('checked');
    const answerId = $('#answerId').val();
    const questionId = $('#answerModal input[name="questionId"]').val();
    // Create the data object to be sent in the AJAX request
    let data = {
        id: answerId,
    };

    data.answer = {
        answerText: answerText,
        isCorrect: isCorrect
    };

    var questionById = questionsArr.find(x => x.id == questionId);
    if (questionById.questionType == QuestionTypes.Matching) {
        // use a different input fields
        data.matchingAnswer = {
            leftSide: $('#leftSide').val(),
            rightSide: $('#rightSide').val()
        }
    }

    // Send the AJAX request
    $.ajax({
        url: `../UpdateAnswer?quizId=${quizId}&questionId=${questionId}`,
        type: 'POST',
        data: JSON.stringify(data),
        contentType: 'application/json',
        success: function (response) {
            // Handle the success response
            if (response.success) {

                $('#answerId').val('');
                $('#answerText').val('');
                $('#isCorrect').prop('checked', false);


                // Close the modal
                $('#answerModal').modal('hide');
                loadAnswers(questionId);
                // Display success message or perform any other actions
                swal.fire({
                    title: 'Success',
                    text: 'Answer updated successfully!',
                    icon: 'success',
                    timer: 3000, // Autohide after 3 seconds
                    showConfirmButton: true
                });
            } else {
                // Display error message
                swal.fire({
                    title: 'Error',
                    text: response.message,
                    icon: 'error',
                    timer: 3000, // Autohide after 3 seconds
                    showConfirmButton: false
                });
            }
        },
        error: function (xhr) {
            // Handle the error response
            swal.fire({
                title: 'Error',
                text: xhr.responseText,
                icon: 'error',
                timer: 3000, // Autohide after 3 seconds
                showConfirmButton: false
            });
        }
    });
}

function deleteAnswer(questionId, answerId) {
    swal.fire({
        title: "Delete Answer",
        text: "Are you sure you want to delete this answer?",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: "Delete",
        cancelButtonText: "Cancel",
    }).then((result) => {
        if (result.isConfirmed) {
            // User confirmed, proceed with deletion
            $.ajax({
                url: "../DeleteAnswer",
                method: "DELETE",
                data: {
                    answerId: answerId,
                    questionId: questionId,
                    quizId: quizId,
                },
                success: function (response) {
                    if (response.success) {
                        // Answer deleted successfully
                        swal.fire({
                            title: "Success",
                            text: "Answer deleted successfully",
                            icon: "success",
                            timer: 3000, // Autohide after 3 seconds
                            showConfirmButton: false,
                        });

                        // Refresh the answer list
                        loadAnswers(questionId);
                    } else {
                        // Failed to delete answer
                        swal.fire({
                            title: "Error",
                            text: response.message,
                            icon: "error",
                        });
                    }
                },
                error: function () {
                    // Error occurred during deletion
                    swal.fire({
                        title: "Error",
                        text: "Failed to delete the answer.",
                        icon: "error",
                    });
                },
            });
        }
    });
}

function loadAnswers(questionId) {
    const answerDiv = document.getElementById(`answer_div_${questionId}`);

    // Clear existing answers
    answerDiv.innerHTML = '';
    var questionById = questionsArr.find(x => x.id == questionId);
    // Call the endpoint to retrieve answers
    $.ajax({
        url: `../GetAnswersByQuestion`,
        type: 'GET',
        data: {
            questionId: questionId,
            quizId: quizId,
        },
        success: function (response) {
          
            if (response.success) {
                const answers = response.data;

                let answerStr = '';

                let correctAnswerAvailable = false;

             
                $.each(answers, (i, v) => {

                    let answerButtons = ` <div>
                                                                        <button class="btn btn-sm btn-secondary" onclick="editAnswer(${questionId}, ${v.id})"><i class="fa fa-pencil"></i></button>
                                                                        <button class="btn btn-sm btn-danger" onclick="deleteAnswer(${questionId}, ${v.id})"><i class="fa fa-trash"></i> </button>               
                                                        </div>`;
                    if (questionById.questionType == QuestionTypes.Matching) {
                        correctAnswerAvailable = true;
                        answerStr += `
                             <li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <!-- Split again in 2 divs one for left side and one for right -->
                                    <div class="row" style="width:80%;margin-left:5px;" >
                                        <div class="col-5 left-answer">
                                            ${v.leftSide}
                                        </div>
                                        <div class="col-1 right-arrow">
                                         <i class="fa fa-long-arrow-right fa-2x"></i>
                                        </div>
                                        <div class="col-5 right-answer">
                                            ${v.rightSide}
                                        </div>
                                    </div>

                                     ${!isApproved ? answerButtons : ''}
                                </div>
                                
                            </li>`;

                    }
                    else {
                        answerStr += `<li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <div>${v.answerText}
                                ${v.isCorrect ? `<span class="badge bg-success">Correct</span>` : ''}</div>
                                    ${!isApproved ? answerButtons : ''}  
                                </div>
                                
                            </li>`;
                    }
                   
                    if (v.isCorrect) {
                        correctAnswerAvailable = true;
                    }
                });

                $(answerDiv).html(answerStr);

                if (correctAnswerAvailable) {
                    // remove the error label
                    // Check if the parent has a div with class "error-text"
                    if ($(answerDiv).parent().find(".error-text").length > 0) {
                        // Remove the div with class "error-text"
                        $(answerDiv).parent().find(".error-text").remove();
                    }
                }
                else {
                    if ($(answerDiv).parent().find(".error-text").length == 0) {
                        // the error
                        let noCorrectAnswerError = `<span class="text-danger error-text"><i class="fa fa-exclamation-triangle"></i> No correct answer available</span>`;

                        $(answerDiv).parent().find("div.card-subtitle").first().after(noCorrectAnswerError);
                    }
                }

            } else {
                // Handle error
                console.log('Failed to load answers.');
            }
        },
        error: function (xhr, status, error) {
            console.log(xhr);
            var errorMessage = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : 'Failed to load the answers.';
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: errorMessage
            });
        }
    });
}



