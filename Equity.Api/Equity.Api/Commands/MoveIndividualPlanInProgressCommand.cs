using Equity.Domain;
using Equity.IndividualEquityPlan;
using LanguageExt;
using MediatR;

namespace Equity.Api.Commands;

public record MoveIndividualPlanInProgressCommand
{
    public record Request() : IRequest<Validation<string, Guid>>;
}

public class MoveIndividualPlanInProgressCommandHandler : IRequestHandler<MoveIndividualPlanInProgressCommand.Request, Validation<string, Guid>>
{
    public async Task<Validation<string, Guid>> Handle(MoveIndividualPlanInProgressCommand.Request request, CancellationToken cancellationToken)
    {
        var individualPlanDto = new IndividualEquityPlanDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            400,
            100,
            20,
            Guid.NewGuid(),
            Status.NotStarted);

        var draftPlanResult = IndividualEquityPlanMapping.toIndividualEquityPlanDraft(individualPlanDto);
        if (draftPlanResult.IsError)
        {
            var errors = draftPlanResult.ErrorValue.Select(DomainError.getErrorMsg).ToList();
            return Validation<string, Guid>.Fail(new Seq<string>(errors));
        }
        
        var inProgressPlanResult = Implementation.IndividualPlanLogic.moveIndividualPlanInProgress(draftPlanResult.ResultValue);
        if (draftPlanResult.IsError)
        {
            var errors = draftPlanResult.ErrorValue.Select(DomainError.getErrorMsg).ToList();
            return Validation<string, Guid>.Fail(new Seq<string>(errors));
        }
        
        throw new NotImplementedException();
    }
}