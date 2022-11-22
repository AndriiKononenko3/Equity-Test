using Equity.Api.Common;
using Equity.Domain;
using Equity.Persistence;
using LanguageExt;
using MediatR;

namespace Equity.Api.Queries;

public class EquityPlanQuery
{
    public record Request(Guid Id) : IRequest<Validation<string, EquityPlanTemplateDto>>;
}

public class EquityPlanQueryHandler : IRequestHandler<EquityPlanQuery.Request, Validation<string, EquityPlanTemplateDto>>
{
    private readonly EquityRepository _equityRepository = new ();
    
    public async Task<Validation<string, EquityPlanTemplateDto>> Handle(
        EquityPlanQuery.Request request,
        CancellationToken cancellationToken)
    {
        var equityResult = await Logic.EquityDomain.API.Query.performanceSharesTemplate(Logic.EquityDomain.EquityPlanIdModule.createUnsafe(request.Id));
        
        if (equityResult.IsError)
        {
            var errors = new List<string>{ equityResult.ErrorValue };
            return Validation<string, EquityPlanTemplateDto>.Fail(new Seq<string>(errors));
        }

        throw new ArgumentException();
    }
}