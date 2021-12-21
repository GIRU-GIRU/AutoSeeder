using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AutoSeederSvc
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private DateTime _timeUserGoesAway = DateTime.UtcNow.AddSeconds(10);
        bool _userIsAway = false;
        IProgramManager _programManager;

        public Worker(ILogger<Worker> logger, IProgramManager programManager)
        {
            _logger = logger;
            _programManager = programManager;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(50, stoppingToken);

                    await DoWork();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.GetBaseException().Message);
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service has been closed gracefully");

            return Task.CompletedTask;
        }

        private async Task DoWork()
        {
            bool userInputFound = await InputChecker.CheckIfKeysAreBeingPressed();

            if (userInputFound)
            {

                _timeUserGoesAway = AddAwayTime();


                if (_userIsAway)
                {
                    await HandleUserBack();
                }

            }
            else if (DateTime.UtcNow > _timeUserGoesAway)
            {

                if (!_userIsAway)
                {
                    await HandleUserAway();
                }
            }
        }
        private Task HandleUserBack()
        {
            _userIsAway = false;
            _logger.LogInformation("User Back");
            _programManager.ClosePrograms();

            return Task.CompletedTask;
        }

        private Task HandleUserAway()
        {
            _userIsAway = true;
            _logger.LogInformation("User AFK");
            _programManager.OpenPrograms(GetProgramsToStart());

            return Task.CompletedTask;
        }

        private DateTime AddAwayTime()
        {
            //TODO hardcoding - add to json config

            return DateTime.UtcNow.AddSeconds(10);
        }

        private List<string> GetProgramsToStart()
        {
            //TODO hardcoding - add to json config

            List<string> programs = new List<string>();

            programs.Add(@"D:\Downloads\Deluge\deluge.exe");

            return programs;
        }


    }
}
