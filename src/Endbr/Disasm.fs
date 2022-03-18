module Endbr.Disasm

open System.Collections
open B2R2
open B2R2.FrontEnd.BinFile
open B2R2.FrontEnd.BinInterface
open B2R2.FrontEnd.BinLifter
open B2R2.FrontEnd.BinLifter.Intel
open Endbr.Cache
open Endbr.BinUtil


let getCallTarget (ins: Instruction) =
  match ins.DirectBranchTarget () |> Utils.tupleToOpt with
  | None -> 0UL
  | Some target -> target

let isEndbr (ins: Instruction) =
  let intelInst = ins :?> IntelInstruction
  match intelInst.Opcode with
  | Opcode.ENDBR32 | Opcode.ENDBR64 -> true
  | _ -> false

let addCallTarget cache (ins: Instruction) =
  let entry = Option.get cache.Handle.FileInfo.EntryPoint
  match ins.DirectBranchTarget () |> Utils.tupleToOpt with
  | None -> ()
  | Some target ->
    if isTextAddr cache.Handle target && target <> (ins.Address + uint64 ins.Length) then
      Cache.setCallTargetCache cache target

let addJumpTarget cache (ins: Instruction) =
  let intelInst = ins :?> IntelInstruction
  match intelInst.Opcode with
  | Opcode.JMPFar | Opcode.JMPNear ->
    match ins.DirectBranchTarget () |> Utils.tupleToOpt with
    | None -> ()
    | Some target ->
      if isTextAddr cache.Handle target then
        Cache.setJumpCache cache ins.Address target
        Cache.setJumpTargetRefCache cache target ins.Address
  | _ -> ()

let parse cache =
  let rec disasm hdl bp =
    if BinaryPointer.IsValid bp then
      match BinHandle.TryParseInstr (hdl, bp=bp) with
      | Ok (ins) ->
        let bp' = BinaryPointer.Advance bp (int ins.Length)
        if isEndbr ins && isTextAddr cache.Handle bp.Addr then
          Cache.setEndbrCache cache bp.Addr
        Cache.setLinearCache cache bp.Addr ins
        if ins.IsCall () then
          addCallTarget cache ins
        if ins.IsBranch () then
          addJumpTarget cache ins
        disasm hdl bp'
      | Error _ ->
        let bp' = BinaryPointer.Advance bp 1
        disasm hdl bp'
    else ()

  let hdl = cache.Handle

  cache.Handle.FileInfo.ExceptionTable
  |> ARMap.iter (fun funcRange _ ->
    Cache.setExceptionCache cache funcRange.Min (int (funcRange.Max - funcRange.Min + 1UL))
  )

  hdl.FileInfo.GetExecutableSections ()
  |> Seq.iter (fun s ->
    if s.Size > 0UL then
      let bp = BinaryPointer.OfSection s
      disasm hdl bp)
