using System;
using System . Collections . Generic;
using System . Diagnostics;
using System . Linq;
using System . Text;
using System . Threading . Tasks;

using SortSupportLib;

namespace CsharpParser
{
    public static  class Utilities
    {
        public static string [ ] GetDictionaryValueEntry ( Dictionary<string , string> dict , string target )
        {
            //return Key and Vvalue from Dictionary that match term requested
            string [ ] results = new string [ 2 ];
            results [ 0 ] = ""; results [ 1 ] = "";
            if ( dict . ContainsKey ( target ) == true )
            {
                results [ 1 ] = ( from d in dict
                                  where d . Key . Contains ( target )
                                  select d . Value ) . FirstOrDefault ( );
                results [ 0 ] = ( from d in dict
                                  where d . Value == results [ 1 ]
                                  select d . Key ) . FirstOrDefault ( );
            }
            else
            {
                //string [ ] str = target . Split ( );
                // results [ 0] = 
            }

            return results;
        }
        public static string [ ] GetDictionaryKeyEntry ( Dictionary<string , string> dict , string target )
        {
            //return Key and Vvalue from Dictionary that match term requested
            string [ ] results = new string [ 2 ];
            results [ 0 ] = ""; results [ 1 ] = "";
            if ( dict . ContainsKey ( target ) == true )
            {
                results [ 1 ] = ( from d in dict
                                  where d . Key . Contains ( target )
                                  select d . Value ) . FirstOrDefault ( );
                results [ 0 ] = ( from d in dict
                                  where d . Value == results [ 1 ]
                                  select d . Key ) . FirstOrDefault ( );
            }
            return results;
        }

        public static List<Tuple<int , string , string>> FindAllDupeMatches ( 
            List<Tuple<int , string , string>> ErrorPaths, 
                Tuple<int , string , string> tuple )
        {
            Tuple<int , string , string> t;
            List<Tuple<int , string , string>> output = new ( );
            foreach ( Tuple<int , string , string> item in ErrorPaths )
            {
                if ( tuple . Item3 . ToUpper ( ) == item . Item3 . ToUpper ( ) )
                {
                    t = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
                    output . Add ( t );
                }
            }
            return output;
        } 
        public  static void SortDupes ( List<Tuple<int , string , string>> ErrorPaths )
        {
            int indx = 0;
            Tuple<int , string , string> [ ] DuplicatesTuple = new Tuple<int , string , string> [ ErrorPaths . Count ];
            foreach ( Tuple<int , string , string> item in ErrorPaths )
            {
                DuplicatesTuple [ indx++ ] = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
            }

            Array . Sort ( DuplicatesTuple , new SortTupleItem2 ( ) );
            string lastitem = "";
            foreach ( var item in DuplicatesTuple )
            {
                if ( lastitem == "" || lastitem != item . Item2 )
                {
                    lastitem = item . Item2;
                    Console . WriteLine ( item . Item2 );
                }
                Console . WriteLine ( $"{item . Item1}  Line : {item . Item3}" );
            }
            Console . WriteLine ( "NOW FOR 3 : \n\n" );

            // create a list of Method Procedure names alone
            string [ ] Methodnames = new string [ ErrorPaths . Count ];
            int ndx = 0;
            foreach ( var item in DuplicatesTuple )
            {
                string [ ] lst3 = item . Item3 . Split ( " " );
                Methodnames [ ndx++ ] = ( lst3 [ lst3 . Length - 1 ] );
            }
            Array . Sort ( Methodnames , new SortString ( ) );
            Console . WriteLine ( $"\n\nList of Method names alone\n\n" );
            foreach ( string item in Methodnames )
            {
                Console . WriteLine ( $"{item}" );
            }


            lastitem = "";
            Array . Sort ( DuplicatesTuple , new SortTupleItem3 ( ) );
            Console . WriteLine ( $"\n\nList of Method line # and name\n\n" );
            foreach ( var item in DuplicatesTuple )
            {
                if ( lastitem == "" || lastitem != item . Item3 )
                {
                    lastitem = item . Item3;
                    Console . WriteLine ( item . Item3 );
                }
                Console . WriteLine ( $"{item . Item1}  Line : {item . Item2}" );
            }
        }
        private static int LoadFilesArray ( string path , string filespec )
        {
            int TotalFiles = 0;
            try
            {
                IEnumerable<string> AllSourcefiles;
                path = Path . GetDirectoryName ( path );
                Program . searchpath = path;
                AllSourcefiles = Directory . EnumerateFiles ( path , filespec );
                if ( AllSourcefiles . Count ( ) > 0 )
                {
                    for ( int x = 0 ; x < AllSourcefiles . Count ( ) ; x++ )
                    {
                        string fname2 = AllSourcefiles . ElementAt ( x );
                        Program . AllFileNames [ x ] = fname2;
                    }
                    TotalFiles = AllSourcefiles . Count ( );
                }
            }
            catch ( Exception ex ) { Debug . WriteLine ( $"" ); }
            return TotalFiles;
        }

    }
}
