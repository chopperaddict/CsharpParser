using System . Diagnostics;
using System . IO;
using System . Text;
using System;
using System . Diagnostics . Tracing;
using System . Reflection;
using System . ComponentModel . Design;
using System . Security . Cryptography . X509Certificates;
using System . Collections;
using System . Runtime . Versioning;
using System . Xml . Linq;
using System . Reflection . Metadata . Ecma335;
using System . Linq;
using System . Xml . Schema;
using Microsoft . VisualBasic;
using System . Threading . Channels;
using System . Data;

namespace CsharpParser
{
    #region declarations
    internal class Program
    {
        private static int Alllinescount = 0;
        private static int linescount = 0;
        private static string path = "";
        public static string buffer { get; set; }
        public static string capsbuffer { get; set; }
        public static string [ ] lines { get; set; }
        public static string [ ] upperlines { get; set; }
        public static string filename { get; set; } = "";
        public static string [ ] rows { get; set; }
        public static string? [ ] srcchtext { get; set; }

        public static string [ ] filesarray = new string [ 500 ];

        public static string underline = "========================================================================================";
        public static List<String> testbuffer { get; set; } = new ( );
        public static Dictionary<string , string> AllMethods = new ( );
        public static Dictionary<string , string> Errormethods = new ( );
        public static int tbindex { get; set; }
        public static int errorindex { get; set; } = 0;
        public static StringBuilder output_public { get; set; } = new ( );
        public static StringBuilder output_internal { get; set; } = new ( );
        public static StringBuilder output_private { get; set; } = new ( );
        public static string fname = "";

        #endregion declarations

