module eShop.Domain.Shared.Command

open System

type CommandMetadata =
    { CausationId: Guid
      CorrelationId: Guid }

type Command<'data> =
    { Data: 'data
      Id: Guid
      Metadata: CommandMetadata }

let createCommand data =
    let id = Guid.NewGuid()
    { Data = data
      Id = id
      Metadata =
          { CausationId = id
            CorrelationId = id } }
