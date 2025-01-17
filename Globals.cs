namespace PdfMergeV1
{
    internal class Globals
    {
        const string CONFIGURATION_FILE_NAME = "PdfMerge.cfg";
        const string CONFIGURATION_FILE_NEW_NAME = "PdfMergeCfg.new";
        const string CONFIGURATION_FILE_OLD_NAME = "PdfMergeCfg.old";

        //Note: These paths must end with a backslash:
        public static string PathToDataFiles { get; set; } = "";
        public static string PathToSourceFiles { get; set; } = "";
        public static string PathToOutputFiles { get; set; } = "";
        public static string PathToFontFiles { get; set; } = "";
        public static string PathToMergeFiles { get; set; } = "";

        public static int ReadCfgFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(CONFIGURATION_FILE_NAME))
                {
                    string? line, key, value;
                    string delimStr = "=";
                    char[] delimiter = delimStr.ToCharArray();
                    string[] split;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.Length > 0)
                        {
                            split = line.Split(delimiter, 2);
                            key = split[0];
                            value = split[1];
                            switch (key)
                            {
                                case "PathToDataFiles":
                                    PathToDataFiles = value;
                                    break;
                                case "PathToSourceFiles":
                                    PathToSourceFiles = value;
                                    break;
                                case "PathToOutputFiles":
                                    PathToOutputFiles = value;
                                    break;
                                case "PathToFontFiles":
                                    PathToFontFiles = value;
                                    break;
                                case "PathToMergeFiles":
                                    PathToMergeFiles = value;
                                    break;
                            }
                        }
                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nError reading configuration file ({CONFIGURATION_FILE_NAME}), or the file was not found in working directory.");
                //Console.WriteLine(ex.ToString());
                return -1;
            }

        }

        public static void WriteCfgFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(CONFIGURATION_FILE_NEW_NAME))
                {
                    sw.WriteLine("PathToDataFiles=" + PathToDataFiles);
                    sw.WriteLine("PathToSourceFiles=" + PathToSourceFiles);
                    sw.WriteLine("PathToOutputFiles=" + PathToOutputFiles);
                    sw.WriteLine("PathToFontFiles=" + PathToFontFiles);
                    sw.WriteLine("PathToMergeFiles=" + PathToMergeFiles);
                }
                if (File.Exists(CONFIGURATION_FILE_OLD_NAME))
                    File.Delete(CONFIGURATION_FILE_OLD_NAME);
                File.Move(CONFIGURATION_FILE_NAME, CONFIGURATION_FILE_OLD_NAME);
                File.Move(CONFIGURATION_FILE_NEW_NAME, CONFIGURATION_FILE_NAME);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing configuration file");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}