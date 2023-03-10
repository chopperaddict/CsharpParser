using System . Collections . Generic;
using System . ComponentModel;
using System . Data;
using System . Diagnostics;
using System . Linq;
using System . Reflection;
using System . Runtime . CompilerServices;
using System . Text;

using SortSupportLib;

namespace CsharpParser
{
    #region declarations
    internal class Program
    {
        public static bool LOGFILENAMES = true;
        public static bool SHOWASSERT = false;
        public static string SHOWONLY = "";

        public static int TotalFiles = 0;
        public static int indexincrement = 0;
        public static int TotalLines = 0;
        public static int TotalMethods = 0;
        public static int TotalExErrors = 0;
        public static int atch = 0;
        public static int linescount = 0;
        public static bool found = false;
        public static bool isfullcomment = false;
        public static string path = "";
        public static string? searchpath;
        public static string FullOutputLine = "";
        public static List<string> FullfilesList { get; set; } = new List<string> ( );
        public static List<string> FulldirectoriesList { get; set; } = new List<string> ( );
        public static string? buffer { get; set; }
        public static string filepath = "";
        public static string searchfile = "";
        public static string fname = "";
        public static string [ ] DictResults = new string [ 2 ];
        public static string? filespec { get; set; }
        public static string? capsbuffer { get; set; }
        public static string? CurrentFile { get; set; }
        public static string [ ]? lines { get; set; }
        public static string [ ]? upperlines { get; set; }
        public static string pathfilename { get; set; } = "";
        public static string [ ]? rows { get; set; }
        public static string? [ ]? srcchtext { get; set; }

        public static List<string> FullPathFileNameList { get; set; } = new ( );
        public static string [ ] AllFileNames = new string [ 1000 ];

        public static int DirectoryCount { get; set; }
        public static string underline = "==============================================================================================";
        public static List<String> testbuffer { get; set; } = new ( );
        public static Dictionary<string , string> AllMethods = new ( );
        public static Tuple<int , string? , string?>? tuple;
        public static List<Tuple<int , string? , string?>> DebugErrors = new ( );
        public static List<Tuple<int , string , string>> ErrorPaths = new ( );
        public static List<Tuple<int , string , string>> AllValidEntries = new ( );

        // declare tuple array (dummy size = 5000) !!
        public static Tuple<int , string , string , string [ ]> [ ] Alltuples = new Tuple<int , string , string , string [ ]> [ 5000 ];
        public static List<Tuple<int , string , string , string [ ]>> AllTupesList = new ( );
        public static int AllProcsIindex = 0;

        public static int tbindex { get; set; }
        public static int errorindex { get; set; } = 0;
        public static StringBuilder output_public { get; set; } = new ( );
        public static StringBuilder output_internal { get; set; } = new ( );
        public static StringBuilder output_private { get; set; } = new ( );

        private static bool CreateIterativeReport = false;

        #endregion declarations

