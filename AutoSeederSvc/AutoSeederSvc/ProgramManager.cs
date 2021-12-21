using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;

namespace AutoSeederSvc
{
    public class ProgramManager
    {
        private readonly ILogger<Worker> _logger;
        private bool processesAreStarting { get; set; }
        private bool processesAreStarted { get; set; }
        List<Process> CurrentlyManagedProcesses = new List<Process>();

        public bool CheckIfActiveProcesses()
        {
            return processesAreStarted || processesAreStarting;
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

            if (processesAreStarted || processesAreStarted) return;


            processesAreStarting = true;

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
                        CurrentlyManagedProcesses.AddRange(existingProcesses);
                    }
                    else
                    {
                        Process process = Process.Start(path);
                        _logger.LogInformation($"Started {process.ProcessName}");
                        CurrentlyManagedProcesses.Add(process);
                    }
                }
               
            
            }
            catch (Exception ex)
            {

                _logger.LogError("Failed to start process - " + ex.GetBaseException().Message);
            }

            processesAreStarted = true;

        }


        public void ClosePrograms()
        {
            if (!processesAreStarted || !processesAreStarted) return;


            string[] existingProcessAsStrings = CurrentlyManagedProcesses.Select(x => x.ProcessName).ToArray();
            _logger.LogInformation($"Killing {String.Join(", ", existingProcessAsStrings)}");

            foreach (var process in CurrentlyManagedProcesses)
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

            CurrentlyManagedProcesses.Clear();
            processesAreStarted = false;
            processesAreStarting = false;
        }


    }
}

