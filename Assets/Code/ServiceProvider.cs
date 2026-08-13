using System;
using System.Collections.Generic;

public class ServiceProvider 
{
    private ServiceProvider() { }

    private static ServiceProvider _instance;
    public static ServiceProvider Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new ServiceProvider();
            }
            return _instance;
        }
        private set => _instance = value;
    }

    private Dictionary<Type, IService> services = new Dictionary<Type, IService>();

    public void AddService<ServiceType>(IService service) where ServiceType : class, IService
    {
        if (!services.ContainsKey(typeof(ServiceType)))
            services.Add(typeof(ServiceType), service);
    }

    public void RemoveService<ServiceType>() where ServiceType : class, IService
    {
        if (services.ContainsKey(typeof(ServiceType)))
            services.Remove(typeof(ServiceType));
    }

    public bool ContainsService<ServiceType>() where ServiceType : class, IService
    {
        return services.ContainsKey(typeof(ServiceType));
    }

    public ServiceType GetService<ServiceType>() where ServiceType : class,  IService
    {
        return services[typeof(ServiceType)] as ServiceType;
    }

    public void ClearAllServices()
    {
        services.Clear();
    }

    public void ClearNonPersistantServices()
    {
        List<Type> nonPersistantServiceTypes = new List<Type>();
        foreach (KeyValuePair<Type, IService> service in services)
        {
            if (!service.Value.IsPersistant)
                nonPersistantServiceTypes.Add(service.Key);
        }
        foreach (Type keyToRemove in nonPersistantServiceTypes)
        {
            services.Remove(keyToRemove);
        }
    }
}
