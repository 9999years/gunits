module gunits.test
open System
open FSharp.Data

open NUnit.Framework
open FsUnit

let html (x : string) = HtmlDocument.Parse x
let elements (x : string) =
    x
    |> html
    |> HtmlDocument.elements

let root (x : string) =
    x
    |> html
    |> HtmlDocument.elements
    |> List.head

let is_card_test expect str =
    str
    |> root
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

[<Test>]
let is_card_test_5 () =
    """
    <td>
    <div class="lst-a">
        <table cellpadding="0" cellspacing="0">
        <tr>
            <td class="lst-td" width="555" valign="bottom">
            <div style="position:relative;zoom:1">
                <input class="lst" value="10mi to in" title="Search" autocomplete="off" id="sbhost" maxlength="2048" name="q" type="text" />
            </div>
            </td>
        </tr>
        </table>
    </div>
    </td>
    """
    |> is_card_test false

[<Test>]
let is_card_test_6 () =
    """
    <div class="g">
    <h3 class="r">
        <a href="/url?q=https://www.runwashington.com/event_category/10mi/&sa=U&ved=0ahUKEwj368622JrYAhWBON8KHRkRArEQFggUMAA&usg=AOvVaw3bG94se03DLP0X5QOr5wOy">
        <b>10mi</b> | RunWashington
        </a>
    </h3>
    <div class="s">
        <div class="kv" style="margin-bottom:2px">
        <cite>
            https://www.runwashington.com/event_category/<b>10mi</b>/
        </cite>
        <div class="_nBb">
            <div style="display:inline" onclick="google.sham(this);" aria-expanded="false" aria-haspopup="true" tabindex="0" data-ved="0ahUKEwj368622JrYAhWBON8KHRkRArEQ7B0IFTAA">
            <span class="_O0" />
            </div>
            <div style="display:none" class="am-dropdown-menu" role="menu" tabindex="-1">
            <ul>
                <li class="_Ykb">
                <a class="_Zkb" href="/url?q=http://webcache.googleusercontent.com/search%3Fq%3Dcache:aHk6HO6sK9IJ:https://www.runwashington.com/event_category/10mi/%252B10mi%2Bto%2Bin%26hl%3Den%26ct%3Dclnk&sa=U&ved=0ahUKEwj368622JrYAhWBON8KHRkRArEQIAgXMAA&usg=AOvVaw01-ge3-wpV8jxjc5-Ig6uu">Cached</a>
                </li>
            </ul>
            </div>
        </div>
        </div>
        <span class="st">
        Distances. All Events (92); Other (1); 12k (1); 17k (1); 20k (1); 50k (1); Relay (1); 1.5mi (1); 2mi (1); 2.71mi (1); Quarter Marathon (1); 2k (1); 15k (1); 34k (1); 8k (2); 4mi (2); <b>10mi</b> (2); Marathon (2); 1k (3); Fun Run (3); Kids' Run (3); 5mi (3); 1mi (4); 10k (9); Half Marathon (10); 5k (36)┬á...
        </span>
    </div>
    </div>
    """
    |> is_card_test false

[<Test>]
let is_card_test_7 () =
    """
    <div class="g">
        <h3></h3>
        <div></div>
    </div>
    """
    |> is_card_test false


[<Test>]
let optionEq_test_1 () =
    HtmlNodeMatch.optionSimpleEq Option.None (Option.Some 100)
    |> should equal true

[<Test>]
let optionEq_test_2 () =
    HtmlNodeMatch.optionSimpleEq (Option.Some 100) (Option.Some 100)
    |> should equal true

[<Test>]
let optionEq_test_3 () =
    HtmlNodeMatch.optionSimpleEq (Option.Some 101) (Option.Some 100)
    |> should equal false

[<Test>]
let optionEq_test_4 () =
    HtmlNodeMatch.optionSimpleEq (Option.Some "xyz") Option.None
    |> should equal true

[<Test>]
let classes_test_1 =
    """<div class="a b c"></div>"""
    |> root
    |> classes
    |> should equal ["a"; "b"; "c"]

[<Test>]
let classes_test_2 =
    """<div class="a"></div>"""
    |> root
    |> classes
    |> should equal ["a"]

[<Test>]
let classes_test_3 =
    """<div class=""></div>"""
    |> root
    |> classes
    |> should equal []
