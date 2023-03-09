using System;
using System . Collections . Generic;
using System . ComponentModel . Design;
using System . Diagnostics;
using System . IO;
using System . Linq;
using System . Runtime . CompilerServices;
using System . Text;
using System . Threading . Tasks;

using SortSupportLib;

namespace CsharpParser
{
    public static class Reporting
    {
        public static void CreateDupesFile ( )
        {
            StringBuilder sbMethods = new ( );
            string buff = $"RESULTS - DUPLICATED METHOD NAMES\n\nThis List should be a fairly comprehensive list of all duplicated Methods identified in the C# files in {Program . searchpath}.\n\n";
            sbMethods . Append ( buff );
            sbMethods . Append ( $"Report Created : {DateTime . Now . ToString ( )}\n\n" );
            sbMethods . Append ( $"FILENAME = C:\\Wpfmain\\Documentation\\MethodsIndex . txt,\n\n" );

            sbMethods . Append ( $"ERRORS - DUPLICATE METHOD NAMES\n" );
            sbMethods . Append ( $"Report Created : {DateTime . Now . ToString ( )}\n" );
            sbMethods . Append ( $"Current file :{Program . pathfilename}\n\n" );

            if ( Program . ErrorPaths . Count > 0 )
            {
                int indx = 0;
                Tuple<int , string , string> [ ] tuples = new Tuple<int , string , string> [ Program . ErrorPaths . Count ];

                // Add current source file to top of list
                foreach ( Tuple<int , string , string> item in  Program . ErrorPaths )
                {
                    Tuple<int , string , string> currenttuple = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
                    List<Tuple<int , string , string>> matches = Utilities . FindAllDupeMatches ( ErrorPaths: Program . ErrorPaths , currenttuple );
                    tuples [ indx++ ] = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
                }
                // TODO Not wrking  very well. 8//3/23

                // Now sort  the tuples              
                Array . Sort ( tuples , new SortTupleItem2 ( ) );
                sbMethods . Clear ( );
                // now add them to StringBuilder 
                for ( int x = 0 ; x < tuples . Length ; x++ )
                {
                    Tuple<int , string , string> tup = tuples [ x ];
                    if ( x == 0 )
                        sbMethods . Append ( $"\n{tup . Item3}\n" );
                    else
                        sbMethods . Append ( $"  Line# {tup . Item1} : {tup . Item3}\n" );
                }
            }
            File . WriteAllText ( $@"C:\wpfmain\Documentation\Duplicateslist.txt" , sbMethods . ToString ( ) );

            // TODO   output   to cmd window ????
            // Utilities . SortDupes ( //Program . ErrorPaths );
        }
        public static void CreateReports ( )
        {
            // crucial file !!!!
            string sb = Program . output_public . ToString ( );
            sb += Program . output_private . ToString ( );
            sb += Program . output_internal . ToString ( );
            string [ ] ln = sb . Split ( "\n" );
            string [ ] allines = new string [ ln . Length ];

            //**********************************//
            // Create  file ALLMETHODS.TXT
            //**********************************//

            // TODO not working 8/3/23

            //Program . path = @"C:\wpfmain\Documentation\AllMethods.txt";
            //string output = "";
            //string dt = Program . TotalMethods > 0 ? Program . TotalMethods . ToString ( ) : "No";
            //string str2 = $"All procedure names in {Program . searchpath} Identified ...\n\nThe Utility (CsharpParser.exe has parsed {Program . searchpath} \nand processed {Program . TotalLines} lines from {Program . TotalFiles} File(s).\n\nIt has identified {dt} Methods.\nOutput created {DateTime . Now . ToString ( )}\n";
            //output = str2;

            //File . AppendAllText ( Program . path , $"{Program . underline}\n" );
            //File . AppendAllText ( Program . path , output );
            //File . AppendAllText ( Program . path , $"{Program . underline}\n" );

            //for ( int t = 0 ; t < ln . Length ; t++ )
            //{
            //    allines [ t ] = ln [ t ] . Trim ( );
            //    if ( allines [ t ] . StartsWith ( "***" ) )
            //        File . AppendAllText ( Program . path , allines [ t ] . Trim ( ) + ")\n" );
            //    else if ( allines [ t ] . Contains ( "(" ) == false && allines [ t ] . Trim ( ) . Contains ( ")" ) == false )
            //        File . AppendAllText ( Program . path , "   " + allines [ t ] . Trim ( ) + ")\n" );
            //    else if ( allines [ t ] . Contains ( ")" ) == false )
            //        File . AppendAllText ( Program . path , allines [ t ] . Trim ( ) + ")\n" );
            //    else
            //        File . AppendAllText ( Program . path , "   " + allines [ t ] . Trim ( ) + "\n\n" );
            //}
            //File . AppendAllText ( Program . path , "\n\nEnd of File C:\\wpfmain\\Documentation\\AllMethods.txt .....\n" );

            //==========================================//
            // Now create file @"C:\Wpfmain\Documentation\MethodsIndex.txt"
            // data comes from the AllMethods Dictionary entries captured here.
            //==========================================//

            // TODO - not working  8/3/23
            //CreateDupesFile ( );
        }

