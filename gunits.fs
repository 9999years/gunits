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
    static member private childrenListCompare (children : Option<List<NodeType>>)
        (os : List<HtmlNode>) =
            match children with
                | Option.Some cs ->
                    match (cs, os) with
                    | (c :: cs, o :: os) ->
                        (new HtmlNodeMatch(c)).Equals(o)
                        && HtmlNodeMatch.childrenListCompare (Option.Some cs) os
                    | ([], []) -> true
                    | ([], _) -> false
                    | (_, []) -> false
                | None -> true

    member this.node = node

    override this.GetHashCode() =
        hash (node)

    override this.Equals(o : obj) =
        match o with
        | :? HtmlNode as n -> this.HtmlNodeEquals(n)
        | _ -> false

    member private this.HtmlNodeEquals (o : HtmlNode) =
        match this.node.class_count with
            | Option.Some c -> c = (classes o |> Array.length)
            | None -> true
        && match this.node.name with
            | Option.Some n -> n = HtmlNode.name o
            | None -> true
        && match this.node.child_count with
            | Option.Some c -> c = (HtmlNode.elements o |> List.length)
            | None -> true
        && match this.node.children with
            | Option.Some children ->
                HtmlNodeMatch.childrenListCompare
                    (Option.Some children) (HtmlNode.elements o)
            | None -> true
        && match this.node.text with
            | StringMatch.Some text -> text = HtmlNode.directInnerText o
            | NonEmpty -> "" <> HtmlNode.directInnerText o
            | Any -> true

// we want to find a:
// div with one class with two children:
    // 1. a div with 2 classes with 2 children:
        // 1.1. span with text
        // 1.2. span with " = "
    // 2. a div with 2 classes and no chidren
let is_card el =
    el =
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
                            Option.Some [
                                { DefaultNode with
                                    name = Option.Some "span"
                                    text = NonEmpty };
                                { DefaultNode with
                                    name = Option.Some "span"
                                    text = StringMatch.Some " = " } ] };
                    { DefaultNode with
                        name = Option.Some "div"
                        class_count = Option.Some 2
                        child_count = Option.Some 2 } ] }

[<EntryPoint>]
let main argv =
    let query =
        argv
        |> String.concat " "
        |> Web.HttpUtility.UrlEncode
    let doc =
        "https://google.com/search?q=" + query
        |> HtmlDocument.Load
    (*let card =*)
        (*doc.Descendents ["div"]*)
        (*|>*)
    printsn doc
    0
