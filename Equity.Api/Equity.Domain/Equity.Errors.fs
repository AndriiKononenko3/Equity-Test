namespace Equity.Domain

type DomainError =
    | PlanNameNotEmpty of string
    | EquityPlanIdNotEmpty of string
    | PlanTypeDoesNotExist of string
    | AllocationReasonDoesNotExist of string
    | DecimalValueExceedsLimit of string
    | NotValidFiatCurrency of string