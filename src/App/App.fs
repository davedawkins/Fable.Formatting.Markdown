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
```
    let fib n =
        if n <= 1 then
            1
        else
            n * (fib (n-1))
```

"""

let markdown md =
    let doc = FSharp.Formatting.Markdown.Markdown.Parse(md)
    FSharp.Formatting.Markdown.Markdown.ToHtml(doc)

let appE = Browser.Dom.window.document.querySelector("#app")

appE.innerHTML <- markdown markdownSrc
