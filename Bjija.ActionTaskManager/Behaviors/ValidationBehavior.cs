//using Bjija.ActionTaskManager.Abstractions.Mediator;
//using System.ComponentModel.DataAnnotations;
//using FluentValidation;

//namespace Bjija.ActionTaskManager.Mediator.Behaviors
//{
//    public class ValidationBehavior<TRequest, TResponse> : IRequestBehavior<TRequest, TResponse>
//        where TRequest : IRequest<TResponse>
//    {
//        private readonly IEnumerable<IValidator<TRequest>> _validators;

//        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
//        {
//            _validators = validators;
//        }

//        public async Task<TResponse> Process(TRequest request, RequestHandlerDelegate<TResponse> next)
//        {
//            if (!_validators.Any()) return await next();

//            var context = new ValidationContext<TRequest>(request);
//            var validationResults = await Task.WhenAll(
//                _validators.Select(v => v.ValidateAsync(context)));

//            var failures = validationResults
//                .SelectMany(r => r.Errors)
//                .Where(f => f != null)
//                .ToList();

//            if (failures.Count > 0)
//                throw new ValidationException(failures);

//            return await next();
//        }
//    }
//}