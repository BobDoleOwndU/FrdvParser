using System;
using System.IO;
using System.Reflection;

namespace FrdvParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (Path.GetExtension(args[0]) == ".frdv")
                {
                    using (FileStream readStream = new FileStream(args[0], FileMode.Open))
                    using (FileStream writeStream = new FileStream(args[0] + ".txt", FileMode.Create))
                    {
                        try
                        {
                            Frdv frdv = new Frdv();
                            frdv.Read(readStream);
                            frdv.WriteToTextFile(writeStream);

                            readStream.Close();
                            writeStream.Close();
                        } //try
                        catch (Exception e)
                        {
                            readStream.Close();
                            writeStream.Close();

                            using (FileStream errorStream = new FileStream($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\stacktrace.txt", FileMode.Create))
                            {
                                StreamWriter writer = new StreamWriter(errorStream);
                                writer.AutoFlush = true;
                                writer.WriteLine(e.Message);
                                writer.Write(e.StackTrace);
                                errorStream.Close();
                            } //using
                        } //catch
                    } //using
                } //if
            } //if
        } //Main
    } //Program
} //FrdvParser
