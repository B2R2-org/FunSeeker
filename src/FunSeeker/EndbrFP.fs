module FunSeeker.EndbrFP

open B2R2
open B2R2.FrontEnd.BinLifter

open System.Collections.Generic
open FunSeeker.Cache
open FunSeeker.Disasm

let returnTwiceDict cache =
  (*
    setjmp, sigsetjmp, vfork, savectx, getcontext
  *)
  let returnTwiceFuncNames = [
    "vfork" ;
    "setjmp" ;
    "sigsetjmp" ;
    "savectx" ;
    "getcontext" ;
  ]

  let stripUnderScore (s: string) =
    let rec loop curr =
      if String.length curr < 2 then curr
      else
        if curr.Chars(0) = '_' then
          loop (curr.Substring (1))
        else curr
    loop s

  cache.Handle.FileInfo.GetLinkageTableEntries ()
  |> Seq.iter (fun sym ->
    let name = sym.FuncName
    let isReturnTwice = List.fold (fun state (str: string) ->
      if stripUnderScore name = str then true else state) false returnTwiceFuncNames
    if isReturnTwice then
      Cache.setReturnTwiceCache cache sym.TrampolineAddress
  )

let inExceptionRange cache (addr: Addr) =
  match SortedList.findGreatestLowerBoundKey addr cache.ExceptionCache with
  | Some funcAddr ->
    let funcSize = cache.ExceptionCache.[funcAddr]
    (addr > funcAddr) && (addr < funcAddr + uint64 funcSize)
  | _ -> false

let eliminateExceptions cache =
  cache.EndbrCache.RemoveWhere (fun addr ->
    if inExceptionRange cache addr then
      true
    else
      cache.NumException <- cache.NumException + 1
      false
  )
  |> ignore

let isReturnTwice cache (inst: Instruction) =
  if inst.IsCall () then
    let target = getCallTarget inst
    if cache.ReturnTwiceCache.Contains target then true
    else false
  else false

let isFallThrough cache (inst: Instruction) =
  if inst.IsCondBranch () then true
  else false

let lookPrevInst cache =
  returnTwiceDict cache
  let prevInstAddr addr =
    match SortedList.findGreatestLowerBoundKey (addr - 1UL) cache.LinearCache with
    | Some prevAddr -> prevAddr
    | _ -> 0UL
  cache.EndbrCache.RemoveWhere (fun addr ->
    let prevAddr = prevInstAddr addr
    let prevInst = cache.LinearCache.[prevAddr]
    if isReturnTwice cache prevInst then
      cache.NumReturnTwice <- cache.NumReturnTwice + 1
      true
    /// elif isFallThrough cache prevInst then false
    else false
  )
  |> ignore

let eliminateEndbrFP cache =
  lookPrevInst cache
  eliminateExceptions cache
