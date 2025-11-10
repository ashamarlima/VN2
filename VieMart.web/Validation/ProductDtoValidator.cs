using FluentValidation;
using VieMart.web.Models;

public class ProductDtoValidator : AbstractValidator<ProductDto>
{
    private static readonly string[] AllowedCategories =
        ["Other", "Phones", "Computers", "Accessories", "Printers", "Cameras"];

    private static readonly string[] AllowedContentTypes =
        ["image/jpeg", "image/png", "image/gif", "image/webp"];

    public ProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Brand)
            .NotEmpty().WithMessage("Brand is required.")
            .MaximumLength(100);

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be ≥ 0.");

        RuleFor(x => x.Category)
            .NotEmpty()
            .Must(c => AllowedCategories.Contains(c))
            .WithMessage("Category is invalid.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        // Different rule when Creating vs Editing (RuleSets)
        RuleSet("Create", () =>
        {
            RuleFor(x => x.ImageFile)
                .NotNull().WithMessage("Product image is required.")
                .Must(f => f != null && AllowedContentTypes.Contains(f.ContentType))
                .WithMessage("Only JPEG, PNG, GIF, WEBP are allowed.");
        });

        RuleSet("Edit", () =>
        {
            RuleFor(x => x.ImageFile)
                .Must(f => f == null || AllowedContentTypes.Contains(f.ContentType))
                .WithMessage("Only JPEG, PNG, GIF, WEBP are allowed.");
        });
    }
}
