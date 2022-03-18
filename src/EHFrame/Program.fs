open B2R2
open B2R2.FrontEnd.BinFile
open B2R2.FrontEnd.BinInterface

let initBinHandle path =
  BinHandle.Init (
    ISA.DefaultISA,
    ArchOperationMode.NoMode,
    true, /// AutoDetect
    None, /// BaseAddress
    fileName = path)

let extractEntriesFromExceptionTable (hdl: BinHandle) =
  let text = hdl.FileInfo.GetTextSections () |> Seq.item 0
  let textStart = text.Address
  let textEnd = text.Address + text.Size
  let elf = hdl.FileInfo :?> ELFFileInfo
  let exnFrame = elf.ELF.ExceptionFrame
  exnFrame
  |> List.fold (fun acc entry ->
    entry.FDERecord
    |> Array.fold (fun acc record ->
      let pcbegin = record.PCBegin
      if textStart <= pcbegin && pcbegin <= textEnd then
        Set.add pcbegin acc
      else acc) acc) Set.empty

let dumpEntries path entries =
  let entryStrings = entries |> Set.toArray |> Array.map (sprintf "%x")
  System.IO.File.WriteAllLines (path, entryStrings)

[<EntryPoint>]
let main argv =
  let binPath = argv.[0]
  let outPath = argv.[1]
  let hdl = initBinHandle binPath
  let entries = extractEntriesFromExceptionTable hdl
  dumpEntries outPath entries
  0
