using System.Collections.Generic;

namespace DecisionTreeEntropyGenerator
{
    public interface ITreeNode
    {
        Datum[] Data
        {
            get;
        }
    }
}
