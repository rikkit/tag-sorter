namespace TagSorter.Fs

open System.Collections.Generic
open IF.Lastfm.Core
open IF.Lastfm.Core.Api

type Node(name :string, ?links :IDictionary<Node,int>) =
    let mutable _links = defaultArg links (new Dictionary<Node,int>() :> IDictionary<Node,int>)
    
    member this.name = name
    member this.links
        with public get() = _links
        and public set value =
            _links <- value

type public Algo(apiKey :string, apiSecret :string, breadth :int) = 
    let lookup = new Dictionary<string, Node>()
    let client = new LastfmClient(apiKey, apiSecret)
    
    member public this.fillGraphAsync(tag :Node, depth :int) =
        this.fillGraph(tag, depth) |> Async.StartAsTask

    member private this.fillGraph(tag :Node, depth :int) =
        async {
            if (depth <= 0) then return ()
            else
                printfn "Loading %s" tag.name
                let! response = client.Tag.GetSimilarAsync(tag.name) |> Async.AwaitTask

                let result =
                    response
                    |> Seq.take breadth
                    |> Seq.mapi (fun i related ->
                        let node =
                            if (lookup.ContainsKey related.Name) then lookup.[related.Name]
                            else (
                                let newNode = new Node(related.Name)
                                lookup.Add(related.Name, newNode)
                                newNode
                            )

                        printfn "   %s %d" node.name i
                        (node, i)
                    )
                    |> dict

                tag.links <- result

                result
                    |> Seq.map (fun kv ->
                        this.fillGraph(kv.Key, depth - 1)
                    )
                    |> Async.Parallel
                    |> Async.RunSynchronously
                    |> ignore
        }
