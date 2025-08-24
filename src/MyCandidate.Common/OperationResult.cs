namespace MyCandidate.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
}

public class OperationResults
{
    public bool Success { get; set; }
    public IEnumerable<string> Messages { get; set; } = Enumerable.Empty<string>();
}
