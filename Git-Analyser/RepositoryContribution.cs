namespace GitUtility
{
    public class RepositoryContribution
    {
        public string RepositoryPath { get; set; }
        public List<Contribution> Contributions { get; set; } = new List<Contribution>();
    }
}