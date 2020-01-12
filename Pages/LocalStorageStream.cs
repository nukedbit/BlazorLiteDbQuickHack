using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Blazored.LocalStorage;
using System.Threading.Tasks;
using LiteDB;

public class LocalStorageStream : Stream
{
    private MemoryStream mem;
    private static string storagekey = "litedb";
    public LocalStorageStream(ILocalStorageService localStorage, byte[] bytes = null){
        this.localStorage = localStorage;
        if(bytes is null){
            bytes = new byte[0];
        }
        this.mem = new MemoryStream();
        this.mem.Write(bytes);
    } 


    public static async Task<LocalStorageStream> Load(ILocalStorageService localStorage){
        var bts = await localStorage.GetItemAsync<byte[]>(storagekey);

        return new LocalStorageStream(localStorage, bts);
    }

    private readonly ILocalStorageService localStorage;

    public override bool CanRead => mem.CanRead;

    public override bool CanSeek => mem.CanSeek;

    public override bool CanWrite => mem.CanWrite;

    public override long Length => mem.Length;

    public override long Position { get => mem.Position; set => mem.Position = value; }

    public override void Flush()
    {
        Console.WriteLine("Flush");
        this.SaveOnStorage();
    }
 

    public override async Task FlushAsync(System.Threading.CancellationToken cancellationToken)
    { 
        await this.SaveOnStorage();
    }

    private async Task SaveOnStorage(){ 
        await localStorage.SetItemAsync(storagekey, this.mem.ToArray());
    }

    public override int Read(byte[] buffer, int offset, int count) => mem.Read(buffer, offset, count);

    public override long Seek(long offset, SeekOrigin origin) => mem.Seek(offset, origin);

    public override void SetLength(long value) => mem.SetLength(value);

    public override void Write(byte[] buffer, int offset, int count)
    {
        mem.Write(buffer, offset, count); 
    }

    public override void Close(){
        Console.WriteLine("close");
        this.SaveOnStorage();
        base.Close();  
    }

    protected override void Dispose(bool disposing){
          Console.WriteLine("Dispose");
         this.SaveOnStorage();
         base.Dispose(disposing); 
    }
}
