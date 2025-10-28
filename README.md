# [CSVToSO]
Minimum Unity version: 2021.2
---
## Overview

Tool for converting structured csv config file into Unity Scriptable Object asset. Converting steps: csv files -> .cs files (compilable checked) -> asset files (Scriptable Object). Dependencies:  
 - NugetForUnity to install other required nuget packages.
 - CSVHelper as csv parsing tool. (nuget package).
 - Scriban as text template engine for generating .cs script (nuget package).
 - Microsoft.CodeAnalysis.CSharp for generated .cs script compilable check (nuget package).

### Key Features

* **[c# config class generating]:** customizable auto parsing of c# from csv.
* **[Compilable check]:** check for compilable before write c# script.
* **[Default setup]** sample of default setup with normal config and localize config supported.

---

## Dependency Installation Guide

### 1: NugetNuGetForUnity install
Required for install and manage other nuget packages.
[Offical link](https://github.com/GlitchEnzo/NuGetForUnity).  
Install with git URL (UPM): https://github.com/GlitchEnzo/NuGetForUnity.git?path=/src/NuGetForUnity.

### 2: CSVHelper
[Offical link](https://joshclose.github.io/CsvHelper/).  
Just search it in the Nuget Packages Manager window (NuGet → Manage NuGet Packages).  
For mine I use version 33.1.0. Just go for the latest version and only fallback if they have breaking changes and my code no longer work.

#### 3: Scriban
[Offical link](https://github.com/scriban/scriban).  
Just search it in the Nuget Packages Manager window (NuGet → Manage NuGet Packages).  
For mine I use version 6.4.0. Just go for the latest version and only fallback if they have breaking changes and my code no longer work.  
There is signed and unsigned version, for my package any of them will work.  

#### 4: Microsoft.CodeAnalysis.CSharp
Just search it in the Nuget Packages Manager window (NuGet → Manage NuGet Packages).  
This part is tricky, I can not find any Unity offical document on what version of Roslyn was use in the version of Unity Editor, so I make this package with requirement of c# 9 support. And minimal unity version for that is Unity 2021.2. This package was created in Unity 2022.3 with Microsoft.CodeAnalysis.CSharp version 4.2.  
The error about rulesets can be ignored or you can just delete the rulesets folder of this package.  

---
## Installation Guide
Install with git URL (UPM): https://github.com/BearBeen/CSVToSO.git.  

You should import the sample Default (in the Samples tab of package detail in UPM window). The default assets will be installed into "Assets/Samples/CSVToSO/1.0.0/Default". Default asset:
 - DefaultExecutor (asset): contain configs for folder setup and reference of compilation setup. Set your csv folder, c# folder and Scriptable Object asset folder here.  
 - LocalizeConfigSchema (asset): contain configs for default localized text config parsing. You will mainly interact with this. \[GenAllSO\] button will generate all the Scriptable Object assets.  
 - NormalConfigSchema (asset): contain configs for default normal config parsing. You will mainly interact with this. \[GenAllCs\] button will generate all the c# classes need. \[GenAllSO\] button will generate all the Scriptable Object assets.  
 - DefaultCompileSetup (asset): contain config for assembly name and name space (of your generated config .cs file).  
 - ConfigParser (folder): contain parser for the default normal config and localized text config.

---
## Quick Start & Usage

Each csv config file will generate 1 c# script and 1 Scriptable Object asset.  
Default normal config (any csv file that end with "Cfg" in its name) structure using 5 header rows (each column is a field in the generated c# class):
 - csv_name: just the name of the column, for csv helper to mapping by the ClassMap (can not empty)  
 - name: c# script property name (can not empty).  
 - type: c# script type (can not empty, must be syntax correct).  
 - type_converter: custom c# string parser for csv helper (must be syntax correct).  
 - default_val: defaul value of the field (must be syntax correct).  

Localized text will be combined base on language code, no matter how many files they are. So you can split those text by their context but the final assets will still group by language code only.  
Default locaize text config (any csv file that end with "Lz" in its name) structure using the first column for key, and all the next column for localized text config. It has only 1 header, the enum name of the localized language code (LanguageCode enum).

### Extending
The source code is really small. If you want fully customize, just download and edit it. Or you can override the existing class and make your setting assets from it.