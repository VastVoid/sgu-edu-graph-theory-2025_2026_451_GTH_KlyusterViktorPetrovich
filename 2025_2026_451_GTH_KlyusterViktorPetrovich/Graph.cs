namespace _2025_2026_451_GTH_KlyusterViktorPetrovich;

public class Graph
{
    private readonly Dictionary<string, Dictionary<string, double?>> _adjacencyList;
    private readonly Dictionary<string, Dictionary<string, double?>> _incomingEdges;

    private readonly HashSet<string> _vertices;

    public bool IsDirected { get; }
    public bool IsWeighted { get; }

    public IReadOnlyCollection<string> Vertices => _vertices.ToList().AsReadOnly();

    public Graph() : this(false, false)
    {
    }

    public Graph(bool isDirected, bool isWeighted)
    {
        IsDirected = isDirected;
        IsWeighted = isWeighted;
        _adjacencyList = new Dictionary<string, Dictionary<string, double?>>();
        _incomingEdges = new Dictionary<string, Dictionary<string, double?>>();
        _vertices = new HashSet<string>();
    }

    public Graph(Graph other)
    {
        IsDirected = other.IsDirected;
        IsWeighted = other.IsWeighted;
        _adjacencyList = new Dictionary<string, Dictionary<string, double?>>();
        _incomingEdges = new Dictionary<string, Dictionary<string, double?>>();
        _vertices = new HashSet<string>(other._vertices);

        foreach (var vertex in other._adjacencyList)
        {
            _adjacencyList[vertex.Key] = new Dictionary<string, double?>(vertex.Value);
        }

        foreach (var vertex in other._incomingEdges)
        {
            _incomingEdges[vertex.Key] = new Dictionary<string, double?>(vertex.Value);
        }
    }

    public Graph(string filePath)
    {
        var (isDirected, isWeighted) = ReadGraphType(filePath);
        IsDirected = isDirected;
        IsWeighted = isWeighted;
        _adjacencyList = new Dictionary<string, Dictionary<string, double?>>();
        _incomingEdges = new Dictionary<string, Dictionary<string, double?>>();
        _vertices = new HashSet<string>();
        LoadFromFile(filePath);
    }

    private static (bool isDirected, bool isWeighted) ReadGraphType(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File {filePath} not found.");

        var lines = File.ReadAllLines(filePath)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToList();

        if (lines.Count < 2)
            throw new InvalidDataException("File must have at least 2 lines (direction and weighted flags).");

        var dirFlag = lines[0].ToUpper();
        var wgtFlag = lines[1].ToUpper();

        if (dirFlag != "D" && dirFlag != "U")
            throw new InvalidDataException("First line must be 'D' (directed) or 'U' (undirected).");
        if (wgtFlag != "W" && wgtFlag != "U")
            throw new InvalidDataException("Second line must be 'W' (weighted) or 'U' (unweighted).");

        return (dirFlag == "D", wgtFlag == "W");
    }

    private Graph(IEnumerable<string> vertices, IEnumerable<(string from, string to, double? weight)> edges,
        bool isDirected, bool isWeighted) : this(isDirected, isWeighted)
    {
        foreach (var v in vertices)
        {
            AddVertex(v);
        }

        foreach (var (from, to, weight) in edges)
        {
            AddEdge(from, to, weight);
        }
    }

    public static Graph CreateUndirectedUnweighted(IEnumerable<string> vertices,
        IEnumerable<(string from, string to)> edges)
    {
        var weightedEdges = edges.Select(e => (e.from, e.to, (double?)null));
        return new Graph(vertices, weightedEdges, false, false);
    }

    public static Graph CreateDirectedUnweighted(IEnumerable<string> vertices,
        IEnumerable<(string from, string to)> edges)
    {
        var weightedEdges = edges.Select(e => (e.from, e.to, (double?)null));
        return new Graph(vertices, weightedEdges, true, false);
    }

    public static Graph CreateUndirectedWeighted(IEnumerable<string> vertices,
        IEnumerable<(string from, string to, double weight)> edges)
    {
        var nullableEdges = edges.Select(e => (e.from, e.to, (double?)e.weight));
        return new Graph(vertices, nullableEdges, false, true);
    }

