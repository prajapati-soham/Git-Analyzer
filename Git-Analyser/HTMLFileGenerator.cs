using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;

namespace GitUtility
{
    public static class HTMLFileGenerator
    {
        public static void GenerateRepositoryHTML(List<RepositoryContribution> allReposContributions, string email)
        {
            foreach (RepositoryContribution repoContribution in allReposContributions)
            {
                string outputPath = $"{repoContribution.RepositoryPath}.html";

                string repoName = Path.GetFileName(repoContribution.RepositoryPath);
                string html = GenerateHTMLContent(repoContribution.RepositoryPath, repoContribution.Contributions, email);

                File.WriteAllText(outputPath, html);

                Console.WriteLine($"HTML file for {repoContribution.RepositoryPath} is generated: {outputPath}");
            }
        }

        public static void GenerateAllRepositoryHTML(List<RepositoryContribution> allReposContributions, string email)
        {
            string currentDirectory = Environment.CurrentDirectory;
            string outputPath = Path.Combine(currentDirectory, "all-repositories.html");

            var allContributions = allReposContributions
                .SelectMany(repoContribution => repoContribution.Contributions)
                .ToList();
            string html = GenerateHTMLContent("All Repositories", allContributions, email);

            File.WriteAllText(outputPath, html);

            Console.WriteLine($"HTML file for all repositories is generated: {outputPath}");
        }

