import { Question, AnswerMultipleChoice, AnswerShortAnswer, AnswerHotspot,  AnswerMatching, AnswerSequence, QuestionTypes } from './classes.js';

let quizId = $('#h_quizId').val();
let isApproved = $('#h_quizApproved').val() == 'value';
$.IsApproved = isApproved;


$(document).ready(function () {
   
   
    $('#hotspotImageDiv').hide();
    // Call the loadQuestions function to fetch and display the questions on page load
    loadQuestions(quizId);

    // drawback of using modules; otherwise create a new js file [without module] and define these functions there.
    // but make sure to supply the required variables
    window.displayQuestions = displayQuestions;
    window.deleteQuestion = deleteQuestion;
    window.addQuestion = addQuestion;
    window.editQuestion = editQuestion;
    window.updateQuestion = updateQuestion;
    window.addAnswer = addAnswer;
    window.editAnswer = editAnswer;
    window.saveAnswer = saveAnswer;
    window.deleteAnswer = deleteAnswer;
    window.updateSequence = updateSequence;
});

var questionsArr = [];

// #region App Functions
async function loadQuestions(quizId) {
    $.ajax({
        url: '../GetQuestions',
        type: 'GET',
        data: { id: quizId },
        success: function (questions) {

            questionsArr = questions.data.filter(x => x.questionType != QuestionTypes.TrueFalse).map(function (v) {
                let question = new Question(v.id, v.questionType, v.questionText, v.answerExplanation);
               
                if (v.questionType == QuestionTypes.MultipleChoice) {
                    question.AnswerMultipleChoices = v.answerMultipleChoices.map(x => new AnswerMultipleChoice(v.id, x.id, x.answerText, x.isCorrect));
                }
                else if (v.questionType == QuestionTypes.ShortAnswer) {
                    question.AnswerShortAnswer = v.answerShortAnswer.map(x => new AnswerShortAnswer(v.id, x.id, x.answerText, x.isCorrect));
                }
                else if (v.questionType == QuestionTypes.Matching) {
                    question.AnswerMatching = v.answerMatching.map(x => new AnswerMatching(v.id, x.id, x.leftSide, x.rightSide));
                }
                else if (v.questionType == QuestionTypes.Hotspot) {
                    // set the image before setting the answers
                    question.HotspotQuestionImage = v.hotspotQuestionImage;
                    question.AnswerHotspot = v.answerHotspot.map(x => new AnswerHotspot(v.id, x.id, x.x, x.y, x.width, x.height));
                }
                else if (v.questionType == QuestionTypes.Sequence) {
                    question.AnswerSequence = v.answerSequence.map(x => new AnswerSequence(v.id, x.id, x.answerText, x.order));

                }
                return question;
            });

            $('#questionsContainer').empty();
             displayQuestions();

        },
        error: function () {
            console.log('Failed to load questions.');
        }
    });
}

