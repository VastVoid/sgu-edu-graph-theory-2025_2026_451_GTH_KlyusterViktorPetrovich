using _2025_2026_451_GTH_KlyusterViktorPetrovich;

var graph = new Graph();

while (true)
{
    Console.WriteLine();
    Console.WriteLine("=== Graph Console Interface ===");
    Console.WriteLine("  Graph: " + (graph.IsDirected ? "Directed" : "Undirected") + ", " +
                      (graph.IsWeighted ? "Weighted" : "Unweighted"));
    Console.WriteLine("  1. Create new graph");
    Console.WriteLine("  2. Load graph from file");
    Console.WriteLine("  3. Save graph to file");
    Console.WriteLine("  4. Add vertex");
    Console.WriteLine("  5. Add edge");
    Console.WriteLine("  6. Remove vertex");
    Console.WriteLine("  7. Remove edge");
    Console.WriteLine("  8. Print adjacency list");
    Console.WriteLine("  9. Print edge list");
    Console.WriteLine("  o. Degree comparison");
    Console.WriteLine("  l. Print vertices with loops");
    Console.WriteLine("  i. Remove isolated vertices (build new graph)");
    Console.WriteLine("  s. Count SCC (BFS)");
    Console.WriteLine("  t. Find vertex to remove to get tree (DFS)");
    Console.WriteLine("  d. Run demo");
    Console.WriteLine("  0. Exit");
    Console.Write("  Choose: ");

    var choice = Console.ReadLine()?.Trim();

    try
    {
        switch (choice)
        {
            case "1":
                CreateNewGraph(ref graph);
                break;
            case "2":
                LoadGraph(ref graph);
                break;
            case "3":
                SaveGraph(graph);
                break;
            case "4":
                AddVertex(graph);
                break;
            case "5":
                AddEdge(graph);
                break;
            case "6":
                RemoveVertex(graph);
                break;
            case "7":
                RemoveEdge(graph);
                break;
            case "8":
                Console.WriteLine();
                Console.WriteLine(graph);
                break;
            case "9":
                PrintEdgeList(graph);
                break;
            case "o":
            case "O":
                OutDegreeComparison(graph);
                break;
            case "l":
            case "L":
                PrintVerticesWithLoops(graph);
                break;
            case "i":
            case "I":
                graph = graph.WithoutIsolatedVertices();
                Console.WriteLine("Isolated vertices removed. New graph:");
                Console.WriteLine(graph);
                break;
            case "s":
            case "S":
                Console.WriteLine();
                if (!graph.IsDirected)
                {
                    Console.WriteLine("SCC is only defined for directed graphs.");
                }
                else
                {
                    var sccCount = graph.CountSCC_BFS();
                    Console.WriteLine($"Number of strongly connected components (BFS): {sccCount}");
                }
                break;
            case "t":
            case "T":
                Console.WriteLine();
                if (graph.IsDirected)
                {
                    Console.WriteLine("This operation is defined for undirected graphs.");
                }
                else
                {
                    var result = graph.FindVertexToRemoveToGetTree_DFS();
                    if (result == null)
                        Console.WriteLine("No vertex can be removed to obtain a tree.");
                    else
                        Console.WriteLine($"Remove vertex '{result}' to obtain a tree.");
                }
                break;
            case "d":
            case "D":
                Demo.Run();
                break;
            case "0":
                Console.WriteLine("Exiting...");
                return;
            default:
                Console.WriteLine("Invalid option.");
                break;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

static void CreateNewGraph(ref Graph graph)
{
    Console.Write("Directed? (y/n): ");
    var dir = Console.ReadLine()?.Trim().ToLower() == "y";
    Console.Write("Weighted? (y/n): ");
    var wgt = Console.ReadLine()?.Trim().ToLower() == "y";

    var newGraph = new Graph(dir, wgt);

    Console.Write("Add vertices (comma-separated): ");
    var vertices = Console.ReadLine()?.Trim();
    if (!string.IsNullOrEmpty(vertices))
    {
        foreach (var v in vertices.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            newGraph.AddVertex(v);
        }
    }

    graph = newGraph;
    Console.WriteLine("New graph created.");
}

static void LoadGraph(ref Graph graph)
{
    Console.Write("Enter file path: ");
    var path = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(path)) return;

    graph = new Graph(path);
    Console.WriteLine("Graph loaded successfully.");
}

static void SaveGraph(Graph graph)
{
    Console.Write("Enter file path: ");
    var path = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(path)) return;

    graph.SaveToFile(path);
    Console.WriteLine("Graph saved successfully.");
}

static void AddVertex(Graph graph)
{
    Console.Write("Vertex name: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(name)) return;
    graph.AddVertex(name);
    Console.WriteLine($"Vertex '{name}' added.");
}

static void AddEdge(Graph graph)
{
    Console.Write("From vertex: ");
    var from = Console.ReadLine()?.Trim();
    Console.Write("To vertex: ");
    var to = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) return;

    double? weight = null;
    if (graph.IsWeighted)
    {
        Console.Write("Weight: ");
        if (double.TryParse(Console.ReadLine()?.Trim(), System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var w))
            weight = w;
    }

    graph.AddEdge(from, to, weight);
    Console.WriteLine($"Edge '{from} -> {to}' added.");
}

static void RemoveVertex(Graph graph)
{
    Console.Write("Vertex name: ");
    var name = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(name)) return;
    graph.RemoveVertex(name);
    Console.WriteLine($"Vertex '{name}' removed.");
}

static void RemoveEdge(Graph graph)
{
    Console.Write("From vertex: ");
    var from = Console.ReadLine()?.Trim();
    Console.Write("To vertex: ");
    var to = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to)) return;
    graph.RemoveEdge(from, to);
    Console.WriteLine($"Edge '{from} -> {to}' removed.");
}

static void OutDegreeComparison(Graph graph)
{
    var term = graph.IsDirected ? "out-degree" : "degree";
    Console.Write("Enter vertex: ");
    var vertex = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(vertex)) return;

    var deg = graph.OutDegree(vertex);
    Console.WriteLine($"{term} of '{vertex}': {deg}");

    var greater = graph.GetVerticesWithGreaterOutDegree(vertex);
    if (greater.Count == 0)
    {
        Console.WriteLine($"No vertices have greater {term}.");
    }
    else
    {
        Console.WriteLine($"Vertices with greater {term}:");
        foreach (var v in greater)
            Console.WriteLine($"  {v} ({term}: {graph.OutDegree(v)})");
    }
}

static void PrintVerticesWithLoops(Graph graph)
{
    var loops = graph.GetVerticesWithLoops();
    if (loops.Count == 0)
    {
        Console.WriteLine("No vertices have loops.");
    }
    else
    {
        Console.WriteLine("Vertices with loops:");
        foreach (var v in loops)
        {
            var w = graph.IsWeighted ? $" (weight: {graph.GetWeight(v, v)})" : "";
            Console.WriteLine($"  {v}{w}");
        }
    }
}

static void PrintEdgeList(Graph graph)
{
    Console.WriteLine();
    var edges = graph.GetEdgeList();
    Console.WriteLine($"Edges ({edges.Count}):");
    foreach (var (from, to, weight) in edges)
    {
        if (graph.IsWeighted)
            Console.WriteLine($"  {from} -> {to}  weight={weight}");
        else
            Console.WriteLine($"  {from} -> {to}");
    }
}
