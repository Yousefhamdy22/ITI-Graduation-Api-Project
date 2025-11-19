using Application.Features.Students.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Application.Features.Students.Commands.StudentAnswer.SubmitStudentAnswerCommand
{
    public class SubmitStudentAnswerCommand : IRequest<Unit>
    {
        [JsonPropertyName("examResultId")]
        public Guid ExamResultId { get; set; }

        [JsonPropertyName("answers")]
        public List<StudentAnswerDto> Answers { get; set; } = new();
    }

    public class StudentAnswerDto
    {
        public Guid QuestionId { get; set; }
        public Guid SelectedAnswerId { get; set; }
    }
}
