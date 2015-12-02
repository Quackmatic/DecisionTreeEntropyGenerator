using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DecisionTreeEntropyGenerator
{
    public class DatumSchema
    {
        public Attribute[] Attributes
        {
            get;
            protected set;
        }

        public Predicate<string> AnswerValidator
        {
            get;
            protected set;
        }

        public int GetAttributeIndex(string attributeName)
        {
            for(int i = 0; i < Attributes.Length; i++)
            {
                if(Attributes[i].Name == attributeName)
                {
                    return i;
                }
            }

            return -1;
        }

        public DatumSchema(
            Attribute[] attributes,
            Predicate<string> answerValidator)
        {
            Attributes = attributes;
            AnswerValidator = answerValidator;
        }

        public static DatumSchema FromXml(XElement element)
        {
            XElement attributesElement = element.Element("Attributes");
            XElement answersElement = element.Element("Answers");

            Attribute[] attributes = attributesElement
                .Elements("Attribute")
                .Select(e => new Attribute(
                        name: e.Value,
                        isIdentifier: e.Attribute("Identifier").GetOrDefault(a => Boolean.Parse(a.Value), false),
                        isQueryable: e.Attribute("Queryable").GetOrDefault(a => Boolean.Parse(a.Value), false)
                    ))
                .ToArray();
            string[] validAnswers = answersElement
                .Elements("Answer")
                .Select(e => e.Value)
                .ToArray();

            return new DatumSchema(
                attributes,
                s => validAnswers.Contains(s));
        }
    }
}