    public static Graph CreateDirectedWeighted(IEnumerable<string> vertices,
        IEnumerable<(string from, string to, double weight)> edges)
    {
        var nullableEdges = edges.Select(e => (e.from, e.to, (double?)e.weight));
        return new Graph(vertices, nullableEdges, true, true);
    }

    public bool HasVertex(string vertex)
    {
        return _vertices.Contains(vertex);
    }

    public bool HasEdge(string from, string to)
    {
        return _adjacencyList.ContainsKey(from) && _adjacencyList[from].ContainsKey(to);
    }

    public double? GetWeight(string from, string to)
    {
        if (!HasEdge(from, to))
            throw new InvalidOperationException($"Edge {from} -> {to} does not exist.");
        return _adjacencyList[from][to];
    }

    public int OutDegree(string vertex)
    {
        if (!HasVertex(vertex))
            throw new InvalidOperationException($"Vertex {vertex} does not exist.");

        if (IsDirected)
            return _adjacencyList[vertex].Count;

        var deg = _adjacencyList[vertex].Count;
        if (_adjacencyList[vertex].ContainsKey(vertex))
            deg++;
        return deg;
    }

    public List<string> GetVerticesWithGreaterOutDegree(string vertex)
    {
        var targetDegree = OutDegree(vertex);
        return _vertices.Where(v => v != vertex && OutDegree(v) > targetDegree).ToList();
    }

    public IReadOnlyDictionary<string, double?> GetNeighbors(string vertex)
    {
        if (!HasVertex(vertex))
            throw new InvalidOperationException($"Vertex {vertex} does not exist.");
        return new Dictionary<string, double?>(_adjacencyList[vertex]);
    }

    public IReadOnlyDictionary<string, double?> GetIncomingNeighbors(string vertex)
    {
        if (!HasVertex(vertex))
            throw new InvalidOperationException($"Vertex {vertex} does not exist.");
        return new Dictionary<string, double?>(_incomingEdges[vertex]);
    }

    public void AddVertex(string vertex)
    {
        if (string.IsNullOrWhiteSpace(vertex))
            throw new ArgumentException("Vertex name cannot be null or empty.");

        if (!_vertices.Add(vertex))
            throw new InvalidOperationException($"Vertex {vertex} already exists.");

        _adjacencyList[vertex] = new Dictionary<string, double?>();
        _incomingEdges[vertex] = new Dictionary<string, double?>();
    }

    public void AddEdge(string from, string to, double? weight = null)
    {
        if (!HasVertex(from))
            throw new InvalidOperationException($"Vertex {from} does not exist.");
        if (!HasVertex(to))
            throw new InvalidOperationException($"Vertex {to} does not exist.");

        if (IsWeighted && weight == null)
            throw new InvalidOperationException("Graph is weighted, weight must be provided.");
        if (!IsWeighted && weight != null)
            throw new InvalidOperationException("Graph is unweighted, weight must not be provided.");

        if (HasEdge(from, to))
            throw new InvalidOperationException($"Edge {from} -> {to} already exists.");

        _adjacencyList[from][to] = weight;
        _incomingEdges[to][from] = weight;

        if (!IsDirected)
        {
            _adjacencyList[to][from] = weight;
            _incomingEdges[from][to] = weight;
        }
    }

    public void RemoveVertex(string vertex)
    {
        if (!HasVertex(vertex))
            throw new InvalidOperationException($"Vertex {vertex} does not exist.");

        foreach (var neighbor in _adjacencyList[vertex].Keys.ToList())
        {
            _incomingEdges[neighbor].Remove(vertex);
            if (!IsDirected)
            {
                _adjacencyList[neighbor].Remove(vertex);
            }
        }

        foreach (var incoming in _incomingEdges[vertex].Keys.ToList())
        {
            _adjacencyList[incoming].Remove(vertex);
            if (!IsDirected)
            {
                _incomingEdges[incoming].Remove(vertex);
            }
        }

        _adjacencyList.Remove(vertex);
        _incomingEdges.Remove(vertex);
        _vertices.Remove(vertex);
    }

