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
        var created = Logic.EquityDomain.PerformanceSharesTemplateModule.createPerformanceSharesTemplateValidated(
            Guid.NewGuid(),
            request.Dto.Name,
            request.Dto.Type,
            request.Dto.AllocationReason,
            request.Dto.Amount,
            request.Dto.Currency);

        if (created.IsError)
        {
            var errors = created.ErrorValue.Select(er => er.ToString()).ToList();
            return Validation<string, Guid>.Fail(new Seq<string>(errors));
        }
        
        var commandEnvelope = Logic.EquityDomain.API.Command.createPerformanceSharesTemplate(created.ResultValue, created.ResultValue.EquityPlanId);
        var result = await Logic.EquityDomain.CommandHandler.sendCommand(commandEnvelope);
        if (result.IsError)
        {
            var errors = result.ErrorValue.Select(x => x.ToString()).ToList();
            return Validation<string, Guid>.Fail(new Seq<string>(errors));
        }

        return Validation<string, Guid>.Success(created.ResultValue.EquityPlanId.Item);
    }
}