namespace eShop.Domain.Shared

open System

type CommandMetadata =
    { CausationId: Guid
      CorrelationId: Guid }

type Command<'data> =
    { Data: 'data
      Id: Guid
      Metadata: CommandMetadata }

module Command =

    let create data =
        let id = Guid.NewGuid()
        { Data = data
          Id = id
          Metadata =
              { CausationId = id
                CorrelationId = id } }
