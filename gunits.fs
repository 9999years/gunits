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

// we want to find a:
// div with one class with two children:
    // 1. a div with 2 classes with 2 children:
        // 1. span with text
        // 2. span with " = "
    // 2. a div with 2 classes and no chidren
let isCard el =
    // el is a div
    if HtmlNode.name el <> "div" then false else
    let children = HtmlNode.elements el
    // 2 children
    if children.Length <> 2 then false else
    let first = children.[0]
    let first_classes = classes first
    // 1ST CHILD
    // has 2 classes
    if first_classes.Length <> 2 then false else
    let first_children = HtmlNode.elements first
    // and 2 children
    if first_children.Length <> 2 then false else
    // 1st child is a span
    if HtmlNode.hasName "span" first_children.[0] == false then false else

    // 2nd child is a div
    if HtmlNode.name children.[1] <> "div" then false else
    // 2nd child has 2 classes
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
