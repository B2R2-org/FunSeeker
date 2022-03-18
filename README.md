# FunSeeker
FunSeeker is function identification tool for Intel CET-enabled binary.
FunSeeker leverages the patterns of CET-relevant instructions to find function
entries. The details of the algorithm is in our paper, "How'd Security Benefit
Reverse Engineers? The Implication of Intel CET on Function Identification,"
which will appear in DSN 2022.

## Build & Run

FunSeeker is written in F#, so you need to install [.NET Core SDK
6.0](https://dotnet.microsoft.com/en-us/download). Also, FunSeeker includes
following NuGet packages: [FSharp.Core
](https://www.nuget.org/packages/FSharp.Core/6.0.1) and
[B2R2.FrontEnd.BinInterface
](https://www.nuget.org/packages/B2R2.FrontEnd.BinInterface/0.6.0-alpha)

Next, you should download and build FunSeeker.
```
$ git clone git@github.com:B2R2-org/FunSeeker.git
$ cd FunSeeker/
$ dotnet build -c Release
```

Now, you are ready to run FunSeeker. You can run it with following command
```
$ src/FunSeeker/bin/Release/net6.0/FunSeeker [binary_path]
```

## Docker
TBD

## Authors
This research project has been conducted by [SoftSec Lab](https://softsec.kais.ac.kr) at KAIST.
- Hyungseok Kim
- Junoh Lee
- Soomin Kim
- Seungil Jung
- Sang Kil Cha

## Citation
If you plan to use FunSeeker in your own research. Please consider citing our paper:
@INPROCEEDINGS{kim:dsn:2022,
  author = {Hyungseok Kim and Junoh Lee and Soomin Kim and Seungil Jung and Sang Kil Cha},
  title = {How'd Security Benefit Reverse Engineers? The Implication of Intel CET on Function Identification},
  booktitle = dsn,
  year = 2022
}
