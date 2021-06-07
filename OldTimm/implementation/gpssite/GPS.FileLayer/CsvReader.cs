using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using Debug = System.Diagnostics.Debug;
using System.Globalization;
using System.IO;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Collections;

namespace GPS.FileLayer
{

    /// <summary>
    /// Original Copyright 2005 Bytecode Pty Ltd. (Java version)
    /// C# (.NET) version (c) 2008 by Auri Rahimzadeh 
    /// at The Auri Group, LLC (TAG), http://www.aurigroup.com.
    /// 
    /// Licensed under the Apache License, Version 2.0 (the "License");
    /// you may not use this file except in compliance with the License.
    /// You may obtain a copy of the License at
    ///
    /// http://www.apache.org/licenses/LICENSE-2.0
    ///
    /// Unless required by applicable law or agreed to in writing, software
    /// distributed under the License is distributed on an "AS IS" BASIS,
    /// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    /// See the License for the specific language governing permissions and
    /// limitations under the License.
    /// </summary>
    public partial class CsvReader
    {
        /// <summary>
        /// The instance of the CSV file we're reading.
        /// </summary>
        private StreamReader _CSVfile;

        /// <summary>
        /// Determines whether the first row of data represents the column names. Defaults to false.
        /// </summary>
        private bool _firstRowIsColumnNames = false;

        /// <summary>
        /// Determines whether there's another line remaining in the CSV file.
        /// </summary>
        private bool _hasNext = true;

        /// <summary>
        /// The separator to use. Normally this would be a comma character, but your CSV
        /// file may vary, hence the reason for your ability to change it.
        /// </summary>
        private char _separator;

        /// <summary>
        /// The quote to use. Normally this would be a double-quote character, but your CSV
        /// file may vary, hence the reason for your ability to change it.
        /// </summary>
        private char _quotechar;

        private int _skipLines;

        private bool _linesSkipped;

        /// <summary>
        /// The filename to read.
        /// </summary>
        private string _sourceFilename = String.Empty;

        /// <summary>
        /// The default _separator to use if none is supplied to the constructor.
        /// </summary>
        public const char DEFAULT_SEPARATOR = ',';

        /// <summary>
        /// The default quote character to use if none is supplied to the constructor. 
        /// </summary>
        public const char DEFAULT_QUOTE_CHARACTER = '"';

        /// <summary>
        /// The default line to start reading.
        /// </summary>
        public const int DEFAULT_SKIP_LINES = 0;

        #region "Class Constructors"

        /// <summary>
        /// Constructs CSV reader using a comma for the _separator. 
        /// </summary>
        /// <param name="filename">String. The path to the CSV source file.</param>
        public CsvReader(string filename)
        {
            _sourceFilename = filename;
            _separator = DEFAULT_SEPARATOR;
        }

        /// <summary>
        /// Constructs CSV reader using a comma for the _separator. 
        /// </summary>
        /// <param name="filename">String. The path to the CSV source file.</param>
        /// <param name="filename">Boolean. Determines whether the first row of data contains the column names.</param>
        public CsvReader(string filename, bool firstRowIsColumnNames)
        {
            _sourceFilename = filename;
            _separator = DEFAULT_SEPARATOR;
            _firstRowIsColumnNames = firstRowIsColumnNames;
        }

        /// <summary>
        /// Constructs CSV reader with supplied _separator. 
        /// </summary>
        /// <param name="filename">String. The path to the CSV source file.</param>
        /// <param name="separator">Char. The delimiter to use for separating entries.</param>
        /// <param name="filename">Boolean. Determines whether the first row of data contains the column names.</param>
        public CsvReader(string filename, char separator, bool firstRowIsColumnNames)
        {
            _sourceFilename = filename;
            _separator = separator;
            _quotechar = DEFAULT_QUOTE_CHARACTER;
            _firstRowIsColumnNames = firstRowIsColumnNames;
        }

        /// <summary>
        /// Constructs CSV reader with supplied _separator and quote char. 
        /// </summary>
        /// <param name="filename">String. The path to the CSV source file.</param>
        /// <param name="separator">Char. The delimiter to use for separating entries.</param>
        /// <param name="quotechar">Char. The character to use for quoted elements.</param>
        /// <param name="filename">Boolean. Determines whether the first row of data contains the column names.</param>
        public CsvReader(string filename, char separator, char quotechar, bool firstRowIsColumnNames)
        {
            _sourceFilename = filename;
            _separator = separator;
            _quotechar = quotechar;
            _skipLines = DEFAULT_SKIP_LINES;
            _firstRowIsColumnNames = firstRowIsColumnNames;
        }

