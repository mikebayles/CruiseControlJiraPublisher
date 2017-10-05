using System;
using System.Collections.Generic;
using System.Linq;
using Exortech.NetReflector;
using ThoughtWorks.CruiseControl.Core;

namespace JiraPublisher
{
    [ReflectorType("jiraPublisher")]
    public class JiraPublisher
    {
        [ReflectorProperty("jiraUrl")]
        public string JiraUrl { get; set; }
        [ReflectorProperty("jiraUsername")]
        public string JiraUsername { get; set; }
        [ReflectorProperty("jiraPassword")]
        public string JiraPassword { get; set; }

        private HashSet<string> _affectedIssues = new HashSet<string>();
        public void Run(IIntegrationResult result)
        {
            if (result.Succeeded)
            {
                foreach (string comment in result.Modifications.Select(m => m.Comment))
                {
                    _affectedIssues.UnionWith(new JiraIssueFinder(comment).FindIssues());
                }

                foreach (string issue in _affectedIssues)
                {
                    try
                    {
                        new JiraWebRequest(JiraUrl, JiraUsername, JiraPassword).AddCommentToIssue(issue, $"Fixed in Build {result.Label}");
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine("Could not add comment for issue {0}:\r\n{1}", issue, e);
                    }
                }
            }
        }
    }
}
