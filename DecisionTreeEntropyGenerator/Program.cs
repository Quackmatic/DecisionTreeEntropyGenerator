using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DecisionTreeEntropyGenerator.Output;

namespace DecisionTreeEntropyGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            XDocument dataSetDocument = XDocument.Parse(Properties.Resources.MovieData);
            XElement dataSetElement = dataSetDocument.Root;
            DataSet dataSet = DataSet.FromXml(dataSetElement);

            Working.Print = s => Console.WriteLine(s);
            QuestionTreeNode node = dataSet.GenerateNode();
            Working.Print = null;
            Console.WriteLine();
            Console.WriteLine();

            ITreeGenerator<string> generator = new TikzTreeGenerator();
            Console.WriteLine(generator.Generate(node));
            Console.ReadKey();
        }
    }
}
