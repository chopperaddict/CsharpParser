using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
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
        //public static bool CreateOneLineItem ( Dictionary<string , string> AllMethods , string pathfilename , string ProcName ,  int type , int x )
        //{
        //    string FullOutputLine = $"{pathfilename} : {ProcName} : {x}";
        //    bool result = Datastore . AddProcname ( x , FullOutputLine , pathfilename , type );
        //    return result;
        //}
    }
}
