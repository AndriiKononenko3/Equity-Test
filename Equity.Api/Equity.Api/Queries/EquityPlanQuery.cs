using Equity.Api.Common;
using Equity.Domain;
using Equity.Persistence;
using LanguageExt;
using MediatR;

namespace Equity.Api.Queries;

public class EquityPlanQuery
{
    public record Request(Guid Id) : IRequest<Validation<string, EquityPlanDto>>;
}

public class EquityPlanQueryHandler : IRequestHandler<EquityPlanQuery.Request, Validation<string, EquityPlanDto>>
{
    private readonly EquityRepository _equityRepository = new ();
    
    public async Task<Validation<string, EquityPlanDto>> Handle(
        EquityPlanQuery.Request request,
        CancellationToken cancellationToken)
    {
        var equityResult = await _equityRepository.GetAsync(request.Id);

        return equityResult.Match(
            equity =>
            {
                var domainObject = EquityPlanMapping.toEquityPlan(equity);
                if (!domainObject.IsError)
                {
                    return Validation<string, EquityPlanDto>.Success(EquityPlanMapping.fromEquityPlan(domainObject.ResultValue));
                }

                var errors = domainObject.ErrorValue.Select(DomainError.getErrorMsg).ToList();
                return Validation<string, EquityPlanDto>.Fail(new Seq<string>(errors));
            },
            () => Validation<string, EquityPlanDto>.Fail(new Seq<string>(new []{ ApplicationErrors.NotFoundError(nameof(EquityPlanDto), request.Id.ToString()) })));
    }
}