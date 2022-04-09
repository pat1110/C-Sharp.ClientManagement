using Client_Management.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Client_Management.Services
{
    class FileDataService
    {
        /// <summary>
        /// Löscht die im Pfad angegebene Datei.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn die Datei nicht gelöscht oder gefunden werden kann.</exception>
        public static void DeleteFile(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new DirectoryNotFoundException(ex.Message);
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Kopiert die in der List vom Typ string angegabnenen Dateien (Dateipfad benötigt) in das Ziel-Verzeignis (Verzeichnispfad benötigt).
        /// </summary>
        /// <param name="files"></param>
        /// <param name="dst"></param>
        /// <returns></returns>
        public static void CopyFiles(List<string> files, string dst)
        {
            foreach (string f in files)
            {
                try
                {
                    if (Directory.Exists(dst))
                    {
                        File.Copy(f, Path.Combine(dst, Path.GetFileName(f)));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    throw new DirectoryNotFoundException(ex.Message);
                }
                catch (DirectoryNotFoundException ex)
                {
                    throw new DirectoryNotFoundException(ex.Message);
                }
                catch (IOException ex)
                {
                    throw new IOException(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        /// <summary>
        /// Generiert oder überschreibt eine Datei, die mit dem Übergabeparameter path vom Typ string übergeben wird (Dateipfad) mit dem Inhalt, der im Übergabeparameter text vom Typ string mitgegeben wird.
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Wird geworfen, wenn etwas mit dem schreiben fehlschlägt.</exception>
        public static async Task WriteText(string txt, string path)
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(path));
                }
                await File.WriteAllTextAsync(path, txt);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new DirectoryNotFoundException(ex.Message);
            }
            catch (IOException ex)
            {
                throw new IOException(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Liest den gesamten Textinhalt von der Datei, die mit dem Übergabeparameter path (Dateipfad) vom Typ String übergeben wird ein und gibt diesen zurück.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Textinhalt der Datei</returns>
        /// <exception cref="Exception">Wird geworfen, wenn die angegebene Datei aus irgeneinem Grund nicht gelesen/gefunden/... werden kann.</exception>
        public static string ReadText(string path)
        {
            try
            {
                string txt = File.ReadAllText(path);
                return txt;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Speichert das übergebene Objekt als XML-Datei (path = Dateipfad).
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        public static void Serialize(object obj, String path)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Liest das übergebene Objekt von der XML-Datei (path = Dateipfad).
        /// </summary>
        /// <param name="objType"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<Object> Deserialize(string objType, String path)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(Settings));
            try
            {
                TextReader reader = new StreamReader(path);
                object obj = deserializer.Deserialize(reader);
                reader.Close();

                switch (objType)
                {
                    case "settings":
                        Settings.SetInstance((Settings)obj);
                        return null;
                    default:
                        return obj;
                }
            } 
            catch(IOException ex)
            {
                throw new Exception("IOException:\n\n" + ex.Message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
