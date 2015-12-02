using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator
{
    public class QuestionTreeNode : ITreeNode, IEnumerable<string>
    {
        private DatumSchema Schema;
        private Dictionary<string, ITreeNode> Children;
        
        public Datum[] Data
        {
            get;
            protected set;
        }

        public string[] DecidableAttributes
        {
            get;
            protected set;
        }

        public string QuestionAttribute
        {
            get;
            protected set;
        }

        public int Level
        {
            get;
            protected set;
        }

        public ITreeNode this[string attributeValue]
        {
            get
            {
                return Children[attributeValue];
            }
        }

        public IEnumerator<string> GetEnumerator()
        {
            return Children.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Children.Keys).GetEnumerator();
        }

        public QuestionTreeNode(DatumSchema schema, IEnumerable<Datum> data, string[] decidableAttributes)
            : this(0, schema, data, decidableAttributes)
        {
        }

        protected QuestionTreeNode(int level, DatumSchema schema, IEnumerable<Datum> data, string[] decidableAttributes)
        {
            Level = level;
            Schema = schema;
            Data = data.ToArray();
            foreach (string attribute in decidableAttributes)
            {
                if (!schema.Attributes.Contains(attribute))
                {
                    throw new InvalidOperationException(String.Format(
                        "The attribute {0} is not contained within the given schema.",
                        attribute));
                }
            }
            DecidableAttributes = decidableAttributes;
            Compute();
        }

        private void Compute()
        {
            var entropy = Entropy(Data
                .GroupBy(d => d.Answer)
                .Select(g => (double)g.Count()));
            Dictionary<string, double> attributeGains = new Dictionary<string, double>();

            foreach(string attribute in DecidableAttributes)
            {
                double gain = entropy - Remainder(Data, attribute);
                for (int i = 0; i < Level; i++)
                    Console.Write(" |");
                Console.WriteLine(" Gain({0}) = {1:0.######}.", attribute, gain);
                attributeGains.Add(attribute, gain);
            }

            QuestionAttribute = attributeGains
                .OrderByDescending(kvp => kvp.Value)
                .First()
                .Key;

            Children = new Dictionary<string, ITreeNode>();
            var byBest = Data
                .GroupBy(d => d[QuestionAttribute]);

            foreach(var group in byBest)
            {
                for (int i = 0; i < Level; i++)
                    Console.Write(" |");
                Console.WriteLine(" If {0} = {1}:", QuestionAttribute, group.Key);
                Children.Add(group.Key,
                    group.Count() == 1 || group.AllEqual(v => v.Answer) ?
                        (ITreeNode)(new AnswerTreeNode(Level + 1, Schema, group.First().Answer)) :
                        (ITreeNode)(new QuestionTreeNode(Level + 1, Schema, group,
                            DecidableAttributes.Where(a => a != QuestionAttribute).ToArray())));
            }
        }

        private double Remainder(IEnumerable<Datum> data, string attribute)
        {
            return data
                .GroupBy(d => d[attribute])
                .Select(g => ((double)g.Count() / data.Count()) * Entropy(
                                 g.GroupBy(d => d.Answer)
                                  .Select(sg => (double)sg.Count())))
               .Sum();
        }

        /// <summary>
        /// Computes the entropy of a question whose probability distribution
        /// is given by the parameter <c>probabilities</c>.
        /// </summary>
        /// <param name="probabilities">The probability distribution for the answers to
        /// the given question. Note that this does not need to be normalised; if the sum of
        /// the enumerable does not sum to one, it will be scaled down appropriately.</param>
        /// <returns>The information entropy.</returns>
        private double Entropy(IEnumerable<double> probabilities)
        {
            double total = probabilities.Sum(),
                   log2 = Math.Log(2);
            return -probabilities
                .Select(d => d / total)
                .Select(d => d * Math.Log(d) / log2)
                .Sum();
        }
    }
}