        /// <summary>
        /// Constructs CSV reader with supplied _separator, quote char, and line number on which to start parsing. 
        /// </summary>
        /// <param name="filename">String. The path to the CSV source file.</param>
        /// <param name="separator">Char. The delimiter to use for separating entries.</param>
        /// <param name="quotechar">Char. The character to use for quoted elements.</param>
        /// <param name="line">Internet. The line number to skip for start reading.</param>
        /// <param name="filename">Boolean. Determines whether the first row of data contains the column names.</param>
        public CsvReader(string filename, char separator, char quotechar, int line, bool firstRowIsColumnNames)
        {
            _sourceFilename = filename;
            _separator = separator;
            _quotechar = quotechar;
            _skipLines = DEFAULT_SKIP_LINES;
            _firstRowIsColumnNames = firstRowIsColumnNames;
        }

        #endregion

        /// <summary>
        /// Reads the entire file into a List with each element being a String[] of tokens.
        /// </summary>
        /// <returns>An ArrayList of String[], with each String[] representing a line of the file.</returns>
        public DataTable ParseAll()
        {
            // Local vars.
            ArrayList allElements = new ArrayList(); // All the string[] representing each line in the CSV file.
            DataTable dtbResult = new DataTable(); // The datatable we'll return with all the CSV elements parsed.
            String[] temp = null; // Temporary string array so we can move the CSV elements into the result datatable.
            int totalColumns = 0; // The total number of columns in the CSV so we know how many columns to use in the datatable.

            // Open the file.
            _CSVfile = System.IO.File.OpenText(_sourceFilename);

            try
            {
                // ************************************************
                // Get all the data from the CSV into an ArrayList.
                // ************************************************

                while (_hasNext)
                {
                    String[] nextLineAsTokens = readNext();
                    if (nextLineAsTokens != null)
                        allElements.Add(nextLineAsTokens);
                }

                // *************************************
                // Convert the ArrayList to a datatable.
                // *************************************

                // Create the columns.
                temp = (String[])allElements[0];
                for (int i = 0; i < temp.Length; i++)
                {
                    // If the first row is the column names, use those, otherwise use default column names.
                    DataColumn dc;
                    if (_firstRowIsColumnNames)
                        dc = new DataColumn(temp[i]);
                    else
                        dc = new DataColumn(String.Concat("Column", i.ToString()));
                    dc.DataType = Type.GetType("System.String");
                    dtbResult.Columns.Add(dc);
                }

                // Remember the total number of columns.
                totalColumns = temp.Length;

                // Add each string array in the ArrayList to the datatable.
                for (int i = 0; i < allElements.Count; i++)
                {
                    // Get the data to parse.
                    temp = (String[])allElements[i];

                    // Create the new datatable row.
                    DataRow dr = dtbResult.NewRow();

                    // Add each element to its appropriate column, up to the total number of columns, or the
                    // number of columns in the string array, whichever is lower.
                    for (int j = 0; j < temp.Length && j < totalColumns; j++)
                    {
                        // Move the data into the datacolumn.
                        dr[j] = temp[j].ToString();
                    }

                    // Add the new row.
                    dtbResult.Rows.Add(dr);
                    dtbResult.AcceptChanges();

                    // Clean up.
                    dr = null;
                }
            }
            catch
            {
                // Nothing... but we need to make sure we close the file at the end. I may add some
                // extra code here to be a bit more proper, but for now, this is just a placeholder.
            }
            finally
            {
                // Close the file.
                _CSVfile.Close();

                // Clean up.
                allElements = null;
            }

            // If there were are than 1 rows in the datatable, and the column headers were in the first
            // line of the file, remove the first row of the datatable since it's redundant data.
            if (_firstRowIsColumnNames && dtbResult.Rows.Count > 1)
            {
                dtbResult.Rows[0].Delete();
                dtbResult.AcceptChanges();
            }

            // Return the result.
            return dtbResult;
        }

        /// <summary>
        /// Reads the next line from the buffer and converts to a string array. 
        /// </summary>
        /// <returns>String[]. A string array with each comma-separated element as a separate entry.</returns>
        public String[] readNext()
        {

            String nextLine = getNextLine();
            return _hasNext ? ParseLine(nextLine) : null;
        }

        /// <summary>
        /// Reads the next line from the file. 
        /// </summary>
        /// <returns>String. The next line from the file without trailing newline.</returns>
        private String getNextLine()
        {
            if (!this._linesSkipped)
            {
                for (int i = 0; i < _skipLines; i++)
                {
                    _CSVfile.ReadLine();
                }
                this._linesSkipped = true;
            }
            String nextLine = _CSVfile.ReadLine();
            if (nextLine == null)
            {
                _hasNext = false;
            }
            return _hasNext ? nextLine : null;
        }

