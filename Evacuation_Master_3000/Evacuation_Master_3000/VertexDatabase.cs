#region

using System;
using System.Collections;
using System.Collections.Generic;

#endregion

namespace Evacuation_Master_3000
{
    class VertexDatabase : IEnumerable<Vertex>
    {
        private readonly List<Vertex> _vertices;

        public VertexDatabase()
        {
            _vertices = new List<Vertex>();
        }

        public IEnumerator<Vertex> GetEnumerator()
        {
            foreach (Vertex vertex in _vertices)
            {
                yield return vertex;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        ///     Adds the vertex to the database and returns true if another vertex is not already nearby, else returns false.
        /// </summary>
        /// <param name="vertex">Vertex to add</param>
        /// <returns>Bool, wether or not the vertex was added.</returns>
        public Vertex Add(Vertex vertex)
        {
            foreach (Vertex v in _vertices)
            {
                if (Math.Abs(v.X - vertex.X) < 0.1 && Math.Abs(v.Y - vertex.Y) < 0.1)
                {
                    return v;
                }
            }
            _vertices.Add(vertex);
            return vertex;
        }

        public void Remove(Vertex vertex)
        {
            _vertices.Remove(vertex);
        }
    }
}