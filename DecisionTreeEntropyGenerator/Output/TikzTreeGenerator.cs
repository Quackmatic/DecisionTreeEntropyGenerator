using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecisionTreeEntropyGenerator.Output
{
    public class TikzTreeGenerator : ITreeGenerator<string>
    {
        public string Generate(ITreeNode node)
        {
            return "\\" + Generate(node, null, 0) + ";";
        }

        private string Generate(ITreeNode node, string answer, int level)
        {
            StringBuilder sb = new StringBuilder();
            if(node is AnswerTreeNode)
            {
                AnswerTreeNode answerTreeNode = node as AnswerTreeNode;
                sb.Append(GenerateAnswer(answerTreeNode));
            }
            else if(node is QuestionTreeNode)
            {
                QuestionTreeNode questionTreeNode = node as QuestionTreeNode;
                sb.Append(GenerateQuestion(questionTreeNode, level));
            }
            if(answer != null)
            {
                sb.AppendFormat(
                    "{0}edge from parent node [left] {{{1}}}",
                    Environment.NewLine,
                    answer);
            }
            return sb.ToString().Indent(level, "  ");
        }

        private string GenerateQuestion(QuestionTreeNode node, int level)
        {
            return String.Format(
                "node [question,label=right:{{{3}}}] {{{0}?}}{1}{2}",
                node.QuestionAttribute,
                Environment.NewLine,
                String.Join(Environment.NewLine, node.Answers
                    .Select(a => String.Format("child {{ {0} }}",
                                     Generate(node[a], a, level + 1)))
                    .ToArray()),
                String.Join(@" \\ ", node.Data.GroupBy(d => d.Answer).Select(
                    g => g.Key + ": \\{{" + String.Join(", ", g.Select(d => d.ToString())) + "\\}}")));
        }

        private string GenerateAnswer(AnswerTreeNode node)
        {
            return String.Format(
                "node [answer,label=right:{{\\{{{1}\\}}}}] {{{0}}}",
                node.Answer,
                String.Join(", ", node.Data.Select(d => d.ToString())));
        }
    }
}
