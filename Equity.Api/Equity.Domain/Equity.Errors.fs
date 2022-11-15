namespace Equity.Domain

type DomainError =
    | PlanNameNotEmpty of string
    | EquityPlanIdNotEmpty of string
    | PlanTypeDoesNotExist of string
    | AllocationReasonDoesNotExist of string
    | DecimalValueExceedsLimit of string
    | NotValidFiatCurrency of string
    static member public getErrorMsg (error: DomainError) =
        match error with
        | PlanNameNotEmpty planNameNotEmptyMsg -> planNameNotEmptyMsg
        | EquityPlanIdNotEmpty equityPlanIdNotEmptyMsg -> equityPlanIdNotEmptyMsg
        | PlanTypeDoesNotExist planTypeDoesNotExistMsg -> planTypeDoesNotExistMsg
        | AllocationReasonDoesNotExist allocationReasonDoesNotExistMsg -> allocationReasonDoesNotExistMsg
        | DecimalValueExceedsLimit decimalValueExceedsLimitMsg -> decimalValueExceedsLimitMsg
        | NotValidFiatCurrency notValidFiatCurrencyMsg -> notValidFiatCurrencyMsg
        
type ApplicationError =
    | NotFound of msg:string * id:string
    static member public getErrorMsg (error: ApplicationError) =
        match error with
        | NotFound (msg, id) -> $"%s{msg} %s{id}"