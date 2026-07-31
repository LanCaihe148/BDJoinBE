

using FluentValidation;

namespace BDJoinSN.Application.Features.Posts.Commands.UpdatePost
{
    public class UpdatePostValidator : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("El ID del post debe ser un número positivo.");

            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("El ID del usuario es requerido.");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("El contenido del post no puede estar vacío.")
                .MaximumLength(5000).WithMessage("El contenido no puede exceder los 5000 caracteres.");
        }
    }
}
