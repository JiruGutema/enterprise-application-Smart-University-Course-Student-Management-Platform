using RabbitMQ.Client;

namespace SmartUniversity.Shared.Kernel.Infrastructure.Messaging;

public sealed class RabbitMqConnection : IDisposable
{
    private readonly ConnectionFactory _factory;
    private IConnection? _connection;

    public RabbitMqConnection(string host, string username, string password)
    {
        _factory = new ConnectionFactory
        {
            HostName = host,
            UserName = username,
            Password = password,
        };
    }

    public async Task<IConnection> GetConnectionAsync()
    {
        if (_connection is { IsOpen: true })
            return _connection;

        _connection = await _factory.CreateConnectionAsync();
        return _connection;
    }

    public void Dispose()
    {
        _connection?.Dispose();
    }
}
