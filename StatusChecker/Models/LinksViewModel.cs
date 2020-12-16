using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatusChecker.Models {
    public class LinksViewModel {
        public string InputLinks { get; set; }

        public IEnumerable<string> Links {
            get { return InputLinks?.Split(Environment.NewLine); }
        }
    }
}
