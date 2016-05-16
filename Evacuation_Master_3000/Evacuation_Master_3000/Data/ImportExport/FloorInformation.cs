using System.Collections.Generic;

namespace Evacuation_Master_3000 {
    internal class FloorInformation {
        public int Level { get; set; }
        public string Header { get; set; }
        public IReadOnlyList<string> Rows { get { return _rows; } }
        private List<string> _rows { get; set; } = new List<string>();
        public string RowInformation {
            get { return null; }
            set { _rows.Add(value); }
        }
    }
}