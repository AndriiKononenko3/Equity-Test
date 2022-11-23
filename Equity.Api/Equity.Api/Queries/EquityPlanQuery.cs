using Equity.Domain;
using LanguageExt;
using MediatR;

namespace Equity.Api.Queries;

public class EquityPlanQuery
{
    public record Request(Guid Id) : IRequest<Validation<string, Logic.EquityDomain.EquityPlanTemplateDto>>;
}

public class EquityPlanQueryHandler : IRequestHandler<EquityPlanQuery.Request, Validation<string, Logic.EquityDomain.EquityPlanTemplateDto>>
{
    public async Task<Validation<string, Logic.EquityDomain.EquityPlanTemplateDto>> Handle(
        EquityPlanQuery.Request request,
        CancellationToken cancellationToken)
    {
        var equityResult = await Logic.EquityDomain.API.Query.performanceSharesTemplate(Logic.EquityDomain.EquityPlanIdModule.createUnsafe(request.Id));
        
        if (equityResult.IsError)
        {
            var errors = new List<string>{ equityResult.ErrorValue };
            return Validation<string, Logic.EquityDomain.EquityPlanTemplateDto>.Fail(new Seq<string>(errors));
        }

        throw new ArgumentException();
    }
}