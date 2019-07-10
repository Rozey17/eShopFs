module eShop.Infrastructure.Bus

open System

type Topic = Topic of string
type SubscriptionId = SubscriptionId of string

type IMessageBus =
    inherit IDisposable
    abstract member Publish<'a when 'a : not struct> : 'a -> Async<unit>
    abstract member TopicPublish<'a when 'a : not struct> : 'a -> Topic -> Async<unit>
    abstract member Subscribe<'a when 'a : not struct> : SubscriptionId -> ('a -> Async<unit>) -> unit
    abstract member TopicSubscribe<'a when 'a : not struct> : SubscriptionId -> Topic -> ('a -> Async<unit>) -> unit

type ISubscriber =
    abstract Action : obj -> Async<unit>
    abstract Type : Type
    abstract Binding : string

type Subscriber<'a> =
    { SubscriptionId: SubscriptionId
      Binding: string
      Action: 'a -> Async<unit> }
    interface ISubscriber with
        member this.Action o =
            o |> unbox<'a> |> this.Action
        member __.Type =
            typeof<'a>
        member this.Binding =
            this.Binding

type Message =
    | Publish of payload:obj * payloadType:Type * topic:Topic option
    | Subscribe of subscriber:ISubscriber
    | Stop of AsyncReplyChannel<unit>

let compareSection (topicSection: string, bindingSection: string) =
    match bindingSection with
    | "#" | "*" -> true
    | _ when bindingSection = topicSection -> true
    | _ -> false

let topicBindingMatch (topic: Topic option) (binding: string) =
    match topic with
    | Some (Topic topic) ->
        let topicSections = topic.Split '.'
        let bindingSections = binding.Split '.'
        if bindingSections.[bindingSections.Length - 1] = "#" then
            Seq.zip topicSections bindingSections
            |> Seq.forall compareSection
        else
            if bindingSections.Length = topicSections.Length then
                Seq.zip topicSections bindingSections
                |> Seq.forall compareSection
            else
                false
    | None ->
        binding = "#"

let rec loop subscribers (exiting: AsyncReplyChannel<unit> option) (agent: MailboxProcessor<Message>) =
    async {
        match exiting with
        | Some channel when agent.CurrentQueueLength = 0 ->
            return channel.Reply()
        | _ ->
            let! msg = agent.Receive()
            match msg with
            | Stop channel ->
                return! loop subscribers (Some channel) agent
            | Subscribe subscriber ->
                return! loop (subscriber::subscribers) exiting agent
            | Publish (message, type', topic) ->
                let matchingSubs =
                    subscribers
                    |> List.filter (fun it -> it.Type = type' && topicBindingMatch topic it.Binding)
                for sub in matchingSubs do
                    sub.Action message |> Async.StartImmediate

                return! loop subscribers exiting agent
    }

type MemoryMessageBus() =
    let agent = MailboxProcessor.Start(loop [] None)
    do agent.Error.Add raise

    interface IDisposable with
        member __.Dispose() =
            agent.PostAndReply Stop

    interface IMessageBus with
        member __.Publish (message: 'a) =
            agent.Post (Publish (box message, typeof<'a>, None))
            async.Zero()
        member __.TopicPublish (message: 'a) topic =
            agent.Post (Publish (box message, typeof<'a>, Some topic))
            async.Zero()
        member __.Subscribe sid action =
            agent.Post (Subscribe { SubscriptionId = sid; Binding = "#"; Action = action })
        member __.TopicSubscribe sid (Topic binding) action =
            agent.Post (Subscribe { SubscriptionId = sid; Binding = binding; Action = action })

let Bus = new MemoryMessageBus() :> IMessageBus
