open System

type Color = Yellow | Red | Blue | Green | Purple

type Card(color:Color, number:int) =
    let cardColor = color
    let cardNumber = number

    member x.Color with get() = cardColor
    member x.Number with get() = cardNumber

    override x.ToString() = sprintf "%A:%d" cardColor cardNumber

type CardHolder(card:Card) = 
    let card = card
    let isOpen = false
    let isKnown = false

    member x.Card with get() = card
    member x.IsOpen with get() = isOpen
    member x.IsKnown with get() = isKnown

type Player(hideCard:Card, handCards, askedHitCards, askedNoHitCards) =
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

let isKnown(cardA:Card, cardB:Card) =
  cardA.Color = cardB.Color || cardA.Number = cardB.Number

let rebuildAskedHitPlayer(player:Player, askCard) =
    let askedAfterHitCards = List.append player.AskedHitCards [askCard]
    Player(player.HideCard, player.HandCards, askedAfterHitCards, player.AskedNoHitCards)

let rebuildAskedNoHitPlayer(player:Player, askCard) =
    let askedAfterNoHitCards = List.append player.AskedNoHitCards [askCard]
    Player(player.HideCard, player.HandCards, player.AskedHitCards, askedAfterNoHitCards)

let selectAskCard(player:Player) =
   player.HandCards
    |> List.sortBy(fun _ -> Guid.NewGuid())
    |> List.head

let removeAskCardFromHandCards(player:Player, askCard) =
  let askCardIndex = 
    (player.HandCards
      |> List.tryFindIndex(fun x -> isKnown(askCard, x))).Value

  List.append player.HandCards.[.. askCardIndex] player.HandCards.[askCardIndex + 1 ..]

let rebuildAskPlayer(player:Player, askAfterHandCards) =
    Player(player.HideCard, askAfterHandCards, player.AskedHitCards, player.AskedNoHitCards)

let phase(player:Player, enemy:Player) =
  let askCard = selectAskCard(player)
  let retPlayer = rebuildAskPlayer(player, removeAskCardFromHandCards(player, askCard))

  let retEnemy = 
    if isKnown(askCard, enemy.HideCard) then
      rebuildAskedHitPlayer(enemy, askCard)
    else
      rebuildAskedNoHitPlayer(enemy, askCard)

  (retPlayer, retEnemy)

let printPlayers p1 p2 p3 =
  [p1; p2; p3] |> List.iter(fun x -> printfn "%A" x)

printfn "*** FBK-START ***"

let shuffledCards =
  cardList
  |> List.sortBy(fun _ -> Guid.NewGuid())

let chunkedCards = shuffledCards |> List.chunkBySize 7

let mutable p1 = Player(chunkedCards.[0].Head, chunkedCards.[0].Tail, [], [])
let mutable p2 = Player(chunkedCards.[1].Head, chunkedCards.[1].Tail, [], [])
let mutable p3 = Player(chunkedCards.[2].Head, chunkedCards.[2].Tail, [], [])

printPlayers p1 p2 p3

let fst, snd = phase(p1, p2)
p1 <- fst
p2 <- snd

let snd2, thd = phase(p2, p3)
p2 <- snd2
p3 <- thd

let thd2, fst2  = phase(p3, p1)
p3 <- thd2
p1 <- fst2

printPlayers p1 p2 p3

// players |> List.iter(fun x -> printfn "%A" x)


printfn "*** FBK-END ***"
