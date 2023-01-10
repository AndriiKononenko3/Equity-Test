module Queries.EquityPlanById

type Query = bigint

type Result = {
    Id : int64
    Name : string
    Type : string
    AllocationReason : string
    EquityValue : decimal
    EquityCurrency : string
}