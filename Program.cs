using SimulationFramework;
using SimulationFramework.Desktop;
using SimulationFramework.Drawing;
using SimulationFramework.Input;
using System.Numerics;
using TravellingSalesman;

Start<Program>(new DesktopPlatform());

partial class Program : Simulation
{
    public Alignment center = Alignment.Center; //Assigns the enum to point to the center value
    public int IDCounter = 0; //Counts the IDs for the nodes
    public List<Node> tempNodeList = new List<Node>(); //List of nodes made
    public List<Edge> tempEdgeList = new List<Edge>();//List of edges
    public List<Circle> NodeCircleList = new List<Circle>(); //List of circles created as a result of placing down nodes.

    //function that is run the frame the window is opened.
    public override void OnInitialize()
    {
        
        Graph TSMGraph = new Graph();
        Window.Title = "MST Finder";
    }

    public void renderAllObjects(ICanvas canvas) {
        foreach (Circle circ in NodeCircleList) {
            canvas.DrawCircle(circ);
        }
    }

    public override void OnRender(ICanvas canvas)
    {
        canvas.Clear(Color.White);
        renderAllObjects(canvas);
        //Checks for LMB being pressed down
        if (Mouse.IsButtonPressed(0)) {
            canvas.Fill(Color.Red); //When creating circles, it will make it so that the circles will be filled.
            Node nodeInstance = new Node(IDCounter);
            Vector2 Pos = Mouse.Position; //Finds vector position of the mouses
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
        }
        
        float mouseX, mouseY;
        mouseX = Mouse.Position.X;
        mouseY = Mouse.Position.Y;
        ImGuiNET.ImGui.Text("Mouse X, Y: ");
        ImGuiNET.ImGui.Text(mouseX.ToString());
        ImGuiNET.ImGui.Text(mouseY.ToString());
        ImGuiNET.ImGui.Text(" ");

        ImGuiNET.ImGui.Text("NodeList length and EdgeList length: ");
        ImGuiNET.ImGui.Text(tempNodeList.Count.ToString());
        ImGuiNET.ImGui.Text(tempEdgeList.Count.ToString());
    }
}