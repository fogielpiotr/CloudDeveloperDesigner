using FluentValidation;

namespace Application.Projects.Command.EditProject
{
    public class EditProjectValidator : AbstractValidator<EditProjectCommand>
    {
        public EditProjectValidator()
        {
            RuleFor(c => c.Name).NotEmpty();
            RuleFor(c => c.Name).MinimumLength(3);
            RuleFor(c => c.Name).MaximumLength(25);
            RuleFor(c => c.Description).NotEmpty();
        }
    }
}
