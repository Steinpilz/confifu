// include Fake lib
#r @"packages\FAKE\tools\FakeLib.dll"
open Fake
open DotNetCli

// Properties
let artifactsDir = "./.artifacts"
let buildDir = artifactsDir + "/build/"
let testDir = artifactsDir + "/test/"
let publishDir = artifactsDir + "/publish/"
let testDlls = testDir + "*.Tests.dll"

let appProjects = "src/app/**/project.json"
let testProjects = "src/test/**/project.json"
let publishProjects = 
    [
        "Confifu.Abstractions", "src/app/Confifu.Abstractions"
        "Confifu", "src/app/Confifu"
    ]
let restoreDir = "src"
// Arguments
let version = getBuildParamOrDefault "version" "0"

// functions

let findGrowl() =
    ["C:\Program Files (x86)\Growl for Windows\growlnotify.exe"; "C:\Program Files\Growl for Windows\growlnotify.exe"] 
        |> Seq.where (fun file -> 
            System.IO.File.Exists(file)
        )
        |> Seq.tryHead

let growlPath = findGrowl()


let runTestsInProject(project: string) =
    tracefn "Running tests in %s" project
    DotNetCli.RunCommand(id) ("test " + project)


let runTests() = 
    tracefn("Running tests...")
    !! testProjects
            |> Seq.iter (fun projects -> 
                runTestsInProject(projects)
            ) 

let packProject(project: string, versionSuffix: Option<string>) = 
    tracefn "Packing project %s" project
    DotNetCli.Pack(fun p -> 
        {
            p with
                Project = project
                Configuration = "Release"
                OutputPath = publishDir
                VersionSuffix = 
                    match versionSuffix with
                        | Some x -> x
                        | None -> ""
        })

// Targets
Target "Clean" (fun _ -> 
    CleanDir artifactsDir
)

Target "Restore" (fun _ -> 
    DotNetCli.Restore(fun p ->
    { p with 
        Project = restoreDir
    })

)

Target "Build" (fun _ -> 
    !! appProjects
        |> Seq.iter (fun projects -> 
            DotNetCli.Build(fun p ->
            { p with 
                Project = projects
                Configuration = "Release"
                
            })
        )
)

Target "Test" (fun _ -> 
    runTests()
)

Target "Watch" (fun _ ->
    use watcher = !! ("src/**/" @@ "*.cs") |> WatchChanges (fun changes ->
        runTests()
    )
    System.Console.ReadLine() |> ignore
    watcher.Dispose()
)

Target "Pack" (fun _ ->
    publishProjects
        |> Seq.iter(fun (project, projectPath) ->
            packProject(projectPath, None)
        )
)

Target "Pack-Pre" (fun _ -> 
    publishProjects
        |> Seq.iter(fun (project, projectPath) ->
            packProject(projectPath, Some (getBuildParamOrDefault "Version" "no-version") )
        ) 
)

Target "Publish" (fun _ -> 
    Paket.Push(fun p ->
        {
            p with
                    ApiKey = environVarOrFail "NUGET_API_KEY"
                    WorkingDir = publishDir
        })
)
//
//Target "Publish-Tags" (fun _ ->
//    Git.Branches.
//)

Target "Default" <| DoNothing
// Dependencies
"Clean"
    ==> "Restore"
    ==> "Build"
    ==> "Default"

"Clean"
    ==> "Pack-Pre"

"Clean"
    ==> "Pack"

// start build
RunTargetOrDefault "Default"