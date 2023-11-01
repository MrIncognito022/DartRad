
import { Quiz, Question, AnswerMultipleChoice, AnswerShortAnswer, AnswerHotspot, AnswerMatching, AnswerSequence, QuestionTypes } from './classes.js';

const app = {
    quiz: null,
    questionsArr: [],
    currentQuestionIndex: 0,
    totalQuestions: 0,
    init: async function () {
        $('#btnNext').hide();
        $('#resultsDiv').hide();

        await this.getQuiz();

        if (this.quiz != null && this.questionsArr.length > 0) {
            this.currentQuestionIndex = 0;
            this.totalQuestions = this.questionsArr.length;
            this.displayQuizInfo();
            $('#btnNext').show();
        }
    },
    getQuiz: async function () {
        let quizId = $('#quizId').val();
        await $.ajax({
            url: "/quiz/GetQuestions",
            type: "GET",
            data: {
                id: quizId
            },
            contentType: "application/json",
            success: (response) => {
                if (response.success) {
                    let d = response.data;
                    let questions = d.questions;
                    app.quiz = new Quiz(d.id, d.title, d.clinicalScenario);
                    //console.log(app.quiz);

                    this.questionsArr = questions.filter(x => x.questionType != QuestionTypes.TrueFalse).map(function (v) {
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
                            question.AnswerHotspot = v.answerHotspot.map(x => new AnswerHotspot(v.id, x.id, x.x, x.y, x.width, x.height))
                        }
                        else if (v.questionType == QuestionTypes.Sequence) {
                            question.AnswerSequence = v.answerSequence.map(x => new AnswerSequence(v.id, x.id, x.answerText, x.order));
                        }
                        return question;
                    });

                    console.log(this.questionsArr);
                }
            },
            error: function (xhr, status, error) {
                console.log(xhr);
            }
        })
    },
    displayQuizInfo: function () {
        console.log(this.quiz);
        $('#quizTitle').text(this.quiz.title);
        $('#quizDescription').text(this.quiz.clinicalScenario);
        $('#totalQuestionsCount').text(this.totalQuestions);

        if (this.questionsArr.length > 0) {
            this.displayQuestion(this.currentQuestionIndex);
        }
    },
    displayQuestion: function (index) {
        $('#currentQuestionNumber').text(index + 1);

        if (index > this.questionsArr.length - 1) {
            return;
        }

        let questionByIndex = this.questionsArr[index];
        // console.log(questionByIndex);
        // display the question
        $('.question-text').text(questionByIndex.QuestionText);

        let answersHtml = questionByIndex.getHtml();
        $('#answerDiv').html(answersHtml);

        if (questionByIndex.QuestionType == QuestionTypes.Matching) {
            initDraggable();
        }

        if (questionByIndex.QuestionType == QuestionTypes.Sequence) {
            initDraggableSingle();
        }

    },
    next: function () {
        // display the explanation
        let currentQuestion = this.questionsArr[this.currentQuestionIndex];
        if (!currentQuestion.ExplanationShown) {
            if (currentQuestion.AnswerExplanation != null) {
                if (currentQuestion.AnswerExplanation != '') {
                    Swal.fire({
                        icon: 'info',
                        title: 'Explanation',
                        html: currentQuestion.AnswerExplanation.replace(/\r\n/g, "<br>"),
                    }).then(() => {
                        this.moveToNextQuestion();
                    });
                }
               
            }
            else {
                this.moveToNextQuestion();
            }
           
        }
        else {
            this.moveToNextQuestion();
        }
    },
    moveToNextQuestion: function () {
        if (this.currentQuestionIndex == app.totalQuestions - 1) {
            this.submit();
            return;
        }

        this.currentQuestionIndex++;
        this.displayQuestion(this.currentQuestionIndex);
    },
    submit: function () {
        // clear the area

        $('#quizBody').hide();
        $('#resultsDiv').show();

        let correctAnswersCount = this.questionsArr.filter(x => x.IsCorrectlyAnswered).length;
        let totalQuestions = this.questionsArr.length;

        
        var answerPercentage = (correctAnswersCount / totalQuestions) * 100;
        $('#resultStr').text(`You have scored ${answerPercentage}%`);
    }
}

$(document).ready(() => {
    app.init();
});

$(document).on("click", ".answer-container", (e) => {
    var radioButton = $(event.target).find(":checkbox");
    if (radioButton.length > 0) {
        // Check the radio button
        var isChecked = radioButton.prop('checked');
        radioButton.prop("checked", !isChecked);
        $(event.target).toggleClass('answer-container-active');

    }
})

