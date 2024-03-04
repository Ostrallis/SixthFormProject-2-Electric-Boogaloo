using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace TravellingSalesman
{

/*    //Creates a Vector2 which will be useful later for visual implementation.
    class Vector2
    {
        List<double> vectorVals = new List<double> { };

        //Creates a vector with the given values.
        public Vector2(double x, double y)
        {
            vectorVals[0] = x;
            vectorVals[1] = y;
        }

        public double resolveDistanceTo(Vector2 other)
        {

            List<double> newvv = new List<double>();
            newvv[0] = other.vectorVals[0] - vectorVals[0];
            newvv[1] = other.vectorVals[1] - vectorVals[1];

            return Math.Sqrt(newvv[0] * newvv[0] + newvv[1] * newvv[1]);
        }
    }
*/
    
    //Defines the matrix object, as it is a struct it will be stored on the heap instead of the stack so that the matrix itself is globally accessable.
    struct Matrix
    {
        private int inf = int.MaxValue;
        private int size;
        public List<List<double>> l1Matrix = new List<List<double>>();
        private List<int> idList = new List<int>();

        public Matrix(int size)
        {
            this.size = size;
            for (int i = 0; i < size; i++)
            {
                List<double> innerList = new List<double>();
                for (int j = 0; j < size; j++)
                {
                    innerList.Add(0); // Initialize inner list values to 0
                }
                l1Matrix.Add(innerList);
            }
        }

        //increases the size to k x k depending on the required size
        public void increaseSize(int sizediff)
        {
            for (int y = 0; y < sizediff; y++)
            {
                this.l1Matrix.Add(new List<double>());
            }

            for (int b = 0; b < idList.Count; b++)
            {
                this.l1Matrix[b][b] = -1;
            }
        }

        //Boilerplate getters and setters
        public void addValue(double value, int x, int y)
        {
            if (x >= size | y >= size)
            {
                throw new IndexOutOfRangeException("Matrix index out of bounds");
            }

            l1Matrix[x][y] = value;
        }

        public int returnNodeID(int nodeIndex)
        {
            return this.idList[nodeIndex];
        }

        //Returns the value of the matrix at the given index
        public Tuple<List<List<double>>, List<int>> matrixreturn()
        {
            Tuple<List<List<double>>, List<int>> matrixdets = new Tuple<List<List<double>>, List<int>>(this.l1Matrix, this.idList);
            return matrixdets;
        }

        //Displays the matrix
        public void displayMatrix()
        {
            int rownum = 1;
            foreach (List<double> row in l1Matrix)
            {
                int copynum = rownum;
                string rowString = "";

                rowString += "[" + copynum.ToString() + "]";

                foreach (double value in row) { rowString += value.ToString() + " "; }
                Console.WriteLine(rowString);
                rownum++;
            }
        }

        //Using a linear search to find the index of the given node
        //Simpler to implement -> Don't need efficiency as max size is small (~20 ID's max)
        private bool searchIDs(int id, List<int> IDList)
        {
            foreach (int i in IDList)
            {
                if (i == id)
                {
                    return true;
                }
            }
            return false;
        }

        //removes any tuples in which the nodes have already been visited for a list of places which we could possibly go to.
        private void pruneMatrix(ref List<Tuple<double, int, int>> flatMat, List<int> IDList)
        {
            for (int i = 0; i < flatMat.Count - 1; i++)
            {
                if (searchIDs(flatMat[i].Item2, IDList))
                {
                    flatMat.RemoveAt(i);
                }
            }
        }

        //sorts the list of tuples depending on the first value of them.
        private void sortFlatMatrix(ref List<Tuple<double, int, int>> flatMat)
        {
            flatMat.Sort((f, s) => f.Item1.CompareTo(s.Item1));
        }
        
        //returns the index of the node with the lowest value
        private Tuple<double, int> search2dList(List<List<double>> vlist, List<int> vID)
        {
            double current = inf;

            for (int i = 0; i < vID.Count; i++)
            {
                for (int ii = 0; ii < vlist.Count; ii++)
                {
                    if (i == ii) { continue; }
                    if (vlist[i][ii] < current) { current = vlist[i][ii]; vID.Add(ii); }
                }
            }

            return new Tuple<double, int>(current, vID[0]);
        }

        //Implementation of a version of Prim's algorithm which is an algorithm that interacts with the matrix directly
        //Take a copy of the matrix and perform the algorithm on the primary list, keeping it as a pointer list.
        public List<Tuple<double, int, int>> primsMST()
        {
            List<Tuple<double, int, int>> visitedflattened = new List<Tuple<double, int, int>>();
            List<int> visitedIndex = new List<int>();
            List<Tuple<double, int, int>> mst = new List<Tuple<double, int, int>>();
            
            //Prim's relies on a visited an unvisited column, creating these would allow for the algorithm to work.
            foreach (double val in l1Matrix[0])
            {
                Tuple<double, int, int> temptup = new Tuple<double, int, int>(val, 0, 0);
                visitedflattened.Add(temptup);
            } //flattens the matrix into a list of tuples, with the first value being the distance and the second value being the index of the edge.
            
            visitedIndex.Add(0);
            //The algorithm will continue until the list of visited nodes is the same size as the matrix.
            while (visitedIndex.Count != 0)
            {
                pruneMatrix(ref visitedflattened, visitedIndex);
                sortFlatMatrix(ref visitedflattened);
                mst.Add(visitedflattened[0]);
            }

            return mst;
        }
    }

    //Defines the class for the edges that connect the nodes
    class Edge
    {
        int ID1;
        int ID2;
        double weight;

        public Edge(int FID, int SID, double Weight)
        {
            this.ID1 = FID;
            this.ID2 = SID;
            this.weight = Weight;
        }
        //Boilerplate getters and setters
        public double getWeight()
        {
            return weight;
        }

        public void setWeight(double weightt) 
        {
            this.weight = weightt;
        }

        public Tuple<int, int> getids()
        {
            Tuple<int, int> idtupp = new Tuple<int, int>(this.ID1, this.ID2);
            return idtupp;
        }

        public void setIDs(int a, int b)
        {
            this.ID1 = a;
            this.ID2 = b;
        }

    }

    //Defines the nodes that the graph will have.
    class Node
    {
        List<Edge> edges = new List<Edge>();
        int nodeID;
        Vector2 nodeLocation;

        public Node(int ID)
        {
            this.nodeID = ID;
        }

        //Boilerplate getters and setters
        public int getNodeID()
        {
            return nodeID;
        }

        public void AddConnector(Edge Connection)
        {
            edges.Add(Connection);
        }

        public List<Edge> getEdges()
        {
            return edges;
        }

        public void setLocation(Vector2 vctPos) {
            this.nodeLocation = vctPos;
        }
        
        public Vector2 getLocation() {
            return this.nodeLocation;
        }

    }
    //Creating the graph as a class so that I can store it in memory and access it as a standalone object.
    class Graph
    {
        public List<Node> nodeList = new List<Node>();
        List<Edge> edgeList = new List<Edge>();

        //Adds a node to the graph
        public void AddNode(Node newNode) {
            nodeList.Add(newNode);

            List<Edge> newEdges = new List<Edge>();
            foreach (Edge curedge in edgeList) {
                edgeList.Add(curedge);
            }
        }

        void fillMatDiagonal(ref Matrix matrix) {
            int index = 0;
            foreach (List<double> col in matrix.l1Matrix) {
                col[index] = double.MaxValue;
                index++;
            }
        }
        
        //A function to create the matrix from the graph.
        public Matrix createMatrix()
        {
            List<Node> usedNodes = new List<Node>();
            List<Edge> currentEdges = new List<Edge>();
            Matrix finMatrix = new Matrix(nodeList.Count); // Defines a group of nodes and edges + the final matrix

            int currentID = 0; //Takes the ID of the edge we are on to know where to place the weights of each node.

            foreach (Node curNode in this.nodeList)
            { //Loops and assigns the node that it is on to the curnode variable.
                currentID = curNode.getNodeID();
                currentEdges = curNode.getEdges(); //Finds the ID and all the edges that the current node has.

                fillMatDiagonal(ref finMatrix); //Sets the value of the matrix nxn values to a large number so that it is not considered a valid point.

                foreach (Edge curEdge in currentEdges)
                { //Loops through the edges of the current node.
                    Tuple<int, int> idTup = curEdge.getids();

                    finMatrix.addValue(curEdge.getWeight(), idTup.Item1, idTup.Item2);
                    finMatrix.addValue(curEdge.getWeight(), idTup.Item2, idTup.Item1);
                    //Adds the weight of the edge to the matrix and then the value on the inverse side of the matrix (2,1 maps to 1,2).
                }
                usedNodes.Add(curNode);
            }
            return finMatrix; //Returns the final matrix.
        }
    }
}