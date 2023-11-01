using AutoMapper;
using DartRad.Areas.ContentCreator.Models;
using DartRad.Configurations;
using DartRad.Data;
using DartRad.Entities;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DartRad.Areas.ContentCreator.Controllers
{
    [Area("ContentCreator")]
    [Authorize(Roles = AppRoles.ContentCreator)]
    [Route("[Area]/[Controller]")]
    public class QuizController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly QuizConfig _quizConfig;

        public QuizController(AppDbContext dbContext, IMapper mapper, IWebHostEnvironment hostEnvironment, IOptions<QuizConfig> options)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            this._hostEnvironment = hostEnvironment;
            _quizConfig = options.Value;
        }

        [HttpGet("List")]
        public async Task<IActionResult> Index(string filter)
        {
            var query = _dbContext.Quiz.Include(x => x.ApprovedByAdmin)
                .Where(x => x.ContentCreatorId == this.GetUserId());

            if (!string.IsNullOrEmpty(filter))
            {
                if (Enum.TryParse(filter, out QuizStatus status))
                {
                    if (status == QuizStatus.Approved)
                    {
                        query = query.Where(x => x.Status == QuizStatus.Approved);
                    }
                    else if(status == QuizStatus.Draft)
                    {
                        query = query.Where(x => x.Status == QuizStatus.Draft);
                    }
                    else if (status == QuizStatus.WaitingForApproval)
                    {
                        query = query.Where(x => x.Status == QuizStatus.WaitingForApproval);
                    }
                    else
                    {
                        query = query.Where(x => x.Status == QuizStatus.Rejected);
                    }
                }
            }

            var quizListFromDb = await query.ToListAsync();
            var convertedObject = _mapper.Map<List<QuizListViewModel>>(quizListFromDb);

            // prepare the filter list
            var statusList = from QuizStatus q in Enum.GetValues(typeof(QuizStatus))
                             select new { Value = (int)q, Text = q.ToString() };

            ViewBag.StatusList = new SelectList(statusList, "Value", "Text", filter);

            return View(convertedObject);
        }

        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            return View(new QuizCreateViewModel());
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create(QuizCreateViewModel quizCreateViewModel)
        {
            if (ModelState.IsValid)
            {
                var quiz = new Quiz
                {
                    Title = quizCreateViewModel.Title,
                    ClinicalScenario = quizCreateViewModel.ClinicalScenario,
                    ContentCreatorId = this.GetUserId(),
                    CreatedAt = DateTime.UtcNow,
                    Status = QuizStatus.Draft,
                    Category = quizCreateViewModel.Category,

                };

                if(quizCreateViewModel.QuizImage != null)
                {
                    if (!ValidationHelper.IsImageFile(quizCreateViewModel.QuizImage))
                    {
                        ModelState.AddModelError("QuizImage", "Uploaded file is not an image");
                        return View(quizCreateViewModel);
                    }
                    string savedFilePath = await FileUploadHelper.SaveFile(quizCreateViewModel.QuizImage, _hostEnvironment.WebRootPath, _quizConfig.QuizImagesFolder);
                    if (string.IsNullOrEmpty(savedFilePath))
                    {
                        ModelState.AddModelError("","An error occrured while saving the quiz image");
                        return View(quizCreateViewModel);
                    }

                    quiz.ImageUrl = savedFilePath;
                }

                _dbContext.Quiz.Add(quiz);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Quiz Created Successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(quizCreateViewModel);
        }

        [HttpGet("{id}/Edit")]
        public async Task<IActionResult> Edit(int id)
        {
            var quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == id && x.ContentCreatorId == this.GetUserId());
            if (quiz == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<QuizUpdateViewModel>(quiz);

            if (!string.IsNullOrEmpty(viewModel.ImageUrl))
            {
                viewModel.ImageUrl = _quizConfig.QuizImagesFolder + "/" + viewModel.ImageUrl;
            }

            return View(viewModel);
        }

        [HttpPost("{id}/Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, QuizUpdateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            var quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == id && x.ContentCreatorId == this.GetUserId());
            if (quiz == null)
            {
                return NotFound();
            }

            if (quiz.Status == QuizStatus.Approved)
            {
                ModelState.AddModelError(string.Empty, "Cannot edit an already approved quiz.");
                return View(viewModel);
            }

            quiz.Title = viewModel.Title;
            quiz.ClinicalScenario = viewModel.ClinicalScenario;
            quiz.Status = QuizStatus.Draft;
            quiz.UpdatedAt = DateTime.UtcNow;
            quiz.Category = viewModel.Category;

            if(viewModel.QuizImage != null)
            {
                if (!ValidationHelper.IsImageFile(viewModel.QuizImage))
                {
                    ModelState.AddModelError("QuizImage", "Uploaded file is not an image");
                    return View(viewModel);
                }
                // remove old image and save new image
                await FileUploadHelper.RemoveFile(quiz.ImageUrl, _quizConfig.QuizImagesFolder, _hostEnvironment.WebRootPath);

                // overwrite with new image
                string savedFilePath = await FileUploadHelper.SaveFile(viewModel.QuizImage, _hostEnvironment.WebRootPath, _quizConfig.QuizImagesFolder);
                quiz.ImageUrl = savedFilePath;
            }

            await _dbContext.SaveChangesAsync();

            TempData[TempDataKeys.SuccessMessage] = "Quiz updated successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet("{id}/Details")]
        public IActionResult Details(int id)
        {
            var quiz = _dbContext.Quiz.Include(x => x.ApprovedByAdmin)
                .Include(x => x.Notes).ThenInclude(y => y.Editor)
                .FirstOrDefault(q => q.Id == id && q.ContentCreatorId == this.GetUserId());


            if (quiz == null)
            {
                return NotFound();
            }

            var viewModel = _mapper.Map<QuizDetailsViewModel>(quiz);
            if (!string.IsNullOrEmpty(viewModel.ImageUrl))
            {
                viewModel.ImageUrl = _quizConfig.QuizImagesFolder + "/" + viewModel.ImageUrl;

            }

            return View(viewModel);
        }

        [HttpGet("{id}/Questions")]
        public async Task<IActionResult> Questions(int id)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == id && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return NotFound();
            }
            ViewBag.Quiz = quizFromDb.Title;
            ViewBag.QuizId = quizFromDb.Id;

            ViewBag.IsApproved = quizFromDb.Status == QuizStatus.Approved;
            return View();
        }

        #region Ajax Calls
        [HttpPost("SendForApproval")]
        public async Task<JsonResult> SendForApproval(QuizSendForApprovalViewModel model)
        {
            var quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == model.Id && x.ContentCreatorId == this.GetUserId());
            if (quiz == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz Not Found"
                });
            }

            if (quiz.Status == QuizStatus.Draft || quiz.Status == QuizStatus.Rejected)
            {
                quiz.Status = QuizStatus.WaitingForApproval;
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Invalid Quiz"
                });
            }

            return Json(new AjaxResponse
            {
                Success = true,
                Message = ""
            });

        }

        [HttpGet("GetQuestion")]
        public async Task<JsonResult> GetQuestionById(int quizId, int questionId)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new
                {
                    Success = false
                });
            }

            var questionFromDb = await _dbContext.Question
                .Include(x => x.HotspotQuestionImage)
                .Where(x => x.QuizId == quizFromDb.Id && x.Id == questionId).FirstOrDefaultAsync();

            if (questionFromDb == null)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Question Not Found"
                });
            }

            var converted = _mapper.Map<QuestionByIdViewModel>(questionFromDb);
            if (!string.IsNullOrEmpty(converted.ExistingImageName))
            {
                converted.ExistingImagePath = _quizConfig.HotspotImagesFolder + "/" + converted.ExistingImageName;
            }
            return Json(new
            {
                Success = true,
                Data = converted
            });
        }

        [HttpGet("GetQuestions")]
        public async Task<JsonResult> GetQuestions(int id)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == id && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new
                {
                    Success = false
                });
            }

            var questionsByQuiz = await _dbContext.Question
                .Include(x => x.AnswerMultipleChoices)
                .Include(x => x.AnswerShortAnswer)
                .Include(x => x.AnswerMatching)
                .Include(x => x.AnswerHotspot)
                .Include(x => x.HotspotQuestionImage)
                .Include(x => x.AnswerSequence.OrderBy(x => x.Order))
                .Where(x => x.QuizId == quizFromDb.Id)
                .ToListAsync();

            var converted = _mapper.Map<List<QuestionListViewModel>>(questionsByQuiz);

            foreach (var q in converted.Where(x => x.QuestionType == QuestionType.Hotspot.ToFriendlyString() ))
            {
                q.HotspotQuestionImage = _quizConfig.HotspotImagesFolder + "/" + q.HotspotQuestionImage;
            }

            ViewBag.QuizId = id;
            ViewBag.Quiz = quizFromDb.Title;
            return Json(new
            {
                Success = true,
                Data = converted
            });
        }

        [HttpPost("CreateQuestion")]
        public async Task<JsonResult> CreateQuestion(int quizId, [FromForm] CreateQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
                if (quizFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Quiz Not Found",
                    });
                }
                await DraftQuiz(quizFromDb.Id);
                if (quizFromDb.Status == QuizStatus.Approved)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Cannot Modify approved Quiz",
                    });
                }


                var question = new Question();
                question.QuestionText = model.QuestionText;
                question.AnswerExplanation = model.AnswerExplanation;
                if (Enum.TryParse(model.QuestionType, out QuestionType parsedQuestionType))
                {
                    question.QuestionType = parsedQuestionType;
                }
                else
                {
                    // Handle invalid enum value
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Invalid Question Type",
                    });
                }

               
                // verify if the same question exists for the quiz with same type

                var questionFromDb = await _dbContext.Question
                   .Where(x => x.QuestionText.ToLower() == question.QuestionText.ToLower()
                   && x.QuestionType == question.QuestionType
                   && x.QuizId == quizId
                   )
                   .FirstOrDefaultAsync();

                if (questionFromDb != null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "The Question Already Exists in the Quiz"
                    });
                }

                if (question.QuestionType == QuestionType.Hotspot)
                {
                    if(model.HotspotImage == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Image is required for Hotspot Question"
                        });
                    }

                    string savedFilePath = await FileUploadHelper.SaveFile(model.HotspotImage, _hostEnvironment.WebRootPath, _quizConfig.HotspotImagesFolder);
                    if (string.IsNullOrEmpty(savedFilePath))
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "An error occured while saving the image"
                        });
                    }

                    question.HotspotQuestionImage = new HotspotQuestionImage
                    {
                        ImageUrl = savedFilePath
                    };
                }

                question.QuizId = quizFromDb.Id;

                _dbContext.Question.Add(question);
                await _dbContext.SaveChangesAsync();

                

                return Json(new AjaxResponse
                {
                    Success = true,
                });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Validation failed\n" + string.Join("\n", errors),
                });
            }
        }

        [HttpPost("UpdateQuestion")]
        public async Task<JsonResult> UpdateQuestion(int quizId, [FromForm] UpdateQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                // verify that quiz exists

                var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
                if (quizFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Quiz Not Found",
                    });
                }
                await DraftQuiz(quizFromDb.Id);
                if (quizFromDb.Status == QuizStatus.Approved)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Cannot Modify approved Quiz",
                    });
                }
                var questionFromDb = await _dbContext.Question
                    .Include(x => x.HotspotQuestionImage)
                    .Where(x => x.QuizId == quizFromDb.Id && x.Id == model.Id).FirstOrDefaultAsync();

                // verify that question exists
                if (questionFromDb == null)
                {
                    return Json(new
                    {
                        Success = false,
                        Message = "Question Not Found"
                    });
                }

                // update question text
                questionFromDb.QuestionText = model.QuestionText;
                questionFromDb.AnswerExplanation = model.AnswerExplanation;
                if (Enum.TryParse(model.QuestionType, out QuestionType parsedQuestionType))
                {
                    // update question type
                    questionFromDb.QuestionType = parsedQuestionType;
                }
                else
                {
                    // Handle invalid enum value
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Invalid Question Type",
                    });
                }

                // verify if the same question text exists for the quiz with same type

                var existingQuestionFromDb = await _dbContext.Question
                   .Where(x => x.QuestionText.ToLower() == model.QuestionText.ToLower()
                   && x.QuestionType == parsedQuestionType
                   && x.QuizId == quizId
                   && x.Id != questionFromDb.Id
                   )
                   .FirstOrDefaultAsync();

                if (existingQuestionFromDb != null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "The Question Already Exists in the Quiz"
                    });
                }
                if (questionFromDb.QuestionType == QuestionType.Hotspot)
                {
                    if (model.HotspotImage != null)
                    {
                        string savedFilePath = await FileUploadHelper.SaveFile(model.HotspotImage, _hostEnvironment.WebRootPath, _quizConfig.HotspotImagesFolder);
                        if (string.IsNullOrEmpty(savedFilePath))
                        {
                            return Json(new AjaxResponse
                            {
                                Success = false,
                                Message = "An error occured while saving the image"
                            });
                        }

                        // remove existing image
                        await FileUploadHelper.RemoveFile(questionFromDb.HotspotQuestionImage.ImageUrl, _quizConfig.HotspotImagesFolder, _hostEnvironment.WebRootPath);

                        questionFromDb.HotspotQuestionImage = new HotspotQuestionImage
                        {
                            ImageUrl = savedFilePath
                        };
                    }
                }
                // update the date
                questionFromDb.UpdatedAt = DateTime.Now;

                _dbContext.Question.Update(questionFromDb);
                await _dbContext.SaveChangesAsync();
                return Json(new AjaxResponse
                {
                    Success = true,
                });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Validation failed\n" + string.Join("\n", errors),
                });
            }
        }

        [HttpDelete("DeleteQuestion")]
        public async Task<JsonResult> DeleteQuestion(int quizId, int questionId)
        {
            // retrieve quiz by id from db for verification
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz Not Found",
                });
            }
            await DraftQuiz(quizFromDb.Id);
            if (quizFromDb.Status == QuizStatus.Approved)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Cannot Modify approved Quiz",
                });
            }
            // retrieve question by id from db
            var questionFromDb = await _dbContext.Question.Include(x => x.HotspotQuestionImage).FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId);
            if (questionFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question Not Found",
                });
            }
            if(questionFromDb.QuestionType == QuestionType.Hotspot)
            {
                // remove image
                // remove existing image
                await FileUploadHelper.RemoveFile(questionFromDb.HotspotQuestionImage.ImageUrl, _quizConfig.HotspotImagesFolder, _hostEnvironment.WebRootPath);

            }

            _dbContext.Question.Remove(questionFromDb);
            await _dbContext.SaveChangesAsync();

            return Json(new AjaxResponse
            {
                Success = true,
                Message = "Question deleted successfully",
            });
        }

        [HttpGet("GetAnswersByQuestion")]
        public async Task<JsonResult> GetAnswersByQuestion(int quizId, int questionId)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz Not Found",
                });
            }

            var questionFromDb = _dbContext.Question
                 .Include(x => x.AnswerShortAnswer)
                 .Include(x => x.AnswerMultipleChoices)
                 .Include(x => x.AnswerMatching)
                 .Include(x => x.AnswerHotspot)
                 .Include(x => x.AnswerSequence.OrderBy(x => x.Order))
                 .FirstOrDefault(q => q.Id == questionId && q.QuizId == quizId);

            if (questionFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question Not Found",
                });
            }

            if (questionFromDb.QuestionType == QuestionType.Multiple_Choice)
            {
                var answers = questionFromDb.AnswerMultipleChoices;
                var converted = _mapper.Map<List<AnswerViewModel>>(answers);
                return Json(new AjaxResponse
                {
                    Success = true,
                    Data = converted,
                });
            }
            if (questionFromDb.QuestionType == QuestionType.Short_Answer)
            {
                var answers = questionFromDb.AnswerShortAnswer;
                var converted = _mapper.Map<List<AnswerViewModel>>(answers);
                return Json(new AjaxResponse
                {
                    Success = true,
                    Data = converted,
                });
            }
            if(questionFromDb.QuestionType == QuestionType.Matching)
            {
                var answers = questionFromDb.AnswerMatching;
                var converted = _mapper.Map<List<AnswerMatchingViewModel>>(answers);
                return Json(new AjaxResponse
                {
                    Success = true,
                    Data = converted,
                });

            }
            if(questionFromDb.QuestionType == QuestionType.Hotspot)
            {
                var answers = questionFromDb.AnswerHotspot;
                var converted = _mapper.Map<List<AnswerHotspotViewModel>>(answers);
                return Json(new AjaxResponse
                {
                    Success = true,
                    Data = converted,
                });
            }
            if(questionFromDb.QuestionType == QuestionType.Sequence)
            {
                var answers = questionFromDb.AnswerSequence;
                var converted = _mapper.Map<List<AnswerSequenceViewModel>>(answers);
                return Json(new AjaxResponse
                {
                    Success = true,
                    Data = converted,
                });
            }

            return Json(new AjaxResponse
            {
                Success = true,
            });
        }

        [HttpPost("AddAnswer")]
        public async Task<JsonResult> AddAnswer(int quizId, int questionId, [FromBody] CreateAnswerViewModel model)
        {
            
            //if (!ModelState.IsValid)
            //{
            //    var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
            //    return Json(new AjaxResponse
            //    {
            //        Success = false,
            //        Message = "Validation failed\n" + string.Join("\n", errors),
            //    });
            //}

            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz not found or unauthorized access",
                });
            }
            await DraftQuiz(quizFromDb.Id);

            if (quizFromDb.Status == QuizStatus.Approved)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Cannot Modify approved Quiz",
                });
            }

            var question = await _dbContext.Question.FindAsync(questionId);
            if (question == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question not found",
                });
            }

            if (question.QuestionType == QuestionType.Multiple_Choice)
            {
                // apply individual validation

                if (string.IsNullOrEmpty(model.Answer.AnswerText))
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Please Provide Answer Text"
                    });
                }

                var answer = new AnswerMultipleChoice();
                answer.AnswerText = model.Answer.AnswerText;
                answer.IsCorrect = model.Answer.IsCorrect;
                answer.QuestionId = question.Id;

                _dbContext.AnswerMultipleChoice.Add(answer);
                await _dbContext.SaveChangesAsync();
            }
            else if (question.QuestionType == QuestionType.Short_Answer)
            {
                if (string.IsNullOrEmpty(model.Answer.AnswerText))
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Please Provide Answer Text"
                    });
                }
                var answer = new AnswerShortAnswer();
                answer.AnswerText = model.Answer.AnswerText;
                answer.IsCorrect = true;
                answer.QuestionId = question.Id;

                _dbContext.AnswerShortAnswer.Add(answer);
                await _dbContext.SaveChangesAsync();
            }
            // in future we'll handle more
            else if(question.QuestionType == QuestionType.Matching)
            {
                if (string.IsNullOrEmpty(model.MatchingAnswer.LeftSide) || string.IsNullOrEmpty(model.MatchingAnswer.RightSide))
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Please Provide Both Answer Sides"
                    });
                }

                var answer = new AnswerMatching();
                answer.CreatedAt = DateTime.Now;
                answer.QuestionId = questionId;
                answer.LeftSide = model.MatchingAnswer.LeftSide;
                answer.RightSide = model.MatchingAnswer.RightSide;

                _dbContext.AnswerMatching.Add(answer);
                await _dbContext.SaveChangesAsync();
            }
            else if(question.QuestionType == QuestionType.Hotspot)
            {
                if(model.HotspotAnswer == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Empty Data Model",
                    });
                }

                if(model.HotspotAnswer.Width ==0 || model.HotspotAnswer.Height == 0)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Invalid Area selected",
                    });
                }

                // 
                var answer = _mapper.Map<AnswerHotspot>(model.HotspotAnswer);
                answer.CreatedAt = DateTime.Now;
                answer.QuestionId = questionId;

                _dbContext.AnswerHotspot.Add(answer);
                await _dbContext.SaveChangesAsync();
            }
            else if(question.QuestionType == QuestionType.Sequence)
            {
                if (string.IsNullOrEmpty(model.Answer.AnswerText))
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Please Provide Answer Text"
                    });
                }
                // get max sequence number for current question
                var allAnswers = _dbContext.AnswerSequence.Where(x => x.QuestionId == question.Id).ToList();
                int maxOrder = 0;
                if (allAnswers.Any())
                {
                    maxOrder = allAnswers.Max(x => x.Order);
                }
                var answer = new AnswerSequence();
               
                answer.AnswerText = model.Answer.AnswerText;
                answer.QuestionId = question.Id;
                answer.Order = maxOrder + 1;

                _dbContext.AnswerSequence.Add(answer);
                await _dbContext.SaveChangesAsync();
            }


            return Json(new AjaxResponse
            {
                Success = true,
                Message = "Answer added successfully",
            });
        }

        [HttpDelete("DeleteAnswer")]
        public async Task<JsonResult> DeleteAnswer(int answerId, int questionId, int quizId)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz not found",
                });
            }
            await DraftQuiz(quizFromDb.Id);
            if (quizFromDb.Status == QuizStatus.Approved)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Cannot Modify approved Quiz",
                });
            }
            var questionFromDb = await _dbContext.Question.FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId);
            if (questionFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question not found",
                });
            }

            if (quizFromDb.Status == QuizStatus.Approved)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Cannot Modify approved Quiz",
                });
            }

            if (questionFromDb.QuestionType == QuestionType.Multiple_Choice)
            {
                var answerFromDb = await _dbContext.AnswerMultipleChoice.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionId);
                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                _dbContext.AnswerMultipleChoice.Remove(answerFromDb);
                await _dbContext.SaveChangesAsync();
            }
            else if (questionFromDb.QuestionType == QuestionType.Short_Answer)
            {
                var answerFromDb = await _dbContext.AnswerShortAnswer.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionId);
                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                _dbContext.AnswerShortAnswer.Remove(answerFromDb);
                await _dbContext.SaveChangesAsync();
            }
            else if (questionFromDb.QuestionType == QuestionType.Matching)
            {
                var answerFromDb = await _dbContext.AnswerMatching.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionId);
                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                _dbContext.AnswerMatching.Remove(answerFromDb);
                await _dbContext.SaveChangesAsync();
            }
            else if(questionFromDb.QuestionType == QuestionType.Hotspot)
            {
                var answerFromDb = await _dbContext.AnswerHotspot.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionId);
                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                _dbContext.AnswerHotspot.Remove(answerFromDb);
                await _dbContext.SaveChangesAsync();
            }
            else if(questionFromDb.QuestionType == QuestionType.Sequence)
            {
                var answerFromDb = await _dbContext.AnswerSequence.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionId);
                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                _dbContext.AnswerSequence.Remove(answerFromDb);
                await _dbContext.SaveChangesAsync();
            }
            return Json(new AjaxResponse
            {
                Success = true,
                Message = "Answer deleted successfully",
            });
        }

        [HttpGet("GetAnswerById")]
        public async Task<JsonResult> GetAnswerById(int answerId, int questionId, int quizId)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz not found",
                });
            }

            var questionFromDb = await _dbContext.Question.FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId);
            if (questionFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question not found",
                });
            }

            if (questionFromDb.QuestionType == QuestionType.Multiple_Choice)
            {
                var answerFromDb = await _dbContext.AnswerMultipleChoice.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionFromDb.Id);

                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                var converted = _mapper.Map<AnswerViewModel>(answerFromDb);
                var response = new AjaxResponse
                {
                    Success = true,
                    Data = converted
                };

                return Json(response);
            }

            else if (questionFromDb.QuestionType == QuestionType.Short_Answer)
            {
                var answerFromDb = await _dbContext.AnswerShortAnswer.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionFromDb.Id);

                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                var converted = _mapper.Map<AnswerViewModel>(answerFromDb);
                var response = new AjaxResponse
                {
                    Success = true,
                    Data = converted
                };

                return Json(response);
            }
            else if (questionFromDb.QuestionType == QuestionType.Matching)
            {
                var answerFromDb = await _dbContext.AnswerMatching.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionFromDb.Id);

                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                var converted = _mapper.Map<AnswerMatchingViewModel>(answerFromDb);
                var response = new AjaxResponse
                {
                    Success = true,
                    Data = converted
                };

                return Json(response);
            }
            else if(questionFromDb.QuestionType == QuestionType.Hotspot)
            {
                var answerFromDb = await _dbContext.AnswerHotspot.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionFromDb.Id);

                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                var converted = _mapper.Map<AnswerHotspotViewModel>(answerFromDb);
                var response = new AjaxResponse
                {
                    Success = true,
                    Data = converted
                };

                return Json(response);
            }
            else if(questionFromDb.QuestionType == QuestionType.Sequence)
            {
                var answerFromDb = await _dbContext.AnswerSequence.FirstOrDefaultAsync(x => x.Id == answerId && x.QuestionId == questionFromDb.Id);

                if (answerFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Answer not found",
                    });
                }

                var converted = _mapper.Map<AnswerSequenceViewModel>(answerFromDb);
                var response = new AjaxResponse
                {
                    Success = true,
                    Data = converted
                };

                return Json(response);
            }
            return Json(new AjaxResponse
            {
                Success = false
            });
        }

        [HttpPost("UpdateAnswer")]
        public async Task<JsonResult> UpdateAnswer(int questionId, int quizId, [FromBody] UpdateAnswerViewModel model)
        {
            if (ModelState.IsValid)
            {

                var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
                if (quizFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Quiz not found",
                    });
                }

                if (quizFromDb.Status == QuizStatus.Approved)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Cannot Modify approved Quiz",
                    });
                }
                await DraftQuiz(quizFromDb.Id);
                var questionFromDb = await _dbContext.Question.FirstOrDefaultAsync(x => x.Id == questionId && x.QuizId == quizId);
                if (questionFromDb == null)
                {
                    return Json(new AjaxResponse
                    {
                        Success = false,
                        Message = "Question not found",
                    });
                }
                if (questionFromDb.QuestionType == QuestionType.Multiple_Choice)
                {
                    var answer = await _dbContext.AnswerMultipleChoice.FirstOrDefaultAsync(x => x.Id == model.Id && x.QuestionId == questionFromDb.Id);

                    if (answer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Answer not found",
                        });
                    }
                    // Update the answer properties
                    answer.AnswerText = model.Answer.AnswerText;
                    answer.IsCorrect = model.Answer.IsCorrect;
                }
                else if (questionFromDb.QuestionType == QuestionType.Short_Answer)
                {
                    var answer = await _dbContext.AnswerShortAnswer.FirstOrDefaultAsync(x => x.Id == model.Id && x.QuestionId == questionFromDb.Id);

                    if (answer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Answer not found",
                        });
                    }
                    // Update the answer properties
                    answer.AnswerText = model.Answer.AnswerText;

                }
                else if (questionFromDb.QuestionType == QuestionType.Matching)
                {
                    if (string.IsNullOrEmpty(model.MatchingAnswer.LeftSide) || string.IsNullOrEmpty(model.MatchingAnswer.RightSide))
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Please Provide Both Answer Sides"
                        });
                    }
                    var answer = await _dbContext.AnswerMatching.FirstOrDefaultAsync(x => x.Id == model.Id && x.QuestionId == questionFromDb.Id);

                    if (answer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Answer not found",
                        });
                    }
                    // Update the answer properties
                    answer.LeftSide = model.MatchingAnswer.LeftSide;
                    answer.RightSide = model.MatchingAnswer.RightSide;
                }
                else if(questionFromDb.QuestionType == QuestionType.Hotspot)
                {
                    if (model.HotspotAnswer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Empty Data Model",
                        });
                    }

                    if (model.HotspotAnswer.Width == 0 || model.HotspotAnswer.Height == 0)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Invalid Area selected",
                        });
                    }

                    var answer = await _dbContext.AnswerHotspot.FirstOrDefaultAsync(x => x.Id == model.Id && x.QuestionId == questionFromDb.Id);
                    if (answer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Answer not found",
                        });
                    }

                    answer.X = model.HotspotAnswer.X;
                    answer.Y = model.HotspotAnswer.Y;
                    answer.Width = model.HotspotAnswer.Width;
                    answer.Height = model.HotspotAnswer.Height;
                }
                else if(questionFromDb.QuestionType == QuestionType.Sequence)
                {
                    var answer = await _dbContext.AnswerSequence.FirstOrDefaultAsync(x => x.Id == model.Id && x.QuestionId == questionFromDb.Id);

                    if (answer == null)
                    {
                        return Json(new AjaxResponse
                        {
                            Success = false,
                            Message = "Answer not found",
                        });
                    }
                    // Update the answer properties
                    answer.AnswerText = model.Answer.AnswerText;
                }
                await _dbContext.SaveChangesAsync();

                return Json(new AjaxResponse
                {
                    Success = true,
                });
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Validation failed\n" + string.Join("\n", errors),
                });
            }
        }

        [HttpPost("UpdateSequence")]
        public async Task<JsonResult> UpdateSequence(int questionId, int quizId, [FromBody] UpdateSequenceViewModel model)
        {
            var quizFromDb = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId && x.ContentCreatorId == this.GetUserId());
            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Quiz not found or unauthorized access",
                });
            }
            await DraftQuiz(quizFromDb.Id);

            if (quizFromDb.Status == QuizStatus.Approved)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Cannot Modify approved Quiz",
                });
            }

            var question = await _dbContext.Question.FindAsync(questionId);
            if (question == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Question not found",
                });
            }

            int order = 1;
            if(model.AnswerIds != null)
            {
                foreach (var id in model.AnswerIds)
                {
                    var answerFromDb = await _dbContext.AnswerSequence.FirstOrDefaultAsync(x => x.Id == id);
                    if(answerFromDb != null)
                    {
                        answerFromDb.Order = order;
                        order++;

                        await _dbContext.SaveChangesAsync();
                    }
                }
            }

            return Json(new AjaxResponse 
            {
                Success = true
            });
        }

        #endregion


        /// <summary>
        /// Update Quiz's status to Draft
        /// </summary>
        /// <param name="quizId"></param>
        /// <returns></returns>
        [NonAction]
        private async Task DraftQuiz(int quizId)
        {
            var quizById = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == quizId);
            if(quizById!= null)
            {
                quizById.Status = QuizStatus.Draft;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
