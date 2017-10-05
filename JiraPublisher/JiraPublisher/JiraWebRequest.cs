using System.IO;
using System.Net;
using System.Text;

namespace JiraPublisher
{
    public class JiraWebRequest
    {
        private const string CommentUrlFormat = "{0}/rest/api/2/issue/{1}/comment";

        private readonly string _jiraUrl;
        private readonly string _jiraUsername;
        private readonly string _jiraPassword;

        public JiraWebRequest(string jiraUrl, string jiraUsername, string jiraPassword)
        {
            _jiraUrl = jiraUrl;
            _jiraUsername = jiraUsername;
            _jiraPassword = jiraPassword;
        }

        public void AddCommentToIssue(string issue, string comment)
        {
            string jsonSkeleton = "{{ \"body\": \"{0}\" }}";
            var json = string.Format(jsonSkeleton, comment);

            SendRequest(string.Format(CommentUrlFormat, _jiraUrl, issue), json);
        }

        private string SendRequest(string url, string body, string method = "POST")
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            var data = Encoding.ASCII.GetBytes(body);

            request.Method = method;
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            request.Headers[HttpRequestHeader.Authorization] = $"Basic {_jiraUsername}:{_jiraPassword}";
            request.ServicePoint.Expect100Continue = false;

            if (!string.IsNullOrEmpty(body))
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            return responseString;
        }

    }
}
