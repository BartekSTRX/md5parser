using System.Collections.Generic;

namespace Parser.Data.Anim
{
    public class Frame
    {
        public Frame(int index, IList<float> parameters)
        {
            Parameters = parameters;
            Index = index;
        }

        public int Index { get; private set; }
        public IList<float> Parameters { get; private set; }
    }
}