using System.ComponentModel.DataAnnotations;
using Columbidae.Auth;
using Microsoft.EntityFrameworkCore;

namespace Columbidae.Server.Core.Service.Impl;

public class SqliteAuthStorage(string dbPath) : IAuthenticationStorage, IDisposable
{
    private readonly DbContext _dbContext = new(dbPath);

    public bool IsAvailable()
    {
        return true;
    }

    public int GetPriority()
    {
        return (int)Math.Log(_dbContext.Devices.Count() + 1);
    }

    public async Task AddDevice(string token, DeviceInfo device)
    {
        await _dbContext.Devices.AddAsync(new DbContext.DeviceToken { Token = token, DeviceInfo = device });
        await _dbContext.SaveChangesAsync();
    }

    public async Task<DeviceInfo?> GetDevice(string token)
    {
        return await _dbContext.Devices.Where(d => d.Token == token).Select(d => d.DeviceInfo).FirstAsync();
    }

    public async Task<bool> HasToken(string token)
    {
        return await _dbContext.Devices.AnyAsync(d => d.Token == token);
    }

    public async Task RemoveDevice(string token)
    {
        await _dbContext.Devices.Where(d => d.Token == token).ExecuteDeleteAsync();
        await _dbContext.SaveChangesAsync();
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    private class DbContext(string dbPath) : Microsoft.EntityFrameworkCore.DbContext
    {
        public DbSet<DeviceToken> Devices { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public class DeviceToken
        {
            public int DeviceTokenId { get; set; }
            public string Token { get; set; }
            [Required] public DeviceInfo DeviceInfo { get; set; }
        }
    }
}