# Fable.Formatting.Markdown

A port of `Fable.Formatting.Markdown` for Fable. This will allow you to use format markdown into HTML inside your Fable application

Example code:

```
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

let markdown md =
    let doc = Fable.Formatting.Markdown.Markdown.Parse(md)
    Fable.Formatting.Markdown.Markdown.ToHtml(doc)

let appE = Browser.Dom.window.document.querySelector("#app")

appE.innerHTML <- markdown markdownSrc
```

## Details of Port to Fable

- Based on `FSharp.Formatting` `v11.4.2`. Only the `Markdown` module has been ported across.

- Regex match groups don't have an Index property. The workaround I used is to specify captures for all of the expression and then sum the size of each preceding capture to get the start of the original capture we wanted.

- `Array.FindAll` - replaced with `Array.filter`

- `TextWriter/StringWriter` replaced with `FableTextWriter`, which performs string concatenation

