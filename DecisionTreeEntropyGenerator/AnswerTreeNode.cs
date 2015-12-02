using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public AnswerTreeNode(int level, DatumSchema schema, string answer)
        {
            Level = level;
            if(schema.AnswerValidator(answer))
            {
                for (int i = 0; i < Level; i++)
                    Console.Write(" |");
                Console.WriteLine(" Answer: {0}", answer);
                Answer = answer;
            }
            else
            {
                throw new InvalidOperationException(String.Format(
                    "The answer {0} is not valid with the given schema.", answer));
            }
        }
    }
}
