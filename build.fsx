// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open DotNetCli

// Properties
let buildDir = "./build/"
let testDir = "./test/"
let testDlls = testDir + "*.Tests.dll"

let appProjects = "src/app/**/*.csproj"
let testProjects = "src/test/**/*.csproj"

// Arguments
let version = getBuildParamOrDefault "version" "0"

// Targets
Target "Clean" (fun _ -> 
    CleanDir buildDir
)

Target "BuildApp" (fun _ -> 
    !! appProjects
        |> MSBuildRelease buildDir "Build"
        |> Log "AppBuild-Output: "
)

Target "BuildTest" (fun _ -> 
    !! testProjects
        |> MSBuildRelease testDir "Build"
        |> Log "AppBuild-Output: "
)

// Target "Test" (fun _ -> 
//     !! testDlls
//         |> xUnit2 (fun p -> { p with HtmlOutputPath = Some(testDir @@ "unit-tests-result.html")})
// )

Target "Default" (fun _ ->
    trace ("Hello World from FAKE " + version)
)

// Dependencies
"Clean"
   // ==> "BuildApp"
    //==> "BuildTest"
    //==> "Test"
    ==> "Default"

// start build
RunTargetOrDefault "Default"