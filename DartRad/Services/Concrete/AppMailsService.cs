using DartRad.Data;
using DartRad.Entities;
using DartRad.Services.Abstract;
using Microsoft.EntityFrameworkCore;

namespace DartRad.Services.Concrete
{
    public class AppMailsService : IAppMailsService
    {
        private readonly IMailService _mailService;
        private readonly AppDbContext _dbContext;

        public AppMailsService(IMailService mailService, AppDbContext dbContext)
        {
            this._mailService = mailService;
            this._dbContext = dbContext;
        }
        public async Task NotifySubscribersOfNewQuiz(string quizUrl)
        {
            var subscribers = await _dbContext.Subscriber.ToListAsync();

            foreach (var subcriber in subscribers)
            {
                string mailContent = $@"
                                <p>Dear {subcriber.FirstName} {subcriber.LastName}</p>
                                <p>We're pleased to inform you that a brand new quiz has just been added to our platform.</p>
                                <p>Feel free to visit the quiz in the link below:</p>
                                <a href=""{quizUrl}"">View Quiz</a>";

                _mailService.Send(new EMailMessage
                {
                    Body = mailContent,
                    ReceiverEmail = subcriber.Email,
                    Subject = "New Quiz",
                    ReceiverName = $"{subcriber.FirstName} {subcriber.LastName}"
                });
            }
        }

        public async Task NotifyContentCreatorQuizStatus(bool isAccepted, int contentCreatorId, string quizName, string quizLink)
        {
            var cc = await _dbContext.ContentCreator.FirstOrDefaultAsync(x => x.Id == contentCreatorId);
            string mailContent = $@"
            <p>Dear {cc.Name}</p>
<p>Your submitted quiz '{quizName}' has been { (isAccepted ? "Accepted" : "Rejected") }.</p>
<p>Click on the following link to view your Quiz.</p>
<a href=""{quizLink}"">View Quiz</a>

            ";

            _mailService.Send(new EMailMessage
            {
                Body = mailContent,
                ReceiverEmail = cc.Email,
                Subject = "New Quiz",
                ReceiverName = cc.Name
            });
        }
    }
}
