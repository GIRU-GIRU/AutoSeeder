using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace AutoSeederSvc
{
    public class ProgramManager : IProgramManager
    {
        private readonly ILogger<Worker> _logger;
        private bool _processesAreStarting { get; set; }
        private bool _processesAreStarted { get; set; }
        List<Process> _currentlyManagedProcesses = new List<Process>();

        public bool CheckIfActiveProcesses()
        {
            return _processesAreStarted || _processesAreStarting;
        }


        public ProgramManager(ILogger<Worker> logger)
        {
            _logger = logger;
        }


        public void OpenPrograms(List<string> paths)
        {
            if (paths == null || paths.Count == 0)
            {
                throw new ArgumentException("Invalid program paths were provided to open");
            }

            if (_processesAreStarted || _processesAreStarting) return;


            _processesAreStarting = true;

            try
            {

                foreach (var path in paths)
                {
                    var programName = Path.GetFileNameWithoutExtension(path);
                    var existingProcesses = Process.GetProcessesByName(programName);

                    if (existingProcesses.Length > 0)
                    {
                        string[] existingProcessAsStrings = existingProcesses.Select(x => x.ProcessName).ToArray();
                        string existingProcessNames = String.Join(", ", existingProcessAsStrings);
                        _logger.LogInformation($"Found {existingProcessNames} already running");
                        _currentlyManagedProcesses.AddRange(existingProcesses);
                    }
                    else
                    {
                        Process process = Process.Start(path);
                        _logger.LogInformation($"Started {process.ProcessName}");
                        _currentlyManagedProcesses.Add(process);
                    }
                }


            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to start process - " + ex.GetBaseException().Message);
            }

            _processesAreStarted = true;

        }


        public void ClosePrograms()
        {
            if (!_processesAreStarted || !_processesAreStarting) return;


            string[] existingProcessAsStrings = _currentlyManagedProcesses.Select(x => x.ProcessName).ToArray();
            _logger.LogInformation($"Killing {String.Join(", ", existingProcessAsStrings)}");

            foreach (var process in _currentlyManagedProcesses)
            {
                try
                {
                    process.Kill();
                }
                catch (Exception ex)
                {
                    _logger.LogError("Failed to kill process - " + ex.GetBaseException().Message);
                }
            }

            _currentlyManagedProcesses.Clear();
            _processesAreStarted = false;
            _processesAreStarting = false;
        }


    }
}

