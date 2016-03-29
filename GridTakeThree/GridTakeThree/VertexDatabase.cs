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
        public bool Add(Vertex vertex)
        {
            foreach (Vertex v in vertices)
            {
                if ((int)v.x == (int)vertex.x && (int)v.y == (int) vertex.y)
                {
                    return false;
                }
            }
            vertices.Add(vertex);
            return true;
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
