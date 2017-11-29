using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Threading.Tasks;

namespace AvalonEdit.Editor
{
    public class ServiceContainer : IServiceProvider, IServiceContainer, IDisposable
    {
        private readonly ConcurrentStack<IServiceProvider> fallbackProviders = new ConcurrentStack<IServiceProvider>();
        private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
        private readonly List<Type> servicesToDispose = new List<Type>();
        private readonly Dictionary<Type, object> taskCompletionSources = new Dictionary<Type, object>(); // object = TaskCompletionSource<T> for various T

        public ServiceContainer()
        {
            services.Add(typeof(ServiceContainer), this);
            services.Add(typeof(IServiceContainer), this);
        }

        public void AddFallbackProvider(IServiceProvider provider)
        {
            this.fallbackProviders.Push(provider);
        }

        public object GetService(Type serviceType)
        {
            object instance;
            lock (services)
            {
                if (services.TryGetValue(serviceType, out instance))
                {
                    ServiceCreatorCallback callback = instance as ServiceCreatorCallback;
                    if (callback != null)
                    {
                        instance = callback(this, serviceType);
                        if (instance != null)
                        {
                            services[serviceType] = instance;
                            OnServiceInitialized(serviceType, instance);
                        }
                        else
                        {
                            services.Remove(serviceType);
                        }
                    }
                }
            }
            if (instance != null)
                return instance;
            foreach (var fallbackProvider in fallbackProviders)
            {
                instance = fallbackProvider.GetService(serviceType);
                if (instance != null)
                    return instance;
            }
            return null;
        }

        public void Dispose()
        {
            Type[] disposableTypes;
            lock (services)
            {
                disposableTypes = servicesToDispose.ToArray();
                //services.Clear();
                servicesToDispose.Clear();
            }
            // dispose services in reverse order of their creation
            for (int i = disposableTypes.Length - 1; i >= 0; i--)
            {
                IDisposable disposable = null;
                lock (services)
                {
                    object serviceInstance;
                    if (services.TryGetValue(disposableTypes[i], out serviceInstance))
                    {
                        disposable = serviceInstance as IDisposable;
                        if (disposable != null)
                            services.Remove(disposableTypes[i]);
                    }
                }
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void OnServiceInitialized(Type serviceType, object serviceInstance)
        {
            IDisposable disposableService = serviceInstance as IDisposable;
            if (disposableService != null)
                servicesToDispose.Add(serviceType);

            dynamic taskCompletionSource;
            if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource))
            {
                taskCompletionSources.Remove(serviceType);
                taskCompletionSource.SetResult((dynamic)serviceInstance);
            }
        }

        public void AddService(Type serviceType, object serviceInstance)
        {
            lock (services)
            {
                services.Add(serviceType, serviceInstance);
                OnServiceInitialized(serviceType, serviceInstance);
            }
        }

        public void AddService(Type serviceType, object serviceInstance, bool promote)
        {
            AddService(serviceType, serviceInstance);
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            lock (services)
            {
                services.Add(serviceType, callback);
            }
        }

        public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            AddService(serviceType, callback);
        }

        public void RemoveService(Type serviceType)
        {
            lock (services)
            {
                object instance;
                if (services.TryGetValue(serviceType, out instance))
                {
                    services.Remove(serviceType);
                    IDisposable disposableInstance = instance as IDisposable;
                    if (disposableInstance != null)
                        servicesToDispose.Remove(serviceType);
                }
            }
        }

        public void RemoveService(Type serviceType, bool promote)
        {
            RemoveService(serviceType);
        }

        public Task<T> GetFutureService<T>()
        {
            Type serviceType = typeof(T);
            lock (services)
            {
                if (services.ContainsKey(serviceType))
                {
                    return Task.FromResult((T)GetService(serviceType));
                }
                else
                {
                    object taskCompletionSource;
                    if (taskCompletionSources.TryGetValue(serviceType, out taskCompletionSource))
                    {
                        return ((TaskCompletionSource<T>)taskCompletionSource).Task;
                    }
                    else
                    {
                        var tcs = new TaskCompletionSource<T>();
                        taskCompletionSources.Add(serviceType, tcs);
                        return tcs.Task;
                    }
                }
            }
        }
    }
}