using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitUtility
{
    internal class Statistics
    {
        public static HashSet<string> TotalContributors(List<Contribution> contributions)
        {
            HashSet<string> contributors;
            contributors = contributions.Select(commit => commit.Email).ToHashSet();
            return contributors;
        }

        public static List<Contributor> TopContributors(List<Contribution> contributions, int topN)
        {
            List<Contributor> topContributors;
            topContributors = Contributor.SortContributorsByCommits(contributions)
                                    .Take(topN)
                                    .ToList();
             return topContributors;
        }

        public static int RankOfTheUser(List<Contribution> contributions, string email)
        {
            List<Contributor> contributors;
            contributors = Contributor.SortContributorsByCommits(contributions).ToList();

            int rank = contributors.FindIndex(contributor => contributor.Email == email);
            return rank != -1 ? rank + 1 : -1;
        }
    }
}