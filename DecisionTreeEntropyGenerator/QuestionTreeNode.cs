using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DecisionTreeEntropyGenerator.Output;

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

        public Attribute[] DecidableAttributes
        {
            get;
            protected set;
        }

        public Attribute QuestionAttribute
        {
            get;
            protected set;
        }

        public IImmutableDictionary<Attribute, string> KnownValues
        {
            get;
            protected set;
        }

        private string GetKnownValueString()
        {
            return String.Join(", ",
                KnownValues.Select(kv => String.Format("{0}={1}", kv.Key.Name, kv.Value)));
        }

        public int Level
        {
            get;
            protected set;
        }

        public string[] Answers
        {
            get;
            protected set;
        }

        public ITreeNode this[string answer]
        {
            get
            {
                return Children[answer];
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

        public QuestionTreeNode(
            DatumSchema schema,
            IEnumerable<Datum> data,
            Attribute[] decidableAttributes,
            int level = 0,
            IImmutableDictionary<Attribute, string> knownValues = null)
        {
            Level = level;
            Schema = schema;
            Data = data.ToArray();
            KnownValues = knownValues ?? ImmutableDictionary<Attribute, string>.Empty;
            foreach (Attribute attribute in decidableAttributes)
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
            var tableAttributes = Schema.Attributes.Where(a => !a.IsQueryable).Concat(DecidableAttributes);
            Working.Print("Current data subset:");
            Working.Print("");
            Working.Printf(@"\begin{{tabular}}{{{0}l}}", "l ".Repeat(tableAttributes.Count()));
            Working.Printf(@"  {0}{1} \\",
                String.Join(" & ", tableAttributes
                    .Select(a => String.Format(@"\textbf{{{0}}}", a.Name))),
                @" & \textbf{Answer}");
            foreach (var datum in Data)
            {
                Working.Printf(@"  {0} & \textbf{{{1}}} \\",
                    String.Join(" & ", tableAttributes
                        .Select(a => datum[a])),
                    datum.Answer);
            }
            Working.Print(@"\end{tabular}");
            Working.Print("");

            var entropyData = Data
                .GroupBy(d => d.Answer)
                .Select(g => new { Answer = g.Key, Count = (double)g.Count() });

            Working.Printf("Entropy calculation: {0}.",
                String.Join(", ",
                    entropyData
                        .Select(i => String.Format(@"\texttt{{{0}}} occurs $ {1} $ time{2}",
                                         i.Answer,
                                         i.Count,
                                         i.Count == 1 ? "" : "s"))));
            var entropy = Entropy(entropyData
                .Select(ans => ans.Count));
            Working.Printf("$$ {0}={1:0.######} $$",
                EntropyWorking(entropyData.Select(e => (int)e.Count), (int)entropyData.Sum(d => d.Count)),
                entropy);

            Dictionary<Attribute, double> attributeGains = new Dictionary<Attribute, double>();

            foreach (Attribute attribute in DecidableAttributes)
            {
                double remainder = Remainder(Data, attribute),
                    gain = entropy - remainder;
                var remainderData = Data
                    .GroupBy(d => d[attribute])
                    .Select(g => new { Value = g.Key, Count = g.Count() });
                Working.Printf(@"Remainder calculation for \texttt{{{1}}} as follows. " +
                               @"Number of occurrences for each value of \texttt{{{1}}}: {0}.",
                    String.Join(", ",
                        remainderData
                            .Select(i => String.Format(@"\texttt{{{0}}} occurs $ {1} $ time{2}",
                                             i.Value,
                                             i.Count,
                                             i.Count == 1 ? "" : "s"))),
                    attribute.Name);
                Working.Printf("$$ Remainder({2})={0}={1:0.######} $$",
                       String.Join("+", remainderData
                                           .Select(g => String.Format(@"\frac{{{0}}}{{{1}}}\left({2}\right)", g.Count, Data.Count(),
                                               EntropyWorking(
                                                   Data
                                                       .Where(d => d[attribute] == g.Value)
                                                       .GroupBy(d => d.Answer)
                                                       .Select(g2 => g2.Count()), g.Count)))),
                       remainder,
                       attribute.Name);
                Working.Printf("Hence, $ Gain({0}) = H - Remainder({0}) = {1:0.######} $.",
                    attribute.Name,
                    gain);
                Working.Print("");
                attributeGains.Add(attribute, gain);
            }

            var questionAttributeGain = attributeGains
                .OrderByDescending(kvp => kvp.Value)
                .First();
            Working.Printf(@"The information gain from \texttt{{{0}}} is the largest, at $ {1:0.######} $ bits - " +
                           @"therefore, this attribute should form the next decision.",
                           questionAttributeGain.Key,
                           questionAttributeGain.Value);
            QuestionAttribute = questionAttributeGain.Key;

            Children = new Dictionary<string, ITreeNode>();
            var byBest = Data
                .GroupBy(d => d[QuestionAttribute]);
            Answers = byBest
                .Select(g => g.Key)
                .ToArray();

            foreach (var group in byBest)
            {
                /* for (int i = 0; i < Level; i++)
                    Console.Write(" |");
                Console.WriteLine(" If {0} = {1}:", QuestionAttribute, group.Key); */
                Working.Print("");
                Working.Printf(@"Assume \texttt{{{0}}} was chosen for the attribute \texttt{{{1}}}.",
                    group.Key,
                    QuestionAttribute);
                Children.Add(group.Key,
                    group.Count() == 1 || group.AllEqual(v => v.Answer) ?
                        (ITreeNode)(new AnswerTreeNode(Schema, group.First().Answer, group, Level + 1)) :
                        (ITreeNode)(new QuestionTreeNode(Schema, group,
                            DecidableAttributes.Where(a => a != QuestionAttribute).ToArray(),
                            Level + 1,
                            KnownValues.Add(QuestionAttribute, group.Key))));
            }
            Working.Print("");
            Working.Printf(@"This accounts for every possibility of the attribute \texttt{{{0}}} " +
                           "at this level of the decision tree.",
                           QuestionAttribute);
        }

        private string EntropyWorking(IEnumerable<int> probabilities, int total)
        {
            /* 
            return String.Join("", probabilities.Select(e =>
                String.Format(@"-\frac{{{0}}}{{{1}}}\log_2(\frac{{{0}}}{{{1}}})",
                    e,
                    total))); */
            return @"H\left(" + String.Join(", ", probabilities.Select(e =>
                String.Format(@"\frac{{{0}}}{{{1}}}",
                    e,
                    total))) + @"\right)";
        }

        private double Remainder(IEnumerable<Datum> data, Attribute attribute)
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
