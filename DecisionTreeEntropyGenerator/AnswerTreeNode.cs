using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeEntropyGenerator.Output;

namespace DecisionTreeEntropyGenerator
{
    public class AnswerTreeNode : ITreeNode
    {
        private DatumSchema Schema;

        public int Level
        {
            get;
            protected set;
        }

        public string Answer
        {
            get;
            protected set;
        }

        public Datum[] Data
        {
            get;
            protected set;
        }

        public AnswerTreeNode(DatumSchema schema, string answer, IEnumerable<Datum> data, int level)
            : this(schema, answer, data.ToArray(), level)
        {

        }

        public AnswerTreeNode(DatumSchema schema, string answer, Datum[] data, int level)
        {
            Level = level;
            if(schema.AnswerValidator(answer))
            {
                /* for (int i = 0; i < Level; i++)
                    Console.Write(" |");
                Console.WriteLine(" Answer: {0}", answer); */
                Answer = answer;
                for(int i = 0; i < data.Length; i++)
                {
                    if(data[i].Answer != Answer)
                    {
                        throw new InvalidOperationException(String.Format(
                            "The datum {0} does not have the answer {1}.", data[i], Answer));
                    }
                }
                Data = data;
                Working.Printf("Every element in the current data subset (\\{{{0}\\}}) now " +
                               "all share the same answer of \\texttt{{{1}}}. Hence, " +
                               "that is the answer that should be declared given this " + 
                               "node of the decision tree has been reached.",
                               String.Join(", ", Data.Select(d => d.ToString())),
                               Answer);
            }
            else
            {
                throw new InvalidOperationException(String.Format(
                    "The answer {0} is not valid with the given schema.", answer));
            }
        }
    }
}