$('#btnNext').click(() => {

    let currentIndex = app.currentQuestionIndex;
    var question = app.questionsArr[currentIndex];
    let allowNext = false;

    if (question.QuestionType == QuestionTypes.MultipleChoice) {
        var selectedAnswers = $('input[type="checkbox"]:checked').map(function () {
            return parseInt( $(this).val());
        }).get();


        if (selectedAnswers.length == 0) {
            Swal.fire({
                icon: 'warning',
                title: 'No Selection',
                text: 'Please select an answer.',
            });
            return;
        }
        else {
            // verify answer
            if (question.verifyMultipleChoiceAnswer(selectedAnswers)) {
                // allow next
                allowNext = true;
            }
        }
    }
    else if (question.QuestionType == QuestionTypes.ShortAnswer) {
        let shortAnswer = $('.text-input').val();
        if (!shortAnswer) {
            Swal.fire({
                icon: 'warning',
                title: 'No Answer',
                text: 'Please write the answer.',
            });
            return;
        }
        else {
            if (question.verifyShortAnswer(shortAnswer)) {
                // allow next
                allowNext = true;
            }
        }
    }
    else if (question.QuestionType == QuestionTypes.Matching) {
        // take the left side list
        let answers = [];
        //$('#leftContainer').find('div.draggable').each((i, v) => {
        //    answers.push({
        //        answerId: $(v).attr('id').replace('ans_', '')
        //    });
        //})

        // take the right side list
        $('#rightContainer').find('div > p').each((i, v) => {
            answers[i] = $(v).text()
        });

        if (question.verifyMatchingAnswer(answers)) {
            allowNext = true;
        }
    }
    else if (question.QuestionType == QuestionTypes.Hotspot) {
        let answerString = $('#clickedArea').val();

        if (answerString == '') {
            Swal.fire({
                icon: 'warning',
                title: 'No Selection',
                text: 'No Area Selected on image, Please Click the image to enlarge and click on the relevant area on the image',
            });
            return;
        }

        let answerObj = JSON.parse(answerString);
        if (question.verifyHotspotAnswer(answerObj.x, answerObj.y)) {
            allowNext = true;
        }
    }
    else if (question.QuestionType == QuestionTypes.Sequence) {
        var container = $('#drag-container');
        var answerIds = container.children().map((i, v) => $(v).attr('id').substring(4)).get();

        if (question.verifyAnswerSequence(answerIds)) {
            allowNext = true;
        }
    }

    if (allowNext == false) {
        if (question.RemainingAnswerTries > 0) {
            Swal.fire({
                icon: 'warning',
                title: 'Incorrect Answer',
                text: `You have ${question.RemainingAnswerTries} more chance to review your answer.`,
            }).then(x => {
                //if (question.RemainingAnswerTries == 1) {
                //    if (question.AnswerExplanation != null) {
                //        if (question.AnswerExplanation != '') {
                //            Swal.fire({
                //                icon: 'info',
                //                title: 'Explanation',
                //                text: question.AnswerExplanation,
                //            });
                //            question.ExplanationShown = true;
                //        }
                       
                //    }
                   
                //}
            });

            return;

        }

    }
   

    // next logic

    if (currentIndex + 1 == app.totalQuestions - 1) {
        $('#btnNext').text("Submit");
        $('#btnNext').addClass('btn-success');
        $('#btnNext').removeClass('btn-primary');
    }

    app.next();
   
})

$(document).on('click', '#imgPreviewDiv', () => {
    let imgSrc = app.questionsArr[app.currentQuestionIndex].HotspotQuestionImage;
    $('#hotspot-answer-selection-image').attr('src', imgSrc);
    $('#imgAreaSelectModal').modal('show');
});

$('#hotspot-answer-selection-image').click(function (event) {
    var image = document.getElementById('hotspot-answer-selection-image');
    const rect = image.getBoundingClientRect();
    const x = event.clientX - rect.left;
    const y = event.clientY - rect.top;

    // set the answer
    $('#clickedArea').val(JSON.stringify({
        x: x,
        y: y
    }));

    // close the modal
    $('#imgAreaSelectModal').modal('hide');

    // hit next
    $('#btnNext').trigger('click');
});

function initDraggable() {
    // create draggable

    var element = document.getElementById('rightContainer');

    var d = dragula([element]);
    d.on('drag', function (el) {
        el.classList.add('dragging');
    });

    d.on('dragend', function (el) {
        el.classList.remove('dragging');
    });

    var containers = $('#leftContainer, #rightContainer');

    // Find the maximum height among all the draggable elements
    var maxHeight = Math.max.apply(null, containers.find('.draggable').map(function () {
        return $(this).outerHeight();
    }).get());

    // Set the height of all container divs to the maximum height
    // containers.height(maxHeight);
    var leftChilds = $('#leftContainer').children();

    $.each(leftChilds, (i, v) => {

        $(v).height(maxHeight);
    });

    var rightChilds = $('#rightContainer').children();

    $.each(rightChilds, (i, v) => {

        $(v).height(maxHeight);
    });
}

// for sequence question
function initDraggableSingle() {
    var element = document.getElementById('drag-container');
    var d = dragula([element]);
    d.on('drag', function (el) {
        el.classList.add('dragging');
    });

    d.on('dragend', function (el) {
        el.classList.remove('dragging');
    });
}