        static void Main ( string [ ] args )
        {
            int TotalFiles = 0, linescounter = 0;
            string [ ] arguments;
            IEnumerable<string>? AllSourcefiles = null;
            string upperline = "";
            bool IsDupe = false;
            filespec = "*.cs";
            path = @"C:\wpfmain\Documentation\AllMethods.txt";


            #region setup
            //***************************//
            // Get user entry details
            //***************************//
            string DEFAULTPATH = @"C:\wpfmain\viewmodels";
            Console . Write ( "Enter filename : " );
            string? argstring = Console . ReadLine ( );
            if ( argstring . Contains ( "," ) )
            {
                int start = argstring . IndexOf ( "," );
                SHOWONLY = argstring . Substring ( start + 1 ) . ToUpper ( ) . Trim ( );
            }
            if ( argstring == "" )
            {
                pathfilename = DEFAULTPATH;
                searchfile = GetFilenameFromPath ( pathfilename . Trim ( ) , out filepath );
                argstring = DEFAULTPATH;
                TotalFiles = DoSetup ( argstring );
            }
            else if ( argstring . Contains ( "1" ) )
            {
                pathfilename = "C:\\Wpfmain\\SProcsHandling.xaml.cs";
                searchfile = GetFilenameFromPath ( pathfilename . Trim ( ) , out filepath );
                AllFileNames [ 0 ] = searchfile;
                FullPathFileNameList . Add ( pathfilename );
                searchpath = filepath;
                TotalFiles = 1;
                if ( LOGFILENAMES )
                    Console . WriteLine ( $"Parsing {pathfilename}..." );
            }
            else if ( argstring . Contains ( "2" ) )
            {
                pathfilename = pathfilename = "C:\\Wpfmain\\utils2.cs";
                searchfile = GetFilenameFromPath ( pathfilename . Trim ( ) , out filepath );
                AllFileNames [ 0 ] = searchfile;
                FullPathFileNameList . Add ( pathfilename );
                searchpath = filepath;
                TotalFiles = 1;
                if ( LOGFILENAMES )
                    Console . WriteLine ( $"Parsing {pathfilename}..." );
            }
            else if ( argstring . Contains ( "," ) )
            {
                arguments = argstring . Split ( ( "," ) );
                if ( arguments [ 1 ] == "" )
                    return;
                else
                {
                    if ( argstring . ToUpper ( ) . Contains ( ".CS" ) )
                    {
                        // Its a source file !!
                        searchfile = GetFilenameFromPath ( argstring . Trim ( ) , out filepath );
                        AllFileNames [ 0 ] = argstring;
                        // PATH+NAME of only file in array
                        pathfilename = argstring;
                        searchpath = filepath;
                        TotalFiles = 1;
                        if ( LOGFILENAMES )
                            Console . WriteLine ( $"Parsing {pathfilename}..." );
                    }
                    else
                    {
                        // its a path
                        // PATH+NAME of 1st file in array
                        // Get iterative list of dirs and files ??
                        argstring = @"C:\wpfmain\";
                        TotalFiles = DoSetup ( argstring );
                    }
                }
            }
            else
            {
                // no digits or commans identified, must be  a file name  ?
                if ( argstring . ToUpper ( ) . Contains ( ".CS" ) )
                {
                    // Its a source file !!
                    searchfile = GetFilenameFromPath ( argstring . Trim ( ) , out filepath );
                    AllFileNames [ 0 ] = argstring;
                    // PATH+NAME of only file in array
                    pathfilename = argstring;
                    TotalFiles = 1;
                }
                else
                {
                    // its a path
                    // PATH+NAME of 1st file in array
                    // Get iterative list of dirs and files ??
                    TotalFiles = DoSetup ( argstring );
                }
            }
            if ( TotalFiles == 0 )
                return;

            File . WriteAllText ( $@"C:\wpfmain\documentation\MethodsIndex.txt" , "" );
            File . WriteAllText ( $@"C:\wpfmain\documentation\Methodsfiles.txt" , "" );
            File . WriteAllText ( $@"C:\wpfmain\documentation\AllOutput.txt" , "" );
            File . WriteAllText ( $@"C:\wpfmain\documentation\AllMethods.txt" , "" );
            File . WriteAllText ( $@"C:\wpfmain\documentation\Duplicateslist.txt" , "" );
            File . WriteAllText ( @"C:\wpfmain\documentation\AllTuples.txt" , "" ); ;
            File . WriteAllText ( @"C:\wpfmain\documentation\allLines.txt" , $"" );
            File . WriteAllText ( @"C:\wpfmain\documentation\allDebugerrors.txt" , $"" );


            #endregion setup

            for ( int z = 0 ; z < TotalFiles ; z++ )
            {
                // GET THE NEXT FILE
                #region setup next file to be parsed
                if ( FullPathFileNameList [ z ] == null ) return;
                // loop through ALL FILES in array
                pathfilename = FullPathFileNameList [ z ];
                CurrentFile = pathfilename;
                if ( LOGFILENAMES )
                    Console . WriteLine ( $"\nParsing {CurrentFile . Trim ( )}" );
                // read current file into memory
                buffer = File . ReadAllText ( FullPathFileNameList [ z ] );
                capsbuffer = buffer . ToUpper ( );
                lines = buffer . Split ( "\n" );
                upperlines = capsbuffer . Split ( "\n" );
                int type = -1;
                TotalLines += lines . Length;
                #endregion setup next file to be parsed
                //****************************************************//
                // loop through next FILE in array and  process all LINES
                //****************************************************//
                for ( int x = 0 ; x < lines . Length ; x++ )
                {
                    if ( IsDupe )
                        x--;
                    //if ( x % 25 == 0 )
                    //Debug . Write ( "" );
                    try
                    {
                        lines [ x ] = Datastore . StripFormattingChars ( lines [ x ] );
                        lines [ x ] = lines [ x ] . Trim ( );
                        upperline = lines [ x ] . ToUpper ( );
                        upperlines [ x ] = upperlines [ x ] . Trim ( );
                        if ( upperlines [ x ] . Length < 10 )
                            continue;


                        if ( SHOWONLY != "" )
                        {
                            if ( upperline . Trim ( ) . StartsWith ( SHOWONLY ) == false )
                                continue;
                        }
                        // Check for commented code
                        if ( isfullcomment && lines [ x ] . Trim ( ) . EndsWith ( "*/" ) == false )
                            continue;
                        else if ( isfullcomment && lines [ x ] . Trim ( ) . EndsWith ( "*/" ) == true )
                        {
                            isfullcomment = false;
                            continue;
                        }
                        if ( lines [ x ] . Trim ( ) . StartsWith ( "//" ) )
                            continue;
                        if ( lines [ x ] . Trim ( ) . StartsWith ( "/*" ) || lines [ x ] . Trim ( ) . StartsWith ( "*" ) )
                        {
                            isfullcomment = true;
                            continue;
                        }
                        // No more commented lines found

                        #region Validate it is a procedure
                        if ( found == false )
                        {
                            if ( Validators . ValidateIsProcedure ( lines [ x ] , upperlines , x ) == false )
                                continue;
                            //************** VERIFY it is a procedure *************//
                            if ( Validators . IsProcedure ( upperlines [ x ] ) == false )
                                continue;
                            ////****************************************************//
                        }
                        #endregion Validate it is a procedure

                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//
                        if ( SHOWASSERT )
                            //Debug . Assert ( x != 257, "public static void Track" , "" );
                            if ( x > 250 && x < 260 )
                                Debug . WriteLine ( x );
                        //^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^//

                        if ( found == true )
                        {
                            // WE are parsing the following lines in a procedure declaration.
                            lines [ x ] = Datastore . StripFormattingChars ( lines [ x ] . Trim ( ) );
                            upperline = lines [ x ] . ToUpper ( );
                            if ( lines [ x ] . Contains ( "," ) && lines [ x ] . EndsWith ( "," ) )
                            {
                                // TODO  not   sure about this
                                Datastore . AddProcRow ( x , $"{lines [ x ]}" , pathfilename , type , isArg: true , crlf: 1 );
                                x++;
                            }
                            // get an array of arguments and ouput them all
                            string [ ] parts = lines [ x ] . Split ( "," );
                            if ( parts . Length > 1 )
                            {
                                foreach ( string item in parts )
                                {
                                    Datastore . AddProcRow ( x , $"{item}" , pathfilename , type , isArg: true , crlf: 1 );
                                }
                            }
                            else
                            {
                                Datastore . AddProcRow ( x , $"{lines [ x ]}" , pathfilename , type , isArg: true , crlf: 1 );
                            }
                            found = false;
                            continue;
                        }

                        //************************//
                        // *** Main entry point  ***
                        //************************//

                        // Check for ATTACHED PROPERTY
                        if ( Validators . CheckForAttachedProperty ( upperlines , x ) == true )
                        {
                            try
                            {
                                // Get entire method code as a block of x lines
                                string method = GetMethodBody ( lines [ x ] , lines , ref x );
                                Datastore . AddProcRow ( x , $"ATTACHED PROPERTY" , pathfilename , type , isHeader: true , crlf: 1 );
                                Datastore . AddProcRow ( x , method , pathfilename , type , crlf: 2 );
                                method . log ( );
                                continue;
                            }
                            catch ( Exception ex )
                            {
                                //Add entry to Error Tuple collection 
                                string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                Program . DebugErrors . Add ( t );
                                Debug . WriteLine ( $"\nERROR Program 294, \n{ex . Message}" ); TotalExErrors++;
                            }
                        }



                        // START of processing the line we have got
                        if ( found == false && upperlines [ x ] . Contains ( '(' ) )
                        {
                            //**********************************//
                            // MAIN ENTRY POINT
                            // Got the start of a procedure ?
                            //**********************************//
                            try
                            {
                                //  Set storage type (Public=1/private=2/internal=3)
                                GetStorageType ( upperline , ref type );
                            }
                            catch ( Exception ex )
                            {
                                //Add entry to Error Tuple collection 
                                string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                Program . DebugErrors . Add ( t );
                                Debug . WriteLine ( $"\nERROR Program 306, \n{ex . Message}" ); TotalExErrors++;
                            }
                            //****************************************//
                            // *** START OF PROCESSING PROPER ***
                            //****************************************//
                            upperline = lines [ x ] . ToUpper ( );
                            lines [ x ] = Datastore . StripFormattingChars ( lines [ x ] . Trim ( ) );
                            upperlines [ x ] = lines [ x ] . Trim ( ) . ToUpper ( );
                            if ( upperlines [ x ] . Contains ( "(" ) && upperlines [ x ] . Contains ( ")" ) )
                            {
                                //*****************//
                                // Output a one liner !!
                                //*****************//
                                try
                                {
                                    string FullOutputLine = $"{pathfilename} : {lines [ x ]} : {x}";
                                    string [ ] proclines = new string [ 1 ];
                                    indexincrement = Validators . ParseAllMethodArgs ( lines , pathfilename , x , ref proclines );
                                    //reset main index value to cover lines we have read in while processing in the above function
                                    Tuple<int , string , string , string [ ]> t = Tuple . Create ( x , proclines [ 2 ] , pathfilename , proclines );
                                    Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                                    Program . AllTupesList . Add ( t );
                                    FullfilesList . Add ( $"{linescounter++} : {lines [ x ]}" );

                                    x += indexincrement;
                                    continue;

                                }
                                catch ( Exception ex )
                                {
                                    //Add entry to Error Tuple collection 
                                    string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                    Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                    Program . DebugErrors . Add ( t );
                                    Debug . WriteLine ( $"\nERROR Program 294, \n{ex . Message}" ); TotalExErrors++;
                                }
                            }
                            else if ( upperlines [ x ] . Contains ( "(" ) )
                            {
                                //it has "(" but no more, so probably multi argument ?
                                // handle method name banner line 1st
                                string [ ] procname = lines [ x ] . Split ( "(" );
                                if ( procname [ 1 ] . Trim ( ) . Contains ( "," ) == true )
                                {
                                    // got multi arguments
                                    File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $"{"" . flines ( )}" );

                                    //**************************************//
                                    // output  header line of multi linner
                                    //**************************************//
                                    try
                                    {
                                        // TODO this is where AddProcname fails

                                        string [ ] proclines = new string [ 1 ];
                                        indexincrement = Validators . ParseAllMethodArgs ( lines , pathfilename , x , ref proclines );
                                        //reset main index value to cover lines we have read in while processing in the above function
                                        Tuple<int , string , string , string [ ]> t = Tuple . Create ( x , proclines [ 2 ] , pathfilename , proclines );
                                        Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                                        Program . AllTupesList . Add ( t );
                                        FullfilesList . Add ( $"{linescounter++} : {lines [ x ]}" );

                                        x += indexincrement;
                                        continue;

                                    }
                                    catch ( Exception ex )
                                    {
                                        //Add entry to Error Tuple collection 
                                        string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                        Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                        Program . DebugErrors . Add ( t );
                                        Debug . WriteLine ( $"\nERROR Program 345 \n{lines [ x ]} : {ex . Message}" );
                                        TotalExErrors++;
                                    }

                                    string [ ] tmp = procname [ 1 ] . Trim ( ) . Split ( "," );
                                    try
                                    {
                                        for ( int w = 0 ; w < tmp . Length ; w++ )
                                        {
                                            try
                                            {
                                                if ( w == tmp . Length - 1 )
                                                    Datastore . AddProcRow ( x , $"{tmp [ w ]}" , pathfilename , type , isArg: true , crlf: 2 );
                                                else
                                                {
                                                    Datastore . AddProcRow ( x , $"{tmp [ w ]}" , pathfilename , type , isArg: true , crlf: 1 );
                                                    $"{tmp [ w ]}" . log ( );
                                                }
                                            }
                                            catch ( Exception ex )
                                            {
                                                //Add entry to Error Tuple collection 
                                                string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                                Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                                Program . DebugErrors . Add ( t );
                                                Debug . WriteLine ( $"\nERROR Program 364, \n{ex . Message}" );
                                                TotalExErrors++;
                                            }
                                        }
                                        continue;
                                    }
                                    catch ( Exception ex )
                                    {
                                        //Add entry to Error Tuple collection 
                                        string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                        Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                        Program . DebugErrors . Add ( t );
                                        Debug . WriteLine ( $"\nERROR Program 294, \n{ex . Message}" ); TotalExErrors++;
                                    }
                                }
                                else if ( upperlines [ x ] . Contains ( "(" ) )
                                {
                                    indexincrement = 0;
                                    bool result = true;

                                    Debug . WriteLine ( $" 1 - 392 - {x} lines[x]" );
                                    File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $" 392 - {x} {lines [ x ]} \n" );
                                    //no commas in line, so either multiple lines or only got (none or one ) arguments
                                    if ( procname [ 1 ] . Trim ( ) == ")" )
                                    {
                                        // got it all on one line, process it
                                        try
                                        {
                                            // no arguments, output full line with 2c/r's
                                            string [ ] proclines = new string [ 1 ];
                                            indexincrement = Validators . ParseAllMethodArgs ( lines , pathfilename , x , ref proclines );
                                            Tuple<int , string , string , string [ ]> t = Tuple . Create ( x , proclines [ 2 ] , pathfilename , proclines );
                                            Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                                            Program . AllTupesList . Add ( t );
                                            FullfilesList . Add ( $"{linescounter++} : {lines [ x ]}" );

                                            x += indexincrement;
                                            continue;
                                        }
                                        catch ( Exception ex )
                                        {
                                            //Add entry to Error Tuple collection 
                                            string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                            Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                            Program . DebugErrors . Add ( t );
                                            Debug . WriteLine ( $"\nERROR Program 394, \n{ex . Message}" );
                                            TotalExErrors++;
                                            x += indexincrement;
                                        }
                                        try
                                        {
                                            if ( procname [ 1 ] != ")" && procname [ 1 ] != " )" )
                                                Datastore . AddProcRow ( x , $"{procname [ 0 ] . Trim ( )} ( )" , pathfilename , type , isAlone: true , crlf: 2 );
                                            continue;
                                        }
                                        catch ( Exception ex )
                                        {
                                            //Add entry to Error Tuple collection 
                                            string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                            Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                            Program . DebugErrors . Add ( t );
                                            Debug . WriteLine ( $"\nERROR Program 394, \n{ex . Message}" );
                                            TotalExErrors++;
                                            x += indexincrement;
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            bool result2 = true;
                                            string pname = "";
                                            Debug . WriteLine ( $" 1 - 440 - {x} lines[x]" );
                                            try
                                            {
                                                File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $" 440 - {x} {lines [ x ]} \n" );
                                                // no ending ), so must be split over > 1 line - so arguments probably exist.
                                                //get rest of argumenst and then add procname to list
                                                string [ ] proclines = new string [ 1 ];
                                                indexincrement = Validators . ParseAllMethodArgs ( lines , pathfilename , x , ref proclines );
                                                //reset main index value to cover lines we have read in while processing in the above function
                                                //***************************************************************//
                                                Tuple<int , string , string , string [ ]> t = Tuple . Create ( x , proclines [ 2 ] , pathfilename , proclines );
                                                Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                                                Program . AllTupesList . Add ( t );
                                                FullfilesList . Add ( $"{linescounter++} : {lines [ x ]}" );

                                                x += indexincrement;
                                                continue;

                                            }
                                            catch ( Exception ex )
                                            {
                                                Debug . WriteLine ( $"{ex . Message}" );
                                            }

                                            try
                                            {
                                                if ( result2 == false )
                                                {
                                                    // Handle a duplicate or failure here cos we cannot easily do it in theAddProcName() procedure
                                                    //List<Tuple<int , string , string>> ErrorPaths
                                                    // Add procname to errors list - OK
                                                    Tuple<int , string , string> dbugtuple = Tuple . Create ( 443 , pathfilename . Trim ( ) , $"Failed to AddProcname\n{lines [ x ]}" );
                                                    Program . DebugErrors . Add ( dbugtuple );
                                                    Tuple<int , string , string> tuple = Tuple . Create ( x , pathfilename . Trim ( ) , pname . Trim ( ) );
                                                    ErrorPaths . Add ( tuple );
                                                    TotalExErrors++;
                                                    x += indexincrement;
                                                    continue;
                                                }
                                                else
                                                {
                                                    x += indexincrement;
                                                    continue;
                                                }
                                            }
                                            catch ( Exception ex )
                                            {
                                                Debug . WriteLine ( $"{ex . Message}" );
                                            }
                                        }
                                        catch ( Exception ex )
                                        {
                                            //Add entry to Error Tuple collection 
                                            string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                                            Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                                            Program . DebugErrors . Add ( t );
                                            Debug . WriteLine ( $"\nERROR Program 441\n {lines [ x ]}, \n{ex . Message}" );
                                            TotalExErrors++;
                                        }
                                        // set flag to trigger args block processing
                                        found = true;
                                        continue;

                                    }
                                }
                                else continue;
                            }
                            else if ( upperlines [ x ] . Contains ( "(" ) )
                            {
                                indexincrement = 0;
                                string methodname = "";
                                try
                                {
                                    string [ ] proclines = new string [ 1 ];
                                    indexincrement = Validators . ParseAllMethodArgs ( lines , pathfilename , x , ref proclines );
                                    //reset main index value to cover lines we have read in while processing in the above function
                                    Tuple<int , string , string , string [ ]> t = Tuple . Create ( x , proclines [ 2 ] , pathfilename , proclines );
                                    Program . Alltuples [ Program . AllProcsIindex++ ] = t;
                                    Program . AllTupesList . Add ( t );
                                    FullfilesList . Add ( $"{linescounter++} : {lines [ x ]}" );

                                    x += indexincrement;
                                    continue;

                                }
                                catch ( Exception ex )
                                {
                                    Debug . WriteLine ( $"\nERROR Program 473, \n{ex . Message}" );
                                    // Handle a duplicate here cos we cannot easily do it in the called procedure
                                    //List<Tuple<int , string , string>> ErrorPaths
                                    Tuple<int , string , string> tuple = Tuple . Create ( x , path . Trim ( ) , methodname . Trim ( ) );
                                    Program . DebugErrors . Add ( tuple );
                                    ErrorPaths . Add ( tuple );
                                    TotalExErrors++;
                                    x += indexincrement;
                                }
                                continue;
                            }
                        }
                    }
                    catch ( Exception ex )  // outer catch()
                    {
                        //Add entry to Error Tuple collection 
                        string errnsg = $"\n{ex . Message}\n{lines [ x ]}";
                        Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errnsg );
                        Program . DebugErrors . Add ( t );
                        Debug . WriteLine ( $"Oooops : \nERROR  \n{ex . Message}" );
                        //                        IsDupe = true;
                        x += indexincrement;
                    }
                }
                TotalLines += linescount;
            }
            Console . WriteLine ( $"Completed  - now creating report  files !!!!!" );

            File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $"" );
            File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $"All Methods/Procedures identified...\nCreated {DateTime . Now}\n"
                + $"=========================================================\n\n" );
            foreach ( string item in FullfilesList )
            {
                File . AppendAllText ( @"C:\wpfmain\documentation\allLines.txt" , $"{item}" );
            }

            Reporting . DoReporting ( );
            Reporting . WriteDebugErrors ( );
        }

        private static int DoIterativeReport ( string fpath )
        {
            DirectoryInfo di = new ( fpath );

            int totalfiles = WalkDirectoryTree ( di , filespec );

            string path1 = $@"C:\wpfmain\documentation\ AllDirectories.txt";
            string path2 = $@"C:\wpfmain\documentation\ AllFiles.txt";
            if ( FulldirectoriesList != null )
            {
                File . WriteAllText ( path1 , $"{underline}\nCsharpParser.EXE : OUTPUT\nList of ALL ({FulldirectoriesList . Count}) directories checked.\nFile Created : {DateTime . Now . ToString ( )}\n{underline}\n\n" );
                foreach ( string item in FulldirectoriesList )
                {
                    File . AppendAllText ( path1 , $"{item}\n" );
                }
            }
            if ( FullfilesList != null )
            {
                File . WriteAllText ( path2 , $"{underline}\nCsharpParser.EXE : OUTPUT\nList of ({FullfilesList . Count}) files checked. \nFile Created : {DateTime . Now . ToString ( )}\n{underline}\n\n" );
                foreach ( string item in FullfilesList )
                {
                    File . AppendAllText ( path2 , $"{item}\n" );
                }
            }
            return totalfiles;
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
            catch ( Exception ex )
            {
                //Add entry to Error Tuple collection 
                string errnsg = $"\n{ex . Message}";
                Tuple<int , string , string> t = Tuple . Create ( -1 , pathfilename , errnsg );
                Program . DebugErrors . Add ( t );
                Debug . WriteLine ( "\nERROR Program 565, \n{ex.Message}" ); TotalExErrors++;
            }
            return output;
        }

        /// <summary>
        ///  parses the current line and returns  a string[] with data split
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="type"></param>
        /// <param name="x"></param>
        /// <returns></returns>
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
                        args [ y ] = Datastore . StripFormattingChars ( args [ y ] . Trim ( ) );
                        if ( y == args . Length - 1 )
                        {
                            if ( args [ y ] . EndsWith ( ")" ) == false )
                                output [ y ] = $"{args [ y ]} )\n";
                            else
                                output [ y ] = $"{args [ y ]}\n";
                        }
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
            catch ( Exception ex )
            {
                //Add entry to Error Tuple collection 
                string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                Program . DebugErrors . Add ( t );
                Debug . WriteLine ( "\nERROR Program 607, \n{ex.Message}" ); TotalExErrors++;
            }
            return lines;
        }

        private static void GetStorageType ( string upperline , ref int type )
        {
            if ( ( upperline . StartsWith ( "PUBLIC" ) || upperline . StartsWith ( "STATIC PUBLIC" ) ) ) type = 1;
            else if ( ( upperline . StartsWith ( "PRIVATE" ) || upperline . StartsWith ( "STATIC PRIVATE" ) ) ) type = 2;
            else if ( ( upperline . StartsWith ( "INTERNAL" ) || upperline . StartsWith ( "STATIC INTERNAL" ) ) ) type = 3;
        }
        private static string GetMethodBody ( string currentline , string [ ] lines , ref int x )
        {
            // *** This only gets called for Attached properties ***
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
            catch ( Exception ex )
            {
                //Add entry to Error Tuple collection 
                string errmsg = $"\n{ex . Message}\n{lines [ x ]}";
                Tuple<int , string , string> t = Tuple . Create ( x , pathfilename , errmsg );
                Program . DebugErrors . Add ( t );
                Debug . WriteLine ( $"oops$ \nERROR Program 664, \n{ex . Message}" ); TotalExErrors++;
            }
            x = index;
            return output;
        }

        public static int WalkDirectoryTree ( System . IO . DirectoryInfo root , string filespec )
        {
            int filesindex = 0;
            FileInfo [ ] files = null;
            DirectoryInfo [ ] subDirs = null;

            // First, process all the files directly under this folder
            try
            {
                files = root . GetFiles ( filespec );
                foreach ( FileInfo item in files )
                {
                    if ( item . Name . ToUpper ( ) . Contains ( ".TXT" ) == false )
                        FullPathFileNameList . Add ( item . FullName );
                }
            }
            // This is thrown if even one of the files requires permissions greater
            // than the application provides.
            catch ( UnauthorizedAccessException e )
            {
                // This code just writes out the message and continues to recurse.
                // You may decide to do something different here. For example, you
                // can try to elevate your privileges and access the file again.
                Debug . WriteLine ( $"\nERROR Program 692 : {e . Message}" );
            }

            catch ( System . IO . DirectoryNotFoundException e )
            {
                Debug . WriteLine ( "\nERROR Program 697 : {e . Message}" );
            }

            if ( files != null )
            {
                try
                {
                    AddFilesToList ( files , ref filesindex );

                    if ( CreateIterativeReport )
                    {
                        foreach ( System . IO . FileInfo fi in files )
                        {
                            // If we want to open, delete or modify the file, then
                            // thea try-catch block is required here to handle the case
                            // where the file has been deleted since the call to TraverseTree().

                            string Files = fi . FullName . ToUpper ( );
                            if ( Files . Contains ( ".G.CS" ) || Files . Contains ( "ASSEMBLY" ) )
                                continue;
                            else
                            {
                                FullfilesList . Add ( fi . FullName );
                            }
                        }
                    }
                }
                catch ( Exception ex )
                {
                    //Add entry to Error Tuple collection 
                    Tuple<int , string , string> t = Tuple . Create ( -1 , pathfilename , $"\n{ex . Message}" );
                    Program . DebugErrors . Add ( t );
                    Debug . WriteLine ( $"\nERROR Program 724, \n{ex . Message}" ); TotalExErrors++;
                }

                // Now find all the subdirectories under this directory.
                subDirs = root . GetDirectories ( );
                DirectoryCount += subDirs . Length;

                foreach ( System . IO . DirectoryInfo dirInfo in subDirs )
                {
                    string dirs = dirInfo . FullName . ToUpper ( ); ;
                    if ( dirs . Contains ( "OBJ" ) || dirs . Contains ( "PROPERTIES" ) || dirs . Contains ( ".GIT" ) || dirs . Contains ( "BIN" )
                          || dirs . Contains ( ".VS" ) || dirs . Contains ( "CONVERTERS" ) || dirs . Contains ( "SCRIPTS" )
                           || dirs . Contains ( "DOCUMENTATION" ) || dirs . Contains ( "IMAGES" ) || dirs . Contains ( "PRINTFILES" )
                            || dirs . Contains ( "SQLSCRIPTS" ) || dirs . Contains ( "USERDATAFILES" )
                           )
                        continue;
                    else
                        FulldirectoriesList . Add ( dirInfo . FullName );
                    // Resursive call for each subdirectory.
                    WalkDirectoryTree ( dirInfo , filespec );
                }
            }
            return filesindex;
        }
        private static void AddFilesToList ( FileInfo [ ] files , ref int filesindex )
        {
            foreach ( System . IO . FileInfo fi in files )
            {
                // If we want to open, delete or modify the file, then
                // thea try-catch block is required here to handle the case
                // where the file has been deleted since the call to TraverseTree().

                string Files = fi . FullName . ToUpper ( );
                if ( Files . Contains ( ".G.CS" ) || Files . Contains ( "ASSEMBLY" ) || Files . Contains ( ".TXT" ) || Files . Contains ( ".DAT" ) )
                    continue;
                else
                {
                    AllFileNames [ filesindex++ ] = fi . Name;
                    FullPathFileNameList . Add ( fi . FullName );

                }
            }
        }

        private static int DoSetup ( string argstring )
        {
            DirectoryInfo di = new ( argstring );
            TotalFiles = WalkDirectoryTree ( di , filespec );
            if ( TotalFiles == 0 )
            {
                return 0;
            }
            //FilesArray mirrors AllFilesList, BUT it holds the file name only
            // whereas AllFilesList holds  fully qualified path+name
            string [ ] tmparray = new string [ TotalFiles ];
            for ( int i = 0 ; i < TotalFiles ; i++ )
                tmparray [ i ] = AllFileNames [ i ];
            // list of file names only
            AllFileNames = tmparray;

            // list of Path+file names 
            pathfilename = FullPathFileNameList [ 0 ];
            TotalFiles = FullPathFileNameList . Count;
            searchpath = argstring;
            return TotalFiles;
        }

        private static void CreateDupesFile ( )
        {
            //StringBuilder sbMethods = new ( );
            //string buff = $"RESULTS - DUPLICATED METHOD NAMES\n\nThis List should be a fairly comprehensive list of all duplicated Methods identified in the C# files in {searchpath}.\n\n";
            //sbMethods . Append ( buff );
            //sbMethods . Append ( $"Report Created : {DateTime . Now . ToString ( )}\n\n" );
            //sbMethods . Append ( $"FILENAME = C:\\Wpfmain\\Documentation\\MethodsIndex . txt,\n\n" );

            //sbMethods . Append ( $"ERRORS - DUPLICATE METHOD NAMES\n" );
            //sbMethods . Append ( $"Report Created : {DateTime . Now . ToString ( )}\n" );
            //sbMethods . Append ( $"Current file :{pathfilename}\n\n" );

            //if ( ErrorPaths . Count > 0 )
            //{
            //    int indx = 0;
            //    Tuple<int , string , string> [ ] tuples = new Tuple<int , string , string> [ ErrorPaths . Count ];

            //    // Add current source file to top of list
            //    foreach ( Tuple<int , string , string> item in ErrorPaths )
            //    {
            //        Tuple<int , string , string> currenttuple = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
            //        List<Tuple<int , string , string>> matches = Utilities . FindAllDupeMatches ( ErrorPaths: ErrorPaths , currenttuple );
            //        tuples [ indx++ ] = Tuple . Create ( item . Item1 , item . Item2 , item . Item3 );
            //    }


            //    // Now sort  the tuples              
            //    Array . Sort ( tuples , new SortTupleItem2 ( ) );

            //    // now add them to StringBuilder 
            //    for ( int x = 0 ; x < tuples . Length ; x++ )
            //    {
            //        Tuple<int , string , string> tup = tuples [ x ];
            //        if ( x == 0 )
            //            sbMethods . Append ( $"\n{tup . Item3}\n" );
            //        else
            //            sbMethods . Append ( $"     {tup . Item2} : Line {tup . Item1}\n" );
            //    }
            //}
            //File . WriteAllText ( $@"C:\wpfmain\Documentation\Duplicateslist.txt" , sbMethods . ToString ( ) );
            //Utilities . SortDupes ( ErrorPaths );
        }
    }
}