        public static void CreateMethodsList ( )
        {
            string str1 = $"{Program . underline}\nCSharpParser.exe has parsed {Program . TotalFiles} files \nfrom [ {Program . searchpath} ] \nThese contained {Program . TotalLines} lines of code\n& a total of {Program . TotalMethods} Methods were identified in them...\n";
            str1 += $"Report Created : {DateTime . Now . ToString ( )}\nSee ALLOUTPUT.TXT for list of Methods discovered...\n{Program . underline}\n\n";
            File . WriteAllText ( @"C:\Wpfmain\Documentation\MethodsFiles . txt" , str1 );
            foreach ( string item in Program . FullPathFileNameList )
            {
                if ( item == "" )
                    break;
                File . AppendAllText ( @"C:\Wpfmain\Documentation\MethodsFiles . txt" , $"{item}\n" );
            }
            File . AppendAllText ( @"C:\Wpfmain\Documentation\MethodsFiles . txt" , $"\n E.O.F METHODSFILES.TXT\n" );
        }
        public static void CreateDuplicatesFile ( StringBuilder sb )
        {
            File . WriteAllText ( $@"C:\wpfmain\Documentation\Duplicateslist.txt" , sb . ToString ( ) );

            //foreach ( KeyValuePair<int,string> item in Errormethods )
            //{
            //    string errpath = ErrorPaths [ Convert . ToInt32 ( item . Value ) ];
            //    string str = $"{errpath} : {item . Key} ";
            //    output += $"{str}\n";
            //    string [ ] fn = str . Split ( " : " );
            //    sortlist . Add ( $"[{fn [ 0 ]}]" , $"{fn [ 1 ]}" );
            //    //                sortlist . Add ( $"{item . Value} : {fn [ 0 ]}" , $"{fn [ 1 ]}  :  {fn [ 0 ]}" );
            //}
            //File . WriteAllText ( $@"C:\wpfmain\Documentation\Errorslist.txt" , output );
        }
        public static string CreateOutfile ( StringBuilder pub , StringBuilder priv , StringBuilder intern )
        {
            string sbcollection = "";
            string publicstring = pub . ToString ( );
            for ( int x = 0 ; x < pub . Length ; x++ )
            {
                if ( pub . Length > 0 )
                {
                    string entry = pub [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "PUBLIC" ) )
                        Program . output_public . Append ( $"{pub [ x ]}\n" );
                }
            }
            for ( int x = 0 ; x < priv . Length ; x++ )
            {
                if ( priv . Length > 0 )
                {
                    string entry = priv [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "PRIVATE" ) )
                        Program . output_private . Append ( $"{priv [ x ]}\n" );
                }
            }
            for ( int x = 0 ; x < intern . Length ; x++ )
            {
                if ( intern . Length > 0 )
                {
                    string entry = intern [ x ] . ToString ( ) . Trim ( ) . ToUpper ( );
                    if ( entry . ToUpper ( ) . StartsWith ( "INTERNAL" ) )
                        Program . output_internal . Append ( $"{intern [ x ]}\n" );
                }
            }
            sbcollection += Program . output_public . ToString ( );
            sbcollection += Program . output_private . ToString ( );
            sbcollection += Program . output_internal . ToString ( );
            // return a string full of data
            return sbcollection;
        }
        public static void CreateAllLIstTuplesReport ( )
        {
            // TODO WORKING Quite Well !! 8//34/23
            string path = @"C:\wpfmain\documentation\AllTuples.txt";
            string sarg = "";
            int linecount = 1;
            Program . TotalMethods = 0;
            File . WriteAllText ( path , "" );

            foreach ( Tuple<int , string , string , string [ ]> item in Program.AllTupesList)
            {
                if ( item == null )
                    break;
                Tuple<int , string , string , string [ ]>  AllTuples = item;
                Debug . Assert ( AllTuples . Item4 [ 0 ] != "GetBankAccounts" , "GetBankAccounts identified" , "did it work ?" );
                Program . TotalMethods++;
                // layout required :
                // Filepath : line# = Item3 : Item1
                // procname         = Item2
                // Args ....                = Item4[]
                // \n
                // so sequence is :
                // Filename : Lineno
                // procname
                // args....

                string Lineno = $"{AllTuples . Item1}";  // line #
                string Filename = $"{AllTuples . Item3}";    // Source file name
                string FilenameLine = $"\n({linecount++}) Ln# {AllTuples . Item1} : {AllTuples . Item3}";  // file name + line #
                int argscount = 0, count=0;
                string [ ] args =new string [ 1 ];
                string Procname = $"{AllTuples . Item4 [ 0 ]});";// procedure name (from Item4[0])
                for(int x = 0 ; x < AllTuples . Item4 .Length ; x++ )
                {
                    if ( AllTuples . Item4 [x] != null )
                        argscount++;
                    else break;
                }

                if ( argscount == 1 )
                {   //No arguments
                    if ( AllTuples . Item4 [ 0 ] . Trim ( ) . Contains ( ",") == false )
                        Procname = $"{AllTuples . Item4 [ 0 ]});";// procedure name (from Item4[0])
                    else
                    {
                        string[] argtmp = AllTuples . Item4 [ 0 ] . Split ( "," );
                        count = 0;
                        argscount = argtmp . Length;
                        args = new string [ argscount ];  
                       for(int y = 0 ; y < argscount ; y++ )
                        {
                            args [ y ] = argtmp [ y ];
                        }
                    }
                }
                else
                {   // 1 or more arguments ?
                    if ( argscount >= 2 )
                    {
                        // Load all args into an array and  format them
                        args = new string [ argscount ];
                        for ( int p = 0 ; p < argscount ; p++ )
                        {
                            if ( AllTuples . Item4 [ p ] == null )
                                break;
                            if ( AllTuples . Item4 [ 0 ] . Trim ( ) . EndsWith ( "(" ) == false )
                                args [ 0 ] = AllTuples . Item4 [ 0 ] + "(\n";        // output FULL line
                            else
                            args [ p  ] = AllTuples . Item4 [ p ];
                        }
                        if ( args [ argscount-1 ] . Trim ( ) . EndsWith ( ")" ) == false )
                            args [ argscount - 1 ] += ");";
                    }
                }
                File . AppendAllText ( path , $"{FilenameLine}\n" );
                File . AppendAllText ( path , $"  {Procname}\n" );

                for ( int t = 0 ; t < argscount ; t++ )
                {
                    if ( t < argscount - 1 )
                        File . AppendAllText ( path , $"    {args [ t ]},\n" );
                    else
                        File . AppendAllText ( path , $"    {args [ t ]});\n" );
                }
            }
            File . AppendAllText ( path , $"\nE.O.F : Created : {DateTime . Now}" );   // arguments
            string tmp = File . ReadAllText ( path );
            File . WriteAllText ( path , $"Report of all Procedures identified in the Directory [{Program . searchpath}] + it's subdirectories.\n\n"
                    + $"A Total of {Program . TotalFiles} files have been examined and processed, resulting in\n"
                    + $"a Total #({Program . TotalMethods}) Methods being identiified. A total of {Program . TotalExErrors} (Unhandled Errors were encountered  during processing...\nThe resulting details are shown below :-\nReport  created {DateTime . Now}\n"
                    + $"====================================================================================\n\n" );
            File . AppendAllText ( path , tmp );   // arguments

            //CreatefullReportFile ( );
        }
        public static void CreatefullReportFile ( )
        {
            string path = @"C:\wpfmain\documentation\AllTuples.txt";
            string dt = File . ReadAllText ( path );
            string [ ] data = dt . Split ( "\n" );
            string line = "";
            string output = "";
            string [ ] sarray = new string [ 1 ];
            string proc = "", spath = "";
            Program . TotalMethods = 0;
            Tuple<int , string , string , string [ ]> tuple = new Tuple<int , string , string , string [ ]> ( 0 , "" , "" , sarray );
            Tuple<int , string , string , string [ ]> [ ] Alltuples = new Tuple<int , string , string , string [ ]> [ Program . AllProcsIindex ];
            // Array [ , ] array = new Array [ Program . AllProcnames . Count , 1 ];
            // parse the tuples data file to create our report
            foreach ( string item in data )
            {
                // Sanity check
                if ( item == "" )
                    continue;
                string [ ] parts = item . Split ( " : " );
                line = parts [ 0 ] . ToString ( );
                spath = parts [ 1 ] . Trim ( );
                proc = parts [ 2 ] . Trim ( );

                // ouput file details

                if ( proc . Contains ( "(" ) == true && proc . Contains ( ")" ) == true
                    || proc . Contains ( "(" ) == true && proc . Contains ( ")" ) == false )
                {
                    // output the file name and line #
                    output += $"\n{spath} : #{line}\n";
                    File . WriteAllText ( path , output );
                    Program . TotalMethods++;
                }
                if ( proc . Contains ( "," ) == false && proc . Contains ( ")" ) == true )
                {
                    // Zero or one argument - output it all on one line
                    output += $"\t{proc}\n";
                    File . WriteAllText ( path , output );
                    continue;
                }
                if ( proc . Contains ( "," ) == true )
                {
                    // looks like we havemmre than one argument
                    string [ ] mainargs = proc . Split ( "," );
                    if ( mainargs . Length > 1 )
                    {
                        string [ ] args = proc . Split ( "(" );
                        if ( args . Length == 1 )
                        {
                            output = $"\t{args [ 0 ]}";
                            File . WriteAllText ( path , output );
                            continue;
                        }
                        else
                        {
                            string [ ] balargs = args [ 1 ] . Split ( "," );
                            for ( int z = 0 ; z < balargs . Length ; z++ )
                            {
                                output += $"\t{balargs [ z ]}\n";
                            }
                            if ( output . Trim ( ) . EndsWith ( ")" ) == false )
                                output = $"{output . Substring ( 0 , output . Length - 1 )} )\n";
                            File . WriteAllText ( path , output );
                            continue;
                        }
                    }
                }
            }
            string outfile = File . ReadAllText ( path );
            string hdr = $"Thiis file contains ALL the Procedures identified in the set of files selected\n"
                + $"A Toal of {Program . TotalFiles} files have been processed,\ncontaining {Program . TotalLines} lines of code.\n\n"
                + $"Thiis file contains ALL the Procedures identified in the set of files selected\n"
                + $"A Total of {Program . TotalMethods} Procedures were identified & are listed below :-"
                + $"\nDate Created : {DateTime . Now . ToString ( )}\n\n";
            File . WriteAllText ( path , hdr );
            File . AppendAllText ( path , outfile );
            File . AppendAllText ( path , "\n\n*** END OF FILE ***" );
            string lineno = "", Filename = "", Procname = "";
            int tupleindx = 0;
            for ( int y = 0 ; y < Program . AllProcsIindex ; y++ )
            {
                Tuple<int , string , string , string [ ]> t = Program . Alltuples [ y ];
                int a = t . Item1;
                string numstr = t . Item2;
                string pathname = t . Item3;
                string [ ] b = t . Item4;

                tuple = Tuple . Create ( a , numstr , pathname , b );
                Alltuples [ tupleindx++ ] = tuple;
                //array [ 0 ] [0] = tuple .Item1, tuple.Item2;
            }
            // Now sort  the tuples
            Array . Sort ( Alltuples , new SortTupleIntStringOnInt ( ) );
            foreach ( Tuple<int , string , string , string [ ]> item in Alltuples )
            {
                Debug . WriteLine ( $"\n{item . Item2} : {item . Item1}" );
                foreach ( string itm in item . Item4 )
                {
                    if ( itm != ")" )
                        Debug . WriteLine ( $"\t\t{itm}" );
                }
            }

            //tuples must a tuples[] array
            // Array . Sort(tuples, new SortTupleItem2 ( ) );

        }
        public static void DoReporting ( )
        {
            // creates list of all Methods into AllOutput.txt

            /// TODO - not working  8/3/23
            /// 


            //            CreateReports ( );
            string str1 = "";
            // read the data already written to the file
            if ( File . Exists ( $@"C:\wpfmain\documentation\AllOutput.txt" ) )
                str1 = File . ReadAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" );
            // now create header for the file
            if ( Program . DirectoryCount == 0 )
                Program . DirectoryCount++;
            string str2 = "";
            if ( Program . DirectoryCount == 1 )
                str2 = $"{Program . underline}\nCsharpParser.EXE has parsed Folder {Program . searchpath} \nbut found NO subdirectories.\nIt has processed a total of {Program . TotalLines} \nlines in the {Program . TotalFiles} files identiified.this folder.\nA total of {Program . TotalMethods} Methods have been documented.\nProceessed on  {DateTime . Now . ToString ( )}\n{Program . underline}\n\n";
            else
                str2 = $"{Program . underline}\nCsharpParser.EXE has parsed through {Program . DirectoryCount} Directories & Subdirectories\n in {Program . searchpath} \nand processed a total of {Program . TotalLines} \nlines.\n It has documented {Program . TotalMethods} Methods in a total of {Program . TotalFiles} files.\nProceessed on  {DateTime . Now . ToString ( )}\n{Program . underline}\n\n";
            //write it all out again, header first.
            File . WriteAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , str2 );
            File . AppendAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , str1 );
            File . AppendAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , $"E.O.F : Contents of {Program . searchpath . ToUpper ( )}" );

            // creates list of all files parsed : MethodsFiles.txt - workiing 7/3/23
            Reporting . CreateMethodsList ( );

            Reporting . CreateReports ( );

            Reporting . CreateAllLIstTuplesReport ( );
            Console . WriteLine ( $"{Program . TotalLines} - All Methods parsed !!!!!" );
            //// Get iterative list of dirs and files ??
            //DoIterativeSearch ( );

        }

        public static void WriteDebugErrors ( )
        {
            string str1 = "";
            // read the data already written to the file

            // TODO working - SORT OF 8/3/23
            if ( File . Exists ( $@"C:\wpfmain\documentation\AllDebugErrors.txt" ) )
                File . WriteAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , "" );
            // now create header for the file
            string str2 = $"{Program . underline}\nCsharpParser.EXE has encountered the  following errors .\nProceessed on  {DateTime . Now . ToString ( )}\n{Program . underline}\n\n";
            //write it all out again, header first.
            foreach ( Tuple<int , string , string> item in Program . DebugErrors )
            {
                File . AppendAllText ( $@"C:\wpfmain\documentation\AllDebugErrors.txt" , $"{item . Item1}\n{item . Item2}\n{item . Item3}\n\n" );
            }


        }
    }
}
