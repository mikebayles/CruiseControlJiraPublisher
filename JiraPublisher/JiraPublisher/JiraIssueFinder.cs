using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace JiraPublisher
{
    public class JiraIssueFinder
    {
        private const string JiraPattern = @"[A-Za-z]{1,4}-\d{1,4}";

        private readonly string _commitMessage;

        public JiraIssueFinder(string commitMessage)
        {
            _commitMessage = commitMessage;
        }

        public HashSet<string> FindIssues()
        {
            HashSet<string> issues = new HashSet<string>();
            foreach (Match match in Regex.Matches(_commitMessage, JiraPattern))
            {
                issues.Add(match.Value.ToUpper());
            }

            return issues;
        }
    }
}
