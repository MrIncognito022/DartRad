

class Question {
    constructor(id, questionType, questionText, answerExplanation) {
        this.Id = id;
        this.QuestionType = questionType;
        this.QuestionText = questionText;
        this.AnswerMatching = [];
        this.AnswerMultipleChoices = [];
        this.AnswerShortAnswer = [];
        this.AnswerSequence = [];
        this.AnswerExplanation = answerExplanation;
    }

    Id;
    QuestionText;
    QuestionType;
    AnswerMultipleChoices;
    AnswerShortAnswer;
    AnswerMatching;
    AnswerHotspot;
    AnswerSequence;
    AnswerExplanation = "";
    HotspotQuestionImage = "";

    getHTML = async function () {
        console.log($.IsApproved);
        let errorText = 'No Correct Answer Exists';
        let noCorrectAnswerError = `<span class="text-danger error-text"><i class="fa fa-exclamation-triangle"></i> ${errorText}</span>`;
        let addAnswerButton = `<button title="Add New Answer" class="btn btn-sm btn-primary" onclick="addAnswer(${this.Id})"><i class="fa fa-plus"></i> Add </button>`;

        let updateSequenceButton = ``;

        let questionButtons = `  <button title="Edit" onclick="editQuestion(${this.Id})" class="btn btn-sm btn-secondary"><i class="fa fa-pencil"></i></button>
                                    <button title="Delete" onclick="deleteQuestion(${this.Id})" class="btn btn-sm btn-danger"><i class="fa fa-trash"></i></a>
                              `;
        let hasAnswers = true;
        let isCorrectAnsAvailable = false;
        let answerHtml = '';
        let dragContainer = "";
        let dragInfoText = ``;

        if (this.QuestionType == QuestionTypes.MultipleChoice) {

            hasAnswers = this.AnswerMultipleChoices.length > 0;
            isCorrectAnsAvailable = this.AnswerMultipleChoices.filter(x => x.IsCorrect == true).length > 0;

            answerHtml = this.AnswerMultipleChoices.reduce(function (prev, curr) {
                return prev + curr.getHTML();
            }, "");

        }
        else if (this.QuestionType == QuestionTypes.ShortAnswer) {
            hasAnswers = this.AnswerShortAnswer.length > 0;
            isCorrectAnsAvailable = this.AnswerShortAnswer.filter(x => x.IsCorrect == true).length > 0;

            answerHtml = this.AnswerShortAnswer.reduce(function (prev, curr) {
                return prev + curr.getHTML();
            }, "");
        }
        else if (this.QuestionType == QuestionTypes.Matching) {
            hasAnswers = this.AnswerMatching.length > 0;
            // all are correct
            isCorrectAnsAvailable = true;

            answerHtml = this.AnswerMatching.reduce(function (prev, curr) {
                return prev + curr.getHTML();
            }, '');
        }
        else if (this.QuestionType == QuestionTypes.Hotspot) {

            hasAnswers = this.AnswerHotspot.length > 0;
            // all are correct
            isCorrectAnsAvailable = true;

            var answerPromises = this.AnswerHotspot.map((curr) => {
                return curr.getHTML(this.HotspotQuestionImage);
            });

            await Promise.all(answerPromises)
                .then((answers) => {
                    answerHtml = answers.join('');
                })
                .catch((error) => {
                    // Handle any errors that occurred
                    console.error(error);
                });

        }
        else if (this.QuestionType == QuestionTypes.Sequence) {

            hasAnswers = this.AnswerSequence.length > 0;
            isCorrectAnsAvailable = hasAnswers; // all answers are correct

            answerHtml = this.AnswerSequence.reduce(function (prev, curr) {
                return prev + curr.getHTML();
            }, "");

            dragContainer = "drag-container";
            dragInfoText = `<span class="text-sm-start text-muted"> Drag to Rearrange the order</span>`;
            updateSequenceButton = `<button title="Update Sequence" class=" btn btn-sm btn-secondary" onclick="updateSequence(${this.Id})"><i class="fa fa-arrows-v"></i> Update Sequence </button>`;
        }


        let hotspotImageButton = "";
        let hotspotImageDiv = "";
        if (this.QuestionType == QuestionTypes.Hotspot) {
            hotspotImageButton = ` <button class="btn btn-sm btn-primary mb-2" type="button" data-bs-toggle="collapse" data-bs-target="#hotspotImageDiv_${this.Id}" aria-expanded="false" aria-controls="collapseExample">
                                 <i class="fa fa-picture-o"></i> View Hotspot Image
                            </button>`;

            hotspotImageDiv = `<div class="collapse " id="hotspotImageDiv_${this.Id}">
                              <div class="card card-body d-flex flex-row justify-content-center ">
                                <div class="hotspot-image-container"> <img src="${this.HotspotQuestionImage}" /></div>
                                 
                              </div>
                            </div>`;
        }

        let answerListContainer = `<ol class="answer-list" id="answer_div_${this.Id}">
                                ${answerHtml}
                            </ol>`;

        if (this.QuestionType == QuestionTypes.Sequence) {
            answerListContainer = `
                            <div class="answer-list ${dragContainer}" style="margin-left: 15px;" id="answer_div_${this.Id}">
                                ${answerHtml}
                            </div>`;
        }


        let body = `<div class="card-body pb-2" >
                            <div class="mx-3">
                            <button class="btn btn-sm btn-primary mb-2" type="button" data-bs-toggle="collapse" data-bs-target="#explanationDiv_${this.Id}" aria-expanded="false" aria-controls="collapseExample">
                                 <i class="fa fa-eye"></i> View Answer Explanation
                            </button>
                           ${hotspotImageButton}
                            <div class="collapse " id="explanationDiv_${this.Id}">
                              <div class="card card-body">
                                  <p class="mb-0">${this.AnswerExplanation == null ? '<span class="text-danger">No Explanation</span>' : this.AnswerExplanation.replace(/\r\n/g, "<br>") }</p>
                              </div>
                            </div>
                            ${hotspotImageDiv}
                            </div>
                           
                            <div class="d-flex justify-content-between mt-2 mb-2 mx-3">
                                <div>
                                    <div class="fw-bold fs-5 card-subtitle mb-2 mr-1" style="display:inline"> Answers </div>
                                    ${dragInfoText}
                                    ${(hasAnswers && isCorrectAnsAvailable) ? '' : noCorrectAnswerError}
                                    </div>

                                ${!$.IsApproved ? `<div>${updateSequenceButton}  ${addAnswerButton}</div>` : ''}
                            </div>
                            ${answerListContainer}
                        </div>`;

        return `
                    <div class="card mt-2" id="questionCard_${this.Id}">
                        <div class="card-header">
                            <div class="d-flex flex-row justify-content-between">
                                <div>
                                    <h5 class="card-text mb-0">${this.QuestionText}</h5>
                                    <small class="card-subtitle mb-2 text-muted">${this.QuestionType}</small>
                                </div>
                                <div>
                                    ${!$.IsApproved ? questionButtons : ''}
                                </div>
                            </div>
                        </div>
                        ${body}
                    </div>`;
    }
}

