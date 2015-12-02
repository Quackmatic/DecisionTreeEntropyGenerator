using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator.Output
{
    public static class Working
    {
        public static Action<string> Print = null;

        public static void Printf(string s, params object[] o)
        {
            Print(String.Format(s, o));
        }
    }
}
