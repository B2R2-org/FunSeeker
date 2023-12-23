module FunSeeker.Report

open B2R2
open FunSeeker.Cache
open FunSeeker.EndbrFP

let printFunctions cache =
  cache.FunctionCache
  |> Seq.iter (fun addr ->
    printf "%x\n" addr
  )
  0

/// DBT + Endbr + ReturnTwice + Exception
let reportType4 cache =
  eliminateEndbrFP cache
  cache.FunctionCache.UnionWith(cache.CallTargetCache) |> ignore
  cache.FunctionCache.UnionWith(cache.EndbrCache) |> ignore

let reportFP cache =
  eliminateEndbrFP cache
  printf "ReturnTwice : %d\n" cache.NumReturnTwice
  printf "Exception : %d\n" cache.NumException
  0

let reportException cache =
  let getExceptionRange cache (addr: Addr) =
    match SortedList.findGreatestLowerBoundKey addr cache.ExceptionCache with
    | Some funcAddr ->
      let funcSize = cache.ExceptionCache.[funcAddr]
      (funcAddr, funcAddr + uint64 funcSize)
    | _ -> (0UL, 0UL)

  cache.EndbrCache
  |> Seq.iter (fun addr ->
    if inExceptionRange cache addr |> not then
      ()
    else
      let fstart, fend = getExceptionRange cache addr
      printf "%x %x %x\n" fstart fend addr
  )
  0

let reportJumpTarget cache =
  Seq.iter (fun addr ->
    printf "%x\n" addr
  ) cache.JumpTargetRefCache.Keys
  0

let reportEndbr cache =
  eliminateEndbrFP cache
  cache.FunctionCache.UnionWith(cache.EndbrCache) |> ignore

let reportCodePointerList cache =
  eliminateSupersetEndbrFP cache
  cache.FunctionCache.UnionWith(cache.EndbrCache) |> ignore
  
let reportSuperset cache =
  cache.FunctionCache.UnionWith(cache.EndbrCache) |> ignore
