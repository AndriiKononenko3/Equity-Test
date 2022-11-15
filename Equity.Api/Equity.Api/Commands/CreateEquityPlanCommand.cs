using Equity.Api.RequestModels;
using Equity.Domain;
using LanguageExt;
using MediatR;

namespace Equity.Api.Commands;

public record CreateEquityPlanCommand
{
    public record Request(CreateEquityPlanRequestModel Dto) : IRequest<Validation<string, Guid>>;
}

public class CreateEquityPlanCommandHandler : IRequestHandler<CreateEquityPlanCommand.Request, Validation<string, Guid>>
{
    public async Task<Validation<string, Guid>> Handle(
        CreateEquityPlanCommand.Request request,
        CancellationToken cancellationToken)
    {
        var created = EquityPlanModule.createEquityPlanValidated(
            Guid.NewGuid(),
            request.Dto.Name,
            request.Dto.Type,
            request.Dto.AllocationReason,
            request.Dto.Amount,
            request.Dto.Currency);
        
        if (created.IsError)
        {
            var errors = created.ErrorValue.Select(DomainError.getErrorMsg).ToList();
            return Validation<string, Guid>.Fail(new Seq<string>(errors));
        }
        
        // saves to db
        return Validation<string, Guid>.Success(created.ResultValue.EquityPlanId.Item);
    }
}