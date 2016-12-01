using System.Collections.Generic;

namespace Parser.Data.Anim
{
    public class Frame
    {
        public Frame(int index, IList<double> parameters)
        {
            Parameters = parameters;
            Index = index;
        }

        public int Index { get; private set; }
        public IList<double> Parameters { get; private set; }
    }
}