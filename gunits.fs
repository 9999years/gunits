open FSharp.Data
open System

[<EntryPoint>]
let main argv =
    let doc = HtmlDocument.Load("https://google.com/search?q=FSharp.Data")
    printf "%s" doc
    0
