module FunSeeker.Tailcall

open FunSeeker.Cache
open FunSeeker.BinUtil

let blockEndOf cache addr =
  let rec loop addr =
    if cache.LinearCache.ContainsKey(addr) then
      let inst = cache.LinearCache.[addr]
      if inst.IsBBLEnd () && (inst.IsCall () |> not) then
        addr
      else loop (addr + uint64 inst.Length)
    else addr
  loop addr

let isColdFunc cache addr fboundary =
  let fstart, fend = fboundary
  cache.JumpTargetRefCache.[addr]
  |> Seq.fold (fun state addr ->
      if addr >= fstart && addr < fend then
        true && state
      else false
    ) true

let tailCallAnalysis cache =
  let functionList = cache.FunctionCache |> Seq.toList |> List.sort
  let functionBoundaryOf cache addr =
    let fIdx' = 
      List.tryFindIndexBack (fun funcAddr ->
        funcAddr <= addr
      ) functionList
    match fIdx' with
    | Some idx ->
      if idx >= functionList.Length then
        (0UL, 0UL)
      else
        let fstart = functionList.[idx]
        let fend = functionList.[idx + 1]
        (fstart, fend)
    | _ -> (0UL, 0UL)
  cache.JumpCache.Keys
  |> Seq.cast
  |> Seq.iter (fun addr ->
    let fstart, fend = functionBoundaryOf cache addr
    let target = cache.JumpCache.[addr]
    if (fstart <> 0UL && fend <> 0UL) && (target < fstart || target >= fend) then
      if isColdFunc cache target (fstart, fend) then ()
      else
        Cache.setFunctionCache cache target
    else ()
  )

let tailCallAnalysis2 cache =
  let functionBoundaryOf cache addr =
    let min, max = textBoundaryOf cache.Handle
    let rec before addr go =
      if addr < min then 0UL
      elif cache.FunctionCache.Contains(addr) then
        if not go then addr
        else before (addr - 1UL) false
      else before (addr - 1UL) go
    and after addr =
      if addr > max then 0UL
      elif cache.FunctionCache.Contains(addr) then addr                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                          
      else after (addr + 1UL)
    (before addr true, after addr)
  cache.JumpCache.Keys
  |> Seq.cast
  |> Seq.iter (fun addr ->
    let fstart, fend = functionBoundaryOf cache addr
    let target = cache.JumpCache.[addr]
    if (fstart <> 0UL && fend <> 0UL) && (target < fstart || target >= fend) then
      Cache.setFunctionCache cache target
  )
