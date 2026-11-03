using FluentValidation;
using FluentValidation.Results;
using EShopper.Application.UseCases.DTO;
using EShopper.Domain;

namespace EShopper.Implementation.UseCases
{
    internal static class HeroSlideRules
    {
        private static readonly string[] MediaTypes = { "image", "video" };

        public static void Validate(CreateHeroSlideDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
            {
                throw Fail("Hero title is required.");
            }

            if (request.Title.Trim().Length > 120)
            {
                throw Fail("Hero title must be 120 characters or fewer.");
            }

            if ((request.Tagline ?? "").Trim().Length > 80)
            {
                throw Fail("Hero tagline must be 80 characters or fewer.");
            }

            if ((request.Subtitle ?? "").Trim().Length > 300)
            {
                throw Fail("Hero subtitle must be 300 characters or fewer.");
            }

            if (!MediaTypes.Contains((request.MediaType ?? "").Trim().ToLowerInvariant()))
            {
                throw Fail("Media type must be image or video.");
            }

            var src = (request.MediaSrc ?? "").Trim();

            if (src.Length > 300 || (src.Length > 0 && !IsSafeAddress(src)))
            {
                throw Fail("Media must be an uploaded file or a http(s) address.");
            }

            var buttonText = (request.ButtonText ?? "").Trim();
            var buttonLink = (request.ButtonLink ?? "").Trim();

            if (buttonText.Length > 40)
            {
                throw Fail("Button text must be 40 characters or fewer.");
            }

            if (buttonText.Length > 0 && buttonLink.Length == 0)
            {
                throw Fail("A button needs a link as well as a label.");
            }

            if (buttonLink.Length > 200 || (buttonLink.Length > 0 && !(buttonLink.StartsWith("/") || IsSafeAddress(buttonLink))))
            {
                throw Fail("Button link must start with / or be a http(s) address.");
            }
        }

        public static void Apply(CreateHeroSlideDto request, HeroSlide slide)
        {
            slide.Tagline = TextRules.NullIfBlank(request.Tagline);
            slide.Title = request.Title.Trim();
            slide.Subtitle = TextRules.NullIfBlank(request.Subtitle);
            slide.ButtonText = TextRules.NullIfBlank(request.ButtonText);
            slide.ButtonLink = TextRules.NullIfBlank(request.ButtonLink);
            slide.MediaType = request.MediaType.Trim().ToLowerInvariant();
            slide.MediaSrc = TextRules.NullIfBlank(request.MediaSrc);
            slide.SortOrder = request.SortOrder;
        }

        private static ValidationException Fail(string message)
        {
            return new ValidationException(new[] { new ValidationFailure("HeroSlide", message) });
        }

        private static bool IsSafeAddress(string value)
        {
            return value.StartsWith("assets/") || value.StartsWith("http://") || value.StartsWith("https://");
        }
    }
}
