using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2 {
    class Vertex {

        static public int vertexId = 0;
        public Vertex parent = null;
        public double lenFromSource = 1000;
        public int id = 0;
        public string name;
        public int x { get; }
        public int y { get; }

        public Vertex(int x, int y, string name) {
            this.x = x;
            this.y = y;
            id = vertexId++;
            this.name = name;
        }
        public List<Egde> egdeList = new List<Egde>();
        public void addEgde(Vertex sourceVertex, Vertex destVertex) {
            egdeList.Add(new Egde(sourceVertex, destVertex));
       }
        public string coords() {
            return $"{x},{y}";
        }
    }
}

