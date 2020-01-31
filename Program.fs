open System

type Color = Yellow | Red | Blue | Green | Purple

type Card(color:Color, number:int) =
    let cardColor = color
    let cardNumber = number

    member x.Color with get() = cardColor
    member x.Number with get() = cardNumber

type CardHolder(card:Card) = 
    let card = card
    let mutable isOpen = false
    let mutable isKnown = false

    member x.Card with get() = card
    member x.IsOpen with get() = isOpen
    member x.IsKnown with get() = isKnown

    member x.Open() = isOpen <- true
    member x.Close() = isOpen <- false 
    member x.Known() = isKnown <- true

type CardList(cardList:List<CardHolder>) = 
    let mutable cards = cardList

    member x.OpenAll() = 
        for card in cards do
            card.Open()

    member x.CloseAll() =
        for card in cards do
            card.Close()

    member x.Add(addCard:Card) =
        cards <- List.append [addCard]


type CardDefine() =
    let cardMaxNumber = 7
    let cardMinNumber = 1

    let playerMinCount = 2
    let playerMaxCount = 6



    


type Player

type PlayerList

type CardBuilder() =
    let makeDeck() =
        let colors = [Yellow ; Red ; Blue; Green; Purple]
        for i in 1 .. 7 do
            

           

type Judgementer() =
    let judgement(card:Card, color:Color, number:int) =
        return card.color == color && card.number == number

type GameMaster() =
    let turn:int
    let playerList:List<Player>
    let judgement()
    let run()
    let makeDeck()
    let makePlayer()


[<EntryPoint>]
let main argv =
    Console.Error.WriteLine("*** FBK-START ***") 
 
 
    Console.Error.WriteLine("*** FBK-END ***") 
    0