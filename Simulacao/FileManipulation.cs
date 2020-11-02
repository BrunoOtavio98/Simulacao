using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Simulacao
{
    class FileManipulation
    {

        string BasePath = AppDomain.CurrentDomain.BaseDirectory + @"\Dados";
        FileStream fs;
        StreamWriter sw;
        public void WriteFile(List<string> data, string name)
        { 
            string path = BasePath + @"\" + name + ".txt";

            DirectoryManipulation();
            try
            {

                using (fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    using (sw = new StreamWriter(fs, Encoding.Default))
                    {
                        sw.WriteLine("Identificação -> Valor (em segundos)");
                        foreach (string item in data)
                        {
                            sw.WriteLine(item);
                        }
                    }
                }
            }
            finally
            {
                if (fs == null)
                    fs.Dispose();
            }
        }

        private void DirectoryManipulation()
        {
            if (!Directory.Exists(BasePath))
            {
             
                Directory.CreateDirectory(BasePath).Attributes &= ~FileAttributes.ReadOnly;
            }
        }
               
    }
}
