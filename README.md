# MiniExcel.CSV.Converter

## Dependencies

- [com.laicasaane.miniexcel](https://github.com/laicasaane/MiniExcel)

# Installation

## Install via Open UPM

You can install this package from the [Open UPM](https://openupm.com/packages/com.laicasaane.miniexcel.csv.converter/) registry.

More details [here](https://github.com/openupm/openupm-cli#installation).

```
openupm add com.laicasaane.miniexcel.csv.converter
```

## Install via Package Manager

Or, you can add this package by opening the **Package Manager** window and entering

```
https://github.com/laicasaane/MiniExcel.CSV.Converter.git?path=Packages/com.laicasaane.miniexcel.csv.converter
```

from the `Add package from git URL` option.

# Usage

Create a settings asset via the menu

```
Assets > Create > MiniExcel > CSV Converter Settings
```

## Settings UI

|                            |Description|
|----------------------------|-----------|
|`Excel Folder`              |Folder that contains `.xlsx` files, relative to the project root.|
|`CSV Folder`                |Folder that contains exported `.csv` files, relative to the project root|
|`Ignore Sheets Start With`  |Will ignore any sheet whose name starts with this string|
|`Post Processor`            |A `ScriptableObject` to process the exported CSV files|
|`Refresh Excel Files` button|Refresh the list of `.xlsx` files|
|`Convert To CSV` button     |Convert only the selected `.xlsx` files into `.csv` files|
|`Excel Files`               |The list of `.xlsx` files.|

## Excel File List

|               |Description|
|---------------|-----------|
|`Checkbox`     |If selected, this `.xlsx` file will be converted.<br>Default is `selected`|
|`Open` button  |Open the `.xlsx` file by any appropriate application (eg. Microsoft Excel).|
|`Locate` button|Show the location of this `.xlsx` in the Explorer.|
|`CSV` button   |Show the location of exported `.csv` files in the Explorer.|