        static void Main ( string [ ] args )
        {
            int filescount = 0;
            IEnumerable<string> AllSourcefiles = null;
            bool DOCONTINUE = false;
            string upperline = "";
            for ( int x = 0 ; x < 500 ; x++ )
            {
                filesarray [ x ] = "";
            }
            path = @"C:\wpfmain\Documentation\AllMethods.txt";
            File . Delete ( path );
            File . Delete ( $@"C:\wpfmain\documentation\AllOutput.txt" );

            if ( args . Length == 0 )
            {
                Console . Write ( "Enter filename : " );
                filename = Console . ReadLine ( );
                if ( filename == "2" )
                {
                    filename = filename = "C:\\Wpfmain\\utils2.cs";
                    filesarray [ 0 ] = filename . Trim ( );
                    filescount = 1;
                }
                else if ( filename == "1" )
                {
                    filename = "C:\\Wpfmain\\SProcsHandling.xaml.cs";
                    filesarray [ 0 ] = filename . Trim ( );
                    filescount = 1;
                }
                else
                {
                    AllSourcefiles = Directory . EnumerateFiles ( @"C:\wpfmain" , "*.*cs" );
                    if ( AllSourcefiles . Count ( ) > 0 )
                    {
                        //                        filesarray = new string [ AllSourcefiles . Count ( ) ];
                        for ( int x = 0 ; x < AllSourcefiles . Count ( ) ; x++ )
                        {
                            string fname2 = AllSourcefiles . ElementAt ( x );
                            filesarray [ x ] = fname2;
                        }
                        filescount = AllSourcefiles . Count ( );
                    }
                }
            }
            else
            {
                filesarray [ 0 ] = args [ 0 ] . Trim ( );
                filescount = 1;
            }

            for ( int z = 0 ; z < filescount ; z++ )
            {
                if ( filesarray [ z ] == null ) return;
                // loop through ALL FILES in array
                filename = filesarray [ z ];

                Debug . WriteLine ( $"{underline . Substring ( 0 , filename . Length + 13 )}\n" );
                Debug . WriteLine ( $"METHODS IN : {filename . Trim ( )}" );
                Debug . WriteLine ( $"{underline . Substring ( 0 , filename . Length + 13 )}\n" );

                // read this file into memory
                buffer = File . ReadAllText ( filesarray [ z ] );
                GetFilenameFromPath ( filename . Trim ( ) , out fname );

                capsbuffer = buffer . ToUpper ( );
                lines = buffer . Split ( "\n" );
                upperlines = capsbuffer . Split ( "\n" );
                Debug . WriteLine ( $"Processing {filename} of {lines . Length} lines" );
                bool found = false;
                int type = -1;

                //****************************************************//
                // loop through next FILE in array and  process all LINES
                //****************************************************//
                for ( int x = 0 ; x < lines . Length ; x++ )
                {
                    try
                    {
                        int testcount = 0;
//                        Debug . Assert ( x != 959 );
                        lines [ x ] = StripFormattingChars ( lines [ x ] );
                        ///upperline = lines [ x ] . Trim ( ) . ToUpper ( );
                        if ( upperlines [ x ] . Length < 10 )
                            continue;
                        lines [ x ] = lines [ x ] . Trim ( );
                        upperlines [ x ] = upperlines [ x ] . Trim ( );


                        // TODO  remove this
                        if ( upperlines [ x ] . StartsWith ( "PUBLIC" ) == false )
                            continue;
                        x = x;
                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
                        if ( upperlines [ x ] . ToUpper ( ) . Contains ( "GETSCROLLSPEED)" ) == true )
                            Debug . Assert ( upperlines [ x ] . ToUpper ( ) . Contains ( "GETSCROLLSPEED)" ) == false );
                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
                        // Discard everything we know we are not interested in first 
                        {
                            if ( upperlines [ x ] . StartsWith ( "PUBLIC" ) || upperlines [ x ] . StartsWith ( "STATIC PUBLIC" ) == false )
                            {
                                testcount++;
                                if ( upperlines [ x ] . StartsWith ( "PRIVATE" ) || upperlines [ x ] . StartsWith ( "STATIC PRIVATE" ) == false )
                                {
                                    testcount++;
                                    if ( upperlines [ x ] . StartsWith ( "INTERNAL" ) || upperlines [ x ] . StartsWith ( "STATIC INTERNAL" ) == false )
                                        testcount++;
                                }
                                if ( testcount != 3 )
                                    continue;
                            }

                            if ( ( upperlines [ x ] . Trim ( ) . StartsWith ( "PUBLIC" ) == true ||
                                upperlines [ x ] . Trim ( ) . StartsWith ( "PRIVATE" ) == true ||
                                upperlines [ x ] . Trim ( ) . StartsWith ( "PROTECTED" ) == true )
                                && ( upperlines [ x ] . Trim ( ) . Contains ( "=" ) == true && upperlines [ x ] . Trim ( ) . Contains ( "NEW" ) == true ) )
                                continue;
                            else if ( upperlines [ x ] . Trim ( ) . StartsWith ( "//" ) == true
                                || upperlines [ x ] . Trim ( ) . StartsWith ( "/*" ) == true
                                || upperlines [ x ] . Trim ( ) . EndsWith ( "*/" ) == true )
                                continue;
                            else if ( upperlines [ x ] . StartsWith ( "//" ) == true )
                                continue;
                            //else if ( found == false && lines [ x ] . Contains ( "(" ) == false )
                            //    continue;
                            else if ( upperlines [ x ] . StartsWith ( "//" ) == true
                                || upperlines [ x ] . StartsWith ( "//*" ) == true
                                || upperlines [ x ] . StartsWith ( "*" ) == true )
                                continue;
                            else if ( upperlines [ x ] . Contains ( "(" ) == false
                                && ( upperlines [ x ] . Contains ( "<" ) == true
                                || upperlines [ x ] . Contains ( ">" ) == true ) )
                                continue;
                            else if ( upperlines [ x ] . Length <= 2 )
                                continue;
                            if ( upperlines [ x ] . Contains ( "=" ) == true && upperlines [ x ] . Contains ( "NEW" ) == true )
                            {
                                // Its a NEW statement, ignore it !
                                int a = upperlines [ x ] . IndexOf ( "=" );
                                int b = upperlines [ x ] . IndexOf ( "NEW" );
                                if ( a < b )
                                    continue;
                            }
                            // Work thru exceptions we do  NOT WANT to list
                            // ignore defaullt delegate declarations
                            if ( upperlines [ x ] . Contains ( "FUNC" ) == true || upperlines [ x ] . Contains ( "ACTION" ) == true )
                                continue;
                        }
                        //************** VERIFY it is a procedure *************//
                        if ( IsProcedure ( upperlines [ x ] ) == false )
                            continue;
                        //****************************************************//


                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
                        //                        Debug . Assert ( upperlines [x] .Contains ( "PARSETABLECOLUMNDATA" ) == false );
                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

                        if ( found == true )
                        {
                            // WE are parsing the following lines in a procedure declaration.
                            lines [ x ] = StripFormattingChars ( lines [ x ] . Trim ( ) );
                            upperline = lines [ x ] . ToUpper ( );

                            Debug . WriteLine ( $"lines[x] ..." );

                            if ( lines [ x ] . Contains ( "," ) && lines [ x ] . EndsWith ( "," ) )
                            {
                                AddProcRow ( $"{lines [ x ]}" , type , isArg: true , crlf: 1 );
                                $"{lines [ x ]}" . log ( );
                                x++;
                            }
                            // get an array of arguments and ouput them all
                            string [ ] rows = ParselinesFromProcedure ( lines , type , ref x );
                            for ( int y = 0 ; y < rows . Length ; y++ )
                            {
                                rows [ y ] = StripFormattingChars ( rows [ y ] . Trim ( ) );
                                AddProcRow ( $"{rows [ y ]}" , type , isArg: true , crlf: 1 );
                                $"{rows [ y ]}" . log ( );

                                if ( y == rows . Length - 1 )
                                {
                                    found = false;
                                    break;
                                }
                            }
                            //AddProcRow ( $"\n" , type , crlf: 1 );
                            continue;
                        }
                        if ( DOCONTINUE )
                        {
                            AddProcRow ( $"{lines [ x ] . Trim ( )}" , type , isArg: true , crlf: 1 );
                            $"{lines [ x ] . Trim ( )}" . log ( );
                            if ( lines [ x ] . Contains ( ");" ) )
                                DOCONTINUE = false;
                            continue;
                        }

                        //************************//
                        // *** Main entry point  ***
                        //************************//

                        // Check for ATTACHED PROPERTY
                        if ( ( upperlines [ x ] . Contains ( "PUBLIC STATIC READONLY DEPENDENCY" )
                            || upperlines [ x ] . Contains ( "DEPENDENCYPROPERTY . REGISTERATTACHED" ) ) )
                        {
                            // Get entire method code as a block of x lines
                            string method = GetMethodBody ( lines [ x ] , lines , ref x );
                            AddProcRow ( $"ATTACHED PROPERTY" , type , isHeader: true , crlf: 1 );
                            AddProcRow ( method , type , crlf: 2 );
                            method . log ( );
                            // AddProcRow ( "\n" , type , crlf: 1 );
                            continue;
                        }

                        //  Set storage type
                        GetStorageType ( upperline , ref type );

                        if ( found == false && upperlines [ x ] . Contains ( '(' ) )
                        {
                            // Got the start of a procedure ?
                            if ( testbuffer != null )
                                testbuffer . Clear ( );

                            // We have already tested for this above and its a procedure mehod !!
                            lines [ x ] = StripFormattingChars ( lines [ x ] . Trim ( ) );

                            // Is it a single line entry ?
                            if ( upperlines [ x ] . Contains ( "(" ) && upperlines [ x ] . Contains ( ")" ) )
                            {
                                // its a oneliner !!
                                // handle method name banner line 1st
                                string [ ] procname = lines [ x ] . Split ( "(" );
                                if ( procname [ 1 ] . Trim ( ) . Contains ( "," ) )
                                {
                                    // got multi arguments
                                    // output  header line
                                    Debug . WriteLine ( procname [ 0 ] . log ( ) );
                                    AddProcname ( $"{procname [ 0 ]} : [ {filename} ]" , type , isFname: true );
                                    string [ ] tmp = procname [ 1 ] . Trim ( ) . Split ( "," );
                                    for ( int w = 0 ; w < tmp . Length ; w++ )
                                    {
                                        if ( w == tmp . Length - 1 )
                                        {
                                            AddProcRow ( $"{tmp [ w ]}" , type , isArg: true , crlf: 2 );
                                            $"{tmp [ w ]}" . log ( );
                                        }
                                        else
                                        {
                                            AddProcRow ( $"{tmp [ w ]}" , type , isArg: true , crlf: 1 );
                                            $"{tmp [ w ]}" . log ( );
                                        }
                                    }
                                    continue;
                                }
                                else
                                {
                                    //only got one argument
                                    if ( procname [ 1 ] . Trim ( ) == ")" )
                                    {
                                        // no arguments, output full line with 2c/r's
                                        AddProcRow ( $"{procname [ 0 ] . Trim ( )} ( )" , type , isAlone: true , crlf: 2 );
                                        $"{procname [ 0 ] . Trim ( )} ( )" . log ( );
                                        continue;
                                    }
                                    else
                                    {
                                        AddProcname ( $"{procname [ 0 ]} : [ {filename} ]" , type , isFname: true );
                                        AddProcRow ( $"{procname [ 1 ] . Trim ( )}" , type , isArg: true , crlf: 2 );
                                        //                                        $"{procname [ 1 ] . Trim ( )}" . log ( );
                                        continue;
                                    }
                                }
                            }
                            else if ( upperlines [ x ] . Contains ( "(" ) )
                            {
                                try
                                {
                                    // it appears to be spread over more than one line
                                    string methodname = $"{lines [ x ] . Substring ( 0 , lines [ x ] . IndexOf ( "(" ) ) . Trim ( )} (";
                                    AddProcname ( $"{methodname} : [{filename}]" , type );
                                    AddProcRow ( $"{methodname}" , type , crlf: 1 );
                                    $"{methodname}" . log ( );

                                    rows = ParselinesFromProcedure ( lines , type , ref x );
                                    for ( int y = 0 ; y < rows . Length ; y++ )
                                    {
                                        rows [ y ] = StripFormattingChars ( rows [ y ] . Trim ( ) );
                                        AddProcRow ( $"{rows [ y ]}" , type , isArg: true , crlf: 1 );
                                        $"{rows [ y ]}" . log ( );

                                        if ( y == rows . Length - 1 )
                                        {
                                            found = false;
                                            break;
                                        }
                                    }
                                    continue;
                                }
                                catch ( Exception ex ) { Debug . WriteLine ( "" ); }
                                continue;
                            }
                        }
                    }
                    catch ( Exception ex )
                    {
                        Debug . WriteLine ( $"Oooops : {ex . Message}" );
                    }
                }
                Alllinescount += filescount;
            }
            Console . WriteLine ( $"Completed  - now creating report  files !!!!!" );
            CreateReports ( );
            string str3 = File . ReadAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" );
            string str2 = $"ALL {Alllinescount} Methods in File or files \ncreated {DateTime . Now . ToString ( )}\n\n";
            File . Delete ( $@"C:\wpfmain\documentation\AllOutput.txt" );
            File . WriteAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , str2 );
            File . AppendAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , str3 );

