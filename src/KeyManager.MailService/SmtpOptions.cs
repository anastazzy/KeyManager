namespace KeyManager.MailService;

public class SmtpOptions
{
    public string FromName { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public Connection Connection { get; set; } = new();
    public Credentials Credentials { get; set; } = new();
}

public class Connection
{
    public string Host { get; set; } = string.Empty;
    public int Port { get; set; }
    public bool IsUseSsl { get; set; }
}

public class Credentials
{
    public string Login { get; set; } = string.Empty;
    public string ApplicationPassword { get; set; } = string.Empty;
}