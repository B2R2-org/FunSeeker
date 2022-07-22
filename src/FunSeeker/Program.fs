open FunSeeker.Cache
open FunSeeker.BinUtil
open FunSeeker.Disasm
open FunSeeker.Report

[<EntryPoint>]
let main argv =
  if Array.length argv <> 2 then
    let hdl = createBinHandleFromPath argv.[0]
    let cache = Cache.initCache hdl
    parse cache
    reportType4 cache
    printFunctions cache
  else
    let ty = argv.[1]
    let hdl = createBinHandleFromPath argv.[0]
    let cache = Cache.initCache hdl
    parse cache
    if ty = "type1" then
      reportType1 cache
      printFunctions cache
    elif ty = "type2" then
      reportType2 cache
      printFunctions cache
    elif ty = "type3" then
      reportType3 cache
      printFunctions cache
    elif ty = "type4" then
      reportType4 cache
      printFunctions cache
    elif ty = "type5" then
      reportType5 cache
      printFunctions cache
    elif ty = "fp" then
      reportFP cache
    elif ty = "exception" then
      reportException cache
    elif ty = "endbr" then
      reportEndbr cache
      printFunctions cache
    elif ty = "jump" then
      reportJumpTarget cache
    else 0
