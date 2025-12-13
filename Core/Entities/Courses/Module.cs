using Core.Common;
using Core.Common.Results;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Courses;


public partial class Module : AuditableEntity
{
    public string Title { get; set; }
    public string Description { get; set; }

    [ForeignKey("Course")]
    public Guid CourseId { get; set; }
    public Course Course { get; set; }

    public ICollection<Lecture> Lectures { get; set; } = new List<Lecture>();

   


    private Module() : base() { }

    private Module(string name, string description, Guid courseId) : base(Guid.NewGuid())
    {
        Title = name;
        Description = description;
        CourseId = courseId;
    }


    public void AddLecture(Lecture lecture)
    {
        if (Lectures == null)
            Lectures = new List<Lecture>();
        Lectures.Add(lecture);
    }

    public static Result<Module> Create(string name, string description, Guid courseId)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Module>.FromError(Error.Validation("Name is required"));
        var module = new Module(name, description, courseId);
        return Result<Module>.FromValue(module);
    }

    public void Update(string name, string description)
    {
        if (!string.IsNullOrWhiteSpace(name))
            Title = name;
        if (!string.IsNullOrWhiteSpace(description))
            Description = description;
    }


    public Result<Success> Delete()
    {
        if (Lectures.Any())
            return Result<Success>.FromError(Error.Validation("Cannot delete module with associated lectures."));
        return Result<Success>.FromValue(new Success());
    }
}
