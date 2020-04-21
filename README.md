# Net Core dans Net Framework

## Injection de dépendances

### IServiceCollection dans .Net Framework

Ajouter le package Nuget `Microsoft.Extensions.DependencyInjection` à l'application afin de pouvoir implémenter une instance de `ServiceCollection`.
Ajouter une classe `Startup` : pas nécessaire mais cela permet de garder une cohérence avec .Net Core.
Il sera alors possible d'enregister ici nos services en utilisant les extensions sur IServiceCollection.  

```c#
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
    }
}
```

Afin de pouvoir utiliser l'injection de dépendance de .Net Core il va être indispensable de définir un resolver.
Pour cela il suffit d'implémenter l'interface `IDependencyResolver` à l'aide du `ServiceProvider`.  

```c#
public class DefaultDependencyResolver : IDependencyResolver
{
    protected IServiceProvider ServiceProvider;


    public DefaultDependencyResolver(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IDependencyScope BeginScope()
    {
        return new DefaultDependencyScope(ServiceProvider.CreateScope());
    }

    public void Dispose()
    {
    }

    public object GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return ServiceProvider.GetServices(serviceType);
    }
}

public class DefaultDependencyScope : IDependencyScope
{
    private readonly IServiceScope _serviceScope;

    public DefaultDependencyScope(IServiceScope serviceScope)
    {
        _serviceScope = serviceScope;
    }

    public void Dispose()
    {
        _serviceScope.Dispose();
    }

    public object GetService(Type serviceType)
    {
        return _serviceScope.ServiceProvider.GetService(serviceType);
    }

    public IEnumerable<object> GetServices(Type serviceType)
    {
        return _serviceScope.ServiceProvider.GetServices(serviceType);
    }
}
```

Il suffira donc de builder notre ServiceProvider et d'utiliser ce resolver au démarrage de l'application.
Il faudra également penser à enregistrer les controllers dans les services.
Dans la méthode statique `WebApiConfig.Register(HttpConfiguration config)`, il suffit d'appeler la méthode `Configure` défini ci dessous.  

```c#
public static class DependencyInjectionBootStrapper
{
    public static void Configure(HttpConfiguration config)
    {
        var startup = new Startup();
        var services = new ServiceCollection();
        services.AddApiControllers();
        startup.ConfigureServices(services);

        config.DependencyResolver = new DefaultDependencyResolver(services.BuildServiceProvider());
    }

    private static void AddApiControllers(this IServiceCollection services)
    {
        foreach (var controllerType in typeof(DependencyInjectionBootStrapper).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(IHttpController).IsAssignableFrom(t)
                 && t.Name.EndsWith("Controller", StringComparison.Ordinal)))
        {
            services.AddScoped(controllerType);
        }
    }
}
```

Pour MVC :
```c#
public static class DependencyInjectionBootStrapper
{
    public static void Configure()
    {
        var startup = new Startup();
        var services = new ServiceCollection();
        services.AddApiControllers();
        startup.ConfigureServices(services);

        DependencyResolver.SetResolver(new DefaultDependencyResolver(services.BuildServiceProvider()));
    }

    private static void AddApiControllers(this IServiceCollection services)
    {
        foreach (var controllerType in typeof(DependencyInjectionBootStrapper).Assembly.GetExportedTypes()
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition)
            .Where(t => typeof(IController).IsAssignableFrom(t)
                 && t.Name.EndsWith("Controller", StringComparison.Ordinal)))
        {
            services.AddScoped(controllerType);
        }
    }
}
```

### Stratégie de migration

Dans le cas d'un projet existant remplacer l'ensemble des enregistrements des services peut être un travail long et fastidieux.
Afin d'entamer une démarche itérative il est possible d'effectuer le remplacement sous un mode de fonctionnement hybride.
Des librairies comme par exemple Autofac ou StructureMap proposent des packages qui contiennent une méthode d'extension `Populate(IServiceCollection)` sur le conteneur IoC, ce qui permet de venir alimenter le conteneur existant avec un IServiceCollection.
Cela permet d'utiliser le package `Microsoft.Extensions.DependencyInjection.Abstractions` de manière progressive sans avoir à migrer la totalité du conteneur

Dans le cas de Unity les méthodes d'alimentation du conteneur sont internal dans le package `Unity.Microsoft.DependencyInjection` mais une méthode d'extension `BuildServiceProvider(IServiceCollection)` est exposée.
Cette méthode permet de construire une instance de `ServiceProvider` qui utilise le conteneur Unity. L'alimentation de conteneur est complétée par les services enregistrés dans l'instance `IServiceCollection` passée en paramètre.  

Il nous suffira de venir écraser le DependencyResolver par celui que nous venons d'implémenter (UnityWebApiActivator.cs)
```c#
public static void Start() 
{
    var startup = new Startup();
    var services = new ServiceCollection();
    startup.ConfigureServices(services);
    var resolver = new DefaultDependencyResolver(UnityConfig.Container.BuildServiceProvider(services));

    GlobalConfiguration.Configuration.DependencyResolver = resolver;
}
```

## Configuration  

Ajouter le package `Microsoft.Extensions.Options` dans l'application et la couche de services.
Ajouter le code suivant dans le Bootstrapper.  

```c#
services.AddOptions();
```

Nous allons maintenant définir une option dans la couche de services qui va nous permettre de déterminer le nombre de produits à afficher.

```c#
public class ProductOptions
{
    public int NbProducts { get; set; }
}
```

Puis injecter cette valeur dans le constructeur de `ProductService`.  

```c#
public ProductService(IOptions<ProductOptions> options)
    => _products = Enumerable.Range(1, options.Value.NbProducts)
        .Select(i => new Product($"Product {i}"))
        .ToDictionary(p => p.Id);
```

Et enfin configurer cette options dans la méthode `Startup.ConfigureServices(IServiceCollection services)`  

```c#
services.Configure<ProductOptions>(o => o.NbProducts = 5);
```