# Samples

This directory contains runnable examples for DvbMetadataDecoder. Run them from
the repository root with the .NET 10 SDK:

```sh
dotnet run Samples/ParsePackages.cs
dotnet run Samples/ParseSections.cs
```

Both scripts read `Samples/testdata.ts` using a path relative to the repository
root. Transport stream captures are excluded from the repository, so provide your
own capture at that path before running either script. `ParsePackages.cs` prints
transport packet and adaptation field details;
`ParseSections.cs` reports decoded SI sections, including PAT, PMT, CAT, TSD, NIT,
and EIT sections when present in the input.