using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serkit_RT_I
{
    public class StringTokenizer
    {
        private string _string;
        public string String { get { return _string; } set { _string = value; } }

        public StringTokenizer(string _string)
        {
            String = _string.Trim();
        }

        public string NextToken()
        {
            String = String.Trim();
            int blank = String.IndexOf(" ");
            int newline = String.IndexOf("\n");
            int returnline = String.IndexOf("\r");
            if (blank == -1) blank = String.Length;
            if (newline == -1) newline = String.Length;
            if (returnline == -1) returnline = String.Length;
            int min = Math.Min(blank, Math.Min(newline, returnline));
            if(min!=-1)
            {
                string sub = String.Substring(0, min);
                String = String.Substring(min).Trim();
                return sub;
            }
            return null;
        }

        public string NextToken(char delimiter)
        {
            String = String.Trim();
            int index = String.IndexOf(delimiter);
            if(index!=-1)
            {
                if (index == 0)
                    return "";
                string sub = String.Substring(0, index);
                String = String.Substring(index+1);
                return sub;
            }
            return null;
        }
    }
}
