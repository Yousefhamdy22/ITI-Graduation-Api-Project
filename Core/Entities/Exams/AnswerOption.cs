using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Common;
using Core.Entities.Students;
using Microsoft.EntityFrameworkCore;


namespace Core.Entities.Exams;

public partial class AnswerOption : AuditableEntity
{
    #region Properties
    public string Text { get; private set; } = default!;
    public bool IsCorrect { get; private set; }
    #endregion

    #region Foreign Keys
    public Guid QuestionId { get; private set; }
    public Question Question { get; private set; } = default!;
    #endregion
}
