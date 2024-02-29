namespace API.Utilities.ViewModels;

public record CustomErrorResponseVM(
    int Code,
    string Status,
    string Message,
    string ErrorDetails);
