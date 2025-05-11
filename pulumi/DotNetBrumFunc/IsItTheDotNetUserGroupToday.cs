using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Scriban;


namespace DotNetBrumFunc
{
    

    public class IsItTheDotNetUserGroupToday
    {
        private readonly ILogger<IsItTheDotNetUserGroupToday> _logger;

        public IsItTheDotNetUserGroupToday(ILogger<IsItTheDotNetUserGroupToday> logger)
        {
            _logger = logger;
        }

        [Function("IsItTheDotNetUserGroupToday")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{ignored:maxlength(0)?}")] HttpRequest req)
        {
            _logger.LogInformation("Request made");
            
            var template = Template.Parse(@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                <title>
                    {{
                        if is_today
                           'YEP'
                        else 
                            'NOPE'
                        end
                    }}
                </title>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css' rel='stylesheet'>
            </head>
            <body class='bg-dark d-flex justify-content-center align-items-center min-vh-100'>
                <div class='text-center'>
                    <h4 class='text-white'>Is it the Birmingham .NET User Group group today?</h4>
                    <h1 class='text-white display-1'>{{
                        if is_today
                            'YEP'
                        else 
                            'NOPE'
                        end
                    }}</h1>
                    <p class='text-white mt-4'>
                        For the next event check here: 
                        <a href='https://www.meetup.com/birmingham-dotnet-and-xamarin-user-group/events/calendar/' 
                           class='text-info'>Birmingham .NET User Group</a>
                    </p>
                </div>
            </body>
            </html>
            ");

            
            
            // Dot net user group is on the 2nd Wednesday of the month
           var response = template.Render(new {
                   is_today = functionIsNthWeekdayOfMonth(DayOfWeek.Wednesday, 2)
               });
           
            
            
           return new ContentResult{ Content = response, ContentType = "text/html", StatusCode = StatusCodes.Status200OK };

        }

        protected bool functionIsNthWeekdayOfMonth(DayOfWeek dayOfWeek, int nthWeekdayOfMonth)
        {
            // Check today is a wednesday
            DateTime today = DateTime.Today;
            if (today.DayOfWeek != dayOfWeek)
            {
                return false;
            }

            int occourenceInMonth = ((today.Day - 1) / 7) + 1;
            return occourenceInMonth == nthWeekdayOfMonth;
        }
    }
}
