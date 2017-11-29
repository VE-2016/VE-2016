using LibGit2Sharp;
using System;
using System.IO;
using VSProvider;

namespace WinExplorer.TeamExplorers
{
    public class GitSharper
    {
        public static string InitRepository(string repoPath)
        {
            //string repoPath = "F:\\ve\\Live-Charts-master";

            var r = Repository.Init(repoPath);

            return r;
        }

        public void AddVSSolution(VSSolution vs)
        {
            string repoPath = Path.GetDirectoryName(vs.solutionFileName);

            using (var repo = new Repository(repoPath))
            {
                foreach (VSProject vp in vs.projects)
                {
                    var compileItems = vp.GetCompileItems();
                    foreach (string file in compileItems)
                    {
                        repo.Index.Stage(file);
                    }
                }

                var author = new Signature("author.Name", "author.Email", DateTime.Now);

                const string shortMessage = "Initial VE-Explorer commit";
                const string commitMessage = shortMessage + "\n\nVE-Explorer";

                Commit commit = repo.Commit(commitMessage, author, author);

                Branch firstCommitBranch = repo.Branches["master"];
                repo.Checkout(firstCommitBranch);
            }
        }
    }
}