using FluentValidation;
using EShopper.Application.UseCases.DTO;

namespace EShopper.Implementation.Validations
{
    public class PictureDtoValidator : AbstractValidator<PictureDto>
    {
        public PictureDtoValidator()
        {
            RuleFor(x => x.Src).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Src is required");
            RuleFor(x => x.Alt).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("Alt is required.");
        }
    }
}
