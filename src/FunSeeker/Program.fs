open FunSeeker.Cache
open FunSeeker.BinUtil
open FunSeeker.Disasm
open FunSeeker.Report

[<EntryPoint>]
let main argv =
  let ftype = System.IO.File.GetAttributes argv.[0]
  if ftype = System.IO.FileAttributes.Normal && Array.length argv = 1 then
    let hdl = createBinHandleFromPath argv.[0]
    let cache = Cache.initCache hdl
    parse cache
    reportType4 cache
    printFunctions cache
  elif ftype = System.IO.FileAttributes.Normal && Array.length argv = 2 then
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
  else 0
