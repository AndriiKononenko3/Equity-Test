namespace Equity.Api.Common;

public record struct ApplicationError(string Msg);

public class ApplicationErrors
{
    public static string NotFoundError(string entityName, string id)
    {
        var appError = new ApplicationError($"EntityName: {entityName} with id {id} not found");
        return appError.Msg;
    }
}