        private static string GenerateHTMLContent(string title, List<Contribution> contributions, string email)
        {         
            string style = AddStyling();

            string html = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <title>{title}</title>
                    <style>{style}</style>
                </head> 
                <body>
                    <div class='container'>
                        <h1>Repository: {title}</h1>";

            DateTime endDate = DateTime.Now;
            DateTime startDate = endDate.AddYears(-1);
            html += GenerateContributionGraph(contributions, email, startDate, endDate);

            html += CalculateAndDisplayStatistics(contributions, email);

            html += @"</div> </body> </html>";

            return html;
        }
        private static string AddStyling()
        {
            return $@"body {{
                        font-family: 'Arial', sans-serif;
                        line-height: 1.6;
                        background-color: #f6f8fa;
                        color: #24292e;
                        margin: 0;
                        padding: 20px;
                     }}
                     .container {{
                        max-width: 1200px;
                        margin: 0 auto;
                        display: flex;
                        justify-content: center;
                        align-items: center;
                        flex-direction: column;
                     }}
                     h1 {{
                        font-size: 2em;
                        margin-bottom: 20px;
                    }}
                    .graph-container {{
                        width: 920px;
                        border: 1px solid #d1d5da;
                        border-radius: 6px;
                        padding: 20px;
                        background-color: #ffffff;
                        box-shadow: 0 1px 3px rgba(27, 31, 35, 0.12);
                        margin-bottom: 40px;
                        position: relative;
                     }}
                     .total-contributions {{
                        position: absolute;
                        top: -15px;
                        left: 20px;
                        background-color: #ffffff;
                        padding: 0 10px;
                        font-weight: bold;
                        border: 1px solid #d1d5da;
                        border-radius: 4px;
                        box-shadow: 0 1px 3px rgba(27, 31, 35, 0.12);
                     }}
                     .month-labels {{
                         display: flex;
                         width: 870px;
                         margin-left: 15px;
                         margin-bottom: 10px;
                     }}
                     .month-label {{
                         width: calc(100% / 12);
                         text-align: center;
                         color: #6a737d;
                     }}
                     .contribution-graph {{
                         display: flex;
                         flex-wrap: wrap;
                         width: 935px;
                      }}
                     .day-label {{
                         display: flex;
                         flex-direction: column;
                         width: 30px;
                         text-align: right;
                         font-size: 10px;
                         padding-right: 5px;
                         color: #6a737d;
                     }}
                    .week {{
                        display: flex;
                        flex-direction: column;
                        margin-right: 2px;
                    }}
                   .day {{
                       width: 13px;
                       height: 13px;
                       background-color: #ebedf0;
                       border: 1px solid #fff;
                       border-radius: 4px;
                       margin-bottom: 2px;
                       transition: background-color 0.3s;
                   }}
                   .day:hover {{
                       border: 1px solid #0366d6;
                       cursor: pointer;
                   }}
                  .legend {{
                      display: flex;
                      float: right;
                      margin-right: 10px;
                      margin-top: 2px;
                      align-items: center;
                      font-size: 14px;
                      color: #6a737d;
                  }}
                  .legend span {{
                      width: 13px;
                      height: 13px;
                      margin: 0px 2px;
                      border-radius: 2px;
                  }}
                  .statistics-container {{
                      display: inline;
                      justify-content: space-between;
                      gap: 20px;
                      flex-wrap: wrap;
                  }}
                  .statistics {{
                       background - color: #ffffff;
                       border: 1px solid #d1d5da;
                       border-radius: 6px;
                       padding: 20px;
                       box-shadow: 0 1px 3px rgba(27, 31, 35, 0.12);
                       width: 920px;
                       margin-bottom: 40px;
                       background-color: white;
                  }}
                  .statistics h2 {{
                      font - size: 1.5em;
                      margin-bottom: 10px;
                      border-bottom: 2px solid #0366d6;
                      padding-bottom: 5px;
                  }}
                 .statistics ul {{
                     list-style: none;
                     padding: 0;
                 }}
                 .statistics li {{
                     margin-bottom: 10px;
                     font-size: 1.1em;
                 }}
                 .statistics ol {{
                     padding-left: 20px;
                 }}
                 .statistics ol li {{
                     margin-bottom: 5px;
                 }}
                 .stat-item {{
                     display: flex;
                     justify-content: space-between;
                     align-items: center;
                 }}
                 .stat-label {{
                     font-weight: bold;
                     color: #586069;
                 }}
                 .stat-value {{
                     color: #24292e;
                 }}";
        }

        private static string GenerateContributionGraph(List<Contribution> totalContributions, string email, DateTime startDate, DateTime endDate)
        {
            int lastYearContributions = Contribution.GetContributionOfLastYear(totalContributions, email);
            string html = $@"<div class='graph-container'>
                                <div class='total-contributions'>{lastYearContributions} contribution{(lastYearContributions > 1 ? "s" : "")} in the last year</div>
                                <div class= 'month-labels'>";

            for (int i = 0; i < 12; i++)
            {
                int month = startDate.Month + i;
                if (month > 12) month %= 12;
  
                string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month).Substring(0, 3);
                html += $"<div class='month-label'>{monthName}</div>";
            }

            html += @"</div>
                      <div class='contribution-graph'>
                        <div class='day-label'>
                            <div>&nbsp;</div>
                            <div style='margin-top: -2px'>Mon</div>
                            <div>&nbsp;</div>
                            <div style='margin-top: -2.5px'>Wed</div>
                            <div>&nbsp;</div>
                            <div style='margin-top: -3px'>Fri</div>
                            <div>&nbsp;</div>
                        </div>";

            DateTime date = startDate;
            for (int week = 0; week < 53; week++)
            {
                html += "<div class='week'>";

                int numberOfemptyDivs = ((int)date.DayOfWeek);

                if (week == 0 && date.ToString("dddd") != "Saturday")
                    for (int i = 0; i < numberOfemptyDivs; i++)
                        html += $"<div class='day' style='background-color: white'></div>";

                for (int day = 0; day < 7; day++)
                {
                    if (week == 0 && day < numberOfemptyDivs) continue;

                    if (date <= endDate)
                    {
                        int contributions = totalContributions
                            .Where(contribution => contribution.Email == email && contribution.Date.Date == date.Date)
                            .Sum(contribution => contribution.Count);

                        string color = GetContributionColor(contributions);
                        string monthName = date.ToString("MMMM", CultureInfo.InvariantCulture);
                        string dayOfMonth = date.ToString("dd");
                        string tooltip = $"{contributions} contribution {(contributions != 1 ? "s" : "")} on {monthName} {dayOfMonth}";

                        html += $"<div class='day' style='background-color: {color}' title='{tooltip}'></div>";
                    }
                    date = date.AddDays(1);
                }
                html += "</div>";
            }

            html += @"</div> 
                       <div class='legend'>
                            Less
                            <span style='background-color: #ebedf0'></span>
                            <span style='background-color: #9be9a8'></span>
                            <span style='background-color: #40c463'></span>
                            <span style='background-color: #30a14e'></span>
                            <span style='background-color: #216e39'></span>
                            More
                        </div>
                    </div>";

            return html;
        }

        private static string GetContributionColor(int contributions)
        {
            if (contributions == 0)
                return "#ebedf0";
            else if (contributions <= 5)
                return "#9be9a8";
            else if (contributions <= 10)
                return "#40c463";
            else if (contributions <= 15)
                return "#30a14e";
            else
                return "#216e39";
        }

        private static string CalculateAndDisplayStatistics(List<Contribution> contributions, string email)
        {
            var currentYearContributions = contributions.Where(contribution => contribution.Date.Year == DateTime.Now.Year).ToList();

            int currentYearContributors = Statistics.TotalContributors(currentYearContributions).Count;
            int lifetimeContributors = Statistics.TotalContributors(contributions).Count;
            List<Contributor> topCurrentYearContributors = Statistics.TopContributors(currentYearContributions, 2);
            List<Contributor> topLifetimeContributors = Statistics.TopContributors(contributions, 2);
            int userCurrentYearRank = Statistics.RankOfTheUser(currentYearContributions, email);
            int userLifetimeRank = Statistics.RankOfTheUser(contributions, email);

            string html = $@"
                <div class=""statistics-container"">
                    <div class=""statistics"">
                    <h2>Statistics (Current Year)</h2>
                    <ul>
                        <li class=""stat-item"">
                            <span class=""stat-label"">Number of Contributors:</span>
                            <span class=""stat-value"">{currentYearContributors}</span>
                        </li>
                        <li class=""stat-item"">
                            <span class=""stat-label"">Top 2 Contributors:</span>
                            <span class=""stat-value"">
                                <ol>";

            foreach (Contributor contributor in topCurrentYearContributors)
                html += $"<li>{contributor.Email} - {contributor.ContributionCount} contribution{(contributor.ContributionCount > 1 ? "s" : "")}</li>";

            html += $@"   </ol>
                        </span>
                    </li>
                    <li class=""stat-item"">
                        <span class=""stat-label"">Your Rank:</span>
                        <span class=""stat-value"">{userCurrentYearRank}</span>
                    </li>
                </ul>
            </div>
            <div class=""statistics"">
                <h2>Statistics (Lifetime)</h2>
                <ul>
                    <li class=""stat-item"">
                        <span class=""stat-label"">Number of Contributors:</span>
                        <span class=""stat-value"">{lifetimeContributors}</span>
                    </li>
                    <li class=""stat-item"">
                        <span class=""stat-label"">Top 2 Contributors:</span>
                        <span class=""stat-value"">
                            <ol>";
            foreach (Contributor contributor in topLifetimeContributors)
            {
                html += $"<li>{contributor.Email} - {contributor.ContributionCount} contribution{(contributor.ContributionCount > 1 ? "s" : "")}</li>";
            }
            html += $@"             </ol>
                                </span>
                            </li>
                            <li class=""stat-item"">
                                <span class=""stat-label"">Your Rank:</span>
                                <span class=""stat-value"">{userLifetimeRank}</span>
                            </li>
                        </ul>
                    </div>
                </div>";

            return html;
        }
    }
}