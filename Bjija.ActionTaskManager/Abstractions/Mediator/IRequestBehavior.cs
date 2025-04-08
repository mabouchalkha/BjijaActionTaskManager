using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bjija.ActionTaskManager.Abstractions.Mediator
{
    /// <summary>
    /// Represents a behavior that can intercept and potentially modify a request and its response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request.</typeparam>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    public interface IRequestBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
    {
        /// <summary>
        /// Processes the request through the behavior pipeline.
        /// </summary>
        /// <param name="request">The request being processed.</param>
        /// <param name="next">The delegate for the next action in the pipeline.</param>
        /// <returns>The response from the pipeline.</returns>
        Task<TResponse> Process(TRequest request, RequestHandlerDelegate<TResponse> next);
    }

    /// <summary>
    /// Represents a delegate to handle a request and return a response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <returns>The response from the handler.</returns>
    public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();
}
