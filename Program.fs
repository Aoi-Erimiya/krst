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

let cardList = [
  for i in 1..7 do
    yield Card(Red, i)
    yield Card(Blue, i)
    yield Card(Green, i)
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

let phase (player:Player) (enemy:Player) =
  let askCard = selectAskCard player
  let retPlayer = rebuildAskPlayer player (removeAskCardFromHandCards player askCard)

  let retEnemy = 
    if isKnown askCard enemy.HideCard then
      rebuildAskedHitPlayer enemy askCard
    else
      rebuildAskedNoHitPlayer enemy askCard

  (retPlayer, retEnemy)

let printPlayers player1 player2 player3 =
  [player1; player2; player3] |> List.iter(fun x -> printfn "%A" x)

let rec play player1 player2 player3 =
    printfn "---"
    let fst, snd = phase player1 player2
    let snd2, thd = phase snd player3
    let thd2, fst2  = phase thd fst

    printPlayers fst2 snd2 thd2
    if thd2.HandCards.Length = 0 then
        ()
    else
        play fst2 snd2 thd2
     

printfn "*** FBK-START ***"

let shuffledCards =
  cardList
  |> List.sortBy(fun _ -> Guid.NewGuid())

let chunkedCards = shuffledCards |> List.chunkBySize 7 
let player1 = buildPlayer chunkedCards.[0]
let player2 = buildPlayer chunkedCards.[1]
let player3 = buildPlayer chunkedCards.[2]

printPlayers player1 player2 player3

play player1 player2 player3

printfn "*** FBK-END ***"
