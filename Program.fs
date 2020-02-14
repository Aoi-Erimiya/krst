open System

type Color = Yellow | Red | Blue | Green | Purple

type Card(color:Color, number:int) =
    let cardColor = color
    let cardNumber = number

    member x.Color with get() = cardColor
    member x.Number with get() = cardNumber

    override x.ToString() = sprintf "%A%d" cardColor cardNumber

type CardHolder(card:Card) = 
    let card = card
    let isOpen = false
    let isKnown = false

    member x.Card with get() = card
    member x.IsOpen with get() = isOpen
    member x.IsKnown with get() = isKnown

type Player(hideCard:Card, handCards:Card list, askedHitCards:Card list, askedNoHitCards:Card list) =
    let hideCard = hideCard
    let handCards = handCards
    let askedHitCards = askedHitCards
    let askedNoHitCards = askedNoHitCards

    member x.HideCard with get() = hideCard
    member x.HandCards with get() = handCards
    member x.AskedHitCards with get() = askedHitCards
    member x.AskedNoHitCards with get() = askedNoHitCards

    override x.ToString() = sprintf "%A/%A/%A/%A" hideCard handCards askedHitCards askedNoHitCards

let buildCardList numberOfPlayers = [
    match numberOfPlayers with
    | 3 ->
        for i in 1..7 do
            yield Card(Red, i)
            yield Card(Blue, i)
            yield Card(Green, i)
    | 4 ->
        for i in 1..7 do
            yield Card(Red, i)
            yield Card(Blue, i)
            yield Card(Green, i)
            yield Card(Yellow, i)
    | 5
    | 6 ->
        for i in 1..7 do
            yield Card(Red, i)
            yield Card(Blue, i)
            yield Card(Green, i)
            yield Card(Yellow, i)
            yield Card(Purple, i)
    | _ ->
        failwithf "Set the number of players between 3 and 6!"
  ]

let isKnown (cardA:Card) (cardB:Card) =
  cardA.Color = cardB.Color || cardA.Number = cardB.Number

let rebuildAskedHitPlayer (player:Player) askCard =
    let askedAfterHitCards = List.append player.AskedHitCards [askCard]
    Player(player.HideCard, player.HandCards, askedAfterHitCards, player.AskedNoHitCards)

let rebuildAskedNoHitPlayer (player:Player) askCard =
    let askedAfterNoHitCards = List.append player.AskedNoHitCards [askCard]
    Player(player.HideCard, player.HandCards, player.AskedHitCards, askedAfterNoHitCards)

let selectAskCard (player:Player) =
    player.HandCards |> List.item (Random().Next(0, player.HandCards.Length))

let removeAskCardFromHandCards (player:Player) askCard =
    player.HandCards |> List.except [askCard]

let buildPlayer (handCards:Card list) =
    Player(handCards.Head, handCards.Tail, [], [])

let rebuildAskPlayer (player:Player) askAfterHandCards =
    Player(player.HideCard, askAfterHandCards, player.AskedHitCards, player.AskedNoHitCards)

let duel (turnPlayer:Player) (otherPlayers:Player list) =
  let askCard = selectAskCard turnPlayer
  let retPlayer = rebuildAskPlayer turnPlayer (removeAskCardFromHandCards turnPlayer askCard)

  let enemy = otherPlayers.Head

  let retEnemy = 
    if isKnown askCard enemy.HideCard then
      rebuildAskedHitPlayer enemy askCard
    else
      rebuildAskedNoHitPlayer enemy askCard

  List.append otherPlayers.Tail [retPlayer] |> List.append [retEnemy]

let printPlayers players =
  players |> List.iter(fun x -> printfn "%A" x)

let rec play (players:Player list) =
    printfn "---"

    let retPlayers = duel players.Head players.Tail
    printPlayers retPlayers

    if retPlayers.Head.HandCards.Length = 0 then
        ()
    else
        play retPlayers

let main =
    printfn "*** FBK-START ***"

    let numberOfPlayers = 4
    let cards = (buildCardList numberOfPlayers)

    let players =
        cards
        |> List.sortBy(fun _ -> Guid.NewGuid())
        |> List.chunkBySize (cards.Length / numberOfPlayers) 
        |> List.map(fun cards -> (buildPlayer cards))

    printPlayers players

    play players

    printfn "*** FBK-END ***"

main
