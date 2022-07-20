using System;

using Bytewizer.TinyCLR.Logging;
using Bytewizer.TinyCLR.Hosting;
using System.Threading;

namespace Bytewizer.TinyCLR.DigitalPortal
{
    public class DisplayWorker : SchedulerService
    {
        private readonly ILogger _logger;
        private readonly MainWindow _mainWindow;
        private int _state;

       
        public DisplayWorker(MainWindow mainWindow, ILoggerFactory loggerFactory)
            : base(0,1,TimeSpan.FromSeconds(30))
        {
            _logger = loggerFactory.CreateLogger(nameof(DisplayWorker));
            _mainWindow = mainWindow;
        }

        public override void Start()
        {
            _logger.HostStarted();

            base.Start();
        }

        protected override void ExecuteAsync()
        {
            if (_state == 0)
            {
                _state = 1;
                _mainWindow.Activate(1, true);
            }
            else
            {
                _state = 0;
                _mainWindow.Activate(0, true);
            }
        }

        public override void Stop()
        {
            _logger.HostStopped();

            base.Stop();
        }
    }
}