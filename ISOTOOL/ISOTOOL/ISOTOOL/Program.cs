using DiscUtils;
using DiscUtils.Fat;
using DiscUtils.Partitions;
using DiscUtils.Setup;
using DiscUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ISOTOOL
{
    class Program
    {
       

        static void Main(string[] args)
        {
            SetupHelper.RegisterAssembly(typeof(Program).Assembly);
            SetupHelper.RegisterAssembly(typeof(FatFileSystem).Assembly);
           
            if (args.Length == 0)
            {
                Console.WriteLine("ISOTOOL.exe EspPartitionDir [IsoFilePath.iso]");
                return;
            }

            string exedir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string diskfile = Path.Combine(exedir, "tmp.img");
            string isopath = Path.Combine(exedir, "Result.iso");
            string fromdir = args[0];
            if (args.Length >1)
            {

                isopath = args[1];
            }
            if (File.Exists(diskfile))
            {
                File.Delete(diskfile);
            }
            DirectoryInfo dirinf = new DirectoryInfo(fromdir);
            List<DirectoryInfo> subdir1 = dirinf.EnumerateDirectories().ToList();
            List<FileInfo> subfile1 = dirinf.EnumerateFiles().ToList();
            List<string> adddir = new List<string>();
            // Dictionary<string, byte[]> createfilesroot = new Dictionary<string, byte[]>();
            Dictionary<string, byte[]> createfiles = new Dictionary<string, byte[]>();
            foreach (FileInfo fileInfo in subfile1)
            {
                string rootfile = fileInfo.FullName;
                string filename = fileInfo.Name;

                byte[] rootdata = File.ReadAllBytes(rootfile);

                createfiles.Add(filename, rootdata);
                Console.WriteLine(filename);
            }

            foreach (DirectoryInfo subdir in subdir1)
            {
                string dir1name = subdir.Name;
                adddir.Add(dir1name);

                Console.WriteLine(dir1name);

                subfile1 = subdir.EnumerateFiles().ToList();
                foreach (FileInfo fileInfo in subfile1)
                {
                    string rootfile = fileInfo.FullName;
                    string filename = dir1name + @"\" + fileInfo.Name;

                    byte[] rootdata = File.ReadAllBytes(rootfile);

                    createfiles.Add(filename, rootdata);
                    Console.WriteLine(filename);
                }



                List<DirectoryInfo> subdir2 = subdir.GetDirectories().ToList();
                foreach (DirectoryInfo subdir2data in subdir2)
                {
                    string dir1name2 = subdir2data.Name;
                    string dirname2 = dir1name + @"\" + dir1name2;

                    adddir.Add(dirname2);

                    Console.WriteLine(dirname2);
                    subfile1 = subdir2data.EnumerateFiles().ToList();
                    foreach (FileInfo fileInfo in subfile1)
                    {
                        string rootfile = fileInfo.FullName;
                        string filename = dirname2 + @"\" + fileInfo.Name;

                        byte[] rootdata = File.ReadAllBytes(rootfile);

                        createfiles.Add(filename, rootdata);
                        Console.WriteLine(filename);
                    }

                }
            }

            int desksize = 610 * 1024 * 1024;
            int volsize = 600 * 1024 * 1024;

            using (FileStream destStream = new FileStream(diskfile, FileMode.CreateNew, FileAccess.ReadWrite))
            {
                using (VirtualDisk dummyDisk = DiscUtils.Raw.Disk.Initialize(destStream, Ownership.Dispose, desksize))
                {
                  
                    BiosPartitionTable guidPartitionTable = BiosPartitionTable.Initialize(dummyDisk);
                    dummyDisk.Signature = new Random().Next();
                    int idxPartition = guidPartitionTable.Create(volsize, WellKnownPartitionType.EFI, false);

                    using (FatFileSystem fsInfo = FatFileSystem.FormatPartition(dummyDisk, idxPartition, "ESP"))
                    {

                        

                        foreach (string dirname in adddir)
                        {
                            fsInfo.CreateDirectory(dirname);
                        }

                        foreach (KeyValuePair<string, byte[]> kv in createfiles)
                        {
                            string filename = kv.Key;
                            using (SparseStream newSparseStream = fsInfo.OpenFile(filename, FileMode.CreateNew, FileAccess.Write))
                            {

                                byte[] rootdata = kv.Value;

                                newSparseStream.Write(rootdata, 0, rootdata.Length);

                            }
                        }

                    }

                }
            }

            Console.WriteLine("Build Disk File:=>"+ diskfile);
            MakeISOTools isotool = new MakeISOTools();
          
            isotool.writeIso(isopath, diskfile);
            if (File.Exists(diskfile))
            {
                File.Delete(diskfile);
            }
            return;

        }

    
    }
}
