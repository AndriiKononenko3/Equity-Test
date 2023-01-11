module Commands.CreateEquityPlanCommand

type Command = {
    Name : string
    Type : string
    AllocationReason : string
    Amount : decimal
    Currency : string
}

type Result = bigint