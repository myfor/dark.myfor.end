/*
 * 定时清理数据库
 * 
 * 变量 isCleared 用于指示是否已经清理了数据库
 * 将在早上 6 点清理数据库
 * 
 */
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using Microsoft.EntityFrameworkCore;

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
                    DateTimeOffset NOW = DateTimeOffset.Now;

                    if (NOW.Hour == 6 && NOW.Minute == 0)
                    {
                        if (isCleared)
                            continue;

                        //  clear DB
                        ClearAllData();

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
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(db.Comments)}");
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(db.Posts)}");
            db.Database.ExecuteSqlRaw($"DELETE FROM {nameof(db.Files)}");
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
