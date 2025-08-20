# VE Count Consolidator

## Overview
VE Count Consolidator is a C# library and CLI tool for aggregating and processing activity data of Volunteer Examiners (VE) for US amateur radio license exams.

## Project Structure
- **VECountConsolidator/**: Library for data retrieval and aggregation (NuGet compatible)
- **VECountConsolidatorCli/**: Command-line tool utilizing the library
- **VECountConsolidator.Tests/**: Unit tests for the library and CLI (xUnit)

## Unit Testing
Unit tests are provided in the `VECountConsolidator.Tests` directory, using the xUnit framework. These tests cover:
- ARRL data extraction
- Consolidator logic and error handling

To run the tests, use the following command:

```
dotnet test
```

## Supported VEC
- [ARRL](http://www.arrl.org/) (currently only ARRL is supported)

Support for other VECs can be added by implementing the `Consolidator.ICountGetter` interface and updating the `Consolidator` class. See below for details.

## How to Add Support for Other VECs

1. **Create a New Class**
   - Add a new class file (e.g., `XYZVEC.cs`) in the `VECountConsolidator` directory.

2. **Implement the Interface**
   - Implement the `Consolidator.ICountGetter` interface:
     - `string Vec { get; }` property (VEC name)
     - `IEnumerable<Person> Extract()` method (returns a list of Person objects)

3. **Implement Data Extraction**
   - In `Extract()`, write logic to retrieve and parse VE data for your VEC. Use `Utils.GetWeb()` for web requests if needed.

4. **Register Your VEC**
   - Add your VEC to the `VEC` enum in `Consolidator.cs`.
   - Update the `Process()` method in `Consolidator.cs` to instantiate your new class when your VEC is selected.

5. **Example Skeleton**
   ```csharp
   internal class XYZVEC : Consolidator.ICountGetter
   {
       public string Vec => "XYZVEC";
       public IEnumerable<Consolidator.Person> Extract()
       {
           // Implement data extraction and parsing logic here
           // Return a list of Person objects
       }
   }
   ```

6. **Update Documentation and Tests**
   - Document your new VEC in the README and code comments.
   - Add or update tests to verify correct data extraction.

## VECountConsolidator Library
- NuGet: [VECountConsolidator](https://www.nuget.org/packages/VECountConsolidator/)
- Supported .NET: .NET 5.0, .NET Core 3.0

### Basic Usage
Call the `Process()` method with `VEC.ARRL` as an argument.

The return value is a list of the following type:

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

## VECountConsolidatorCli (Command Line Tool)

### Usage
```
dotnet run --project VECountConsolidatorCli [options]
```

## Build Instructions
```
dotnet build VE_Count_Consolidator.sln
```

## License
This project is licensed under the MIT License.
