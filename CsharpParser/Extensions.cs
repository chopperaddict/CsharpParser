using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Text;
using System . Threading . Tasks;

namespace CsharpParser
{
    public static class ExtensionMethods
    {
        public static string log (
            this string data ,
            [CallerLineNumber] int line = -1 ,
            //[CallerFilePath] string path = null ,
            [CallerMemberName] string name = null )
        {
            string output = "";
            return $"{data} : Ln {line}";
            //output = line < 0 ? "No line  : " : "Line " + $"{line} : ";
            //output += path == null ? "No file path" : $"\t{path}  : ";
            //output += name == null ? " No member name" : $"( {name} )";
            //Debug . WriteLine ( $"{line} : {output}\n" );
        }

    }
}
