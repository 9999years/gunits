module gunits.test
open System
open FSharp.Data

open NUnit.Framework
open FsUnit

let is_card_test expect str =
    str
    |> HtmlDocument.Parse
    |> HtmlDocument.elements
    |> List.head
    |> gunits.is_card
    |> should equal expect

[<Test>]
let is_card_test_1 () =
    """<div class="_Tsb">
    <div class="_Qeb _HOb">
        <span>10 miles</span>
        <span> = </span>
    </div>
    <div class="_Peb _rkc">633600 inches</div>
    </div>"""
    |> is_card_test true

[<Test>]
let is_card_test_2 () =
    """<div>
    <div class="_Qeb _HOb">
        <span>10 miles</span>
        <span> = </span>
    </div>
    <div class="_Peb _rkc">633600 inches</div>
    </div>"""
    |> is_card_test false

[<Test>]
let is_card_test_3 () =
    """<div>
    <div class="_Qeb _HOb">
        <span>10 miles</span>
        <span></span>
    </div>
    <div class="_Peb _rkc">633600 inches</div>
    </div>"""
    |> is_card_test false

[<Test>]
let is_card_test_4 () =
    """<div>
    <div class="_Qeb _HOb">
        10 miles =
    </div>
    <div class="_Peb _rkc">633600 inches</div>
    </div>"""
    |> is_card_test false
