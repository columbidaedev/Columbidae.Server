using System.Collections.ObjectModel;

namespace Columbidae.Server.Core.Service.Impl;

public class FilesystemMessageCache(string resourcePath) : IMessageCache
{
    private readonly Collection<Resource> _resources = [];

    private struct Resource
    {
        public string Token;
        public string Caption;
        public string DirName;
    }

    private string _createFilename()
    {
        return Guid.NewGuid().ToString().Replace("-", string.Empty);
    }

    public Task<Stream> CreateResourceReceiver(string token, string caption, ulong size = 0L)
    {
        var dirname = _createFilename();
        var dirpath = Path.Join(resourcePath, dirname);
        _resources.Add(new Resource
        {
            Token = token,
            Caption = caption,
            DirName = dirname
        });
        if (!Directory.Exists(dirpath))
        {
            Directory.CreateDirectory(dirpath);
        }

        if (size > 0)
        {
            return Task.FromResult<Stream>(new FileStream(Path.Join(dirpath, caption), new FileStreamOptions
            {
                Mode = FileMode.CreateNew,
                Access = FileAccess.Write,
                PreallocationSize = (long)size
            }));
        }

        return Task.FromResult<Stream>(new FileStream(Path.Join(dirpath, caption), FileMode.CreateNew));
    }

    public async Task<Stream> CreateResourceSender(string token)
    {
        var path = await GetResourcePath(token);
        return new FileStream(path, FileMode.Open);
    }

    public Task<string> GetResourcePath(string token)
    {
        var resource = _resources.FirstOrDefault(r => r.Token == token);
        if (resource.Token == "")
        {
            throw new ArgumentException("Token not found");
        }

        return Task.FromResult(Path.Join(resourcePath, resource.DirName, resource.Caption));
    }

    public bool IsAvailable()
    {
        return true;
    }

    public int GetPriority()
    {
        return 5;
    }
}