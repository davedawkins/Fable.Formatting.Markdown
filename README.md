# Fable.Formatting.Markdown [![Nuget](https://img.shields.io/nuget/v/Fable.Formatting.Markdown.svg?maxAge=0&colorB=brightgreen)](https://www.nuget.org/packages/Fable.Formatting.Markdown)

A port of `Fable.Formatting.Markdown` for Fable. This will allow you to format markdown into HTML inside your Fable application.

The benefit of using this package over (say) [markedjs](https://github.com/markedjs/marked) is that we have the same formatting behaviour and syntax as `FSharp.Formatting`, and of course a rich FSharp-first API for parsing markdown text into an intermediate document.

For documentation, refer to [Markdown Parser](https://fsprojects.github.io/FSharp.Formatting/markdown.html). Note that in general `FSharp.Formatting` in those documents is `Fable.Formatting` in this package.

## Fable Example

```fsharp
module App

let markdownSrc = """
##Markdown Example

This is *italic*, and this is **bold**, and this is a `symbol`.

**Unordered List**

- Unordered item
- Next item
- Last item

**Numbered List**

1. First step
1. Second step
1. Third step

**Code**

    let fib n =
        if n <= 1 then
            1
        else
            n * (fib (n-1))

"""

let appE = Browser.Dom.window.document.querySelector("#app")

appE.innerHTML <- Fable.Formatting.Markdown.Markdown.ToHtml(markdownSrc)
```

## Feliz Example

```fsharp
module Main

open Feliz
open Browser.Dom
open Fable.Formatting.Markdown

[<ReactComponent>]
let MarkdownDiv (x:string) =
    Html.div [
        prop.dangerouslySetInnerHTML (Markdown.ToHtml x)
    ]

[<ReactComponent>]
let app (md : string) =
    let (src, setSrc) = React.useState(md)
    Html.div [
        MarkdownDiv src
        Html.textarea [
            prop.value src
            prop.onChange (fun s -> setSrc(s))
        ]
    ]

ReactDOM.render(
    app "**Hello World** from *Markdown*",
    document.getElementById "feliz-app"
)
```

Result:

<img width="282" alt="image" src="https://user-images.githubusercontent.com/285421/128925663-b337781b-c973-4f16-a352-cba92d9f69ed.png">

## Details of Port to Fable

- Based on `FSharp.Formatting` `v11.4.2`. Only the `Markdown` module has been ported across.

- Regex match groups don't have an Index property. The workaround I used is to specify captures for all of the expression and then sum the size of each preceding capture to get the start of the original capture we wanted.

- `Array.FindAll` - replaced with `Array.filter`

- `TextWriter/StringWriter` replaced with `FableTextWriter`, which performs string concatenation

