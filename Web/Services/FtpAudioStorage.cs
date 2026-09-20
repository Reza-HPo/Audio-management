using FluentFTP;

namespace Web.Services;

public class FtpAudioStorage
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _audioPath;
    private readonly string _publicBaseUrl;

    public FtpAudioStorage(IConfiguration configuration)
    {
        _host = configuration["Ftp:Host"]
            ?? throw new InvalidOperationException("Ftp:Host تنظیم نشده است.");

        _port = int.TryParse(configuration["Ftp:Port"], out var port)
            ? port
            : 21;

        _username = configuration["Ftp:Username"]
            ?? throw new InvalidOperationException("Ftp:Username تنظیم نشده است.");

        _password = configuration["Ftp:Password"]
            ?? throw new InvalidOperationException("Ftp:Password تنظیم نشده است.");

        _audioPath = configuration["Ftp:AudioPath"] ?? "/audio";

        _publicBaseUrl = configuration["Ftp:PublicBaseUrl"]
            ?? throw new InvalidOperationException("Ftp:PublicBaseUrl تنظیم نشده است.");
    }

    private AsyncFtpClient CreateClient()
    {
        var client = new AsyncFtpClient(
            _host,
            _username,
            _password,
            _port);

        client.Config.EncryptionMode = FtpEncryptionMode.None;
        client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;

        return client;
    }

    public async Task<string> UploadAsync(
        Stream stream,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var remotePath =
            $"{_audioPath.TrimEnd('/')}/{fileName}";

        Console.WriteLine("======================================");
        Console.WriteLine("FTP UPLOAD START");
        Console.WriteLine($"Host: {_host}");
        Console.WriteLine($"Port: {_port}");
        Console.WriteLine($"User: {_username}");
        Console.WriteLine($"Remote Path: {remotePath}");
        Console.WriteLine($"File: {fileName}");
        Console.WriteLine("======================================");

        await using var client = CreateClient();

        Console.WriteLine("FTP: Connecting...");

        await client.Connect(cancellationToken);

        Console.WriteLine("FTP: Connected!");

        var status = await client.UploadStream(
            stream,
            remotePath,
            FtpRemoteExists.Overwrite,
            true);

        Console.WriteLine($"FTP: Upload status = {status}");

        if (status != FtpStatus.Success)
        {
            throw new IOException(
                $"آپلود فایل به FTP ناموفق بود. وضعیت: {status}");
        }

        var publicUrl =
            $"{_publicBaseUrl.TrimEnd('/')}/{fileName}";

        Console.WriteLine($"FTP: Public URL = {publicUrl}");
        Console.WriteLine("FTP UPLOAD FINISHED");
        Console.WriteLine("======================================");

        return publicUrl;
    }

    public async Task DeleteAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var remotePath =
            $"{_audioPath.TrimEnd('/')}/{fileName}";

        await using var client = CreateClient();

        await client.Connect(cancellationToken);

        if (await client.FileExists(remotePath, cancellationToken))
        {
            await client.DeleteFile(
                remotePath,
                cancellationToken);
        }
    }
}