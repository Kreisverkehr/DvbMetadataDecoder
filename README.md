# DvbMetadataDecoder

DvbMetadataDecoder is a .NET library for decoding metadata from DVB transport
streams.

## Motivation

This repository is a clean, independent implementation rather than a continuation
of the existing TsDecoder fork. It aims to keep the decoder understandable and
straightforward to improve.

## Project status

The library currently decodes MPEG transport stream packets and selected Service
Information (SI) sections. It includes parsing for PAT, PMT, CAT, TSD, NIT, and
EIT sections, section CRC checking, transport packet adaptation fields, and DVB
text decoding. A growing set of MPEG-2 and DVB descriptors is also available.

This is an early work in progress: protocol and descriptor coverage is incomplete,
and APIs and behavior may change. It is not yet ready for production use.

## Samples

The [Samples](Samples/README.md) directory contains scripts for inspecting
transport stream packets and decoded SI sections. They require a transport stream
capture at `Samples/testdata.ts`; captures are not included in the repository.

## Requirements

- .NET 10 SDK (including support for file-based C# apps to run the samples)

## Build

From the repository root:

```sh
dotnet build
```

Run the sample scripts from the repository root:

```sh
dotnet run Samples/ParsePackages.cs
dotnet run Samples/ParseSections.cs
```

## License

DvbMetadataDecoder is available under the MIT license. See [LICENSE](LICENSE).