class Answer {
    constructor(questionId, id) {
        this.QuestionId = questionId;
        this.Id = id;
    }

    getAnswerButtons = function () {
        return `<div>
                     <button class="btn btn-sm btn-secondary" onclick="editAnswer(${this.QuestionId}, ${this.Id})"><i class="fa fa-pencil"></i></button>
                     <button class="btn btn-sm btn-danger" onclick="deleteAnswer(${this.QuestionId}, ${this.Id})"><i class="fa fa-trash"></i> </button>               
                </div>`;
         
    }
}

class AnswerMultipleChoice extends Answer {
    constructor(questionId, id, answerText, isCorrect) {
        super(questionId);
        this.Id = id;
        this.AnswerText = answerText;
        this.IsCorrect = isCorrect;
    }

    getHTML = function () {
        return `<li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <div>${this.AnswerText}
                                ${this.IsCorrect ? `<span class="badge bg-success">Correct</span>` : ''}</div>

                                ${!$.IsApproved ? this.getAnswerButtons() : ''}
                                </div>
                            </li>`;
    }

}

class AnswerShortAnswer extends Answer {
    constructor(questionId, id, answerText, isCorrect) {
        super(questionId, id);
        this.AnswerText = answerText;
        this.IsCorrect = isCorrect;
    }

