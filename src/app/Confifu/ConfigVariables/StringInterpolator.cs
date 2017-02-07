using System;
using System.Text;

namespace Confifu.ConfigVariables
{
    public class StringInterpolator
    {
        readonly Func<string, string> variablesReplacer;
        readonly string text;

        public StringInterpolator(string text, Func<string, string> variablesReplacer)
        {
            this.text = text;
            this.variablesReplacer = variablesReplacer;
        }

        public string Interpolate()
        {
            var res = new StringBuilder(text.Length);
            int openBracketsCount = 0;
            int closeBracketsCount = 0;

            var currentVar = new StringBuilder(50);

            foreach(var c in text)
            {
                if (c == '{')
                {
                    closeBracketsCount = 0;
                    openBracketsCount++;
                    if ((openBracketsCount & 1) == 0)
                        res.Append("{");
                    else
                        currentVar.Clear();

                    continue;
                }

                if (c == '}')
                {
                    closeBracketsCount++;
                    if ((closeBracketsCount & 1) == 0)
                        res.Append(c);
                    else if ( (openBracketsCount & 1) == 1)
                    { 
                        // found variable
                        res.Append(variablesReplacer(currentVar.ToString()));
                        currentVar.Clear();
                        openBracketsCount--;
                    }
                    continue;
                }

                if (openBracketsCount % 2 == 1)
                    currentVar.Append(c);
                else
                    res.Append(c);
                
            }

            return res.ToString();
        }
    }
}