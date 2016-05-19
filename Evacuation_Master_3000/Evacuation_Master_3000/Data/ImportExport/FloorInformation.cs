using System.Collections.Generic;

namespace Evacuation_Master_3000 {
    internal class FloorInformation {
        public int Level { get; set; }
        public string Header { get; set; }
        public readonly List<string> Rows = new List<string>();
        public string RowInformation
        {
            get { return null; }
            set { Rows.Add(value); }
        }

    }
}