using FluentValidation;

namespace Application.Projects.Command.AddProject
{
    public class AddProjectValidator : AbstractValidator<AddProjectCommand>
    {
        public AddProjectValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Name).MinimumLength(3);
            RuleFor(c => c.Name).MaximumLength(25);
            RuleFor(c => c.Description).NotEmpty();
        }
    }
}
