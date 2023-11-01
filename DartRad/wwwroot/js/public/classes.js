class Quiz {
    constructor(id, title, clinicalScenario) {
        this.id = id;
        this.title = title;
        this.clinicalScenario = clinicalScenario;
    }
}

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
        this.RemainingAnswerTries = 2;
        this.IsCorrectlyAnswered = false;
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
    ExplanationShown = false;

    // Answer
    RemainingAnswerTries;
    IsCorrectlyAnswered;

    getHtml = () => {
        let answerHtml = '';
        if (this.QuestionType == QuestionTypes.MultipleChoice) {

            $.each(this.AnswerMultipleChoices, (i, v) => {
                answerHtml += `
                <div class="answer-container form-check">
                    <input type="checkbox" class="form-check-input" id="ans_${i}" name="mcqa" value="${v.Id}" />
                    <label class="form-check-label" for="ans_${i}">${v.AnswerText}</label>
               </div>
                `;
            });

        }
        else if (this.QuestionType == QuestionTypes.ShortAnswer) {

            answerHtml += `
               <label>Your Answer:</label>
               <input type="text" class="form-control text-input" ></input>
            `;
        }
        else if (this.QuestionType == QuestionTypes.Matching) {
            let leftItems = '';
            let rightItems = '';

            let rightItemsArr = this.AnswerMatching.map(x => x.RightSide);
            rightItemsArr = shuffle(rightItemsArr);

            $.each(this.AnswerMatching, (i, v) => {
                leftItems += `<div class="draggable" id="ans_${v.Id}">
                           <p class="mb-0">${v.LeftSide}</p>
                        </div>`;
                rightItems += `
                              <div class="draggable">
                           <p class="mb-0">${rightItemsArr[i]}</p>
                        </div>`;
            });

            answerHtml = ` 
                <p class="text-sm text-secondary">Re-arrange the items on the right to match the left side</p>
                <div class="d-flex flex-row justify-content-between">
                    <div id="leftContainer" class="w-48">
                        ${leftItems}
                    </div>
                    <div id="rightContainer" class="w-48">
                         ${rightItems}
                    </div>
                </div>`;

        }
        else if (this.QuestionType == QuestionTypes.Hotspot) {
            answerHtml =
                `<p class="text-center"><i class="fa fa-search-plus"></i> Click the image to enlarge</p>
                <div id="imgPreviewDiv" style="background-image:url('${this.HotspotQuestionImage}')">
                </div>
                <input  type="hidden" id="clickedArea"/>`;

        }
        else if (this.QuestionType == QuestionTypes.Sequence) {
            var answerItems = '';
            var tempList = this.AnswerSequence.map(x => ({ Id: x.Id, AnswerText: x.AnswerText }));
            var shuffledList = shuffle(tempList);

            $.each(shuffledList, (i, v) => {
                answerItems += `<div class="draggable-single ps-3" id="ans_${v.Id}">
                                   <p class="mb-0">${v.AnswerText}</p>
                                </div>`;
            });
            answerHtml = ` 
                <p class="text-sm text-secondary">Re-arrange the items to correct sequence.</p>
               <div id="drag-container" class="w-100">
                        ${answerItems}
               </div>`;

        }
        return answerHtml;
    }

    verifyMultipleChoiceAnswer = (answerIds) => {
        this.RemainingAnswerTries--;

        var isIncorrect = false;
        let correctAnswers = this.AnswerMultipleChoices.filter(x => x.IsCorrect);
        if (answerIds.length == correctAnswers.length) {
            $.each(correctAnswers, (i, v) => {
                if (!answerIds.includes(v.Id)) {
                    isIncorrect = true;
                    return false;
                }
            });
            this.IsCorrectlyAnswered = !isIncorrect;
            return this.IsCorrectlyAnswered;
        }
        return false;
    }

    verifyShortAnswer = (answerText) => {
        this.RemainingAnswerTries--;
        let correctAnswer = this.AnswerShortAnswer.find(x => x.AnswerText.trim().toLowerCase() == answerText.trim().toLowerCase());
        this.IsCorrectlyAnswered = (correctAnswer != null); 
        return correctAnswer != null;
    }

    verifyMatchingAnswer = (rightSideAnswers) => {
        this.RemainingAnswerTries--;
        let isCorrect = true;
        $.each(this.AnswerMatching, (i, v) => {
            if (v.RightSide != rightSideAnswers[i]) {
                isCorrect = false;
                return;        
            }
        });
        this.IsCorrectlyAnswered = isCorrect;
        return isCorrect;
    }

    verifyHotspotAnswer = (x, y) => {
        this.RemainingAnswerTries--;

        var isIdentified = false;
        $.each(this.AnswerHotspot, (i, v) => {

            let minX = v.X;
            let minY = v.Y;
            let maxX = v.X + v.Width;
            let maxY = v.Y + v.Height;

            if ((x >= minX && x <= maxX) && (y >= minY && y <= maxY)) {
                isIdentified = true;
                return;
            }
        });
        this.IsCorrectlyAnswered = isIdentified;
        return isIdentified;
    }

    verifyAnswerSequence = (answerIds) => {
        this.RemainingAnswerTries--;
        var isIncorrectMatch = false;
       
        for (var i = 0; i < this.AnswerSequence.length; i++) {
            if (this.AnswerSequence[i].Id != answerIds[i]) {
                isIncorrectMatch = true;
                break;
            }
        }
        this.IsCorrectlyAnswered = !isIncorrectMatch;
        return this.IsCorrectlyAnswered;
    }
}

class Answer {
    constructor(questionId, id) {
        this.QuestionId = questionId;
        this.Id = id;
    }
}

class AnswerMultipleChoice extends Answer {
    constructor(questionId, id, answerText, isCorrect) {
        super(questionId);
        this.Id = id;
        this.AnswerText = answerText;
        this.IsCorrect = isCorrect;
    }
}

class AnswerShortAnswer extends Answer {
    constructor(questionId, id, answerText, isCorrect) {
        super(questionId, id);
        this.AnswerText = answerText;
        this.IsCorrect = isCorrect;
    }
}

class AnswerMatching extends Answer {
    constructor(questionId, id, leftSide, rightSide) {
        super(questionId, id);
        this.LeftSide = leftSide;
        this.RightSide = rightSide;
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
}

class AnswerSequence extends Answer {
    constructor(questionId, id, answerText, order) {
        super(questionId, id);
        this.AnswerText = answerText;
        this.Order = order;
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

export { Quiz, Question, AnswerMultipleChoice, AnswerShortAnswer, AnswerMatching, AnswerHotspot, AnswerSequence, QuestionTypes };

function shuffle(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
    return array;
}