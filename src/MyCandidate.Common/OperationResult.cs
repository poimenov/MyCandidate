namespace MyCandidate.Common;

public class OperationResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public Exception? Exception { get; set; }
}

public class OperationResults
{
    public bool Success { get; set; }
    public IEnumerable<string> Messages { get; set; } = Enumerable.Empty<string>();
}

public class OperationResult<T> : OperationResult
{
    public T? Result { get; set; }
}
