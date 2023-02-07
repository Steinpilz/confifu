#r "paket:
framework: net6.0
nuget FSharp.Core 6.0.3
nuget Steinpilz.DevFlow.Fake5 prerelease
nuget Microsoft.Build 17.3.2
nuget Microsoft.Build.Framework 17.3.2
nuget Microsoft.Build.Tasks.Core 17.3.2
nuget Microsoft.Build.Utilities.Core 17.3.2
//"

open Fake.Core
open Steinpilz.DevFlow.Fake

let param = Lib.setup <| fun p -> 
    { p with
        PublishProjects = p.AppProjects
        NuGetFeed = 
            { p.NuGetFeed with 
                ApiKey = Environment.environVarOrFail <| "NUGET_API_KEY" |> Some
            }
    }

Target.runOrDefault "Pack"
