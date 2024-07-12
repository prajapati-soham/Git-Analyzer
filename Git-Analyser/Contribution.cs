namespace GitUtility
{
    public class Contribution
    {
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public string Email { get; set; }

        public static int GetContributionOfLastYear(List<Contribution> contributions, string email)
        {
            DateTime oneYearAgo = DateTime.Now.AddYears(-1); // Calculates the date exactly one year ago from the current date.

            int lastYearContributions = contributions
                .Where(contribution => contribution.Email == email && contribution.Date >= oneYearAgo)
                .Sum(contribution => contribution.Count);

            return lastYearContributions;
        }
    }
}