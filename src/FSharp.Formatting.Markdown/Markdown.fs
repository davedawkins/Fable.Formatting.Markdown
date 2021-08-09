// --------------------------------------------------------------------------------------
// F# Markdown (Main.fs)
// (c) Tomas Petricek, 2012, Available under Apache 2.0 license.
// --------------------------------------------------------------------------------------

namespace FSharp.Formatting.Markdown

open System
open System.IO
open System.Collections.Generic

open FSharp.Collections
open FSharp.Patterns
open FSharp.Formatting.Markdown.Parser
//open FSharp.Formatting.Common


/// <summary>
/// Representation of a Markdown document - the representation of Paragraphs
/// uses an F# discriminated union type and so is best used from F#.
/// </summary>
/// <namespacedoc>
///   <summary>Functionality for processing markdown documents, converting to HTML, LaTeX, ipynb and scripts</summary>
/// </namespacedoc>
type MarkdownDocument(paragraphs, links) =
  /// Returns a list of paragraphs in the document
  member x.Paragraphs : MarkdownParagraphs = paragraphs

  /// Returns a dictionary containing explicitly defined links
  member x.DefinedLinks : IDictionary<string, string * option<string>> = links

/// Static class that provides methods for formatting
/// and transforming Markdown documents.
type Markdown internal () =
  /// Parse the specified text into a MarkdownDocument. Line breaks in the
  /// inline HTML (etc.) will be stored using the specified string.
  static member Parse(text:string, ?newline, ?parseOptions) =
    let newline = defaultArg newline Environment.NewLine
    let parseOptions = defaultArg parseOptions MarkdownParseOptions.None
    //use reader = new StringReader(text)
    let lines =
        (text
            .Replace("\r\n", "\n")
            .Split( [| '\n'; '\r' |] )
        |> Array.mapi (fun i s ->
            s, { StartLine = i+1; StartColumn = 0; EndLine = i+1; EndColumn = s.Length }))
        |> Seq.toList
    // let lines =
    //   [ let line = ref ""
    //     let mutable lineNo = 1
    //     while (line := reader.ReadLine(); line.Value <> null) do
    //       yield (line.Value, { StartLine = lineNo; StartColumn = 0; EndLine = lineNo; EndColumn = line.Value.Length })
    //       lineNo <- lineNo + 1
    //     if text.EndsWith(newline) then
    //       yield ("", { StartLine = lineNo; StartColumn = 0; EndLine = lineNo; EndColumn = 0 }) ]
      //|> Utils.replaceTabs 4
    let links = Dictionary<_, _>()
    //let (Lines.TrimBlank lines) = lines
    let ctx : ParsingContext =
        { Newline = newline
          IsFirst = true
          Links = links
          CurrentRange = Some(MarkdownRange.zero)
          ParseOptions=parseOptions }
    let paragraphs =
      lines
      |> List.skipWhile (fun (s, _n) -> String.IsNullOrWhiteSpace s)
      |> parseParagraphs ctx
      |> List.ofSeq
    MarkdownDocument(paragraphs, links)

  /// Transform the provided MarkdownDocument into HTML
  /// format and write the result to a given writer.
  static member WriteHtml(doc:MarkdownDocument, writer, ?newline, ?substitutions, ?crefResolver) =
    let newline = defaultArg newline Environment.NewLine
    let substitutions = defaultArg substitutions []
    let crefResolver = defaultArg crefResolver (fun _ -> None)
    HtmlFormatting.formatMarkdown writer false false doc.DefinedLinks substitutions newline crefResolver doc.Paragraphs

  /// Transform Markdown text into HTML format. The result
  /// will be written to the provided TextWriter.
  static member WriteHtml(markdownText: string, writer:FableTextWriter, ?newline, ?substitutions, ?crefResolver) =
    let doc = Markdown.Parse(markdownText, ?newline=newline)
    Markdown.WriteHtml(doc, writer, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)

  /// Transform the provided MarkdownDocument into HTML
  /// format and return the result as a string.
  static member ToHtml(doc:MarkdownDocument, ?newline, ?substitutions, ?crefResolver) =
    use wr = new FableTextWriter()
    Markdown.WriteHtml(doc, wr, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)
    wr.ToString()

  /// Transform Markdown document into HTML format.
  /// The result will be returned as a string.
  static member ToHtml(markdownText: string, ?newline, ?substitutions, ?crefResolver) =
    let doc = Markdown.Parse(markdownText, ?newline=newline)
    Markdown.ToHtml(doc, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)

  /// Transform the provided MarkdownDocument into LaTeX
  /// format and write the result to a given writer.
  //static member WriteLatex(doc:MarkdownDocument, writer, ?newline, ?substitutions, ?crefResolver) =
  //  let newline = defaultArg newline Environment.NewLine
  //  let substitutions = defaultArg substitutions []
  //  let crefResolver = defaultArg crefResolver (fun _ -> None)
  //  LatexFormatting.formatMarkdown writer doc.DefinedLinks substitutions newline crefResolver doc.Paragraphs

  /// Transform Markdown document into LaTeX format. The result
  /// will be written to the provided TextWriter.
  //static member WriteLatex(markdownText, writer:TextWriter, ?newline, ?substitutions, ?crefResolver) =
  //  let doc = Markdown.Parse(markdownText, ?newline=newline)
  //  Markdown.WriteLatex(doc, writer, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)

  /// Transform the provided MarkdownDocument into LaTeX
  /// format and return the result as a string.
  //static member ToLatex(doc:MarkdownDocument, ?newline, ?substitutions, ?crefResolver) =
  //  let sb = new System.Text.StringBuilder()
  //  use wr = new StringWriter(sb)
  //  Markdown.WriteLatex(doc, wr, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)
  //  sb.ToString()

  /// Transform Markdown text into LaTeX format. The result will be returned as a string.
  //static member ToLatex(markdownText: string, ?newline, ?substitutions, ?crefResolver) =
  //  let doc = Markdown.Parse(markdownText, ?newline=newline)
  //  Markdown.ToLatex(doc, ?newline=newline, ?substitutions=substitutions, ?crefResolver=crefResolver)

  /// Transform the provided MarkdownDocument into Pynb and return the result as a string.
  //static member ToPynb(doc: MarkdownDocument, ?newline, ?substitutions, ?crefResolver) =
  //  let newline = defaultArg newline Environment.NewLine
  //  let substitutions = defaultArg substitutions []
  //  let crefResolver = defaultArg crefResolver (fun _ -> None)
  //  PynbFormatting.formatAsPynb doc.DefinedLinks substitutions newline crefResolver doc.Paragraphs

  /// Transform the provided MarkdownDocument into Fsx and return the result as a string.
  //static member ToFsx(doc: MarkdownDocument, ?newline, ?substitutions, ?crefResolver) =
  //  let newline = defaultArg newline Environment.NewLine
  //  let substitutions = defaultArg substitutions []
  //  let crefResolver = defaultArg crefResolver (fun _ -> None)
  //  FsxFormatting.formatAsFsx doc.DefinedLinks substitutions newline crefResolver doc.Paragraphs

  /// Transform the provided MarkdownDocument into Md and return the result as a string.
  static member ToMd(doc: MarkdownDocument, ?newline, ?substitutions, ?crefResolver) =
    let newline = defaultArg newline Environment.NewLine
    let substitutions = defaultArg substitutions []
    let crefResolver = defaultArg crefResolver (fun _ -> None)
    MarkdownFormatting.formatAsMd doc.DefinedLinks substitutions newline crefResolver doc.Paragraphs
