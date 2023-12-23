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
    if ty = "superendbr" then
      superparse cache
      reportSupersetEndbr cache
      printFunctions cache
    elif ty = "superset" then
      superparse cache
      reportSuperset cache
      printFunctions cache
    else
      parse cache
      if ty = "exception" then
        reportException cache
      elif ty = "endbr" then
        reportEndbr cache
        printFunctions cache
      elif ty = "jump" then
        reportJumpTarget cache
      else 0
