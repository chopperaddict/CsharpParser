using System . Diagnostics;
using System . IO;
using System . Text;
using System;
using System . Diagnostics . Tracing;
using System . Reflection;
using System . ComponentModel . Design;

namespace CsharpParser
{
    internal class Program
    {
        private static string path = "";
        public static string buffer { get; set; }
        public static string capsbuffer { get; set; }
        public static string [ ] lines { get; set; }
        public static List<String> testbuffer { get; set; } = new ( );
        public static int tbindex { get; set; }
        public static StringBuilder output_public { get; set; } = new ( );
        public static StringBuilder output_internal { get; set; } = new ( );
        public static StringBuilder output_private { get; set; } = new ( );

        static void Main ( string [ ] args )
        {
            string  filename ="",  srcchtext = "", upperline="";
            path = @"C:\wpfmain\AllMethods.txt";
            if ( args . Length == 0 )
            {
                Console . Write ( "Enter filename : " );
                filename = Console . ReadLine ( );
            }
            else
            {
                filename = args [ 0 ] . Trim ( );
                srcchtext = args [ 1 ] . Trim ( );
            }
            buffer = File . ReadAllText ( filename );
            capsbuffer = buffer . ToUpper ( );
            lines = buffer . Split ( "\n" );
            string procline="";
            bool found=false;
            int type=-1;

            for ( int x = 0 ; x < lines . Length ; x++ )
            {
                try
                {
                    upperline = lines [ x ] . Trim ( ) . ToUpper ( );

                    // condition has to FAIL to break;
                    //           Debug . Assert ( x != 649, "reached lne 649" );

                    //************************//
                    // *** Main entry point  ***
                    //************************//
                    if ( found == false && upperline . Contains ( '(' ) )
                    {
                        // Got the start of a procedure ?
                        if ( testbuffer != null )
                            testbuffer . Clear ( );

                        if ( ( upperline . StartsWith ( "PUBLIC" ) || upperline . StartsWith ( "STATIC PUBLIC" ) )
                            || ( upperline . StartsWith ( "PRIVATE" ) || upperline . StartsWith ( "STATIC PRIVATE" ) )
                            || ( upperline . StartsWith ( "INTERNAL" ) || upperline . StartsWith ( "STATIC INTERNAL" ) ) )
                        {
                            List<string>headerbuff  = new ();
                            upperline = lines [ x ] . ToUpper ( );
                            if ( upperline . Contains ( "(" ) && upperline . Contains ( ")\r" ) )
                            {
                                lines [ x ] = StripFormattingChars ( lines [ x ] . Trim ( ) );
                                upperline = lines [ x ] . ToUpper ( );
                                int a = upperline.IndexOf("(");
                                AddProcRow ( $"\n{lines [ x ] . Substring ( 0, a ) . Trim ( )}", 1, true );
                                int b = upperline.IndexOf(")");
                                string argsonly = lines[x].Substring(++a, lines[x].Length -(a + 1) ).Trim() ;
                                string []allargs = argsonly.Split(",");
                                if ( allargs . Length > 1 )
                                {
                                    for ( int y = 0 ; y < allargs . Length ; y++ )
                                    {
                                        if ( y == allargs . Length - 1 )
                                            AddProcRow ( $"{allargs [ y ]} )", 1, false );
                                        else
                                            AddProcRow ( $"{allargs [ y ]},", 1, false );
                                    }
                                }
                                else
                                {
                                    if( allargs . Contains("(") && allargs . Contains ( ")" ) )
                                        AddProcRow ( $"{allargs [ 0 ]} )", 1, false );
                                }
                                continue;
                            }
                            continue;
                        }
                        if ( found == true )
                        {
                            //    // handle the next line of args
                            //    if ( lines [ x ] . IndexOf ( " )" ) >= 0 )
                            //    {
                            //        lines [ x ] = StripFormattingChars ( lines [ x ] );
                            //        testbuffer . Add ( $"{lines [ x ]}\n" );
                            //        writeArgs ( testbuffer, type );
                            //        found = false;
                            //        continue;
                            //    }
                            //    else
                            //    {
                            //        lines [ x ] = StripFormattingChars ( lines [ x ] );
                            //        testbuffer . Add ( lines [ x ] );
                            //        continue;
                            //    }
                            //    if ( lines [ x ] . IndexOf ( "PUBLIC" ) >= 0 )
                            //    {
                            //        procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "PUBLIC" ),
                            //            lines [ x ] . IndexOf ( "(" ) );
                            //        output_public . Append ( $"{lines [ x ]}" );
                            //    }
                            //    if ( lines [ x ] . IndexOf ( "PRIVATE" ) >= 0 )
                            //    {
                            //        procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "PRIVATE" ),
                            //            lines [ x ] . IndexOf ( "(" ) );
                            //        output_private . Append ( $"{lines [ x ]}" );
                            //    }
                            //    if ( lines [ x ] . IndexOf ( "INTERNAL" ) >= 0 )
                            //    {
                            //        procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "INTERNAL" ),
                            //            lines [ x ] . IndexOf ( "(" ) );
                            //        output_internal . Append ( $"{lines [ x ]}" );
                            //    }
                        }
                    }
                }
                catch ( Exception ex )
                {
                    Debug . WriteLine ( $"Oooops : {ex . Message}" );
                }
            }


            string underline="========================================================================================";
            string sb = CreateOutfile ( output_public, output_private, output_internal );
            File . WriteAllText ( path, filename );
            File . WriteAllText ( path, $"{underline . Substring ( 0, filename . Length )}\n" );
            File . AppendAllText ( path, sb );

            Console . WriteLine ( $"All Methods parsed !!!!!" );
        }
        private static void AddProcRow ( string line, int type, bool IsFirst )
        {
            line = line . Trim ( );
            if ( IsFirst )
            {
                //if(line.Contains(")"))
                if ( type == 1 )
                    output_public . Append ( $"{line} (\n" );
                else if ( type == 2 )
                    output_private . Append ( $"{line} (\n" );
                else if ( type == 3 )
                    output_internal . Append ( $"{line} (\n" );
            }
            else
            {
                //    line+=
                if ( type == 1 )
                    output_public . Append ( $"\t{line}\n" );
                else if ( type == 2 )
                    output_private . Append ( $"\t{line}\n" );
                else if ( type == 3 )
                    output_internal . Append ( $"\t{line}\n" );
                if ( line . Contains ( ")" ) && type == 1 )
                    output_public . Append ( $"\n" );
                else if ( line . Contains ( ")" ) && type == 1 )
                    output_private. Append ( $"\n" );
                else if ( line . Contains ( ")" ) && type == 1 )
                    output_internal. Append ( $"\n" );
            }
        }

