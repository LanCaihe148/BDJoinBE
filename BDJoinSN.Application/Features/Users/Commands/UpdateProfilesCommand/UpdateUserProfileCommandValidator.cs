
using FluentValidation;

namespace BDJoinSN.Application.Features.Users.Commands.UpdateProfilesCommand
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator() 
        {
            RuleFor(x => x.Name)
                .MaximumLength(50).WithMessage("El nombre no puede exceder 50 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("El nombre solo puede contener letras y espacios")
                .When(x => !string.IsNullOrWhiteSpace(x.Name));

            RuleFor(x => x.LastName)
                .MaximumLength(50).WithMessage("El apellido no puede exceder 50 caracteres")
                .Matches(@"^[a-zA-ZÀ-ÿ\s]+$").WithMessage("El apellido solo puede contener letras y espacios")
                .When(x => !string.IsNullOrWhiteSpace(x.LastName));

            RuleFor(x => x.Biography)
                .MaximumLength(500).WithMessage("La biografía no puede exceder 500 caracteres");

            RuleFor(x => x.Location)
                .MaximumLength(100).WithMessage("La ubicación no puede exceder 100 caracteres");

            

            RuleFor(x => x.Birthday)
                .Must(NoSerFutura).WithMessage("La fecha de nacimiento no puede ser futura")
                .Must(SerRazonable).WithMessage("La fecha de nacimiento no es válida")
                .Must(TenerAlMenos13Anios).WithMessage("Debes tener al menos 13 años para usar esta red social")
                .When(x => x.Birthday.HasValue);
        }
        

        private bool NoSerFutura(DateTime? birthday)
            => !birthday.HasValue || birthday.Value.Date <= DateTime.UtcNow.Date;

        private bool SerRazonable(DateTime? birthday)
            => !birthday.HasValue || birthday.Value.Year >= 1900;

        private bool TenerAlMenos13Anios(DateTime? birthday)
        {
            if (!birthday.HasValue) return true;
            var hoy = DateTime.UtcNow.Date;
            var edad = hoy.Year - birthday.Value.Year;
            if (birthday.Value.Date > hoy.AddYears(-edad)) edad--;
            return edad >= 13;
        }
    }
}
