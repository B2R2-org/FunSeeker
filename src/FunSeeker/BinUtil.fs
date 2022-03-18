module FunSeeker.BinUtil

open System.Collections.Generic
open B2R2
open B2R2.FrontEnd.BinInterface
open B2R2.FrontEnd.BinLifter

let createBinHandleFromPath filepath =
  BinHandle.Init (
    ISA.DefaultISA,
    ArchOperationMode.NoMode,
    true, /// AutoDetect
    None, /// BaseAddress
    fileName = filepath)

let textSectionOf (hdl: BinHandle) =
  hdl.FileInfo.GetTextSections ()
  |> Seq.tryFind (fun s ->
    if s.Name = ".text" then true else false)

let textBoundaryOf (hdl: BinHandle) =
  match textSectionOf hdl with
  | Some s ->
    (s.Address, s.Address + s.Size)
  | _ -> (0UL, 0UL)

let isTextAddr (hdl: BinHandle) addr =
  let stext, etext = textBoundaryOf hdl
  (addr >= stext) && (addr < etext)

let isX86 (hdl: BinHandle) =
  hdl.FileInfo.ISA.Arch = Arch.IntelX86
