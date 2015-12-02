using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator.Output
{
    public interface ITreeGenerator<TOutput>
    {
        TOutput Generate(ITreeNode node);
    }
}
