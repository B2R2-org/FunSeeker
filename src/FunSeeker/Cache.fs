module FunSeeker.Cache

open System.Collections.Generic
open B2R2
open B2R2.FrontEnd.BinInterface
open B2R2.FrontEnd.BinLifter

type Cache = {
  /// B2R2 BinHandle
  Handle: BinHandle

  /// Type (Addr, Instruction) SortedList
  /// Store B2R2 Instruction for superset disassembly
  SupersetCache: SortedList<Addr, Instruction>

  /// Type (Addr, Instruction) SortedList
  /// Store B2R2 Instruction for linear disassembly
  LinearCache: SortedList<Addr, Instruction>

  /// Type Addr set
  /// Set of call target address
  CallTargetCache: HashSet<Addr>

  /// Type Addr set
  /// Set of jump instruction address
  mutable JumpCache: SortedList<Addr, Addr>

  JumpTargetRefCache: SortedList<Addr, HashSet<Addr>>

  /// Type Addr set
  /// Set of function entry address
  FunctionCache: HashSet<Addr>

  /// Type Addr set
  /// Set of endbr instruction address
  EndbrCache: HashSet<Addr>

  /// Type Addr set
  /// Set of endbr-func address
  ReturnTwiceCache: HashSet<Addr>

  /// Type (Addr, int) SortedList
  /// Store function boundary in exception table
  mutable ExceptionCache: SortedList<Addr, int>

  mutable NumException: int
  mutable NumReturnTwice: int
}

module Cache =
  let initCache hdl = {
    Handle = hdl
    SupersetCache = SortedList<Addr, Instruction> ()
    LinearCache = SortedList<Addr, Instruction> ()
    CallTargetCache = HashSet<Addr> ()
    JumpCache = SortedList<Addr, Addr> ()
    JumpTargetRefCache = SortedList<Addr, HashSet<Addr>> ()
    FunctionCache = HashSet<Addr> ()
    EndbrCache = HashSet<Addr> ()
    ReturnTwiceCache = HashSet<Addr> ()
    ExceptionCache = SortedList<Addr, int> ()
    NumException = 0
    NumReturnTwice = 0
  }

  let setSupersetCache cache (addr: Addr) (inst: Instruction) =
    if cache.SupersetCache.ContainsKey (addr) then ()
    else cache.SupersetCache.[addr] <- inst

  let setLinearCache cache (addr: Addr) (inst: Instruction) =
    if cache.LinearCache.ContainsKey (addr) then ()
    else cache.LinearCache.[addr] <- inst

  let initFunctionCache cache =
    cache.FunctionCache = HashSet<Addr> ()

  let setFunctionCache cache (addr: Addr) =
    cache.FunctionCache.Add(addr) |> ignore

  let setEndbrCache cache (addr: Addr) =
    cache.EndbrCache.Add(addr) |> ignore

  let setCallTargetCache cache (addr: Addr) =
    cache.CallTargetCache.Add(addr) |> ignore
  
  let setJumpCache cache (addr: Addr) (target: Addr) =
    cache.JumpCache.[addr] <- target

  let setReturnTwiceCache cache (addr: Addr) =
    cache.ReturnTwiceCache.Add(addr) |> ignore

  let setExceptionCache cache (addr: Addr) (size: int) =
    cache.ExceptionCache.[addr] <- size

  let setJumpTargetRefCache cache (target: Addr) (source: Addr) =
    if cache.JumpTargetRefCache.ContainsKey(target) |> not then
      cache.JumpTargetRefCache.[target] <- HashSet<Addr> ()
      cache.JumpTargetRefCache.[target].Add(source) |> ignore
    else
      cache.JumpTargetRefCache.[target].Add(source) |> ignore
