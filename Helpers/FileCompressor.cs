using System.IO.Compression;
namespace Alga.wwwcore.Helpers;

static class FileCompressor
{
    // Сжатие одного файла указанным форматом
    // public static void CompressFile(string path, CompressionType type, CompressionLevel level = CompressionLevel.SmallestSize)
    // {
    //     // .br → *.br, .gz → *.gz
    //     var ext = type switch
    //     {
    //         CompressionType.Brotli => ".br",
    //         CompressionType.Gzip  => ".gz",
    //         _ => throw new ArgumentOutOfRangeException(nameof(type))
    //     };

    //     var outPath = path + ext;

    //     // FileOptions.SequentialScan снижает лишнее кеш‑трэшинг,
    //     // а 32 Кб буфер оптимален под 4 Кб страничный размер и SSD.
    //     const int BufferSize = 32 * 1024;

    //     using var source = new FileStream(
    //         path, FileMode.Open, FileAccess.Read, FileShare.Read,
    //         BufferSize, FileOptions.SequentialScan);

    //     using var dest = new FileStream(
    //         outPath, FileMode.Create, FileAccess.Write, FileShare.None,
    //         BufferSize, FileOptions.WriteThrough);

    //     Stream compressionStream = type switch
    //     {
    //         CompressionType.Brotli => new BrotliStream(dest, level, leaveOpen: false),
    //         _                      => new GZipStream(dest, level, leaveOpen: false)
    //     };

    //     // копируем блоками — CopyToAsync внутри уже использует ArrayPool
    //     source.CopyTo(compressionStream, BufferSize);
    // }

    public static void CompressFile(string path,
                                    CompressionType type,
                                    CompressionLevel level = CompressionLevel.SmallestSize)
    {
        var ext     = type switch
        {
            CompressionType.Brotli => ".br",
            CompressionType.Gzip  => ".gz",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
        var outPath = path + ext;

        const int BufferSize = 32 * 1024;

        using var source = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            BufferSize, FileOptions.SequentialScan);

        using var dest = new FileStream(
            outPath, FileMode.Create, FileAccess.Write, FileShare.None,
            BufferSize, FileOptions.WriteThrough);

        // ВАЖНО: оборачиваем в using, чтобы гарантированно вызвать Dispose/Flush
        using var compressionStream = type switch
        {
            CompressionType.Brotli => (Stream)new BrotliStream(dest, level, leaveOpen: false),
            CompressionType.Gzip  => (Stream)new GZipStream(dest, level, leaveOpen: false),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        source.CopyTo(compressionStream, BufferSize);
        // Dispose у compressionStream вызовется в конце блока using
    }

    // Пакетная параллельная обработка
    public static void CompressMany(IEnumerable<string> files)
    {
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount },
            file =>
            {
                CompressFile(file, CompressionType.Brotli);
                CompressFile(file, CompressionType.Gzip);
            });
    }

    public enum CompressionType { Brotli, Gzip }
}