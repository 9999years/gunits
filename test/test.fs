namespace test
open System
open FSharp.Data
open NUnit.Framework
open FsUnit

[<Test>]
let is_card_test () =
    """<div class="_Tsb">
    <div class="_Qeb _HOb">
        <span>10 miles</span>
        <span> = </span>
    </div>
    <div class="_Peb _rkc">633600 inches</div>
    </div>"""
    |> HtmlDocument.Parse
    |> is_card
    |> should equal true
