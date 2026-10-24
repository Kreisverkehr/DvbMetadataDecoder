# DvbMetadataDecoder

DvbMetadataDecoder is a .NET library for decoding metadata from DVB transport
streams.

## Motivation

This repository starts a clean, independent implementation instead of continuing
the existing TsDecoder fork. The fork has become difficult to reason about, which
makes locating and fixing bugs unnecessarily hard. A small, clearly structured
codebase should make the decoder easier to understand, test, and improve.

## Project status

This project is at its initial scaffolding stage. It currently contains an empty
.NET library project and repository setup only. No decoder implementation, public
API, or samples are available yet; those will be designed as the project develops.

The project is a work in progress. APIs and behavior may change, and it is not
ready for production use.

## Requirements

- .NET 10 SDK

## Build

From the repository root:

```sh
dotnet build
```

## License

DvbMetadataDecoder is available under the MIT license. See [LICENSE](LICENSE).