async function displayQuestions() {
    
    let allQuestionsHtml = '';
    for (const question of questionsArr) {
        allQuestionsHtml += await question.getHTML();
    }

    $('#questionsContainer').html(allQuestionsHtml);
    initDraggable();
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

function addQuestion() {
    // Get the question data from the modal inputs
    var questionType = $('#questionType').val();
    var questionText = $('#questionText').val();
    var answerExplanation = $('#answerExplanation').val();
   
    // Create a new FormData object
    var formData = new FormData();

    // Append the question data to the FormData object
    formData.append('questionType', questionType);
    formData.append('questionText', questionText);
    formData.append('answerExplanation', answerExplanation);
    let selectedQuestionType = $('#questionType option:selected').text();
    if (selectedQuestionType == QuestionTypes.Hotspot) {
        if ($('#hotspotImageUpload')[0].files.length > 0) {
            // Get the selected image file from the file input
            var imageFile = $('#hotspotImageUpload')[0].files[0];

            // Append the image file to the FormData object
            formData.append('hotspotImage', imageFile);
        }
        else {
            // show error
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: "Please Upload Hotspot Image"
            });
            return;
        }
       
    }
   
    // Send the AJAX request to the server to create the question
    $.ajax({
        url: '../CreateQuestion?quizId=' + quizId,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
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
                    $('#answerExplanation').val('');
                    $('imagePreview').css('background-image', '');

                    // Refresh the questions list
                    loadQuestions(quizId);
                });
            } else {
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
                console.log(response);
                // Populate the modal fields with the existing question data
                $('#questionId').val(response.data.id);
                $('#questionText').val(response.data.questionText);
                $('#questionType').val(response.data.questionType);
                $('#questionType').trigger('change');
                $('#answerExplanation').val(response.data.answerExplanation);
                $('#hotspotImageName').val(response.data.existingImageName);

                
                // set the background image
                $('#imagePreview').css('background-image', `url("${response.data.existingImagePath}")`);

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
    var answerExplanation = $('#answerExplanation').val();

    var formData = new FormData();

    // Append the question data to the FormData object
    formData.append('id', questionId);
    formData.append('questionType', questionType);
    formData.append('questionText', questionText);
    formData.append('answerExplanation', answerExplanation);
    let selectedQuestionType = $('#questionType option:selected').text();
    if (selectedQuestionType == QuestionTypes.Hotspot) {
        if ($('#hotspotImageUpload')[0].files.length > 0) {
            // Get the selected image file from the file input
            var imageFile = $('#hotspotImageUpload')[0].files[0];
            // Append the image file to the FormData object
            formData.append('hotspotImage', imageFile);
        }
    }

    // Send the AJAX request to the server to create the question
    $.ajax({
        url: '../UpdateQuestion?quizId=' + quizId,
        type: 'POST',
        data: formData,
        contentType: false,
        processData: false,
        success: function (response) {
            if (response.success) {
                // Close the modal
                $('#questionModal').modal('hide');
                swal.fire({
                    title: "Question Updated successfully!",
                    icon: "success",
                    timer: 3000, // Duration in milliseconds (3 seconds)
                    buttons: false, // Hide the "OK" button
                }).then(function () {
                    // Refresh the question list or perform any other actions
                    // Clear the modal inputs
                    $('#questionType').val('0');
                    $('#questionText').val('');
                    $('#answerExplanation').val('');
                    $('imagePreview').css('background-image', '');

                    // Refresh the questions list
                    loadQuestions(quizId);
                });
            } else {
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

function addAnswer(questionId) {
    $('#divRegularAnswer').hide();
    $('#divMatchingAnswers').hide();
    $('#divHotspotAnswer').hide();
    $('#isCorrectContainer').show();

    resetAnswerInputModal()

    var questionById = questionsArr.find(x => x.Id == questionId);

    if (questionById.QuestionType == QuestionTypes.Matching) {
        // hide regular fields and diplay fields relevant to this type
        $('#divMatchingAnswers').show();
    }
    else if (questionById.QuestionType == QuestionTypes.Hotspot) {
        // load image from question into container and use JCrop
        $('#divHotspotAnswer').show();
        $('#hotspot-answer-image-preview').attr('src', questionById.HotspotQuestionImage);
        $('#hotspot-answer-selection-image').attr('src', questionById.HotspotQuestionImage);
    }
    else {
        $('#divRegularAnswer').show();
    }

    if (questionById.QuestionType == QuestionTypes.ShortAnswer || questionById.QuestionType == QuestionTypes.Sequence) {
        // hide checkbox
        $('#isCorrectContainer').hide();
    }

    // Clear the input fields
    $('#answerModal input[name="questionId"]').val(questionId);
    $('#answerText').val('');
    $('#leftSide').val('');
    $('#rightSide').val('');
    $('#isCorrect').prop('checked', false);

    // Display the addAnswerModal
  $('#answerModal').modal('show');

  $('#answerModalLabel').text("Add Answer");

   

}

function editAnswer(questionId, answerId) {
    $('#answerModal input[name="questionId"]').val(questionId);
    $('#isCorrectContainer').show();

    $('#divRegularAnswer').hide();
    $('#divMatchingAnswers').hide();
    $('#divHotspotAnswer').hide();
    resetAnswerInputModal()
    var questionById = questionsArr.find(x => x.Id == questionId);

    if (questionById.QuestionType == QuestionTypes.Matching) {
        // hide regular fields and diplay fields relevant to this type
        $('#divMatchingAnswers').show();
    }
    else if (questionById.QuestionType == QuestionTypes.Hotspot) {
        $('#divHotspotAnswer').show();
    }
    else {
        $('#divRegularAnswer').show();
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
                var q = questionsArr.find(x => x.Id == questionId);
                if (q.QuestionType == QuestionTypes.ShortAnswer || q.QuestionType == QuestionTypes.Sequence) {
                    // hide checkbox
                    $('#isCorrectContainer').hide();
                }
                else if (q.QuestionType == QuestionTypes.Matching) {
                    $('#leftSide').val(response.data.leftSide);
                    $('#rightSide').val(response.data.rightSide);
                }
                else if (q.QuestionType == QuestionTypes.Hotspot) {
                    // copy-paste the image onto canvas
                    $('#answerImageArea').val(JSON.stringify(response.data));
                    // get the image and set it to preview
                    let questionImageSrc = q.HotspotQuestionImage;
                    $('#hotspot-answer-image-preview').attr('src', questionImageSrc);

                    // get the answer image canvas from question list and copy-paste it onto current selection
                    var answerCroppedImage = $('#answer_' + response.data.id).find('img')[0];
                    // paste the cropped image onto the canvas
                    var canvas = document.getElementById('croppedImage');
                    var ctx = canvas.getContext('2d');
                    canvas.width = answerCroppedImage.width;
                    canvas.height = answerCroppedImage.height
                    ctx.drawImage(answerCroppedImage, 0, 0);

                    // load the question image onto the selection container
                    $('#hotspot-answer-selection-image').attr('src', questionImageSrc);
                    // initialize the area select with coordinates from db

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

    var questionById = questionsArr.find(x => x.Id == questionId);
    if (questionById.QuestionType == QuestionTypes.Matching) {
        // use a different input fields
        data = {
            matchingAnswer: {
                leftSide: $('#leftSide').val(),
                rightSide: $('#rightSide').val()
            }
        }
    }

    if (questionById.QuestionType == QuestionTypes.Hotspot) {
        var area = JSON.parse($('#answerImageArea').val());
        data = {
            hotspotAnswer: {
                x: parseInt(area.x),
                y: parseInt( area.y),
                width: area.width,
                height: area.height
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
                    
                    resetAnswerInputModal()
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

    var questionById = questionsArr.find(x => x.Id == questionId);
    if (questionById.QuestionType == QuestionTypes.Matching) {
        // use a different input fields
        data.matchingAnswer = {
            leftSide: $('#leftSide').val(),
            rightSide: $('#rightSide').val()
        }
    }
    else if (questionById.QuestionType == QuestionTypes.Hotspot) {
        let json = $('#answerImageArea').val();
        let parsed = JSON.parse(json);
        data.hotspotAnswer = {
            x: parsed.x,
            y: parsed.y,
            width: parsed.width,
            height: parsed.height
        };
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

                

                resetAnswerInputModal();
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
  
    var questionById = questionsArr.find(x => x.Id == questionId);
    // Call the endpoint to retrieve answers
    $.ajax({
        url: `../GetAnswersByQuestion`,
        type: 'GET',
        data: {
            questionId: questionId,
            quizId: quizId,
        },
        success: async function (response) {

            if (response.success) {
                if (questionById.QuestionType == QuestionTypes.MultipleChoice) {
                    questionById.AnswerMultipleChoices = response.data.map(x => new AnswerMultipleChoice(questionId, x.id, x.answerText, x.isCorrect));
                }
                else if (questionById.QuestionType == QuestionTypes.ShortAnswer) {
                    questionById.AnswerShortAnswer = response.data.map(x => new AnswerShortAnswer(questionId, x.id, x.answerText, x.isCorrect));
                }
                else if (questionById.QuestionType == QuestionTypes.Matching) {
                    questionById.AnswerMatching = response.data.map(x => new AnswerMatching(questionId, x.id, x.leftSide, x.rightSide));
                }
                else if (questionById.QuestionType == QuestionTypes.Hotspot) {
                    questionById.AnswerHotspot = response.data.map(x => new AnswerHotspot(questionId, x.id, x.x, x.y, x.width, x.height));
                }
                else if (questionById.QuestionType == QuestionTypes.Sequence) {
                    questionById.AnswerSequence = response.data.map(x => new AnswerSequence(questionId, x.id, x.answerText, x.order));
                }
                // get the questionHTML and simply update it
                $('#questionCard_' + questionId).replaceWith(await questionById.getHTML());
                initDraggable();

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

function resetAnswerInputModal() {
    $('#answerId').val('');
    $('#answerText').val('');
    $('#leftSide').val('');
    $('#rightSide').val('');
    $('#isCorrect').prop('checked', false);


    // for HOTSPOT
    // clear cropped value

    $('#answerImageArea').val('');
    // clear cropped image

    var canvas = document.getElementById('croppedImage');
    var ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
}
// #endregion

function updateSequence(questionId) {
    // get container
    var container = $('#answer_div_' + questionId);
    var answerIds = container.children().map((i, v) => $(v).attr('data-answerId')).get();
   
    swal.fire({
        title: 'Update Sequence Order',
        text: 'Are you sure you want to update the sequence of the answers ?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Update',
        cancelButtonText: 'Cancel'
    }).then((result) => {
        if (result.isConfirmed) {

            $.ajax({
                url: `../UpdateSequence?quizId=${quizId}&questionId=${questionId}`,
                type: "POST",
                data: JSON.stringify({
                    AnswerIds: answerIds.map(x => parseInt(x))
                }),
                contentType: 'application/json',
                success: function (res) {
                    if (res.success) {
                        swal.fire({
                            title: 'Success',
                            text: 'Sequence updated successfully!',
                            icon: 'success',
                            timer: 3000, // Autohide after 3 seconds
                            showConfirmButton: true
                        });

                        loadAnswers(questionId);
                    }
                    else {
                        swal.fire({
                            title: 'Error',
                            text: res.message,
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
            })
          
        }
    });
}
// #region Events
$('#btnCreateQuestion').click(function () {
    $('#questionId').val('');
    $('#questionType').val('0');
    $('#questionText').val('');
    $('#questionType').trigger('change');
    $('#imagePreview').css('background-image', '');
    $('#answerExplanation').val('');

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

$('#questionType').on('change', function () {
    let selectedQuestionType = $('#questionType option:selected').text();
    if (selectedQuestionType == QuestionTypes.Hotspot) {
        // reveal the image upload and resize control
        $('#hotspotImageDiv').show();
    }
    else {
         // hide the image upload and resize control
        $('#hotspotImageDiv').hide();
    }
});

$('#hotspotImageUpload').on('change', function (e) {
    // Get the selected file
    var file = e.target.files[0];

    // Create a FileReader object to read the file
    var reader = new FileReader();
    // Set up the FileReader onload event
    reader.onload = function (e) {
        // Set the image source to the data URL
        var image = new Image();
        image.src = e.target.result;
        image.onload = function () {
           
            if (image.width >= 200 && image.height >= 200) {
                var imageUrl = e.target.result;
                // Set the image as the background of the preview div
                $('#imagePreview').css('background-image', 'url(' + imageUrl + ')');
            }
            else {
                $('#hotspotImageUpload').val('');
                $('#imagePreview').css('background-image', '');
                swal.fire({
                    title: 'Error',
                    text: "Please select an image with a minimum width of 200px and a minimum height of 200px.",
                    icon: 'error'
                });
              
            }
        }
       
    };

    // Read the selected file as a data URL
    reader.readAsDataURL(file);
});

$('#hotspot-answer-image-preview').click(function () {

    $('#custom-image-area-select-modal').modal('show');
    $('#answerModal').modal('hide');

});

$('#btnImageSelectionFinalize').click(function () {
    // if there a selection was made then display it on prevous modal
   
    var areas = $('#hotspot-answer-selection-image').selectAreas('areas');
    //console.log(areas);
    if (areas.length == 0) {
        swal.fire({
            title: 'Error',
            text: "No area was selected, Please select an area ",
            icon: 'error'
        });
        return;
    }

    var originalImageEl = $('#hotspot-answer-selection-image');

    const originalImage = new Image(originalImageEl[0].width, originalImageEl[0].height);
    originalImage.src = originalImageEl.attr('src');

   // console.log(originalImage);
    var area = areas[0];
    $('#answerImageArea').val(JSON.stringify(area));
    // Crop coordinates and dimensions
    var cropX = area.x; // x coordinate of the top-left corner of the crop area
    var cropY = area.y; // y coordinate of the top-left corner of the crop area
    var cropWidth = area.width; // width of the crop area
    var cropHeight = area.height; // height of the crop area

    originalImage.addEventListener('load', function () {
       
        const canvas = document.getElementById('croppedImage');
        const ctx = canvas.getContext('2d');

        // Calculate the scale factor to map coordinates
        var scaleFactor = originalImage.naturalWidth / originalImage.width;
        // Calculate the scaled crop coordinates and dimensions
        var scaledX = cropX * scaleFactor;
        var scaledY = cropY * scaleFactor;
        var scaledWidth = cropWidth * scaleFactor;
        var scaledHeight = cropHeight * scaleFactor;

        canvas.width = cropWidth;
        canvas.height = cropHeight;

        ctx.drawImage(originalImage, scaledX, scaledY, scaledWidth, scaledHeight, 0, 0, cropWidth, cropHeight);
      
    });
  

    $('#custom-image-area-select-modal').modal('hide');
});

$('#custom-image-area-select-modal').on('hidden.bs.modal', function () {
    $('#hotspot-answer-selection-image').selectAreas('destroy');
    $('#answerModal').modal('show');
});
$('#custom-image-area-select-modal').on('shown.bs.modal', function () {
    $("#hotspot-answer-selection-image").selectAreas({
        minSize: [25, 25],    // Minimum size of a selection
        maxSize: [400, 400],  // Maximum size of a selection
        maxAreas: 1,
        allowDelete: true,
        onLoaded: function () {

        },
        
    });

    // check for existing coordinates
    var selectedAreaStr = $('#answerImageArea').val();
    if (selectedAreaStr != '') {
        var parsed = JSON.parse(selectedAreaStr);
        var areaOptions = {
            x: parsed.x,
            y: parsed.y,
            width: parsed.width,
            height: parsed.height,
        };
        $("#hotspot-answer-selection-image").selectAreas('add', areaOptions);
    }
});


// #endregion

// #region Extra Functions
var drake = null;
function initDraggable() {
    // create draggable
    if (drake != null) {
        drake.destroy();
    }
    var elementsArr = [];
    var elements = document.getElementsByClassName('drag-container');

    $.each(elements,(i, v) => {
        elementsArr.push(v);
    })
    drake = dragula(elementsArr, {
        accepts: function (el, target, source, sibling) {
            // Return true to allow dropping the cloned element only within its container
            return target === el.parentNode;
        }
    });

    drake.on('drag', function (el) {
        el.classList.add('dragging');
    });

    drake.on('dragend', function (el) {
        el.classList.remove('dragging');
    });

   
}
// #endregion