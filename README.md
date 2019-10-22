# VE Count Consolidator

## Supported VECs
- [ARRL](http://www.arrl.org/)

The library can be expanded by implementing additional class derived from `VECountConsolidator.Consolidator` class. 

## Components
This repository contains two components:

- VECountConsolidator -- Library that handles fetching and processing of the data.
- VECountConsolidatorCli -- CLI frontend that uses the library above.

### VECountConsolidator
This library is available from [NuGet](https://www.nuget.org/packages/VECountConsolidator/).

Library handling the process. Caller should call Process() function with appropriate `enum` currently only supports `VEC.ARRL`.

Return the list of the following format.

    public class Person
    {
        public string Call;
        public int Count;
        public string Name;
        public State State;
        public string Vec;
    }

    public class State
    {
        public string StateCode;
        public string StateName;
    }

### VECountConsolidatorCli

Command line arguments for VECountConsolidatorCli is below:

    VECountConsolidatorCli 1.5.0.0
    Copyright (c) 2018 Hideki Saito
    ERROR(S):
    Required option 'm, mode' is missing.

      -m, --mode    Required. Mode of operations. (Currently supported: create)

      --help        Display this help screen.

      --version     Display version information.