    public void RemoveEdge(string from, string to)
    {
        if (!HasEdge(from, to))
            throw new InvalidOperationException($"Edge {from} -> {to} does not exist.");

        _adjacencyList[from].Remove(to);
        _incomingEdges[to].Remove(from);

        if (!IsDirected)
        {
            _adjacencyList[to].Remove(from);
            _incomingEdges[from].Remove(to);
        }
    }

    public List<(string from, string to, double? weight)> GetEdgeList()
    {
        var edges = new List<(string from, string to, double? weight)>();
        var visited = new HashSet<string>();

        foreach (var (from, neighbors) in _adjacencyList)
        {
            foreach (var (to, weight) in neighbors)
            {
                if (IsDirected)
                {
                    edges.Add((from, to, weight));
                }
                else
                {
                    var edgeKey = string.Compare(from, to, StringComparison.Ordinal) < 0
                        ? $"{from}|{to}"
                        : $"{to}|{from}";
                    if (visited.Add(edgeKey))
                    {
                        edges.Add((from, to, weight));
                    }
                }
            }
        }

        return edges;
    }

    public void SaveToFile(string filePath)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        using var writer = new StreamWriter(filePath);

        writer.WriteLine(IsDirected ? "D" : "U");
        writer.WriteLine(IsWeighted ? "W" : "U");

        var isolatedVertices = _vertices.Where(v => _adjacencyList[v].Count == 0 && _incomingEdges[v].Count == 0).ToList();
        foreach (var v in isolatedVertices)
        {
            writer.WriteLine(v);
        }

        var processedUndirected = new HashSet<string>();
        foreach (var (from, neighbors) in _adjacencyList)
        {
            foreach (var (to, weight) in neighbors)
            {
                if (!IsDirected)
                {
                    var key = string.Compare(from, to, StringComparison.Ordinal) < 0
                        ? $"{from}|{to}"
                        : $"{to}|{from}";
                    if (!processedUndirected.Add(key)) continue;
                }

                if (IsWeighted)
                    writer.WriteLine($"{from} {to} {weight}");
                else
                    writer.WriteLine($"{from} {to}");
            }
        }
    }

    private void LoadFromFile(string filePath)
    {
        var lines = File.ReadAllLines(filePath)
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToList();

        foreach (var line in lines.Skip(2))
        {
            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var v0 = parts[0];
            var v1 = parts.Length > 1 ? parts[1] : null;

            if (!HasVertex(v0))
                AddVertex(v0);
            if (v1 != null && !HasVertex(v1))
                AddVertex(v1);

            if (parts.Length == 1)
            {
                continue;
            }
            else if (parts.Length == 2)
            {
                AddEdge(v0, v1!);
            }
            else if (parts.Length == 3)
            {

                if (double.TryParse(parts[2], System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var weight))
                {
                    AddEdge(v0, v1!, weight);
                }
                else
                {
                    throw new InvalidDataException($"Invalid weight value: {parts[2]}");
                }
            }
        }
    }

    public override string ToString()
    {
        var result = new System.Text.StringBuilder();
        result.AppendLine(IsDirected ? "Directed" : "Undirected");
        result.AppendLine(IsWeighted ? "Weighted" : "Unweighted");
        result.AppendLine($"Vertices ({_vertices.Count}): {string.Join(", ", _vertices.OrderBy(v => v))}");
        result.AppendLine("Adjacency list:");

        foreach (var vertex in _vertices.OrderBy(v => v))
        {
            result.Append($"  {vertex}: ");
            var neighbors = _adjacencyList[vertex];
            if (neighbors.Count == 0)
            {
                result.AppendLine("(none)");
            }
            else
            {
                var neighborStrs = new List<string>();
                foreach (var (neighbor, weight) in neighbors)
                {
                    if (IsWeighted)
                        neighborStrs.Add($"{neighbor}({weight})");
                    else
                        neighborStrs.Add(neighbor);
                }

                result.AppendLine(string.Join(", ", neighborStrs));
            }
        }

        return result.ToString();
    }
}
