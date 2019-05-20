# How to use dapper

### **1.** Install NugetPack

```
dotnet add package Dapper --version 1.60.6
```

### **2.** Create connection factory:

```csharp
public class DbConnectionFactory : IDbConnectionFactory
{
    public string ConnectionString { get; }

    public DbConnectionFactory(MssqlConnectionEntity item)
    {
        ConnectionString = $"Data Source={item.ServerName};" +
                            $"Initial Catalog={item.DataBaseName};" +
                            $"User id={item.UserName};" +
                            $"Password={item.Password};";
    }

    public SqlConnection GetConnection()
    {
        return new SqlConnection(ConnectionString);
    }
}
```
and class to hold connection settings
```csharp
public class MssqlConnectionEntity
{
    public string ServerName { get; set; }
    public string DataBaseName { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}
```

### **3.** Create dapper wrapper:

```csharp
public class DapperWrapper : IDapperWrapper
{
    public IDbConnection _connection { get; }

    public DapperWrapper(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<TResult> ScopeQueryAsync<TResult>(Func<IDbConnection, Task<TResult>> funcAsync)
    {
        using (_connection)
        {
            _connection.Open();
            return await funcAsync(_connection);
        }
    }
    public async Task ScopeExecuteAsync(Func<IDbConnection, Task> funcAsync)
    {
        using (_connection)
        {
            _connection.Open();
            await funcAsync(_connection);
        }
    }
}
```

### **4.** Register DI and take settigns from .json file:

**appsettings.development.json**
```csharp
  "SqlConnection": {
    "ServerName": "localhost",
    "DataBaseName": "citizenbudget",
    "UserName": "sa",
    "Password":  "Kleopatra2019@"
  },
    "Host": {
    "Port": 5201,
    "server.urls": "localhost"
  }
```

**Program.cs**
```csharp
public static IWebHostBuilder CreateWebHostBuilder(string[] args)
{
    var config = new ConfigurationBuilder()
        .AddEnvironmentVariables()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", true, true)
        .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true, true)
        .Build();

    var serverPort = config.GetValue<int?>("Host:port") ?? 5000;
    var serverUrls = config.GetValue<string>("Host:server.urls") ?? string.Format("http://*:{0}", serverPort);

    var builder = new WebHostBuilder()
            .UseKestrel()
            .UseConfiguration(config)
            .UseUrls($"http://{serverUrls}:{serverPort}")
            .UseStartup<Startup>();

    return builder;
}
```

**Startup.cs**
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Mssql
    var ConnectionString = Configuration.GetSection("SqlConnection").Get<MssqlConnectionEntity>();
    services.AddSingleton<IDbConnectionFactory>(s => new DbConnectionFactory(ConnectionString));
    services.AddTransient<IDbConnection>(s => s.GetRequiredService<IDbConnectionFactory>().GetConnection());
    // Dapper
    services.AddSingleton<IDapperWrapper>(s => new DapperWrapper(s.GetRequiredService<IDbConnection>()));
}
```