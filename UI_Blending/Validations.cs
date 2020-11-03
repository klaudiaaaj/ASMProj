using System;
using System.Collections.Generic;
using System.Text;
using FluentValidation;

namespace UI_app
{
    public class ImageClassValidator : AbstractValidator<ImageClass>
    {
        public ImageClassValidator()
        {
            List<int> weighList = new List<int>() { 1366, 1920, 3840 };
            List<int> heightsList = new List<int>() { 768, 1080, 2160 };

            RuleFor(x => x.heightBacground)
                .NotEmpty()
                .GreaterThan(0)
                .Must(x => heightsList.Contains(x))
                   .OnAnyFailure(x =>
                   {
                       throw new ArgumentException($"Parameter {nameof(x.heightBacground)} is invalid.");
                   });

            RuleFor(x => x.widthBackground)
               .NotEmpty()
               .GreaterThan(0)
               .Must(x => weighList.Contains(x))
                  .OnAnyFailure(x =>
                  {
                      throw new ArgumentException($"Parameter {nameof(x.widthBackground)} is invalid.");
                  });
            RuleFor(x => x.heightSource)
                .NotEmpty()
                .Equal(x => x.heightBacground)
                .OnAnyFailure(x =>
                {
                    throw new ArgumentException($"Parameter {nameof(x.heightSource)} is invalid.");
                });
            RuleFor(x => x.widthSource)
              .NotEmpty()
              .Equal(x => x.widthBackground)
              .OnAnyFailure(x =>
              {
                  throw new ArgumentException($"Parameter {nameof(x.widthSource)} is invalid.");
              });
        }
    }
}