using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitUtility
{
    public class Contributor
    {
        public string Email { get; set; }
        public int ContributionCount { get; set; }

        public static IEnumerable<Contributor> SortContributorsByCommits(List<Contribution> contributions)
        {
            IEnumerable<Contributor> contributors;
            contributors = contributions
                               .GroupBy(contribution => contribution.Email)
                               .Select(group => new Contributor
                               {
                                   Email = group.Key,
                                   ContributionCount = group.Sum(contribution => contribution.Count)
                               })
                               .OrderByDescending(contributor => contributor.ContributionCount);
            return contributors;
        }
    }
}