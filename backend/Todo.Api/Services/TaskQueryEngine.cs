namespace Todo.Api.Services
{
    public class TaskQueryEngine : ITaskQueryEngine
    {
        public IQueryable<Models.Task> ApplyFilters(IQueryable<Models.Task> query, List<string>? tags)
        {
            if (tags == null || tags.Count == 0) return query;
            return query.Where(t => t.Tags.Any(tag => tags.Contains(tag)));
        }

        public IEnumerable<Models.Task> ApplyOrdering(IEnumerable<Models.Task> tasks, string? title)
        {
            if (string.IsNullOrWhiteSpace(title)) return tasks;
            return tasks.OrderBy(t => WeightedEditDistance(t.Title, title));
        }

        // Damerau-Levenshtein algorithm with custom weights for insertions, deletions, substitutions, and transpositions
        public static int WeightedEditDistance(string source, string target, int insertCost = 1, int deleteCost = 2, int substituteCost = 1, int transposeCost = 1)
        {
            if (string.IsNullOrEmpty(source)) return target.Length * insertCost;
            if (string.IsNullOrEmpty(target)) return source.Length * deleteCost;

            int[,] dp = new int[source.Length + 2, target.Length + 2];
            dp[0, 0] = 0;

            for (int i = 0; i <= source.Length; i++) dp[i + 1, 1] = i * deleteCost;
            for (int j = 0; j <= target.Length; j++) dp[1, j + 1] = j * insertCost;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (source[i - 1] == target[j - 1]) ? 0 : substituteCost;

                    // Min(substitution, insertion, deletion)
                    dp[i + 1, j + 1] = Math.Min(dp[i, j] + cost, Math.Min(dp[i + 1, j] + insertCost, dp[i, j + 1] + deleteCost));

                    // Damerau transposition
                    if (i > 1 && j > 1 && source[i - 1] == target[j - 2] && source[i - 2] == target[j - 1])
                    {
                        dp[i + 1, j + 1] = Math.Min(dp[i + 1, j + 1], dp[i - 1, j - 1] + transposeCost);
                    }
                }
            }

            return dp[source.Length + 1, target.Length + 1];
        }
    }
}
