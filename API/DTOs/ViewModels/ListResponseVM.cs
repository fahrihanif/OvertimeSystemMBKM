namespace API.Utilities.ViewModels;

public record ListResponseVM<TEntity>(
    int Code,
    string Status,
    string Message,
    IEnumerable<TEntity> Data);
