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
        var equityResult = await _equityRepository.GetAsync(request.Id);

        return equityResult.Match(
            equity =>
            {
                var domainObject = EquityPlanTemplateMapping.toEquityPlan(equity);
                if (!domainObject.IsError)
                {
                    return Validation<string, EquityPlanTemplateDto>.Success(EquityPlanTemplateMapping.fromEquityPlan(domainObject.ResultValue));
                }

                var errors = domainObject.ErrorValue.Select(DomainError.getErrorMsg).ToList();
                return Validation<string, EquityPlanTemplateDto>.Fail(new Seq<string>(errors));
            },
            () => Validation<string, EquityPlanTemplateDto>.Fail(new Seq<string>(new []{ ApplicationErrors.NotFoundError(nameof(EquityPlanTemplateDto), request.Id.ToString()) })));
    }
}