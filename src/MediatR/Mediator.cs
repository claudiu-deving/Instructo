using System.Collections.Concurrent;
using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Messager;

public sealed class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ConcurrentDictionary<Type, Type> _requestHandlerTypes;
    private readonly ConcurrentDictionary<Type, Type[]> _notificationHandlerTypes;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider=serviceProvider;
        _requestHandlerTypes=new ConcurrentDictionary<Type, Type>();
        _notificationHandlerTypes=new ConcurrentDictionary<Type, Type[]>();
    }

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handler = GetRequestHandler(requestType, responseType);
        if(handler==null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
        }

        var behaviors = GetPipelineBehaviors<TResponse>(requestType, responseType);

        if(behaviors.Any())
        {
            return await ExecuteWithPipeline<TResponse>(request, handler, behaviors, cancellationToken);
        }

        return await ExecuteHandler<TResponse>(request, handler, cancellationToken);
    }

    private async Task<TResponse> ExecuteWithPipeline<TResponse>(object request, object handler, IEnumerable<object> behaviors, CancellationToken cancellationToken)
    {
        var behaviorList = behaviors.ToList();

        RequestHandlerDelegate<TResponse> handlerDelegate = (ct) => ExecuteHandler<TResponse>(request, handler, ct);

        for(int i = behaviorList.Count-1; i>=0; i--)
        {
            var behavior = behaviorList[i];
            var currentDelegate = handlerDelegate;

            handlerDelegate=(ct) => ExecuteBehavior<TResponse>(behavior, request, currentDelegate, ct);
        }

        return await handlerDelegate(cancellationToken);
    }

    private async Task<TResponse> ExecuteBehavior<TResponse>(object behavior, object request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var behaviorType = behavior.GetType();
        var handleMethod = behaviorType.GetMethod("Handle");

        if(handleMethod==null)
        {
            throw new InvalidOperationException($"Handle method not found on behavior {behaviorType.Name}");
        }

        var result = handleMethod.Invoke(behavior, [request, next, cancellationToken]);

        if(result is Task<TResponse> taskResult)
        {
            return await taskResult;
        }

        if(result is Task task)
        {
            await task;
            return (TResponse)task.GetType().GetProperty("Result")?.GetValue(task)!;
        }

        return (TResponse)result!;
    }

    private async Task<TResponse> ExecuteHandler<TResponse>(object request, object handler, CancellationToken cancellationToken)
    {
        var handlerType = handler.GetType();
        var handleMethod = handlerType.GetMethod("Handle");

        if(handleMethod==null)
        {
            throw new InvalidOperationException($"Handle method not found on handler {handlerType.Name}");
        }

        var result = handleMethod.Invoke(handler, [request, cancellationToken]);

        if(result is Task<TResponse> taskResult)
        {
            return await taskResult;
        }

        if(result is Task task)
        {
            await task;
            return (TResponse)task.GetType().GetProperty("Result")?.GetValue(task)!;
        }

        return (TResponse)result!;
    }

    public async Task<Unit> Send(IRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var responseType = typeof(Unit);

        var handler = GetRequestHandler(requestType, responseType);
        if(handler==null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
        }

        var behaviors = GetPipelineBehaviors<Unit>(requestType, responseType);

        if(behaviors.Any())
        {
            return await ExecuteWithPipeline<Unit>(request, handler, behaviors, cancellationToken);
        }

        return await ExecuteHandler<Unit>(request, handler, cancellationToken);
    }

    public async Task<object?> Send(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var requestInterfaces = requestType.GetInterfaces()
            .Where(i => i.IsGenericType&&i.GetGenericTypeDefinition()==typeof(IRequest<>))
            .ToList();

        if(requestInterfaces.Count==0)
        {
            // Check for IRequest (void)
            if(requestType.GetInterfaces().Any(i => i==typeof(IRequest)))
            {
                return await Send((IRequest)request, cancellationToken);
            }

            throw new InvalidOperationException($"Request type {requestType.Name} does not implement IRequest<T> or IRequest");
        }

        var requestInterface = requestInterfaces.First();
        var responseType = requestInterface.GetGenericArguments()[0];

        var handler = GetRequestHandler(requestType, responseType);
        if(handler==null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.Name}");
        }

        var behaviors = GetPipelineBehaviors<object>(requestType, responseType);

        if(behaviors.Any())
        {
            var result = await ExecuteWithPipelineObject(request, handler, behaviors, responseType, cancellationToken);
            return result;
        }

        return await ExecuteHandlerObject(request, handler, responseType, cancellationToken);
    }

    public async Task Publish(object notification, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(notification);

        var notificationType = notification.GetType();
        var handlers = GetNotificationHandlers(notificationType);

        var tasks = handlers.Select(handler =>
        {
            var handlerType = handler.GetType();
            var handleMethod = handlerType.GetMethod("Handle");

            if(handleMethod==null)
            {
                throw new InvalidOperationException($"Handle method not found on handler {handlerType.Name}");
            }

            var result = handleMethod.Invoke(handler, [notification, cancellationToken]);

            if(result is Task task)
            {
                return task;
            }

            return Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }

    public async Task Publish<TNotification>(TNotification notification, CancellationToken cancellationToken = default)
        where TNotification : INotification
    {
        ArgumentNullException.ThrowIfNull(notification);

        var notificationType = typeof(TNotification);
        var handlers = GetNotificationHandlers(notificationType);

        var tasks = handlers.Select(handler =>
        {
            var handlerType = handler.GetType();
            var handleMethod = handlerType.GetMethod("Handle");

            if(handleMethod==null)
            {
                throw new InvalidOperationException($"Handle method not found on handler {handlerType.Name}");
            }

            var result = handleMethod.Invoke(handler, [notification, cancellationToken]);

            if(result is Task task)
            {
                return task;
            }

            return Task.CompletedTask;
        });

        await Task.WhenAll(tasks);
    }

    private object? GetRequestHandler(Type requestType, Type responseType)
    {
        var key = requestType;

        // Check if we have cached the handler type for this request type
        if(_requestHandlerTypes.TryGetValue(key, out var cachedHandlerType))
        {
            return _serviceProvider.GetService(cachedHandlerType);
        }

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType);

        if(handler==null&&responseType==typeof(Unit))
        {
            // Try void handler
            handlerType=typeof(IRequestHandler<>).MakeGenericType(requestType);
            handler=_serviceProvider.GetService(handlerType);
        }

        if(handler!=null)
        {
            // Cache the handler type, not the instance
            _requestHandlerTypes.TryAdd(key, handlerType);
        }

        return handler;
    }

    private IEnumerable<object> GetNotificationHandlers(Type notificationType)
    {
        var key = notificationType;

        // Check if we have cached the handler types for this notification type
        if(_notificationHandlerTypes.TryGetValue(key, out var cachedHandlerTypes))
        {
            return cachedHandlerTypes.Select(handlerType => _serviceProvider.GetService(handlerType))
                                    .Where(handler => handler!=null)
                                    .Cast<object>();
        }

        var handlerType = typeof(INotificationHandler<>).MakeGenericType(notificationType);
        var handlers = _serviceProvider.GetServices(handlerType);

        var handlerList = handlers.ToList();
        if(handlerList.Any())
        {
            // Cache the handler types
            var handlerTypeArray = handlerList.Select(h => h.GetType()).ToArray();
            _notificationHandlerTypes.TryAdd(key, handlerTypeArray);
        }

        return handlerList;
    }

    private IEnumerable<object> GetPipelineBehaviors<TResponse>(Type requestType, Type responseType)
    {
        var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, responseType);
        var behaviors = _serviceProvider.GetServices(behaviorType);
        return behaviors;
    }

    private async Task<object?> ExecuteWithPipelineObject(object request, object handler, IEnumerable<object> behaviors, Type responseType, CancellationToken cancellationToken)
    {
        var behaviorList = behaviors.ToList();

        // Use reflection to create the proper delegate type
        var delegateType = typeof(RequestHandlerDelegate<>).MakeGenericType(responseType);
        var handlerDelegate = CreateHandlerDelegate(request, handler, responseType, cancellationToken);

        for(int i = behaviorList.Count-1; i>=0; i--)
        {
            var behavior = behaviorList[i];
            var currentDelegate = handlerDelegate;

            handlerDelegate=CreateBehaviorDelegate(behavior, request, currentDelegate, responseType, cancellationToken);
        }

        var result = handlerDelegate.DynamicInvoke(cancellationToken);

        if(result is Task task)
        {
            await task;
            var taskType = task.GetType();

            if(taskType.IsGenericType)
            {
                return taskType.GetProperty("Result")?.GetValue(task);
            }

            return Unit.Value;
        }

        return result;
    }

    private async Task<object?> ExecuteHandlerObject(object request, object handler, Type responseType, CancellationToken cancellationToken)
    {
        var handlerType = handler.GetType();
        var handleMethod = handlerType.GetMethod("Handle");

        if(handleMethod==null)
        {
            throw new InvalidOperationException($"Handle method not found on handler {handlerType.Name}");
        }

        var result = handleMethod.Invoke(handler, [request, cancellationToken]);

        if(result is Task task)
        {
            await task;
            var taskType = task.GetType();

            if(taskType.IsGenericType)
            {
                return taskType.GetProperty("Result")?.GetValue(task);
            }

            return Unit.Value;
        }

        return result;
    }

    private Delegate CreateHandlerDelegate(object request, object handler, Type responseType, CancellationToken cancellationToken)
    {
        var delegateType = typeof(RequestHandlerDelegate<>).MakeGenericType(responseType);
        var method = typeof(Mediator).GetMethod(nameof(ExecuteHandlerObject), BindingFlags.NonPublic|BindingFlags.Instance);

        return Delegate.CreateDelegate(delegateType, this, method);
    }

    private Delegate CreateBehaviorDelegate(object behavior, object request, Delegate next, Type responseType, CancellationToken cancellationToken)
    {
        var delegateType = typeof(RequestHandlerDelegate<>).MakeGenericType(responseType);
        var behaviorType = behavior.GetType();
        var handleMethod = behaviorType.GetMethod("Handle");

        if(handleMethod==null)
        {
            throw new InvalidOperationException($"Handle method not found on behavior {behaviorType.Name}");
        }

        return (CancellationToken ct) =>
        {
            var result = handleMethod.Invoke(behavior, [request, next, ct]);
            return result;
        };
    }
}