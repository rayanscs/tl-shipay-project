using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TL.Shipay.Project.Application.Filters
{
    public class FluentValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public FluentValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var parameter in context.ActionArguments)
            {
                var value = parameter.Value;
                if (value == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(value.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(value);
                    var result = await validator.ValidateAsync(validationContext);

                    if (!result.IsValid)
                    {
                        context.Result = new BadRequestObjectResult(new
                        {
                            mensagem = "Dados inválidos",
                            erros = result.Errors.Select(e => new
                            {
                                campo = e.PropertyName,
                                mensagem = e.ErrorMessage
                            }).ToList()
                        });
                        return;
                    }
                }
            }

            await next();
        }
    }
}