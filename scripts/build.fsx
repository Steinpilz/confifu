// include Fake lib
#r @"..\packages\FAKE\tools\FakeLib.dll"
#r @"..\packages\Steinpilz.DevFlow.Fake\tools\Steinpilz.DevFlow.Fake.Lib.dll"


open Fake
open Steinpilz.DevFlow.Fake 

Lib.setup(fun p -> 
    { p with 
        PublishProjects = !!"src/app/**/*.csproj"
        UseDotNetCliToPack = true
        UseDotNetCliToTest = true
        NuGetFeed = 
            { p.NuGetFeed with 
                ApiKey = environVarOrFail <| "NUGET_API_KEY" |> Some
            }
    }
)

RunTargetOrDefault "Watch"