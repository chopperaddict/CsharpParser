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
        public static string [ ] lines { get; set; }
        public static List<String> testbuffer { get; set; } = new ( );
        public static int tbindex { get; set; }
        public static StringBuilder output_public { get; set; } = new ( );
        public static StringBuilder output_internal { get; set; } = new ( );
        public static StringBuilder output_private { get; set; } = new ( );

        static void Main ( string [ ] args )
        {
            string  filename ="",  srcchtext = "";
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
            try
            {
                buffer = File . ReadAllText ( filename );
                lines = buffer . Split ( "\n" );
                string lineend="", fulldeclaration="", procline="", arguments="";
                bool found=false;
                int type=-1;
                int pointer = -1;
                for ( int x = 0 ; x < lines . Length ; x++ )
                {
                    string entry = lines [ x ] . Trim ( ) . ToUpper ( );

                    Debug . WriteLine ( x ) ;
                    //************************//
                    // *** Main entry point  ***
                    //************************//
                    if ( found == false && entry . Contains ( '(' ) )
                    {
                        if ( testbuffer != null )
                            testbuffer . Clear ( );

                        if ( entry . StartsWith ( "PUBLIC" ) || entry . StartsWith ( "STATIC PUBLIC" ) )
                        {
                            lines[x] = StripFormattingChars ( lines [ x ] );
                            type = 1;
                            if ( lines [ x ] . Contains ( ")" ) )
                            {
                                // It's a one liner - process it
                                testbuffer . Add (  lines [ x ] );
                                writeArgs ( testbuffer, type );
                                continue;
                            }
                            else
                            {
                                tbindex = 0;
                                // must  be a multiline declaration
                                testbuffer . Add ( lines [ x ] );
                                found = true;
                                continue;
                            }
                        }
                    }
                    else if ( entry . StartsWith ( "PRIVATE" ) || entry . StartsWith ( "STATIC PRIVATE" ) )
                    {
                        lines [ x ] = StripFormattingChars ( lines [ x ] );
                        type = 2;
                        if ( lines [ x ] . Contains ( ")" ) )
                        {
                            // It's a one liner - process it
                            testbuffer . Add ( lines [ x ] );
                            writeArgs ( testbuffer, type );
                            continue;
                        }
                        else
                        {
                            tbindex = 0;
                            // must  be a multiline declaration
                            testbuffer . Add ( lines [ x ] );
                            found = true;
                            continue;
                        }
                    }
                    else if ( entry . StartsWith ( "INTERN" ) || entry . StartsWith ( "STATIC INTERN" ) )
                    {
                        lines [ x ] = StripFormattingChars ( lines [ x ] );
                        type = 3;
                        if ( lines [ x ] . Contains ( ")" ) )
                        {
                            // It's a one liner - process it
                            testbuffer . Add ( lines [ x ] );
                            writeArgs ( testbuffer, type );
                            continue;
                        }
                        else
                        {
                            tbindex = 0;
                            // must  be a multiline declaration
                            testbuffer . Add ( lines [ x ] );
                            found = true;
                            continue;
                        }
                    }
                    else if(found == false)
                        continue;

                    if ( found == true )
                    {
                        // handle the next line of args
                        if ( lines [ x ] . IndexOf ( " )" ) >= 0 )
                        {
                            testbuffer . Add ( lines [ x ] );
                            writeArgs ( testbuffer, type );
                            found = false;
                            continue;
                        }
                        else
                        {
                            testbuffer . Add ( lines [ x ] );
                            continue;
                        }
                        if ( lines [ x ] . IndexOf ( "PUBLIC" ) >= 0 )
                        {
                            procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "PUBLIC" ),
                                lines [ x ] . IndexOf ( "(" ) );
                            output_public . Append ( $"{lines [ x ]}\n" );
                        }
                        if ( lines [ x ] . IndexOf ( "PRIVATE" ) >= 0 )
                        {
                            procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "PRIVATE" ),
                                lines [ x ] . IndexOf ( "(" ) );
                            output_public . Append ( $"{lines [ x ]}\n" );
                        }
                        if ( lines [ x ] . IndexOf ( "INTERNAL" ) >= 0 )
                        {
                            procline = lines [ x ] . Substring ( lines [ x ] . IndexOf ( "INTERNAL" ),
                                lines [ x ] . IndexOf ( "(" ) );
                            output_public . Append ( $"{lines [ x ]}\n" );
                        }
                    }

                    if ( type != -1 )
                    {
                        // Now parse the argments out
                        int indx = lines [x].IndexOf("(")+1;
                        lineend = lines [ x ] . Substring ( indx );
                        if ( lineend . Length > 0 && ( lineend . Contains ( "," ) ) )
                        {
                            string[] tmp1 = lineend.Split(",");
                            writeArgs ( testbuffer, type );
                        }
                    }

                    else if ( found == true )
                    {
                        if ( type == 1 )
                            output_public . Append ( $"\n\t{lines [ x ]}" );
                        else if ( type == 2 )
                            output_private . Append ( $"\n\t{lines [ x ]}" );
                        else if ( type == 3 )
                            output_internal . Append ( $"\n\t{lines [ x ]}" );
                        if ( entry . Contains ( ")" ) )
                        {
                            found = false;
                            type = -1;
                        }
                    }
                }
                StringBuilder[] sb = CreateOutfile ( output_public, output_private, output_internal );
                File . WriteAllText ( path, "\n\n PUBLIC METHODS\n" );
                File . AppendAllText ( path, sb [ 0 ] . ToString ( ) );
                if ( sb [ 1 ] != null )
                {
                    File . AppendAllText ( path, "\n\n PRIVATE METHODS\n" );
                    File . AppendAllText ( path, sb [ 1 ] . ToString ( ) );
                }
                if ( sb [ 2 ] != null )
                {
                    File . AppendAllText ( path, "\n\n INTERNAL METHODS\n" );
                    File . AppendAllText ( path, sb [ 2 ] . ToString ( ) );
                }
                Console . WriteLine ( $"All Methods parsed !!!!!" );
            }
            catch ( Exception ex )
            {
                
                Debug . WriteLine ( $"Oooops : {ex . Message}" );
            }
        }

        private static string StripFormattingChars( string line)
        {
            if ( line == null )
                return "";
            while ( true )
            {
                if ( line . StartsWith ( "\t" ))
                    line = line . Substring ( 1 );
                else
                    break;
            }
            while ( true )
            {
                if ( line . StartsWith ( "\r" ))
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
                if ( line . EndsWith ( "\r" ) )
                    line = line . Substring ( 0, line . Length - 1 );
                else
                    break;
            }
            return line;
        }
        private static StringBuilder [ ] CreateOutfile ( StringBuilder pub, StringBuilder priv, StringBuilder intern )
        {
            StringBuilder[]  sbcollection =new StringBuilder [3];
            for ( int x = 0 ; x < pub . Length ; x++ )
            {
                string entry = pub [ x ] .ToString(). Trim ( ) . ToUpper ( );
                if ( entry . StartsWith ( "PUBLIC" ) )
                    output_public . Append ( $"{pub [ x ]}\n" );
                if ( entry . StartsWith ( "PRIVATE" ) )
                    output_private . Append ( $"{pub [ x ]}\n" );
                if ( entry . StartsWith ( "INTERNAL" ) )
                    output_internal . Append ( $"{pub [ x ]}\n" );
            }
            sbcollection [ 0 ] = output_public;
            return sbcollection;
        }

        private static void writeArgs ( List<string> data, int type, string line = "" )
        {
            string newliner = "\n";
            for ( int x = 0 ; x < data . Count ; x++ )
            {
                if ( data [ x ] . StartsWith ( "\t" ) )
                {
                    // Strip out any leading tabs
                    while ( true )
                    {
                        data [ x ] = data [ x ] . Substring ( 1 );
                        if ( data [ x ] . StartsWith ( "\t" ) == false )
                            break;
                    }
                    if ( data [ x ] . EndsWith ( "\r" ))
                        data [ x ] = data [ x ] . Substring ( 0, data [ x ]. Length - 1 );
                }
                //for ( int y = 0 ; y < data . Count ; y++ )
                //{
                    if ( x == 0 )
                    {
                        if ( type == 1 )
                            output_public . Append ( $"{data [ x ]}{newliner}\n" );
                        else if ( type == 2 )
                            output_private . Append ( $"{data [ x ]}{newliner}\n" );
                        else if ( type == 3 )
                            output_internal . Append ( $"{data [ x ]}\n" );
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