    getHTML = function () {
        return            `<li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <div>${this.AnswerText}
                                ${this.IsCorrect ? `<span class="badge bg-success">Correct</span>` : ''}</div>

                                ${!$.IsApproved ? this.getAnswerButtons() : ''}
                                </div>
                            </li>`; 
    }

}

class AnswerMatching extends Answer {
    constructor(questionId, id, leftSide, rightSide) {
        super(questionId, id);
        this.LeftSide = leftSide;
        this.RightSide = rightSide;
    }

    getHTML = function () {
       
        return ` <li class="card-text ">
                                <div class="d-flex flex-row justify-content-between">
                                       <!-- Split again in 2 divs one for left side and one for right -->
                                    <div class="row" style="width:80%;margin-left:5px;" >
                                        <div class="col-5 left-answer">
                                            ${this.LeftSide}
                                        </div>
                                        <div class="col-1 right-arrow">
                                         <i class="fa fa-long-arrow-right fa-2x"></i>
                                        </div>
                                        <div class="col-5 right-answer">
                                            ${this.RightSide}
                                        </div>
                                    </div>

                                     ${!$.IsApproved ? this.getAnswerButtons() : ''}
                                </div>
                                
                            </li>`;
    }
}

class AnswerHotspot extends Answer {
    constructor(questionId, id, x, y, width, height) {
        super(questionId, id);
        this.X = x;
        this.Y = y;
        this.Width = width;
        this.Height = height;
    }

    getHTML = function (imgSrc) {

        const loadImage = src =>
            new Promise((resolve, reject) => {
                const img = new Image();
                img.onload = () => resolve(img);
                img.onerror = reject;
                img.src = src;
            })
            ;

        var x = loadImage(imgSrc).then(originalImage => {

            if (originalImage.width > 1100) {
                originalImage.width = 1100;
            }

            var originalWidth = originalImage.width;
            var scaleFactor = originalImage.naturalWidth / originalWidth;

            var cropX = this.X; // x coordinate of the top-left corner of the crop area
            var cropY = this.Y; // y coordinate of the top-left corner of the crop area
            var cropWidth = this.Width; // width of the crop area
            var cropHeight = this.Height; // height of the crop area

            // Calculate the scaled crop coordinates and dimensions
            var scaledX = cropX * scaleFactor;
            var scaledY = cropY * scaleFactor;
            var scaledWidth = cropWidth * scaleFactor;
            var scaledHeight = cropHeight * scaleFactor;

            var canvas = document.createElement("canvas");
            canvas.width = cropWidth;
            canvas.height = cropHeight;
            var context = canvas.getContext('2d');



            context.drawImage(originalImage, scaledX, scaledY, scaledWidth, scaledHeight, 0, 0, cropWidth, cropHeight);

            return `<li class="card-text" id="answer_${this.Id}">
                            <div class="d-flex flex-row justify-content-between">
                                 <img src="${canvas.toDataURL()}" alt="Cropped Image">
                                    ${!$.IsApproved ? this.getAnswerButtons() : ''}
                            </div>

                        </li>`;
         
        });
        return x;

        
    }
}

// this will contain a button to update the order
class AnswerSequence extends Answer {
    constructor(questionId, id, answerText, order) {
        super(questionId, id);
        this.AnswerText = answerText;
        this.Order = order;
    }

    getHTML = function () {
        return  `<div class="card-text draggable" data-answerId="${this.Id}" style="cursor:pointer">
                                <div class="d-flex px-2 flex-row justify-content-between ">
                                       <div><span style="font-size:15px" class="text-muted">${this.Order}.</span> ${this.AnswerText}
                                ${this.IsCorrect ? `<span class="badge bg-success">Correct</span>` : ''}</div>

                                ${!$.IsApproved ? this.getAnswerButtons() : ''}
                                </div>
                            </div>`;
    }
}

const QuestionTypes = {
    MultipleChoice: "Multiple Choice",
    TrueFalse: "True False",
    ShortAnswer: "Short Answer",
    Matching: "Matching",
    Hotspot: "Hotspot",
    Sequence: "Sequence"
}

export { Question, AnswerMultipleChoice, AnswerShortAnswer, AnswerMatching, AnswerHotspot, AnswerSequence, QuestionTypes };