namespace API.Utilities.ViewModels;

public record SingleResponseVM<TEntity>(
    int Code,
    string Status,
    string Message,
    TEntity Data);
