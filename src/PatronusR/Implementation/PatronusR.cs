using PatronusR.Interfaces;
using PatronusR.Types;

namespace PatronusR.Implementation
{
    /// <summary>
    /// Default mediator implementation
    /// </summary>
    public class PatronusR : IPatronusR
    {
        private readonly IServiceProvider _serviceProvider;

        public PatronusR(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
                throw new InvalidOperationException($"No handler found for request type {requestType.Name}");

            var method = handlerType.GetMethod("Handle");
            var result = method.Invoke(handler, new object[] { request, cancellationToken });

            if (result is Task<TResponse> taskResult)
                return await taskResult;

            throw new InvalidOperationException($"Handler for {requestType.Name} did not return expected Task<{typeof(TResponse).Name}>");
        }

        public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestType = request.GetType();

            // Find the IRequest<T> interface
            var requestInterface = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterface == null)
                throw new ArgumentException($"Request type {requestType.Name} does not implement IRequest<>");

            var responseType = requestInterface.GetGenericArguments()[0];
            var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);

            var handler = _serviceProvider.GetService(handlerType);
            if (handler == null)
                throw new InvalidOperationException($"No handler found for request type {requestType.Name}");

            var method = handlerType.GetMethod("Handle");
            var result = method.Invoke(handler, new object[] { request, cancellationToken });

            if (result is Task task)
            {
                await task;

                // Get the result from Task<T>
                if (responseType == typeof(Unit))
                    return Unit.Value;

                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty?.GetValue(task);
            }

            throw new InvalidOperationException($"Handler for {requestType.Name} did not return expected Task");
        }
    }
}
