using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JiraPublisher;

namespace JiraPublisherTests
{
    [TestClass]
    public class JiraIssueFinderTests
    {
        [TestMethod]
        public void Test2LetterProjectCodeAtBeginning()
        {
            var issues = new JiraIssueFinder("PH-123: Fixed a bug").FindIssues();
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PH-123", issues.First());
        }

        [TestMethod]
        public void Test2LetterProjectCodeNotAtBeginning()
        {
            var issues = new JiraIssueFinder("Fixed a bug in PH-123").FindIssues();
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PH-123", issues.First());
        }

        [TestMethod]
        public void Test3LetterProjectCodeAtBeginning()
        {
            var issues = new JiraIssueFinder("PDW-45: Fixed a bug").FindIssues();
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PDW-45", issues.First());
        }

        [TestMethod]
        public void Test3LetterProjectCodeNotAtBeginning()
        {
            var issues = new JiraIssueFinder("Fixed a bug in PDW-45").FindIssues();
            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PDW-45", issues.First());
        }

        [TestMethod]
        public void TestMultipleIssuesInCommitMessage()
        {
            var issues = new JiraIssueFinder("PH-123, PDW-456: Fixed 2 bugs").FindIssues();
            Assert.AreEqual(2, issues.Count);

            //Should order since HashSet can't guarantee 
            var ordered = issues.OrderBy(a => a).ToArray();
            Assert.AreEqual("PDW-456", ordered[0]);
            Assert.AreEqual("PH-123", ordered[1]);
        }

        [TestMethod]
        public void TestSameIssueTwiceInMessage()
        {
            var issues = new JiraIssueFinder("PH-123, PH-123: Fixed 2 bugs").FindIssues();

            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PH-123", issues.First());
        }

        [TestMethod]
        public void TestLowerCaseMessage()
        {
            var issues = new JiraIssueFinder("ph-123 fixed a bug").FindIssues();

            Assert.AreEqual(1, issues.Count);
            Assert.AreEqual("PH-123", issues.First());
        }

        [TestMethod]
        public void TestMultipleCommitsMessageOnNewLines()
        {
            string input =
                @"Merged revision(s) 71403, 71421-71422, 71427, 71448, 71451, 71458, 71464-71465 from trunk:
PH-755: Updated some classes 
........
PH-755 addition: Parsing the data
........
PH-755: don't display negative values
........
PH-428: Refactored to use the other API
PH-829: Removed DataStore
PH-814: Handled login failures by retrying
........
PH-XXX Project for Windows work (No story yet) 
- Remove inadvertent commit to trunk. ";
            var issues = new JiraIssueFinder(input).FindIssues();
            Assert.AreEqual(4, issues.Count);

            //Should order since HashSet can't guarantee 
            var ordered = issues.OrderBy(a => a).ToArray();
            Assert.AreEqual("PH-428", ordered[0]);
            Assert.AreEqual("PH-755", ordered[1]);
            Assert.AreEqual("PH-814", ordered[2]);
            Assert.AreEqual("PH-829", ordered[3]);
        }
    }
}