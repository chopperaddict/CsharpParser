using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

namespace CsharpParser
{
    public static class Datastore
    {
        public static string [ ] DictResults = new string [ 2 ];

        public static bool AddProcname ( int lineindex , string line , string pathfilename , int type=1 , bool isFname = false , string [ ] fullMethod = null )
        {
            //Add the Procedure name to our Tuples collection "Alltuples"
            // it is :  public static Tuple<int , string , string , string [ ]>
            // that can contain the whole lot if we have them, as when we get fullMethod !
            string [ ] parts = null;
            string [ ] splitter = null;
            string [ ] procargs=null;
            if ( fullMethod != null )
            {
                // We have the entire procedure call in string[] fullMethod
                procargs = fullMethod;
                splitter = procargs[1].Trim() . Split ( "(" );
                if ( splitter . Length == 1 )
                    return false;

                //for ( int y = 0 ; y < fullMethod . Length ; y++ )
                //{
                //    if ( y == 0)
                //    {
                //        procargs [ y ] = fullMethod [ y ] . Substring ( 0 , procargs [ y ] . Length - 1 );
                //    }
                //    else
                //    procargs [ y ] = fullMethod [ y ] . Trim ( );
                //}
                string [ ] argumentsalone = new string [ procargs . Length - 1 ];
                // add procedure name to row zero
                argumentsalone [ 0 ] = procargs [ 1];
                for (int y = 1 ; y < procargs.Length - 1 ; y++ )
                {
                    //if(y != 2)
                    //    argumentsalone [ y-1 ] = procargs [ y  ];
                    //else
                        argumentsalone [ y ] = procargs [ y+1 ];

                }
                Tuple<int , string , string , string [ ]> t = Tuple . Create ( lineindex , procargs [1] , procargs [ 0 ] , argumentsalone );
                Program . Alltuples [ Program . AllProcsIindex++ ] = t;

            }
            else
            {
                // We have to parse the procedure from the current line received
                parts = line . Split ( " : " );
                if ( parts . Length == 1 )
                    return false;
                string tUpperProcname = parts [ 1 ] . ToUpper ( );
                string UpperFileName = $"{pathfilename . ToUpper ( )}";
                // add all available data into an new Tuple and add to  Alltuples
                splitter = parts [ 1 ] . Split ( "(" );
                if ( splitter . Length == 1 )
                    return false;
                string arguments = splitter [ 1 ];
                procargs = arguments . Split ( "," );
            }
            try
            {
                for ( int y = 0 ; y < procargs . Length ; y++ )
                {
                    if ( procargs [ y ] . EndsWith ( ")" ) )
                    {
                        procargs [ y ] = procargs [ y ] . Substring ( 0 , procargs [ y ] . Length - 1 );
                    }
                    procargs [ y ] = procargs [ y ] . Trim ( );
                }
                string [ ] split1 = procargs [ 1 ] . Split ( "(" );
                //AllProcnames is a Tuple<int, string, string[]>
                // FORMAT = int line # - string Procedure name (NO "(") - string filepathname - string[] procargs
                // NB:  procargs last line does NOT have the ")" in it either
                Tuple<int , string , string , string [ ]> t = Tuple . Create ( lineindex , split1 [ 0 ] , pathfilename , procargs );
                Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                return true;
            }catch(Exception ex) {
                Debug . WriteLine ( $"ERROR line 42, {ex.Message}" );
                return false;
            }
        }
        public static bool AddProcRow ( int x , string line , string path , int type ,
              bool isAlone = false , bool isArg = false ,
              bool isHeader = false ,
              int crlf = 1 )
        {
            int linescount = 0;
            string [ ] parts = new string [ 1 ];
            try
            {
                line = line . Trim ( );
                // Santy check
                if ( type == -1 ) type = 1;
                string debgoutput = "";
                if ( line . Contains ( "Oooops" ) == false && line . Contains ( "Cannot  add" ) == false )
                {
                    if ( isAlone )
                        debgoutput = $"{line}";
                    else if ( isArg )
                        debgoutput = $"   {line}";
                    else
                        debgoutput = $"{line}";

                    if ( crlf == 1 )
                        debgoutput += "\n";
                    else if ( crlf == 2 )
                        debgoutput += "\n\n";
                }

                if ( type == 1 )
                    Program . output_public . Append ( debgoutput );
                else if ( type == 2 )
                    Program . output_private . Append ( debgoutput );
                else if ( type == 3 )
                    Program . output_internal . Append ( debgoutput );
                // NB lines[x] format is "Path : Procedurename : Line#"
                parts = line . Split ( " : " );
                // format = "9999, Procname, FullPath"   ????
                Tuple<int , string , string> tuple = Tuple . Create ( x , parts [ 0 ] , parts [ 1 ] );
                // Add this line to our store of all (captured) lines
                Program . AllValidEntries . Add ( tuple );
                // if 1, it is a follow on argument(s)  line
                if ( parts . Length == 1 )
                    Program . AllMethods . Add ( line . Trim ( ) , $"{path . Trim ( )} : {x}" );
                // normal header line
                else if ( parts . Length == 3 )
                    Program . AllMethods . Add ( parts [ 1 ] . Trim ( ) , $"{parts [ 2 ] . Trim ( )} : {parts [ 0 ] . Trim ( )}" );

                linescount++;

                if ( type == 1 )
                    Program . output_public . Append ( $"{line}\n" );
                else if ( type == 2 )
                    Program . output_private . Append ( $"{line}\n" );
                else if ( type == 3 )
                    Program . output_internal . Append ( $"{line} \n" );
                return true;
            }
            catch ( Exception ex )
            {
                //Add entry to Error Tuple collection 
                Tuple<int , string , string> t = Tuple . Create ( x , path , parts [ 0 ] );
                Program . ErrorPaths . Add ( t );

                Debug . WriteLine ( $"\nErrorPaths updated with {x} : {parts [ 0 ]} : {path}" );
                return false;
            }
            return true;
        }
        public static string StripFormattingChars ( string line )
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


    }
}
