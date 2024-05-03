using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using Serilog.Context;
using SharpCifs.Smb;
using Steeltoe.Common.Net;
using SharpCifs.Util.Sharpen;
using System.Diagnostics.CodeAnalysis;


namespace Fctr.Edison.NetCore.Core.File
{
    [ExcludeFromCodeCoverage]
    public class FileManager
    {
        public async virtual Task<byte[]> ReadFileContentAsync(string storagePath, string path, string userDomain = null,
            string username = null, string password = null, ILogger logger = null)
        {
            long dnsResolveElapsedMs = 0;
            string hostIp = storagePath;

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                IPAddress address = Dns.GetHostAddresses(storagePath).FirstOrDefault();
                dnsResolveElapsedMs = sw.ElapsedMilliseconds;

                if (address != null)
                {
                    hostIp = address.ToString();
                    path = path.Replace(storagePath, hostIp);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Host address not found!");
            }
            sw.Restart();
            long fileLength = 0;
            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var credential = new NetworkCredential(username, password, userDomain);
                    using (var share = new WindowsNetworkFileShare(Path.GetDirectoryName(path), credential))
                    {
                        Stream outStream = new FileStream(path, FileMode.Open, FileAccess.Read);
                        MemoryStream memStream = new MemoryStream();
                        fileLength = outStream.Length;
                        await outStream.CopyToAsync(memStream).ConfigureAwait(false);
                        return memStream.ToArray();
                    }
                }
                else
                {
                    NtlmPasswordAuthentication auth = new NtlmPasswordAuthentication(userDomain, username, password);

                    SmbFile file = new SmbFile(path, auth);
                    Stream outStream = await file.GetInputStreamAsync().ConfigureAwait(false);
                    MemoryStream memStream = new MemoryStream();
                    fileLength = outStream.Length;
                    await outStream.CopyToAsync(memStream).ConfigureAwait(false);

                    return memStream.ToArray();
                }
            }
            catch (SmbException ex)
            {
                logger.LogError(ex.Message);
                throw;
            }
            finally 
            {
                using (LogContext.PushProperty("ContentPath", path))
                using (LogContext.PushProperty("IpResolveElapsedMs", dnsResolveElapsedMs))
                using (LogContext.PushProperty("ContentReadElapsedMs", sw.ElapsedMilliseconds))
                using (LogContext.PushProperty("FileLength", fileLength))
                using (LogContext.PushProperty("NASIp", hostIp))
                    logger.LogDebug("Content read done");
            }
        }

        public async virtual Task WriteFileContentAsync(string storagePath, string path, string folderPath, byte[] bytes, string userDomain = null,
            string username = null, string password = null, ILogger logger = null)
        {
            long dnsResolveElapsedMs = 0;
            string hostIp = storagePath;

            Stopwatch sw = Stopwatch.StartNew();
            try
            {
                IPAddress address = Dns.GetHostAddresses(storagePath).FirstOrDefault();
                dnsResolveElapsedMs = sw.ElapsedMilliseconds;

                if (address != null)
                {
                    hostIp = address.ToString();
                    path = path.Replace(storagePath, hostIp);
                    folderPath = folderPath.Replace(storagePath, hostIp);
                }
            }
            catch
            {
                logger.LogError("Host address not found!");
            }

            sw.Restart();

            try
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    var credential = new NetworkCredential(username, password, userDomain);
                    var index = folderPath.LastIndexOf("\\");
                    var mainPath = "";
                    if (index > 0)
                        mainPath = folderPath.Substring(0, index);

                    using (var share = new WindowsNetworkFileShare(Path.GetDirectoryName(mainPath), credential))
                    {
                        if (!System.IO.File.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }
                        if (System.IO.File.Exists(path))
                        {
                            await using (var stream = new FileStream(path, FileMode.Truncate, FileAccess.Write, FileShare.None))
                            {
                                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                            }
                        }
                        else
                        {
                            await using (var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                            {
                                await stream.WriteAsync(bytes, 0, bytes.Length).ConfigureAwait(false);
                            }
                        }
                    }
                }
                else
                {
                    NtlmPasswordAuthentication auth = new NtlmPasswordAuthentication(userDomain, username, password);
                    SmbFile file = new SmbFile(folderPath, auth);
                    if (!file.Exists())
                        file.Mkdirs();
                    file = new SmbFile(path, auth);
                    if (!file.Exists())
                        file.CreateNewFile();
                    using (OutputStream writeStream = await file.GetOutputStreamAsync().ConfigureAwait(false))
                    {
                        writeStream.Write(bytes);
                    }
                }
            }
            catch (SmbException ex)
            {
                logger.LogError(ex.Message);
            }
            finally
            {
                using (LogContext.PushProperty("ContentPath", path))
                using (LogContext.PushProperty("IpResolveElapsedMs", dnsResolveElapsedMs))
                using (LogContext.PushProperty("ContentWriteElapsedMs", sw.ElapsedMilliseconds))
                using (LogContext.PushProperty("FileLength", bytes.Length))
                using (LogContext.PushProperty("NASIp", hostIp))
                    logger.LogDebug("Content write done");
            }
        }
    }
}
