using AutoMapper;
using DartRad.Areas.ContentCreator.Models;
using DartRad.Configurations;
using DartRad.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DartRad.Controllers
{
    [Route("[Controller]")]
    [AllowAnonymous]
    public class QuizController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        bool filterByStatus = true;
        private readonly QuizConfig _quizConfig;

        public QuizController(
            AppDbContext dbContext, 
            IMapper mapper, 
            IOptions<QuizConfig> options,
            IWebHostEnvironment hostEnvironment
            )
        {
            this._dbContext = dbContext;
            this._mapper = mapper;
            this._hostEnvironment = hostEnvironment;
            _quizConfig = options.Value;
        }

        [HttpGet("List")]
        public async Task<IActionResult> List()
        {
            var query = _dbContext.Quiz
                .Include(x => x.ContentCreator).AsQueryable();

            if (filterByStatus)
            {
                query = query.Where(x => x.Status == QuizStatus.Approved);
            }

            var quizList = await query.ToListAsync();
            var vm = _mapper.Map<List<PublicQuizListViewModel>>(quizList);

            foreach (var q in vm)
            {
               q.Questions = _dbContext.Question.Where(x => x.QuizId == q.Id && x.QuestionType != QuestionType.True_False).Count();

               q.ImageUrl = _quizConfig.QuizImagesFolder + "/" + q.ImageUrl;
            }

            return View(vm);
        }

        // Endpoint to display the quiz questions
        [HttpGet("{id}")]
        public async Task<IActionResult> TakeQuiz(int id)
        {
            // Retrieve the quiz with the given ID and pass it to the view
            if(id == 0)
            {
                return NotFound();
            }

            var query =  _dbContext.Quiz.Where(x => x.Id == id);
            if (filterByStatus)
            {
                query = query.Where(x => x.Status == QuizStatus.Approved);
            }
            var quizFromDb = await query.FirstOrDefaultAsync();

            if(quizFromDb == null)
            {
                return NotFound();
            }
            ViewBag.QuizId = quizFromDb.Id;
            return View();
        }


        [HttpPost("{id}")]
        public async Task<JsonResult> SubmitQuiz(int id,[FromBody] PublicQuizSubmitViewModel submission)
        {
            // verify quiz id
            if (id == 0)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Invalid Quiz"
                });
            }

            var query = _dbContext.Quiz.Where(x => x.Id == id);
            if (filterByStatus)
            {
                query = query.Where(x => x.Status == QuizStatus.Approved);
            }
            var quizFromDb = await query.FirstOrDefaultAsync();

            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Invalid Quiz"
                });
            }

            // validate answers
            var questionsWithAnswers = await _dbContext.Question
                .Include(x => x.AnswerMultipleChoices)
                .Include(x => x.AnswerShortAnswer)
                .Include(x => x.AnswerMatching)
                .ToListAsync();

            float points = 0;
            if(submission.Answers == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Invalid Submission"
                });

            }
            foreach (var ans in submission.Answers)
            {
                // match the answer agains correct answer of question
                var question = questionsWithAnswers.First(x => x.Id == ans.QuestionId);
                if(question.QuestionType == QuestionType.Multiple_Choice)
                {
                    var correctAnswers = question.AnswerMultipleChoices.Where(x => x.IsCorrect).ToList();
                    if(correctAnswers.Any(x => x.Id == ans.SelectedAnswerId))
                    {
                        points++;
                    }
                }
                else if(question.QuestionType == QuestionType.Short_Answer)
                {
                    var correctAnswers = question.AnswerShortAnswer.Select(x => x.AnswerText.ToLower()).ToList();
                    if (correctAnswers.Contains(ans.WrittenAnswer.ToLower()))
                    {
                        points++;
                    }
                }
                else if(question.QuestionType == QuestionType.Matching)
                {
                    var correctAnswers = question.AnswerMatching.Where(x => x.QuestionId == ans.QuestionId).ToList();
                    bool isFalseMatch = false;
                    foreach (var match in ans.MatchedAnswers)
                    {
                        string correctRight = correctAnswers.First(x => x.Id == match.AnswerId).RightSide;
                        if(correctRight.Trim() != match.Text.Trim())
                        {
                            isFalseMatch = true;
                            break;
                        }
                    }
                    if (!isFalseMatch)
                    {
                        points++;
                    }
                }
            }

            var totalQuestions = (float)submission.Answers.Count;
            var answerPercentage = (points / totalQuestions) * 100;

            HttpContext.Session.SetString("answerPercentage", Convert.ToInt32( answerPercentage).ToString());
            // Redirect or show a result page indicating the completion of the quiz

            return Json(new AjaxResponse
            {
                Success = true,
                Message = "",
            });

        }

        // Endpoint to display the quiz result
        [HttpGet("{id}/result")]
        public async Task<IActionResult> QuizResult(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            var quizFromDb = await _dbContext.Quiz.Where(x => x.Id == id
      //    && x.Status == QuizStatus.Approved
            ).FirstOrDefaultAsync();

            if (quizFromDb == null)
            {
                return NotFound();
            }

            var vm = new QuizResultViewModel
            {
                QuizTitle = quizFromDb.Title,
                Score = HttpContext.Session.GetString("answerPercentage") + " %"
            };
         
            return View(vm);
        }

        #region Ajax
        [HttpGet("GetQuestions")]
        public async Task<JsonResult> GetQuestions(int id)
        {
            // Retrieve the quiz with the given ID and pass it to the view


            var query = _dbContext.Quiz.Where(x => x.Id == id);
            if (filterByStatus)
            {
                query = query.Where(x => x.Status == QuizStatus.Approved);
            }
            var quizFromDb = await query.FirstOrDefaultAsync();

            if (quizFromDb == null)
            {
                return Json(new AjaxResponse
                {
                    Success = false,
                    Message = "Invalid Quiz"
                });
            }
            // Display the quiz questions and provide a form for submitting answers
            var vm = _mapper.Map<PublicQuizViewModel>(quizFromDb);

            var questions = await _dbContext.Question
                .Include(x => x.AnswerMultipleChoices)
                .Include(x => x.AnswerShortAnswer)
                .Include(x => x.AnswerMatching)
                .Include(x => x.AnswerHotspot)
                .Include(x => x.HotspotQuestionImage)
                .Include(x => x.AnswerSequence.OrderBy(x => x.Order))
                .Where(x => x.QuizId == quizFromDb.Id).ToListAsync();

            vm.Questions = _mapper.Map<List<QuestionListViewModel>>(questions);

            foreach (var q in vm.Questions)
            {
                //if (q.QuestionType == QuestionType.Matching.ToFriendlyString())
                //{
                //    List<string> rightAnswers = q.AnswerMatching.Select(x => x.RightSide).ToList();
                //    Shuffle(rightAnswers);
                //    for (int i = 0; i < q.AnswerMatching.Count; i++)
                //    {
                //        q.AnswerMatching[i].RightSide = rightAnswers[i];
                //    }
                //}
                if(q.QuestionType == QuestionType.Hotspot.ToFriendlyString())
                {
                    q.HotspotQuestionImage = _quizConfig.HotspotImagesFolder + "/" + q.HotspotQuestionImage;
                }
            }
            return Json(new AjaxResponse
            {
                Data = vm,
                Success = true
            });
        }

        void Shuffle<T>(List<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        #endregion
    }
}