        private static string StripFormattingChars ( string line )
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
                    line = line . Substring ( 0, line . Length - 1 );
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
                    line = line . Substring ( 0, line . Length - 1 );
                else
                    break;
            }
            return line;
        }
        private static string CreateOutfile ( StringBuilder pub, StringBuilder priv, StringBuilder intern )
        {
            string sbcollection = "";
            for ( int x = 0 ; x < pub . Length ; x++ )
            {
                if ( pub . Length > 0 )
                {
                    string entry = pub [ x ] .ToString(). Trim ( ) . ToUpper ( );
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
            sbcollection += output_public . ToString ( );
            // return a string full of data
            return sbcollection;
        }

        private static void writeArgs ( List<string> data, int type, string line = "" )
        {
            string newliner = "\n";
            for ( int x = 0 ; x < data . Count ; x++ )
            {
                //if ( data [ x ] . StartsWith ( "\t" ) )
                if ( x == 0 )
                {
                    if ( type == 1 )
                        output_public . Append ( $"{data [ x ]}{newliner}\n" );
                    else if ( type == 2 )
                        output_private . Append ( $"{data [ x ]}{newliner}\n" );
                    else if ( type == 3 )
                        output_internal . Append ( $"{data [ x ]}{newliner}\n" );
                }
                else
                {
                    if ( type == 1 )
                        output_public . Append ( $"\t{data [ x ]}\n" );
                    else if ( type == 2 )
                        output_private . Append ( $"\t{data [ x ]}\n" );
                    else if ( type == 3 )
                        output_internal . Append ( $"\t{data [ x ]}\n" );
                }
                //}

            }
        }
    }
}
