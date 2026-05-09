using _2025_2026_451_GTH_KlyusterViktorPetrovich;

public static class Demo
{
    public static void Run()
    {
        Console.WriteLine("=== Демонстрация класса Graph ===\n");

        Console.WriteLine("1. Конструктор по умолчанию (пустой неориентированный невзвешенный граф)");
        var defaultGraph = new Graph();
        Console.WriteLine(defaultGraph);
        Console.WriteLine();

        Console.WriteLine("2. Конструктор с параметрами (ориентированный взвешенный)");
        var directedWeighted = new Graph(true, true);
        directedWeighted.AddVertex("A");
        directedWeighted.AddVertex("B");
        directedWeighted.AddVertex("C");
        directedWeighted.AddVertex("D");
        directedWeighted.AddEdge("A", "B", 1.5);
        directedWeighted.AddEdge("B", "C", 2.0);
        directedWeighted.AddEdge("C", "D", 3.0);
        directedWeighted.AddEdge("D", "A", 0.5);
        directedWeighted.AddEdge("A", "C", 1.0);
        Console.WriteLine(directedWeighted);
        Console.WriteLine();

        Console.WriteLine("3. Конструктор-копия");
        var copyGraph = new Graph(directedWeighted);
        Console.WriteLine("Копия исходного графа (с удалением вершины B):");
        copyGraph.RemoveVertex("B");
        Console.WriteLine(copyGraph);
        Console.WriteLine("Исходный граф не изменился:");
        Console.WriteLine(directedWeighted);
        Console.WriteLine();

        Console.WriteLine("4. Конструктор из файла (graph_directed_weighted.txt)");
        var fileGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_directed_weighted.txt");
        Console.WriteLine(fileGraph);
        Console.WriteLine();

        Console.WriteLine("5. Фабричные методы для удобства тестирования");
        var testGraph = Graph.CreateUndirectedUnweighted(
            new[] { "X", "Y", "Z" },
            new[] { ("X", "Y"), ("Y", "Z"), ("X", "Z") }
        );
        Console.WriteLine(testGraph);
        Console.WriteLine();

        Console.WriteLine("=== Демонстрация методов ===");
        Console.WriteLine();

        Console.WriteLine("Создаём пустой неориентированный невзвешенный граф:");
        var g = new Graph(false, false);

        Console.WriteLine("Добавляем вершины: v1, v2, v3, v4, v5");
        g.AddVertex("v1");
        g.AddVertex("v2");
        g.AddVertex("v3");
        g.AddVertex("v4");
        g.AddVertex("v5");

        Console.WriteLine("Добавляем рёбра: v1-v2, v2-v3, v3-v4, v1-v4, v4-v4 (петля)");
        g.AddEdge("v1", "v2");
        g.AddEdge("v2", "v3");
        g.AddEdge("v3", "v4");
        g.AddEdge("v1", "v4");
        g.AddEdge("v4", "v4");

        Console.WriteLine(g);

        Console.WriteLine($"Существует ли вершина v3? {g.HasVertex("v3")}");
        Console.WriteLine($"Существует ли ребро v1-v2? {g.HasEdge("v1", "v2")}");

        Console.WriteLine("\nСписок рёбер:");
        foreach (var (from, to, weight) in g.GetEdgeList())
            Console.WriteLine($"  {from} - {to}");

        Console.WriteLine("\nУдаляем ребро v1-v2");
        g.RemoveEdge("v1", "v2");
        Console.WriteLine(g);

        Console.WriteLine("Удаляем вершину v3");
        g.RemoveVertex("v3");
        Console.WriteLine(g);

        Console.WriteLine("Пытаемся удалить несуществующую вершину:");
        try
        {
            g.RemoveVertex("v999");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\nПытаемся добавить существующее ребро:");
        try
        {
            g.AddEdge("v1", "v4");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Ошибка: {ex.Message}");
        }

        Console.WriteLine("\n=== Сохранение и загрузка ===");
        Console.WriteLine("Сохраняем граф в файл 'test_output.txt'");
        g.SaveToFile("test_output.txt");
        Console.WriteLine("Файл сохранён. Загружаем его обратно:");
        var loadedFromFile = new Graph("test_output.txt");
        Console.WriteLine(loadedFromFile);

        Console.WriteLine("\n=== Демонстрация сравнения степени вершин ===");
        Console.WriteLine("Загружаем ориентированный граф из файла:");
        var outDegGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_directed_unweighted.txt");
        Console.WriteLine(outDegGraph);

        var term = outDegGraph.IsDirected ? "out-degree" : "degree";
        foreach (var v in outDegGraph.Vertices)
            Console.WriteLine($"  {term}({v}) = {outDegGraph.OutDegree(v)}");

        Console.WriteLine($"\nВершины с {term} больше, чем у вершины '1':");
        var greater = outDegGraph.GetVerticesWithGreaterOutDegree("1");
        foreach (var v in greater)
            Console.WriteLine($"  {v} ({term}: {outDegGraph.OutDegree(v)})");

        Console.WriteLine("\nЗагружаем неориентированный граф из файла:");
        var undirectedGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_undirected_unweighted.txt");
        Console.WriteLine(undirectedGraph);

        foreach (var v in undirectedGraph.Vertices)
            Console.WriteLine($"  degree({v}) = {undirectedGraph.OutDegree(v)}");

        Console.WriteLine("\nВершины со степенью больше, чем у вершины 'F':");
        var greaterUndir = undirectedGraph.GetVerticesWithGreaterOutDegree("F");
        foreach (var v in greaterUndir)
            Console.WriteLine($"  {v} (degree: {undirectedGraph.OutDegree(v)})");

        Console.WriteLine("\n=== Task II 1.13: SCC (BFS) ===");
        var sccGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_scc_demo.txt");
        Console.WriteLine(sccGraph);
        var sccCount = sccGraph.CountSCC_BFS();
        Console.WriteLine($"Number of strongly connected components (BFS): {sccCount}");

        Console.WriteLine("\n=== Задание: Boruvka MST ===");
        var mstGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_undirected_weighted.txt");
        Console.WriteLine("Original:");
        Console.WriteLine(mstGraph);
        var mst = mstGraph.BoruvkaMST();
        Console.WriteLine("Boruvka MST:");
        Console.WriteLine(mst);
        var mstWeight = mst.GetEdgeList().Sum(e => e.weight ?? 0);
        Console.WriteLine($"Total MST weight: {mstWeight}");

        Console.WriteLine("\n=== Task II 1.19: Check: Make tree with removing 1 vetrex (DFS) ===");
        var treeGraph = new Graph("2025_2026_451_GTH_KlyusterViktorPetrovich/graphs/graph_remove_vertex_tree_demo.txt");
        Console.WriteLine(treeGraph);
        var candidate = treeGraph.FindVertexToRemoveToGetTree_DFS();
        if (candidate != null)
            Console.WriteLine($"Remove vertex '{candidate}' to obtain a tree.");
        else
            Console.WriteLine("No such vertex.");

        Console.WriteLine("=== Демонстрация завершена ===");
    }
}
