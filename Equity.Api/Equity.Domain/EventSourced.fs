namespace EventSourced

type TransactionId =
  | TransactionId of System.Guid
  with
    static member New () =
      TransactionId <| System.Guid.NewGuid()

type EventSource = System.Guid

type EventProducer<'Event> =
  'Event list -> 'Event list

type EventMetadata =
  {
    Source : EventSource
    RecordedAtUtc : System.DateTime
    Transaction : TransactionId
  }

type EventEnvelope<'Event> =
  {
    Metadata : EventMetadata
    Event : 'Event
  }

type EventHandler<'Event> =
  EventEnvelope<'Event> list -> Async<unit>

type EventResult<'Event> =
  Result<EventEnvelope<'Event> list, string>

type EventStore<'Event> =
  {
    Get : unit -> Async<EventResult<'Event>>
    GetStream : EventSource -> Async<EventResult<'Event>>
    Append : EventEnvelope<'Event> list-> Async<Result<unit, string>>
    OnError : IEvent<exn>
    OnEvents : IEvent<EventEnvelope<'Event> list>
  }

type EventListener<'Event> =
  {
    Subscribe : EventHandler<'Event> -> System.Guid
    Unsubscribe : System.Guid -> unit
    Notify : EventEnvelope<'Event> list -> unit
  }

type EventStorage<'Event> =
  {
    Get : unit -> Async<EventResult<'Event>>
    GetStream : EventSource -> Async<EventResult<'Event>>
    Append : EventEnvelope<'Event> list -> Async<unit>
  }

type Projection<'State,'Event> =
  {
    Init : 'State
    Update : 'State -> 'Event -> 'State
  }

type QueryResult =
  | Handled of obj
  | NotHandled
  | QueryError of string

type QueryHandler<'Query> =
  {
    Handle : 'Query -> Async<QueryResult>
  }

type InMemoryReadModel<'Event, 'State> =
  {
    EventHandler : EventHandler<'Event>
    State : unit -> Async<'State>
  }

type CommandEnvelope<'Command> =
  {
    EventSource : EventSource
    Transaction : TransactionId
    Command : 'Command
  }

type CommandHandler<'Command, 'Event> =
  {
    Handle : CommandEnvelope<'Command> -> Async<Result<EventEnvelope<'Event> list,string>>
    HandleBatch : CommandEnvelope<'Command> list -> Async<Result<EventEnvelope<'Event> list,string>>
    OnError : IEvent<exn>
  }

type Behaviour<'Command,'Event> =
  'Command -> EventProducer<'Event>