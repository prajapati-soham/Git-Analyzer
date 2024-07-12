using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using LibGit2Sharp;
using static System.Net.Mime.MediaTypeNames;

namespace GitUtility
{
    internal class GitAnalyzer
    {
        static void Main(string[] args)
        {
            //If you want to take an input from command line.
            /*if (args.Length < 2)
            {
                Console.WriteLine("Usage: GitAnalyzer <root-directory> <email>");
                return;
            }

            int emailIndex = args.Length - 1;
            string email = args[emailIndex];

            string rootDirectory = args[0];
            for (int i = 1; i < emailIndex; i++)
            {
                rootDirectory += " " + args[i];
            }*/

            Console.WriteLine("Enter Root Directory: ");
            string rootDirectory = Console.ReadLine();

            Console.WriteLine("Enter Email id: ");
            string email = Console.ReadLine();

            if (string.IsNullOrEmpty(rootDirectory) || string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Root directory or email cannot be empty.");
                return;
            }

            List<string> repositories = FindGitRepositories(rootDirectory);
            List<RepositoryContribution> contributions = AnalyzeContributions(repositories, email);

            if (contributions.Count > 0)
            {
                HTMLFileGenerator.GenerateRepositoryHTML(contributions, email);
                HTMLFileGenerator.GenerateAllRepositoryHTML(contributions, email);
            }
            else
            {
                Console.WriteLine("No contributions found for the specified email.");
            }
        }

        static List<string> FindGitRepositories(string rootDirectory)
        {
            List<string> repositories = new List<string>();
            try
            {
                foreach (string dir in Directory.GetDirectories(rootDirectory, "*", SearchOption.AllDirectories))
                {
                    if (Directory.Exists(Path.Combine(dir, ".git")))
                    {
                        repositories.Add(dir);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error finding repositories: {ex.Message}");
            }
            return repositories;
        }

        static List<RepositoryContribution> AnalyzeContributions(List<string> repositories, string email)
        {
            List<RepositoryContribution> contributions = new List<RepositoryContribution>();

            foreach (string repoPath in repositories)
            {
                try
                {
                    using (Repository repo = new Repository(repoPath))
                    {
                        RepositoryContribution repoContribution = new RepositoryContribution { RepositoryPath = repoPath };

                        foreach (var commit in repo.Commits)
                        {
                            string authorEmail = commit.Author.Email;
                            DateTime date = commit.Author.When.Date;
                            var contribution = repoContribution.Contributions.Find(c => (c.Date.Date == date.Date && c.Email == authorEmail));

                            if (contribution == null)
                            {
                                contribution = new Contribution { Date = date.Date, Count = 1, Email = authorEmail };
                                repoContribution.Contributions.Add(contribution);
                            }
                            else
                            {
                                contribution.Count++;
                            }
                        }
                        contributions.Add(repoContribution);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error analyzing repository {repoPath}: {ex.Message}");
                }
            }
            return contributions;
        }
    }
}