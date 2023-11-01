using AutoMapper;
using DartRad.Areas.Editor.Models;
using DartRad.Areas.ContentCreator.Models;
using DartRad.Configurations;
using DartRad.Data;
using DartRad.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using DartRad.Services.Abstract;

namespace DartRad.Areas.Editor.Controllers
{
    [Area("Editor")]
    [Authorize(Roles = AppRoles.Editor)]
    public class QuizController : Controller
    {
        private readonly AppDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IAppMailsService _appMailsService;
        private readonly QuizConfig _quizConfig;
        public QuizController(AppDbContext dbContext, IMapper mapper, IOptions<QuizConfig> option, IAppMailsService appMailsService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            this._appMailsService = appMailsService;
          
            _quizConfig = option.Value;
        }

        public async Task<IActionResult> Index()
        {
            var quizzes = await _dbContext.Quiz
            .Include(q => q.ContentCreator)
            .Where(q => q.Status == QuizStatus.WaitingForApproval)
            .ToListAsync();

            var converted = _mapper.Map<List<PendingQuizListViewModel>>(quizzes);

            return View(converted);
        }

        public async Task<IActionResult> Details(int id)
        {
            var quizFromDb = await _dbContext.Quiz
                .Include(x => x.Notes)
                    .ThenInclude(x => x.Editor)
                .Include(x => x.ContentCreator)
                .FirstAsync(x => x.Id == id);
                
            if (quizFromDb == null || quizFromDb.Status != QuizStatus.WaitingForApproval)
            {
                return NotFound();
            }

            // get questions by quiz
            var questionsFromDb = await _dbContext.Question
                .Include(x => x.AnswerShortAnswer)
                .Include(x => x.AnswerMultipleChoices)
                .Include(x => x.AnswerMatching)
                .Include(x => x.AnswerHotspot)
                .Include(x => x.HotspotQuestionImage)
                .Include(x => x.AnswerSequence.OrderBy(x => x.Order))
                .Where(x => x.QuizId == quizFromDb.Id)
                .ToListAsync();

            var questionsVm = _mapper.Map<List<QuestionListViewModel>>(questionsFromDb);
            

            foreach (var q in questionsVm.Where(x => x.QuestionType == QuestionType.Hotspot.ToFriendlyString()))
            {
                q.HotspotQuestionImage = _quizConfig.HotspotImagesFolder + "/" + q.HotspotQuestionImage;
            }
            var quizVm = _mapper.Map<PendingQuizDetailsViewModel>(quizFromDb);

            if(!string.IsNullOrEmpty(quizVm.ImageUrl))
            {
                quizVm.ImageUrl = _quizConfig.QuizImagesFolder + "/" + quizVm.ImageUrl;
            }

            quizVm.Questions = questionsVm;
            return View(quizVm);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(int id)
        {
            var quiz = await _dbContext.Quiz.FindAsync(id);
            if (quiz == null)
            {
                return NotFound();
            }

            if (quiz.Status != QuizStatus.WaitingForApproval)
            {
                return NotFound();
            }

            quiz.Status = QuizStatus.Approved;
            quiz.ApprovedBy = this.GetUserId();

            await _dbContext.SaveChangesAsync();
            TempData[TempDataKeys.SuccessMessage] = "Quiz Approved";

            await NotifySubscribers(id);

            await NotifyContentCreator(id, quiz.ContentCreatorId, quiz.Title, true);

            return RedirectToAction(nameof(Index));
        }

       

        [HttpPost]
        public async Task<IActionResult> Reject(RejectQuizViewModel model)
        {
            var quiz = await _dbContext.Quiz.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (quiz == null)
            {
                return NotFound();
            }

            if (quiz.Status != QuizStatus.WaitingForApproval)
            {
                return NotFound();
            }

            // Update the quiz status to rejected

            quiz.Status = QuizStatus.Rejected;

            _dbContext.Quiz.Update(quiz);

            if (!string.IsNullOrEmpty(model.Note))
            {
                // Create a new quiz note

                var note = new QuizNote
                {
                    Note = model.Note,
                    AdminId = this.GetUserId(),
                    CreatedAt = DateTime.Now,
                    QuizId = quiz.Id
                };

                _dbContext.QuizNote.Add(note);

            }

            await _dbContext.SaveChangesAsync();

            TempData[TempDataKeys.SuccessMessage] = "Quiz Rejected";

            await NotifyContentCreator(model.Id, quiz.ContentCreatorId, quiz.Title, false);
            return RedirectToAction(nameof(Index));
        }


        private async Task NotifyContentCreator(int quizId,int contentCreatorId, string title, bool isAccepted)
        {
            string quizUrl = Url.Action("Details", "Quiz", new
            {
                id = quizId,
                Area = "ContentCreator"
            }, Url.ActionContext.HttpContext.Request.Scheme);
            await _appMailsService.NotifyContentCreatorQuizStatus(isAccepted, contentCreatorId, title, quizUrl);
        }

        private async Task NotifySubscribers(int id)
        {
            // send new quiz email
            string quizUrl = Url.Action("TakeQuiz", "Quiz", new
            {
                id = id,
                Area = ""
            }, Url.ActionContext.HttpContext.Request.Scheme);

            await _appMailsService.NotifySubscribersOfNewQuiz(quizUrl);
        }
    }
}
