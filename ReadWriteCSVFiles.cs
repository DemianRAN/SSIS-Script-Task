#region Namespaces
using System;
using System.Data;
using Microsoft.SqlServer.Dts.Runtime;
using System.Windows.Forms;
using System.IO;
using System.Text;
#endregion

namespace ST_xxxxxxxxxxxxxxxxxx
{
	[Microsoft.SqlServer.Dts.Tasks.ScriptTask.SSISScriptTaskEntryPointAttribute]
	public partial class ScriptMain : Microsoft.SqlServer.Dts.Tasks.ScriptTask.VSTARTScriptObjectModelBase
	{
        public void Main()
        {
            try
            {
                Int32 ctr = 0;
                string FilePath = Dts.Variables["User::FolderPath"].Value.ToString();
                string InFilePath = Dts.Variables["User::ImportPath"].Value.ToString();

                var filePaths = Directory.GetFiles(FilePath, "*.csv");

                string Line = string.Empty;

                foreach (string csvfile in filePaths)
                {
                    string filename = Path.GetFileName(csvfile);
                    using (StreamReader sr = new StreamReader(csvfile))
                    using (StreamWriter sw = new StreamWriter(InFilePath + filename, false, Encoding.UTF8))
                    {
                        System.IO.StreamReader SourceFile = new System.IO.StreamReader(csvfile);

                        ctr = 0;
                        while ((Line = SourceFile.ReadLine()) != null)
                        {
                            if (ctr != 0)
                            {
                                Line = Line.Trim();
                                if (Line.StartsWith("\"") && Line.EndsWith("\""))
                                {
                                    Line = Line.TrimStart('"').TrimEnd('"');
                                }
                            }
                            sw.WriteLine(Line);
                            sw.Flush();
                            ctr++;
                        }
                        sw.Close();
                    }
                }

                Dts.TaskResult = (int)ScriptResults.Success;

            }
            catch (Exception ex)
            {
                Dts.Events.FireError(0, "Exception from Script Task", ex.Message + "\r" + ex.StackTrace, String.Empty, 0);
                Dts.TaskResult = (int)ScriptResults.Failure;
            }
        }

        enum ScriptResults
        {
            Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
            Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        };
        #endregion
	}
}
