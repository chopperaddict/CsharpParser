using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Text;
using System . Threading . Tasks;

namespace CsharpParser
{
    public static class Validators
    {
        public static bool ValidateIsProcedure ( string line , string [ ] upperlines , int x )
        {
            int testcount = 0;
            //^^^^^^^^^^^^^^^^^^^^^^^^^{^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
            if ( upperlines [ x ] . Contains ( "GETSCROLLSPEED)" ) == true )
                if ( Program . SHOWASSERT )
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
                        return false;
                }

                // block various  types
                if ( ( upperlines [ x ] . Trim ( ) . StartsWith ( "PUBLIC" ) == true ||
                    upperlines [ x ] . Trim ( ) . StartsWith ( "PRIVATE" ) == true ||
                    upperlines [ x ] . Trim ( ) . StartsWith ( "PROTECTED" ) == true ||
                    upperlines [ x ] . Trim ( ) . Contains ( "PARTIAL CLASS" ) == true )
                    && ( upperlines [ x ] . Trim ( ) . Contains ( "=" ) == true && upperlines [ x ] . Trim ( ) . Contains ( "NEW" ) == true ) )
                    return false;
                else if ( upperlines [ x ] . Trim ( ) . Contains ( "PARTIAL CLASS" ) ||
                    upperlines [ x ] . Trim ( ) . Contains ( "(" ) == false )
                    return false;
                else if ( upperlines [ x ] . Trim ( ) . Contains ( "=" ) &&
                    upperlines [ x ] . Trim ( ) . Contains ( "(" ) )
                {
                    int eq = upperlines [ x ] . IndexOf ( "=" );
                    int parenth = upperlines [ x ] . IndexOf ( "(" );
                    if ( parenth > eq )
                        return false;
                    else return true;
                }
                else if ( upperlines [ x ] . Trim ( ) . StartsWith ( "//" ) == true
                    || upperlines [ x ] . Trim ( ) . StartsWith ( "/*" ) == true
                    || upperlines [ x ] . Trim ( ) . EndsWith ( "*/" ) == true )
                    return false;
                else if ( upperlines [ x ] . StartsWith ( "//" ) == true )
                    return false;
                else if ( upperlines [ x ] . StartsWith ( "//" ) == true
                    || upperlines [ x ] . StartsWith ( "//*" ) == true
                    || upperlines [ x ] . StartsWith ( "*" ) == true )
                    return false;
                else if ( upperlines [ x ] . Contains ( "(" ) == false
                    && ( upperlines [ x ] . Contains ( "<" ) == true
                    || upperlines [ x ] . Contains ( ">" ) == true ) )
                    return false;
                else if ( upperlines [ x ] . Length <= 2 )
                    return false;
                if ( upperlines [ x ] . Contains ( "=" ) == true && upperlines [ x ] . Contains ( "NEW" ) == true )
                {
                    // Its a NEW statement, ignore it !
                    int a = upperlines [ x ] . IndexOf ( "=" );
                    int b = upperlines [ x ] . IndexOf ( "NEW" );
                    if ( a < b )
                        return false;
                }
                // Work thru exceptions we do  NOT WANT to list
                // ignore defaullt delegate declarations
                if ( upperlines [ x ] . Contains ( "FUNC" ) == true || upperlines [ x ] . Contains ( "ACTION" ) == true )
                    return false;
            }
            return true;
        }
        public static bool IsProcedure ( string upperline )
        {
            if ( ( upperline . StartsWith ( "PUBLIC" ) || upperline . StartsWith ( "STATIC PUBLIC" )
                || upperline . StartsWith ( "PRIVATE" ) || upperline . StartsWith ( "STATIC PRIVATE" )
                || upperline . StartsWith ( "PROTECTED" ) || upperline . StartsWith ( "STATIC PROTECTED" )
                || upperline . StartsWith ( "INTERNAL" ) || upperline . StartsWith ( "STATIC INTERNAL" ) )
                && upperline . Contains ( "PARTIAL CLASS" ) == false )
                return true;
            return false;
        }

        public static bool CheckForAttachedProperty ( string [ ] upperlines , int x )
        {
            if ( ( upperlines [ x ] . Contains ( "PUBLIC STATIC READONLY DEPENDENCY" )
                || upperlines [ x ] . Contains ( "DEPENDENCYPROPERTY . REGISTERATTACHED" ) ) )
            {
                return true;
            }
            return false;
        }

        public static int ParseAllMethodArgs ( string [ ] lines , string filepathname , int x , ref string [ ] proclines )
        {
            // Parse all the arguments  into a string[] array in proclines[]
            string arglines = "", buffer = "", inputstring = "";
            int count = 0;
            List<string> inbuff = new ( ); ;
            string inbuffer = "";
            string balancestring = "";

            while ( true )
            {
                //read lines in until we have the complete set of arguments
                //count++;
                inputstring = lines [ x + count++ ] . Trim ( );
                buffer += $"{inputstring}";
                if ( inputstring . Contains ( ")" ) )
                    break;
            }
            
            int parenthesis = buffer . IndexOf ( "(" );            
            arglines = buffer . Substring ( 0 , parenthesis + 1 );
            balancestring = buffer . Substring ( parenthesis + 1 );

            inbuff . Add ($"{x}");
            inbuff . Add ( filepathname );
            inbuff . Add ( arglines );
            while ( true )
            {
                parenthesis = balancestring . IndexOf ( "," );
                if ( parenthesis == -1 )
                {
                    inbuff . Add ( balancestring );
                    break;
                }
                else
                {
                    inbuff . Add ( balancestring . Substring ( 0 , parenthesis ) );
                    inbuffer = balancestring . Substring ( parenthesis + 1 );
                    balancestring = inbuffer;
                }
            }
            proclines = new string [ inbuff . Count ];
            for ( int y = 0 ; y < inbuff . Count ; y++ )
            {
                proclines [ y ] = inbuff [ y ];
            }

            return inbuff . Count;

            //for ( int w = 0 ; w < argslist . Count ; w++ )
            //{
            //    // Sanity check
            //    if ( inputstring . Trim ( ) . StartsWith ( "{" ) )
            //    {
            //        string s = argslist [ w ] . Trim ( ) . Substring ( 0 , argslist [ w ] . Trim ( ) . IndexOf ( "{" ) );
            //        inbuff . Add ( s );
            //        int lt = argslist [ w ] . Length - s . Length;
            //        if ( lt < 0 )
            //        {
            //            // there is stuff beyond the "(" - sort it out here
            //            string rest = argslist [ w ] . Trim ( ) . Substring ( lt ) . Trim ( );
            //            if ( rest != "" )
            //            {
            //                inbuff = ParseArgsFromLine ( rest );
            //                for ( int u = 0 ; u < inbuff . Count ; u++ )
            //                {
            //                    inbuff . Add ( inbuff [ u ] );
            //                }
            //            }
            //        }
            //    }
            //    else if ( inputstring . Trim ( ) . Contains ( "(" ) && ( inputstring . Trim ( ) . Contains ( ")" ) ) )
            //    {
            //        // TODO   parse all the lines  we  have for thid sprocedure
            //        // its a one liner
            //        offset = inputstring . IndexOf ( "(" );
            //        //Add procedure name to buffer
            //        inbuff . Add ( inputstring . Substring ( 0 , offset + 1 ) );
            //        //just leave anyhing else in the buffer
            //        arglines = inputstring . Substring ( offset + 1 );

            //        if ( arglines . Trim ( ) . Contains ( ")" ) )
            //        {   //YES
            //            if ( arglines . Trim ( ) . Contains ( "," ) == false )
            //            {   // only a single arg at most - Add ending ) to output of procedure name
            //                inbuff [ inbuff . Count - 1 ] += ")";
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //  ***it's a multi line  - process of some sort ***
            //        // it may be proc name line ?
            //        if ( inputstring . Contains ( "(" ) )
            //        {
            //            offset = inputstring . IndexOf ( "(" );
            //            inbuff . Add ( inputstring . Substring ( 0 , offset + 1 ) );
            //            //if ( inputstring . Substring ( offset ) . Trim ( ) == "" )
            //            //    continue;
            //        }
            //        else
            //        {
            //            int end = inputstring . IndexOf ( "(" );
            //            inbuff . Add ( inputstring . Substring ( 0 , end + 1 ) );
            //            if ( end == inputstring . Length )
            //                continue;
            //        }
            //    }
            //    for ( int y = 0 ; y < inbuff . Count ; y++ )
            //    {
            //        proclines [ y ] = inbuff [ y ];
            //    }
            //    //return inbuff . Count;
            //    // we have  continuation line(s)
            //    if ( inputstring . Contains ( "," ) == false && inputstring . Contains ( ")" ) == false )
            //    {
            //        // its the last line
            //        inbuff . Add ( inputstring );
            //        //continue;
            //    }
            //    else if ( inputstring . Contains ( "," ) == false )
            //    {
            //        // Its just a continuation argument li ne
            //        inbuff . Add ( inputstring );
            //        //continue;
            //    }
            //    //                        inbuff . Add ( inputstring . Substring ( 0 , offset + 1 ) );
            //    arglines = inputstring . Substring ( offset , arglines . Length );
            //    if ( arglines . Length > 0 )
            //    {
            //        //Add argument to list
            //        if ( arglines . EndsWith ( "(" ) )
            //        {
            //            inbuff . Add ( arglines );
            //            //continue;
            //        }
            //        inbuff . Add ( arglines . Substring ( offset - 1 ) );
            //        arglines = arglines . Substring ( offset + 1 );
            //        if ( arglines . Contains ( "," ) == false )
            //        {
            //            // it's a one liner (but with >1 args) -parse and  add  to list and exit
            //            parts2 = arglines . Split ( "," );
            //            // break;
            //        }
            //        else
            //        {
            //            //got more than 1 argument on this line - split  em up
            //            parts2 = arglines . Split ( "," );
            //            try
            //            {
            //                for ( int p = 0 ; p < parts2 . Length ; p++ )
            //                {
            //                    if ( parts2 [ p ] == null || parts2 [ p ] == "" )
            //                        break;
            //                    else
            //                        inbuff . Add ( parts2 [ p ] );
            //                }
            //                // break;
            //            }
            //            catch ( Exception ex )
            //            {
            //                Debug . WriteLine ( $"Oooops : \nERROR  \n{ex . Message}" );
            //            }
            //        }
            //    }

            //    //else inbuff . Add ( inputstring );
            //    proclines = new string [ inbuff . Count ];
            //    //proclines [ 0 ] = filepathname;

            //    for ( int y = 0 ; y < inbuff . Count ; y++ )
            //    {
            //        proclines [ y ] = inbuff [ y ];
            //    }
            //}
            //return inbuff . Count;
        }

        public static List<string> ParseArgsFromLine ( string argsline )
        {
            List<string> outbuff = new ( );
            if ( argsline . Length > 0 )
            {
                if ( argsline . Contains ( "," ) == false )
                    outbuff . Add ( argsline );
                else
                {
                    string [ ] parser = argsline . Split ( "," );

                    foreach ( string item in parser )
                    {
                        outbuff . Add ( item );
                    }
                }
            }
            return outbuff;
        }
    }
}
