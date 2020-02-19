open System

type Color = Yellow | Red | Blue | Green | Purple

type Card(color:Color, number:int) =
    let cardColor = color
    let cardNumber = number

    member x.Color with get() = cardColor
    member x.Number with get() = cardNumber

    override x.ToString() = 
        sprintf "%c%d" ((sprintf "%A" cardColor).Chars 0) cardNumber

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

    override x.ToString() = sprintf "%A-%A->%A/%A" hideCard handCards askedHitCards askedNoHitCards

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

let think (turnPlayer:Player) (otherPlayers:Player list) =
    // 見えるカードを全部まとめる
    let openCards =
        List.append [turnPlayer] otherPlayers
         |> List.map(fun player -> List.append player.AskedHitCards player.AskedNoHitCards) 
         |> List.append [turnPlayer.HandCards]
         |> List.concat
         |> List.append [turnPlayer.HideCard]

    // 相手のhitCardを見る
    let enemyHitCards = otherPlayers.Head.AskedHitCards

    // 同じ数字のものが複数ある？→あれば数字側
    let maxNumber =
        fst(
            enemyHitCards
            |> List.countBy(fun card -> card.Number)
            |> List.maxBy (fun n -> 
                let fst, snd = n
                snd)
        )

    if maxNumber > 1 then
        // では同じ数字で出ていない勢力を探す
        let openColors = 
            openCards
                |> List.choose(fun card -> if card.Number = maxNumber then Some(card) else None)
                |> List.map(fun card -> card.Color)
        
        let rejectColors =
            [Color.Red; Color.Blue; Color.Green; Color.Yellow]
            |> List.except openColors
        
        // 出ていない勢力が1種である→特定
        if rejectColors.Length = 1 then
            Card(rejectColors.Head, maxNumber)
        else
            None
    // 出ていない勢力が複数ある→ではhitCardで違う数字のものは同勢力である　→　特定

    // 同じ勢力が複数ある？→あれば勢力で絞る
    let maxColor =
        fst(
            enemyHitCards
            |> List.countBy(fun card -> card.Color)
            |> List.maxBy (fun n -> 
                let fst, snd = n
                snd)
        )

    // 残っている数字が1種である→特定
    let rejectNumbers = 
        [1 .. 7]
        |> List.except (openCards |> List.filter(fun card -> card.Number = maxColor))
    
    if rejectNumbers.Length = 1 then
         Card(maxColor, rejectNumbers.Head) else None
    
    // これで特定できない場合は、自分の手札が1枚以上あるなら待つ
    
    
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