            Console . WriteLine ( $"{Alllinescount} - All Methods parsed !!!!!" );
        }

        private static void AddProcname ( string line , int type , bool isFname = false )
        {
            string fname = "", leadin = "";
            string [ ] tmp = line . Split ( " : " );
            try
            {
                AddProcRow ( $"*** {tmp [ 1 ]}\n" , type , crlf: 1 );
                AddProcRow ( $"{tmp [ 0 ] . Trim ( )}" , type , isHeader: true , crlf: 1 );
                try
                {
                    //if ( tmp [ 0 ] . Trim ( ) . EndsWith ( "(" ) )
                    //    leadin = tmp [ 0 ] . Trim ( ).Substring ( 0 , leadin . Length - 1 );
                    AllMethods . Add ( $"{tmp [ 0 ] . Trim ( )}" , filename.Trim() );
                }
                catch ( Exception ex )
                {
                    Errormethods . Add ( $"({errorindex++})\n   {filename}" . ToString ( ) , tmp [ tmp . Length - 1 ] . Trim ( ) );
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( "" ); }
        }

        private static void AddProcRow ( string line , int type ,
            //bool isFirst = false , bool isLast = false ,
            bool isAlone = false , bool isArg = false ,
            //bool isMain = false,
            bool isHeader = false ,
            int crlf = 1 )
        {
            try
            {
                line = line . Trim ( );
                // Santy check
                if ( type == -1 ) type = 1;
                string debgoutput = "";

                // handle ful output list file 1st
                //if ( line == "EOF" )
                //    debgoutput = line;
                //else
                //{
                if ( line . Contains ( "Oooops" ) == false && line . Contains ( "Cannot  add" ) == false )
                {
                    if ( isAlone )
                        debgoutput = $"{line}";
                    if ( isArg )
                        debgoutput = $"   {line}";
                    else
                        debgoutput = $"{line}";

                    if ( crlf == 1 )
                        debgoutput += "\n";
                    else if ( crlf == 2 )
                        debgoutput += "\n\n";
                    //}
                }
                File . AppendAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , debgoutput );
                linescount++;

                // handle Dictonary output 2nd
                if ( line == "EOF" )
                    line = "";

                if ( type == 1 )
                    output_public . Append ( $"{line}\n" );
                else if ( type == 2 )
                    output_private . Append ( $"{line}\n" );
                else if ( type == 3 )
                    output_internal . Append ( $"{line} \n" );
            }
            catch ( Exception ex ) { Debug . WriteLine ( "" ); }
            return;
        }

