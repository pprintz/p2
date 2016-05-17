using System.Collections.Generic;

namespace Evacuation_Master_3000 {
    internal class FloorInformation {
        public string Header { get; set; }
        public IReadOnlyList<string> Rows => _rows;
        private readonly List<string> _rows = new List<string>();
    }
}