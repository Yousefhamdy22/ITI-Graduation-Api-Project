using Application.Features.Students.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Students.Commands.Students.UpdateStudent
{
    public record UpdateStudentCommand(
          Guid StudentId,
          string Gender
      ) : IRequest<StudentDto>;
}