        private static string StripFormattingChars ( string line )
        {
            try
            {
                if ( line == null )
                    return "";
                while ( true )
                {
                    if ( line . StartsWith ( "\t" ) )
                        line = line . Substring ( 1 );
                    else
                        break;
                }
                while ( true )
                {
                    if ( line . EndsWith ( "\t" ) )
                        line = line . Substring ( 0 , line . Length - 1 );
                    else
                        break;
                }
                while ( true )
                {
                    if ( line . StartsWith ( "\r" ) )
                        line = line . Substring ( 1 );
                    else
                        break;
                }
                while ( true )
                {
                    if ( line . EndsWith ( "\r" ) )
                        line = line . Substring ( 0 , line . Length - 1 );
                    else
                        break;
                }
                if ( line . EndsWith ( "\r" ) )
                    line = line . Substring ( 0 , line . Length - 1 );
                if ( line . EndsWith ( "," ) )
                    line = line . Substring ( 0 , line . Length - 1 );
            }
            catch ( Exception ex ) { Debug . WriteLine ( "" ); }
            return line;
        }
        private static string CreateOutfile ( StringBuilder pub , StringBuilder priv , StringBuilder intern )
        {
            string sbcollection = "";
            string publicstring = pub . ToString ( );
            for ( int x = 0 ; x < pub . Length ; x++ )
            {
                if ( pub . Length > 0 )
                {
                    string entry = pub [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "PUBLIC" ) )
                        output_public . Append ( $"{pub [ x ]}\n" );
                }
            }
            for ( int x = 0 ; x < priv . Length ; x++ )
            {
                if ( priv . Length > 0 )
                {
                    string entry = priv [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "PRIVATE" ) )
                        output_private . Append ( $"{priv [ x ]}\n" );
                }
            }
            for ( int x = 0 ; x < intern . Length ; x++ )
            {
                if ( intern . Length > 0 )
                {
                    string entry = intern [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "INTERNAL" ) )
                        output_internal . Append ( $"{intern [ x ]}\n" );
                }
            }
            sbcollection += output_public . ToString ( );
            sbcollection += output_private . ToString ( );
            sbcollection += output_internal . ToString ( );
            // return a string full of data
            return sbcollection;
        }

        // not used
        private static void writeArgs ( List<string> data , int type , string line = "" )
        {
            //    string newliner = "\n";
            //    for ( int x = 0 ; x < data . Count ; x++ )
            //    {
            //        //if ( data [ x ] . StartsWith ( "\t" ) )
            //        if ( x == 0 )
            //        {
            //            if ( type == 1 )
            //                output_public . Append ( $"{data [ x ]}{newliner}\n" );
            //            else if ( type == 2 )
            //                output_private . Append ( $"{data [ x ]}{newliner}\n" );
            //            else if ( type == 3 )
            //                output_internal . Append ( $"{data [ x ]}{newliner}\n" );
            //        }
            //        else
            //        {
            //            if ( type == 1 )
            //                output_public . Append ( $"\t{data [ x ]}\n" );
            //            else if ( type == 2 )
            //                output_private . Append ( $"\t{data [ x ]}\n" );
            //            else if ( type == 3 )
            //                output_internal . Append ( $"\t{data [ x ]}\n" );
            //        }
            //        //}

            //    }
        }
        public static string GetFilenameFromPath ( string path , out string fullpath )
        {
            string output = "";
            fullpath = path;
            if ( path == "" )
                return "";
            try
            {
                string [ ] parts = path . Split ( "\\" );
                output = parts [ parts . Length - 1 ];
                fullpath = "";
                for ( int x = 0 ; x < parts . Length - 1 ; x++ )
                {
                    fullpath += $"{parts [ x ]}\\";
                }
                return output;
            }
            catch ( Exception ex ) { Debug . WriteLine ( "" ); }
            return output;
        }
        private static void CreateErrorsFile ( Dictionary<string , string> Errormethods )
        {
            SortedList<string , string> sortlist = new ( );
            string output = "";
            string string3 = "duplicate procedure names ??\n==================\n\nThese methods could not be added to our ALLMETHODS dictionary, so they are\neither duplicates in a different file, or Overloaded versions, probably in the same file.\n\nThis data  is created  by running\nthe [C:\\users\\ianch\\repos\\CSharpParser] Console App that\ncan handle single files or complete directories.\nIt does NOT currently parse directories iteratively\n";
            string date = DateTime . Now . ToString ( );
            string3 += $"Data Created : {date}\n\n";
            output = string3;
            foreach ( KeyValuePair<string , string> item in Errormethods )
            {
                string str = $"{item . Value} : {item . Key} ";
                output += $"{str}\n";
                string [ ] fn = item . Key . Split ( "  " );
                sortlist . Add ( $"{item . Value} : {fn [ 0 ]}" , $"{fn [ 1 ]}  :  {fn [ 0 ]}" );
            }
            File . WriteAllText ( $@"C:\wpfmain\Documentation\Errorslist.txt" , output );
            string3 = "Duplicated Methods\n==============\n\nThis data  is created  by running\nthe [C:\\users\\ianch\\repos\\CSharpParser] Console App that\ncan handle single files or complete directories.\nIt does NOT currently parse directories iteratively\n";
            date = DateTime . Now . ToString ( );
            string3 += $"Data Created : {date}\n\n";
            List<string> AllProcnames = new ( );
            foreach ( KeyValuePair<string , string> item in sortlist )
            {
                string [ ] string1;
                string [ ] string2;
                string1 = item . Key . Split ( ":" );
                string2 = item . Value . Split ( ":" );
                string procname = string1 [ 0 ] . Substring ( 0 , string1 [ 0 ] . Length - 2 );
                string [ ] pname = procname . Trim ( ) . Split ( " " );
                AllProcnames . Add ( pname [ pname . Length - 1 ] );
                string3 += $"{string1 [ 0 ] . Substring ( 0 , string1 [ 0 ] . Length - 2 ) . PadRight ( 60 )} {string2 [ 1 ]}\n";
            }
            var sortedList = AllProcnames . OrderBy ( x => x ) . ToList ( );
            File . WriteAllText ( $@"C:\wpfmain\Documentation\Duplicateslist.txt" , string3 );
        }
        private static string [ ] ParselinesFromProcedure ( string [ ] lines , int type , ref int x )
        {
            x++;
            string [ ] args = lines [ x ] . Split ( "," );
            string [ ] output = new string [ args . Length ];
            try
            {
                if ( args . Length > 1 )
                {
                    for ( int y = 0 ; y < args . Length ; y++ )
                    {
                        args [ y ] = StripFormattingChars ( args [ y ] . Trim ( ) );
                        if ( y == args . Length - 1 )
                            output [ y ] = $"{args [ y ]} )\n";
                        else
                            output [ y ] = $"{args [ y ]}\n ";
                    }
                    return output;
                }
                else
                {
                    string [ ] str = new string [ 1 ];
                    str [ 0 ] = lines [ x ];
                    return str;
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( "" ); }
            return lines;
        }
        private static bool IsProcedure ( string upperline )
        {
            if ( ( upperline . StartsWith ( "PUBLIC" ) || upperline . StartsWith ( "STATIC PUBLIC" ) )
                || ( upperline . StartsWith ( "PRIVATE" ) || upperline . StartsWith ( "STATIC PRIVATE" ) )
                || ( upperline . StartsWith ( "PROTECTED" ) || upperline . StartsWith ( "STATIC PROTECTED" ) )
                || ( upperline . StartsWith ( "INTERNAL" ) || upperline . StartsWith ( "STATIC INTERNAL" ) ) )
                return true;
            return false;
        }
        private static void GetStorageType ( string upperline , ref int type )
        {
            if ( ( upperline . StartsWith ( "PUBLIC" ) || upperline . StartsWith ( "STATIC PUBLIC" ) ) ) type = 1;
            else if ( ( upperline . StartsWith ( "PRIVATE" ) || upperline . StartsWith ( "STATIC PRIVATE" ) ) ) type = 2;
            else if ( ( upperline . StartsWith ( "INTERNAL" ) || upperline . StartsWith ( "STATIC INTERNAL" ) ) ) type = 3;
        }
        /// <summary>
        /// GetMethodBody  Parse out a block of code lines and return the cleaned
        /// up presentation block to caller  to output it.
        /// </summary>
        /// <param name="currentline"></param>
        /// <param name="lines"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        private static string GetMethodBody ( string currentline , string [ ] lines , ref int x )
        {
            // Parse out a block of code lines and return the cleaned
            // up presentation block to caller  to output it.
            int index = x;
            string output = "";
            bool start = true;
            string [ ] args;
            try
            {
                while ( true )
                {
                    if ( start )
                    {
                        // handle first line
                        string [ ] tmpargs = new string [ 1 ];
                        tmpargs [ 0 ] = $"{currentline . TrimStart ( ) . TrimEnd ( )}\n";
                        args = tmpargs [ 0 ] . Split ( "," );
                        for ( int w = 0 ; w < args . Length ; w++ )
                        {
                            if ( tmpargs [ w ] . Contains ( ");" ) )
                                output += $"\t{tmpargs [ w ] . Trim ( )}\n\n";
                            else if ( tmpargs [ w ] . Length > 2 )
                                output += $"{tmpargs [ w ] . Trim ( )}\n";
                        }
                        start = false;
                        index++;
                        continue;
                    }
                    else
                    {
                        args = lines [ index++ ] . Split ( "," );
                    }
                    for ( int w = 0 ; w < args . Length ; w++ )
                    {
                        if ( args [ w ] . Contains ( ");" ) )
                        {
                            output += $"\t{args [ w ] . Trim ( )}\n\n";
                            x = index;
                            return output;
                        }
                        else if ( args [ w ] . Length > 2 )
                            output += $"\t{args [ w ] . Trim ( )}\n";
                    }
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"oops$" ); }
            x = index;
            return output;
        }
        private static void CreateReports ( )
        {
            string sb = output_public . ToString ( );
            sb += output_private . ToString ( );
            sb += output_internal . ToString ( );
            string [ ] ln = sb . Split ( "\n" );
            string [ ] allines = new string [ ln . Length ];

            //==========================================//
            // Create output file ALLMETHODS.TXT
            //==========================================//
            path = @"C:\wpfmain\Documentation\AllMethods.txt";

            File . AppendAllText ( path , $"Methods in : {fname}\n" );
            File . AppendAllText ( path , $"Created : {DateTime . Now . ToString ( )}\n" );
            File . AppendAllText ( path , $"{underline . Substring ( 0 , fname . Length + 13 )}\n\n" );
            File . AppendAllText ( path , $"This list is created after parsing through all C# source files in the \nWpfMain root folder, looking for procedure/Methods" + $" and contains the \ndetails of the {ln . Length} Methods identiified in those file(s)... \n\n" );
            File . AppendAllText ( path , $"{underline . Substring ( 0 , fname . Length + 13 )}\n" );

            for ( int t = 0 ; t < ln . Length ; t++ )
            {
                allines [ t ] = ln [ t ] . Trim( );
                if ( allines [ t ] . StartsWith ( "***" ) )
                    File . AppendAllText ( path , allines [ t ] .Trim() + ")\n" );
                else if ( allines [ t ] . Contains ( "(" ) == false && allines [ t ] . Trim ( ) . Contains ( ")" ) == false )
                    File . AppendAllText ( path , "   " + allines [ t ] . Trim ( ) + ")\n" );
                else if ( allines [ t ] . Contains ( ")" ) == false )
                    File . AppendAllText ( path , allines [ t ] . Trim ( ) + ")\n" );
                else
                    File . AppendAllText ( path , "   " + allines [ t ] . Trim ( ) + "\n\n" );
            }
            File . AppendAllText ( path , "\n\nEnd of File C:\\wpfmain\\Documentation\\AllMethods.txt .....\n" );

            //==========================================//
            // Now create file @"C:\Wpfmain\Documentation\MethodsIndex.txt"
            // data comes from the AllMethods Dictionary entries captured here.
            //==========================================//
            StringBuilder sbMethods = new ( );
            sbMethods . Append ( $"FILENAME = C:\\Wpfmain\\Documentation\\MethodsIndex.txt,\n\n RESULTS - ALL DOCUMENTED METHODS\n\nThis List should be a fairly comprehensive list of all available Methods in the c# files in the root of the WPFMAIN project.\nSome methods names are duplicated in different files, and these are listed in ErrorsList.txt\n\n" );
            sbMethods . Append ( $"Created : {DateTime . Now . ToString ( )}\n\n" );

            foreach ( KeyValuePair<string , string> item in AllMethods )
            {
                string str = $"{item . Value . Trim ( )} : {item . Key . Trim ( )} ";
                sbMethods . Append ( $"{str}\n" );
                Debug . WriteLine ( str );
            }
            Debug . WriteLine ( $"END OF RESULTS - ALL DOCUMENTED METHODS" );


            Debug . WriteLine ( $"ERRORS - DUPLICATE METHOD NAMES" );
            sbMethods . Append ( $"ERRORS - DUPLICATE METHOD NAMES\n" );
            sbMethods . Append ( $"Created : {DateTime . Now . ToString ( )}\n" );
            if ( Errormethods . Count > 0 )
            {
                foreach ( KeyValuePair<string , string> item in Errormethods )
                {
                    string str = $"{item . Value . Trim ( )} : {item . Key . Trim ( )} ";
                    sbMethods . Append ( $"{str}\n" );
                    Debug . WriteLine ( $"{item . Value} : {item . Key} " );
                }
                Debug . WriteLine ( $"END OF ERRORS - DUPLICATE METHOD NAMES" );
                CreateErrorsFile ( Errormethods );
            }
            string content = $"{sbMethods . ToString ( )}\n";
            File . WriteAllText ( @"C:\Wpfmain\Documentation\MethodsIndex.txt" , content );
        }
    }
}
