using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astar {
    class Vertex {

        public Vertex parent = null;
        public double lenFromSource = 1000;
        public double lenToDestination;
        public string name;
        public int x { get; }
        public int y { get; }

        public Vertex(int x, int y, string name) {
            this.x = x;
            this.y = y;
            this.name = name;
        }
        public List<Egde> egdeList = new List<Egde>();
        public void addEgde(Vertex sourceVertex, params Vertex[] connectedVertices) {
            foreach (Vertex connectedVertex in connectedVertices) {
                egdeList.Add(new Egde(sourceVertex, connectedVertex));
            }
       }
        public string getCoords() {
            return $"{x},{y}";
        }
    }
}

