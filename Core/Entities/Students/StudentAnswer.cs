using Core.Common;
using Core.Entities.Exams;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Students;


public partial class StudentAnswer : AuditableEntity
{
    public Guid ExamResultId { get; private set; }
    public Guid QuestionId { get; private set; }
    public Guid? SelectedAnswerId { get; private set; }
    public bool IsCorrect { get; private set; }
    public DateTimeOffset AnsweredAt { get; private set; }

    // Navigation Properties
    public ExamResult ExamResult { get; private set; } = default!;
    public Question Question { get; private set; } = default!;
    public AnswerOption? SelectedAnswer { get; private set; }

    private StudentAnswer() { }

    private StudentAnswer(Guid examResultId, Guid questionId, Guid? selectedAnswerId)
    {
        Id = Guid.NewGuid();
        ExamResultId = examResultId;
        QuestionId = questionId;
        SelectedAnswerId = selectedAnswerId;
        AnsweredAt = DateTimeOffset.UtcNow;
    }

    public static StudentAnswer Create(Guid examResultId, Guid questionId, Guid? selectedAnswerId)
    {
        if (examResultId == Guid.Empty) throw new ArgumentException("ExamResultId must be provided", nameof(examResultId));
        if (questionId == Guid.Empty) throw new ArgumentException("QuestionId must be provided", nameof(questionId));

        return new StudentAnswer(examResultId, questionId, selectedAnswerId);
    }

    public void MarkCorrect()
    {
        IsCorrect = true;
    }

    public void MarkIncorrect()
    {
        IsCorrect = false;
    }

}
