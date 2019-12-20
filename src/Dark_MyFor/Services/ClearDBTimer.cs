using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
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
            
                while (true)
                {
                    
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