        /// <summary>
        /// Parses an incoming String and returns an array of elements. 
        /// </summary>
        /// <param name="nextLine">String. The string to parse.</param>
        /// <returns>String[]. The comma-tokenized list of elements, or null if nextLine is null.</returns>
        private String[] ParseLine(String nextLine)
        {
            if (nextLine == null)
            {
                return null;
            }

            ArrayList tokensOnThisLine = new ArrayList();
            StringBuilder sb = new StringBuilder();
            bool inQuotes = false;
            do
            {
                if (inQuotes)
                {
                    // continuing a quoted section, reappend newline
                    sb.Append("\n");
                    nextLine = getNextLine();
                    if (nextLine == null)
                        break;
                }
                for (int i = 0; i < nextLine.Length; i++)
                {
                    char c = nextLine.ToCharArray()[i];
                    if (c == _quotechar)
                    {
                        // this gets complex... the quote may end a quoted block, or escape another quote.
                        // do a 1-char lookahead:
                        if (inQuotes  // we are in quotes, therefore there can be escaped quotes in here.
                            && nextLine.Length > (i + 1)  // there is indeed another character to check.
                            && nextLine.ToCharArray()[i + 1] == _quotechar)
                        { // ..and that char. is a quote also.
                            // we have two quote chars in a row == one quote char, so consume them both and
                            // put one on the token. we do *not* exit the quoted text.
                            sb.Append(nextLine.ToCharArray()[i + 1]);
                            i++;
                        }
                        else
                        {
                            inQuotes = !inQuotes;
                            // the tricky case of an embedded quote in the middle: a,bc"d"ef,g
                            if (i > 2 //not on the begining of the line
                                    && nextLine.ToCharArray()[i - 1] != _separator //not at the begining of an escape sequence 
                                    && nextLine.Length > (i + 1) &&
                                    nextLine.ToCharArray()[i + 1] != _separator //not at the	end of an escape sequence
                            )
                            {
                                sb.Append(c);
                            }
                        }
                    }
                    else if (c == _separator && !inQuotes)
                    {
                        tokensOnThisLine.Add(sb.ToString());
                        sb = new StringBuilder(); // start work on next token
                    }
                    else
                    {
                        sb.Append(c);
                    }
                }
            } while (inQuotes);
            tokensOnThisLine.Add(sb.ToString());
            return (String[])tokensOnThisLine.ToArray(System.Type.GetType("System.String"));
        }

    }

	/// <summary>
	/// Represents a reader that provides fast, non-cached, forward-only access to CSV data.  
	/// </summary>
	public partial class CsvReader
	{
        public static string FolderPath =
            System.Web.HttpContext.Current.Server.MapPath(@"~/Files/ImportFiles/");
        static OdbcConnection oConn;

        public CsvReader()
        { }

        /// <summary>
        /// Read the file to get DataTable
        /// </summary>
        /// <param name="fileName">the file name</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string fileName)
        {
            oConn = ODBCHelper.CreateCSVConnection(FolderPath);
            return ODBCHelper.ExecuteDataTable(oConn,FolderPath,fileName);
        }

        /// <summary>
        /// Read the file to get DataTable
        /// </summary>
        /// <param name="folderParh">the folder path</param>
        /// <param name="fileName">the file name</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteDataTable(string folderParh, string fileName)
        {
            oConn = ODBCHelper.CreateCSVConnection(FolderPath);
            return ODBCHelper.ExecuteDataTable(oConn, FolderPath, fileName);
        }

        /// <summary>
        /// Get DataTable
        /// </summary>
        /// <param name="folderParh">the folder path</param>
        /// <param name="sql">sql query</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecuteSql(string folderParh, string sql)
        {
            oConn = ODBCHelper.CreateCSVConnection(folderParh);

            return ODBCHelper.ExecuteDataTable(oConn, sql); ;
        }

        //public static DataTable ExecuteSql(string sql)
        //{
        //    DataTable dt = new DataTable();
        //    string connectionString = @"Password=123456;Persist Security Info=True;User ID=sa;Initial Catalog=GPS;Data Source=xuxiaoyang-pc\xuxiaoyang;";
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        SqlDataAdapter adapter = new SqlDataAdapter();
        //        adapter.SelectCommand = new SqlCommand(sql, connection);
        //        try
        //        {
        //            adapter.Fill(dt);
        //        }
        //        catch (Exception ex)
        //        {
        //            throw ex;
        //        }
        //    }
        //    return dt;
        //}
	}
}
