#r "paket:
framework: netstandard20
nuget Steinpilz.DevFlow.Fake5 prerelease
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
