using Infrastructure.Interface;
using Core.Entities.Students;
using Application.Features.Students.Dtos;
using MediatR;
using Microsoft.Extensions.Caching.Hybrid;

namespace Application.Features.Students.Commands.StudentAnswer.SubmitStudentAnswerCommand
{
    public class SubmitStudentAnswerCommandHandler
        : IRequestHandler<SubmitStudentAnswerCommand, Unit>
    {
        private readonly IStudentAnswerRepository _studentAnswerRepo;
        private readonly IAnswerOptionRepository _answerOptionRepo;
        private readonly IExamResultRepository _examResultRepo;
        private readonly HybridCache _cache;

        public SubmitStudentAnswerCommandHandler(
            IStudentAnswerRepository studentAnswerRepo,
            IAnswerOptionRepository answerOptionRepo,
            IExamResultRepository examResultRepo,
            HybridCache cache)
        {
            _studentAnswerRepo = studentAnswerRepo;
            _answerOptionRepo = answerOptionRepo;
            _examResultRepo = examResultRepo;
            _cache = cache;
        }

        public async Task<Unit> Handle(SubmitStudentAnswerCommand request, CancellationToken ct)
        {
            //1. Validate exam result existence 
            if (request.Answers == null || !request.Answers.Any())
                throw new ArgumentException("Answers list cannot be empty.");

            var examResult = await _examResultRepo.FindAsync(er => er.Id == request.ExamResultId);
            if (examResult == null)
                throw new KeyNotFoundException("Exam result not found.");


            //2. Process each answer
            foreach (var answer in request.Answers)
            {
                var option = await _answerOptionRepo.FindAsync(o => o.Id == answer.SelectedAnswerId);

                if (option == null || option.QuestionId != answer.QuestionId)
                    throw new ArgumentException($"Invalid selected option for question {answer.QuestionId}");

                // Create domain entity (fully qualified to avoid namespace collision)
                var studentAnswer = Core.Entities.Students.StudentAnswer.Create(
                    request.ExamResultId,
                    answer.QuestionId,
                    answer.SelectedAnswerId);

                // set correctness
                if (option.IsCorrect)
                {
                    studentAnswer.MarkCorrect();
                    examResult.EvaluateAnswer(true);
                }
                else
                {
                    studentAnswer.MarkIncorrect();
                    examResult.EvaluateAnswer(false);
                }

                await _studentAnswerRepo.AddAsync(studentAnswer, ct);
            }


            //3. Update exam result
            await _examResultRepo.UpdateAsync(examResult, ct);

            //4. Clear cache
            await _cache.RemoveAsync($"ExamResult_{request.ExamResultId}_Answers", ct);

            return Unit.Value;
        }
    }
}
