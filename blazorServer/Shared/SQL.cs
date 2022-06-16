using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Npgsql;

namespace blazorServer.Shared
{
    public class ResponseEventArgs : EventArgs
    {
        public string? Message { get; } = string.Empty;
        public ResponseEventArgs(string? msg) { Message = msg; }
    }
    public enum Driver {Db2, PostgreSQL }
    public class SQL : IDisposable
    {
        private bool disposed = false;
        private readonly SafeHandle Handle = new Microsoft.Win32.SafeHandles.SafeFileHandle(IntPtr.Zero, true);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                Handle.Dispose();
                // Free any other managed objects here.
                Table.Dispose();
            }
            disposed = true;
        }

        public event CompletedEventHandler? Completed;
        public delegate void CompletedEventHandler(object sender, ResponseEventArgs? e);

        public string ConnectionString { get; }
        public string Instruction { get; }
        public Driver Driver { get; } = Driver.Db2;
        public DataTable Table { get; private set; } = new DataTable();
        public TimeSpan Elapsed => Ended.Subtract(Started);
        public DateTime Started { get; private set; }
        public bool Busy { get; private set; }
        public DateTime Ended { get; private set; }
        public string Response { get; private set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public object? Tag { get; set; }

        public SQL(string sqlConnectionString, string sqlInstruction, Driver sqlDriver = Driver.Db2)
        {
            ConnectionString = sqlConnectionString ?? string.Empty;
            Instruction = sqlInstruction ?? string.Empty;
            Driver = sqlDriver;
        }
        public async Task<string> ExecuteAsync()
        {
            Execute();
            while (Response != null) { await Task.Delay(25); }
            return Response ?? string.Empty;
        }
        public void Execute(bool RunInBackground = true)
        {
            Started = DateTime.Now;
            Ended = DateTime.MinValue;
            Busy = true;
            Response = string.Empty;

            if (RunInBackground)
            {
                var withBlock = new BackgroundWorker();
                withBlock.DoWork += Execute;
                withBlock.RunWorkerCompleted += Executed;
                withBlock.RunWorkerAsync();
            }
            else
            {
                Execute();
                Executed();
            }
        }
        private void Execute(object? sender, DoWorkEventArgs e)
        {
            if (sender != null) { ((BackgroundWorker)sender).DoWork -= Execute; }
            Execute();
        }
        private void Execute()
        {
            if (ConnectionString.Any() & Instruction.Any())
            {
                if (Driver == Driver.PostgreSQL)
                {
                    using NpgsqlConnection Connection_postgre = new(ConnectionString);
                    try
                    {
                        Connection_postgre.Open();
                        using NpgsqlDataAdapter Adapter = new(Instruction, Connection_postgre);
                        Table = new DataTable();
                        Adapter.Fill(Table);
                        Connection_postgre.Close();
                        Ended = DateTime.Now;
                        Table.Locale = CultureInfo.InvariantCulture;
                        Table.Namespace = "<PostgreSQL>";
                        Response = JsonConvert.SerializeObject(Table, Formatting.Indented);
                    }
                    catch (NpgsqlException ex)
                    {
                        Connection_postgre.Close();
                        Ended = DateTime.Now;
                        Response = ex.Message;
                    }
                }
                else
                    using (OdbcConnection Connection_Db2 = new(ConnectionString))
                    {
                        try
                        {
                            Connection_Db2.Open();
                            try
                            {
                                using OdbcDataAdapter Adapter = new(Instruction, Connection_Db2);
                                Table = new DataTable();
                                Adapter.Fill(Table);
                                Connection_Db2.Close();
                                Ended = DateTime.Now;
                                Table.Locale = CultureInfo.InvariantCulture;
                                Table.Namespace = "<DB2>";
                                Response = JsonConvert.SerializeObject(Table, Formatting.Indented);
                            }
                            catch (OdbcException odbcGeneral)
                            {
                                Connection_Db2.Close();
                                Ended = DateTime.Now;
                                Response = odbcGeneral.Message;
                            }
                            catch (ArgumentOutOfRangeException outOfRange)
                            {
                                Connection_Db2.Close();
                                Ended = DateTime.Now;
                                Response = outOfRange.Message;
                            }
                        }
                        catch (OdbcException odbcOpenException)
                        {
                            Connection_Db2.Close();
                            Ended = DateTime.Now;
                            Response = odbcOpenException.Message;
                        }
                    }
            }
            else
            {
                List<string> missing = new();
                if (ConnectionString.Length == 0) { missing.Add("connection"); }
                if (Instruction.Length == 0) { missing.Add("instruction"); }
                Response = $"Missing {string.Join(" and ", missing)}";
            }
        }
        private void Executed(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (sender != null)
            {
                var withBlock = (BackgroundWorker)sender;
                withBlock.RunWorkerCompleted -= Executed;
                withBlock.Dispose();
            }
            Executed();
        }
        private void Executed()
        {
            Busy = false;
            Ended = Ended == DateTime.MinValue ? DateTime.Now : Ended;
            Completed?.Invoke(this, new ResponseEventArgs(Response));
        }
    }
}
