using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIServiceUserAgent
{
    class Prediction
    {
        public double Probability { get; set; }

        public string TagId { get; set; }

        public string TagName { get; set; }

        public BoundingBox BoundingBox { get; set; }
    }
}
