using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GridTakeThree
{
    
    class VertexDatabase : IEnumerable<Vertex>
    {
        List<Vertex> vertices;

        /// <summary>
        /// Adds the vertex to the database and returns true if another vertex is not already nearby, else returns false.
        /// </summary>
        /// <param name="vertex">Vertex to add</param>
        /// <returns>Bool, wether or not the vertex was added.</returns>
        public Vertex Add(Vertex vertex)
        {
            foreach (Vertex v in vertices)
            {
                if (Math.Abs(v.x-vertex.x)<0.1 && Math.Abs(v.y-vertex.y)<0.1)
                {
                    return v;
                }
            }
            vertices.Add(vertex);
            return vertex;
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            foreach (Vertex vertex in vertices)
            {
                yield return vertex;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public VertexDatabase()
        {
            vertices = new List<Vertex>();
        }
    }
}
