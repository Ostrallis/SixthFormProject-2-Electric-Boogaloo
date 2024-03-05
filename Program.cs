using SimulationFramework;
using SimulationFramework.Desktop;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System.Numerics;
using TravellingSalesman;

Start<Program>(new DesktopPlatform());

partial class Program : Simulation
{
    public bool drawingActive;
    public Alignment center = Alignment.Center; //Assigns the enum to point to the center value
    public int IDCounter = 0; //Counts the IDs for the nodes
    public List<Node> tempNodeList = new List<Node>(); //List of nodes made
    public List<Edge> tempEdgeList = new List<Edge>();//List of edges
    public List<Circle> NodeCircleList = new List<Circle>(); //List of circles created as a result of placing down nodes.
    public List<Tuple<Vector2, Vector2>> EdgePolygonList = new List<Tuple<Vector2, Vector2>>();

    static class Globals {
        public static Graph TSMGraph = new Graph(); 
        public static List<Tuple<double, int, int>> mst = new List<Tuple<double, int, int>>();
    }

    void printAllArrays() {
        //foreach(Node n in tempNodeList) {
        //    Console.WriteLine(n.getLocation().X.ToString() + " " + n.getLocation().Y.ToString());
        //}
        //Globals.mst = Globals.TSMGraph.createMatrix().primsMST();

        Globals.TSMGraph.createMatrix().displayMatrix();

        //foreach(Tuple<double, int, int> curtup in Globals.mst) {
        //    Console.WriteLine(curtup.Item1.ToString() + " " + curtup.Item2.ToString() + " " + curtup.Item3.ToString());
        //}
    }

    //function that is run the frame the window is opened.
    public override void OnInitialize()
    {
        drawingActive = true;
        Window.Title = "MST Finder";
    }

    public void renderAllObjects(ICanvas canvas) {
        canvas.Clear(Color.White);
        canvas.Fill(Color.Red); //When creating circles, it will make it so that the circles will be filled.
        foreach (Circle circ in NodeCircleList) {
            canvas.DrawCircle(circ);
        }

        canvas.Stroke(Color.Black);
        canvas.StrokeWidth(0);
        foreach (Edge ed in tempEdgeList) {
            Tuple<int, int> curLine = new Tuple<int, int>(ed.getids().Item1, ed.getids().Item2);
            Vector2 firstpos = tempNodeList[curLine.Item1].getLocation();
            Vector2 secondpos = tempNodeList[curLine.Item2].getLocation(); //Grabs the edges and finds the locations of the nodes they're connected to
            EdgePolygonList.Add(new Tuple<Vector2, Vector2>(firstpos, secondpos));

            //Console.WriteLine(firstpos.X.ToString() + " " + firstpos.Y.ToString() + " " + secondpos.X.ToString() + " " + secondpos.Y.ToString()); //Used to debug the locations that the edges go between
            canvas.DrawLine(firstpos, secondpos);
        }
    }

    public override void OnRender(ICanvas canvas)
    {
        //Checks for LMB being pressed down
        if (Mouse.IsButtonPressed(0) && drawingActive) {
            Node nodeInstance = new Node(IDCounter);
            Vector2 Pos = Mouse.Position; //Finds vector position of the mouses
            nodeInstance.setLocation(Pos);
            Circle CurCirc = new Circle(Pos, 20, center);
            NodeCircleList.Add(CurCirc);


            //After it creates a node, it will create edges connecting that node to the one that was just created.
            foreach (Node cNode in tempNodeList) {
                Edge tempEdge = new Edge(IDCounter, cNode.getNodeID(), 0);
                tempEdgeList.Add(tempEdge);
            }
            tempNodeList.Add(nodeInstance);


            //Assigns edge weights to the edges when they are created in the function above
            foreach (Edge curEdge in tempEdgeList) { 
                if (curEdge.getWeight() == 0) { //Will only check edges that haven't been filled in yet
                    Tuple<int, int> edgeIDs = curEdge.getids();
                    Node curNode1 = tempNodeList[edgeIDs.Item1];
                    Node curNode2 = tempNodeList[edgeIDs.Item2];
    

                    double edgeWeight = Vector2.Distance(curNode1.getLocation(), curNode2.getLocation());
                    curEdge.setWeight(edgeWeight);
                } else {
                    continue;
                }
            }
            IDCounter++;
        }
        
        if (Mouse.IsButtonPressed(MouseButton.Right)) {
            drawingActive = false;
            foreach (Node fornode in tempNodeList) {
                Globals.TSMGraph.AddNode(fornode);
            }
            foreach (Edge cured in tempEdgeList) {
                Globals.TSMGraph.AddEdge(cured);
            }

            printAllArrays();
        }

        //float mouseX, mouseY;
        //mouseX = Mouse.Position.X;
        //mouseY = Mouse.Position.Y;
        //ImGuiNET.ImGui.Text("Mouse X, Y: ");
        //ImGuiNET.ImGui.Text(mouseX.ToString());
        //ImGuiNET.ImGui.Text(mouseY.ToString());
        //ImGuiNET.ImGui.Text(" ");

        //ImGuiNET.ImGui.Text("NodeList length and EdgeList length: ");
        //ImGuiNET.ImGui.Text(tempNodeList.Count.ToString());
        //ImGuiNET.ImGui.Text(tempEdgeList.Count.ToString());
        renderAllObjects(canvas);
    }
}