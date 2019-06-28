#!/bin/bash
mono .paket/paket.exe restore --references-file src/eShop/paket.references
dotnet restore src/eShop
dotnet build src/eShop
