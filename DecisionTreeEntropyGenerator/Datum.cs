using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator
{
    public class Datum
    {
        public DatumSchema Schema
        {
            get;
            protected set;
        }

        public string Answer
        {
            get;
            protected set;
        }

        private Dictionary<Attribute, string> Values;

        public string this[Attribute attribute]
        {
            get
            {
                if (Values.ContainsKey(attribute))
                {
                    return Values[attribute];
                }
                else
                {
                    throw new InvalidOperationException(String.Format("This datum has no attribute named {0}.",
                        attribute));
                }
            }
        }

        public Datum(DatumSchema schema, string answer, params string[] values)
        {
            Schema = schema;
            if(values.Length == Schema.Attributes.Length)
            {
                Values = new Dictionary<Attribute, string>();
                for(int i = 0; i < values.Length; i++)
                {
                    Values[Schema.Attributes[i]] = values[i];
                }
            }
            if (schema.AnswerValidator(answer))
            {
                Answer = answer;
            }
        }

        public override string ToString()
        {
            var identifiers = Values
                .Where(kv => kv.Key.IsIdentifier);
            if (identifiers.Count() == 1)
            {
                return identifiers.First().Value;
            }
            else
            {
                return String.Format("({0})",
                    String.Join("; ",
                        identifiers
                            .Select(kv => String.Format("{0}={1}",
                                kv.Key,
                                kv.Value))
                            .ToArray()));
            }
        }
    }
}
