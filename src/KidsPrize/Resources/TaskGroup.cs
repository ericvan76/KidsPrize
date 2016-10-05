using System;
using System.Collections.Generic;

namespace KidsPrize.Resources
{
    public class TaskGroup
    {
        public DateTime EffectiveDate { get; set; }
        public IEnumerable<string> Tasks { get; set; }
    }
}