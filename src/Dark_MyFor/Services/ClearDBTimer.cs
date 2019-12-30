/*
 * 定时清理数据库
 * 
 * 变量 isCleared 用于指示是否已经清理了数据库
 * 将在早上 6 点清理数据库
 * 
 */
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace Dark_MyFor.Services
{
    public class ClearDBTimer : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                bool isCleared = false;

                while (true)
                {
                    //  获取北京时间
                    DateTimeOffset NOW = DateTimeOffset.Now.ToUniversalTime().AddHours(8);

                    if (NOW.Hour == 6 && NOW.Minute == 0)
                    {
                        if (isCleared)
                            continue;

                        //  clear DB
                        ClearAllData();
                        //  clear files
                        ClearAllFiles();

                        isCleared = true;
                    }
                    else
                    {
                        if (isCleared)
                            isCleared = false;
                    }
                }
            });

            return Task.CompletedTask;
        }

        private void ClearAllData()
        {
            using var db = new DB.DarkContext();
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(DB.DarkContext.Comments)}");
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(DB.DarkContext.Posts)}");
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(DB.DarkContext.Files)}");
        }

        private void ClearAllFiles()
        {
            Directory.Delete(Domain.File.SaveThumbnailPath, true);
            Directory.Delete(Domain.File.SaveTempPath, true);
            Directory.Delete(Domain.File.SavePath, true);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
