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

(*let confirm_length (n : int) (arr : Collections.List<'a>) =*)
    (*arr.Length = n*)

(*let class_count el n =*)
    (*classes el*)
    (*|> confirm_length n*)

(*let child_count el n =*)
    (*HtmlNode.elements el*)
    (*|> confirm_length n*)

type StringMatch =
    | Some of string
    | NonEmpty // *any* non-empty text
    | Any // *any* text
    | None // non-empty text

type ChildrenMatch<'T> =
    | Some of 'T list
    | Any
    | None

type HtmlNodeMatch (class_count: int, name: string, child_count: int, children: ChildrenMatch<HtmlNodeMatch>, text: StringMatch) =
    member this.class_count = class_count
    member this.name        = name
    member this.child_count = child_count
    member this.children    = children
    member this.text        = text

    override this.GetHashCode() =
        hash (class_count, name, child_count, text, children)

    override this.Equals(o : obj) =
        match o with
        | :? HtmlNode as n -> this.HtmlNodeEquals(n)
        | _ -> false

    member this.HtmlNodeEquals (o : HtmlNode) =
        this.class_count    = (classes o |> Array.length)
        (*&& this.name        = o |> HtmlNode.name*)
        (*&& this.child_count = o |> HtmlNode.elements |> len*)
        && match this.children with
            | ChildrenMatch.None -> this.child_count = 0
            | ChildrenMatch.Any -> true
            | ChildrenMatch.Some children -> children = HtmlNode.elements o
        && match this.text with
            | StringMatch.Some text -> text = HtmlNode.directInnerText o
            | StringMatch.NonEmpty -> "" <> HtmlNode.directInnerText o
            | StringMatch.None -> "" = HtmlNode.directInnerText o
            | StringMatch.Any -> true

// we want to find a:
// div with one class with two children:
    // 1. a div with 2 classes with 2 children:
        // 1. span with text
        // 2. span with " = "
    // 2. a div with 2 classes and no chidren
let isCard el =
    // el is a div
    if HtmlNode.name el <> "div" then false else
    // w/ 2 children
    if HtmlNode.elements el |> List.length <> 2 then false else
    true

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
