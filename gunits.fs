module gunits
open FSharp.Data
open System

/// cast and print
let inline prints  s = string s |> printf  "%s"
let inline printsn s = string s |> printfn "%s"

let inline split (delimiter : string) (str : string) =
    ([| delimiter |], StringSplitOptions.RemoveEmptyEntries)
    |> str.Split

let classes (el : HtmlNode) : string[] =
    el
    |> HtmlNode.attributeValue "class"
    |> split " "

type StringMatch =
    | Some of string
    | NonEmpty // any non-empty text
    | Any // any text

[<Struct>]
type NodeType = {
    class_count: Option<int>
    name: Option<string>
    child_count: Option<int>
    text: StringMatch
    children: Option<List<NodeType>>
}

let DefaultNode = {
    class_count = None
    name = None
    child_count = None
    text = Any
    children = None
}

type HtmlNodeMatch (node : NodeType) =
    static member private childrenListCompare
        (children : List<NodeType>) (others : List<NodeType>) =
            let compared =
                List.compareWith (fun c o -> c.Equals(o) |> Convert.ToInt32)
                    children others
            not <| (compared |> Convert.ToBoolean)

    static member private optionEq (comparer : 'T -> 'T -> bool)
        (a : Option<'T>) (b : Option<'T>) =
            match (a, b) with
            | (Option.Some A, Option.Some B) -> comparer A B
            | (Option.None, _) -> true
            | (_, Option.None) -> true

    static member private optionSimpleEq (a : Option<'T>) (b : Option<'T>) =
        HtmlNodeMatch.optionEq (fun A B -> A = B) a b

    static member private stringMatchEq (a : StringMatch) (b : StringMatch) =
        match (a, b) with
            | (StringMatch.Some A,   StringMatch.Some B)   -> A = B
            | (StringMatch.NonEmpty, StringMatch.Some B)   -> "" <> B
            | (StringMatch.Some A,   StringMatch.NonEmpty) -> "" <> A
            | (_, _) -> true

    member this.node = node

    member this.class_count = this.node.class_count
    member this.name        = this.node.name
    member this.child_count = this.node.child_count
    member this.text        = this.node.text
    member this.children    = this.node.children

    new (node : HtmlNode) =
        HtmlNodeMatch
            {   class_count = Option.Some (classes node |> Array.length)
                name        = Option.Some <| HtmlNode.name node
                text        = StringMatch.Some <| HtmlNode.directInnerText node
                child_count = Option.Some (HtmlNode.elements node |> List.length)
                children    = Option.Some
                    <| [ for n in HtmlNode.elements node do
                            yield (new HtmlNodeMatch(n)).node ] }

    override this.GetHashCode() =
        hash (node)

    override this.Equals(o : obj) =
        match o with
        | :? HtmlNode as n -> this.Equals(new HtmlNodeMatch(n))
        | :? HtmlNodeMatch as n -> this.NodeMatchEquals(n)
        | _ -> false

    member private this.NodeMatchEquals (o : HtmlNodeMatch) =
        HtmlNodeMatch.optionSimpleEq this.class_count o.class_count
        && HtmlNodeMatch.optionSimpleEq this.name o.name
        && HtmlNodeMatch.optionSimpleEq this.child_count o.child_count
        && HtmlNodeMatch.stringMatchEq this.text o.text
        && HtmlNodeMatch.optionEq
            HtmlNodeMatch.childrenListCompare this.children o.children

// we want to find a:
// div with one class with two children:
    // 1. a div with 2 classes with 2 children:
        // 1.1. span with text
        // 1.2. span with " = "
    // 2. a div with 2 classes and no chidren
// OR
// a td with no classes and 1 child; an h2 with no classes with 1 child; a bold
//
// <div class="_Tsb">
//   <div class="_Qeb _HOb">
//       <span>10 miles</span>
//       <span> = </span>
//   </div>
//   <div class="_Peb _rkc">633600 inches</div>
// </div>

let is_card (el : HtmlNode) =
    ( new HtmlNodeMatch(
        { DefaultNode with
            class_count = Option.Some 1
            child_count = Option.Some 2
            name = Option.Some "div"
            children =
                Option.Some [
                    { DefaultNode with
                        class_count = Option.Some 2
                        child_count = Option.Some 2
                        name = Option.Some "div"
                        children =
                            Option.None }
                            (*Option.Some [*)
                                (*{ DefaultNode with*)
                                    (*name = Option.Some "span"*)
                                    (*text = NonEmpty }*)
                                (*{ DefaultNode with*)
                                    (*name = Option.Some "span"*)
                                    (*text = StringMatch.Some " = " } ] }*)
                    { DefaultNode with
                        name = Option.Some "div"
                        class_count = Option.Some 2
                        child_count = Option.Some 0 } ] } )
                (*Option.None } )*)
    ).Equals(el) ||

    ( new HtmlNodeMatch(
        { DefaultNode with
            name        = Option.Some "td"
            class_count = Option.Some 0
            child_count = Option.Some 1
            children    =
                Option.Some [
                    { DefaultNode with
                        name        = Option.Some "h2"
                        child_count = Option.Some 1
                        class_count = Option.Some 0
                        children    =
                            Option.Some [
                                { DefaultNode with
                                    name        = Option.Some "b"
                                    class_count = Option.Some 0
                                    child_count = Option.Some 0 } ] } ] })
    ).Equals(el)

[<EntryPoint>]
let main argv =
    let query =
        argv
        |> String.concat " "
        |> Web.HttpUtility.UrlEncode
    (*let doc =*)
        (*"https://google.com/search?q=" + query*)
        (*|> HtmlDocument.Load*)
    let doc =
        argv.[0]
        |> IO.File.OpenRead
        |> HtmlDocument.Load
    let card =
        HtmlDocument.descendants true (fun t -> true) doc
        |> Seq.filter is_card
    (*let txt =*)
        (*card*)
        (*|> Seq.map (fun el -> HtmlNode.innerText el)*)
    card |> Seq.head |> printsn
    (*txt |> Seq.toList |> printsn*)
    0
