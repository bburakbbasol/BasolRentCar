using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace Rent.WebApi.Filters
{
    public class TimeControllerFilter : IActionFilter
    {
        private readonly ILogger<TimeControllerFilter> _logger;
        private Stopwatch _stopwatch;

        // Başlangıç ve bitiş saatleri tanımlıyoruz
        private readonly TimeSpan _startTime = new TimeSpan(18, 0, 0);  
        private readonly TimeSpan _endTime = new TimeSpan(01, 0, 0);   

        public TimeControllerFilter(ILogger<TimeControllerFilter> logger)
        {
            _logger = logger;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var currentTime = DateTime.Now.TimeOfDay;

            // Belirlenen saat aralığı dışındaysa işlemi iptal et
            if (currentTime < _startTime || currentTime > _endTime)
            {
                context.Result = new ContentResult
                {
                    StatusCode = 403, // Forbidden
                    Content = $"Giriş sadece {_startTime} - {_endTime} saatleri arasında yapılabilir."
                };
                return;
            }

            _stopwatch = Stopwatch.StartNew(); // Zamanlayıcıyı başlat
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (_stopwatch != null)
            {
                _stopwatch.Stop();
                var elapsedMilliseconds = _stopwatch.ElapsedMilliseconds;
                var actionName = context.ActionDescriptor.DisplayName;
                _logger.LogInformation($"Action {actionName} executed in {elapsedMilliseconds} ms");
            }
        }
    }
}
