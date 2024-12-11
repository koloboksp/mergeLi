using System.IO;
using Atom.Variant;
using Atom;
using System;
using UnityEngine;
using System.Threading.Tasks;
using Atom.Timers;
using Save;

namespace Assets.Scripts.Core.Storage
{
    //*********************************************************************************************
    public class BaseStorage : IStorage
    {
        public static readonly string ConfigurationsDirectory = Application.persistentDataPath + Path.DirectorySeparatorChar;
        private static readonly string mPath = ConfigurationsDirectory + "UserStorageInfo.dat";

        private readonly StoragePool<MemoryPacking> mMemoryPacking = new();

        private byte[] mPass;
        private Task mInitializeTask;

        public async Task InitializeAsync()
        {
            var timer = new SmallTimer();
            mInitializeTask = Task.Run(LoadFromFile);
            await mInitializeTask;
            Debug.Log($"<color=#99ff99>Time initialize {nameof(BaseStorage)}: {Conversion.ToString((float)timer.Update(), 2)}.</color>");
        }
        //-----------------------------------------------------------------------------------------
        public Task GetInitializeTask()
        {
            return mInitializeTask;
        }
        //-----------------------------------------------------------------------------------------
        private void LoadFromFile()
        {
            if (!File.Exists(mPath))
                mPass = PasswordCreation();
            else
            {
                try
                {
                    mPass = PasswordReading();
                }
                catch
                {
                    mPass = PasswordCreation();
                }
            }
        }
        //-----------------------------------------------------------------------------------------
        private static byte[] UIToPass()
        {
            var ui = SystemInfo.deviceUniqueIdentifier;
            ui = ui.Replace("-", "");
            ui = ui.ToLower();

            var str = GuidEx.Empty.ToString().ToCharArray();

            var end = ui.Length < 32 ? ui.Length : 32;

            for (var i = 0; i != end; i++)
                str[i] = ui[i];

            return new GuidEx(new string(str)).ToByteArray();
        }
        //-----------------------------------------------------------------------------------------
        private static byte[] PasswordCreation()
        {
            /*
#if UNITY_EDITOR || UNITY_STANDALONE
            var pass = UIToPass();
#else
            var pass = GuidEx.NewGuid().ToByteArray(); 
#endif
            */
            var pass = GuidEx.NewGuid().ToByteArray(); 

            var result = new byte[Memory.GetSizeOfPackedGuidEx() * 2];
            Memory.Memcpy(result, 0, pass, 0, Memory.GetSizeOfPackedGuidEx());
            var md5 = Conversion.ToMd5(pass, 0, pass.Length);
            Memory.Memcpy(result, Memory.GetSizeOfPackedGuidEx(), md5.ToByteArray(), 0, Memory.GetSizeOfPackedGuidEx());
            Conversion.ByteArrayToFile(mPath, result);
            return pass;
        }
        //-----------------------------------------------------------------------------------------
        private static byte[] PasswordReading()
        {
            var data = Conversion.FileToByteArray(mPath);

            if(data.Length != Memory.GetSizeOfPackedGuidEx() * 2)
                throw new Exception($"Error in data size.");

            var offset = 0;
            var passSave = Memory.UnpackingGuidEx(data, ref offset).ToByteArray();
            var md5Save = Memory.UnpackingGuidEx(data, ref offset);

            var md5 = Conversion.ToMd5(passSave, 0, passSave.Length);

            if(md5Save != md5)
                throw new Exception($"Data corrupted.");

            /*
#if UNITY_EDITOR || UNITY_STANDALONE
            if(new GuidEx(passSave) != new GuidEx(UIToPass()))
                throw new Exception($"Data corrupted.");
#endif
            */
            
            return passSave;
        }
        //-----------------------------------------------------------------------------------------
        public Task<string> Load(string path)
        {
            try
            {
                var data = !File.Exists(path) ? null : Unpacking(Conversion.FileToByteArray(path));
                return Task.FromResult((string)data);
            }
            catch(Exception ex)
            {
                throw new Exception($"Error while reading the file: {path}.", ex);
            }
        }
        //-----------------------------------------------------------------------------------------
        public void Save(string path, Var data)
        {
            Conversion.ByteArrayToFile(path, Packing(data));
        }
        
        public void Save(string path, string data)
        {
            Conversion.ByteArrayToFile(path, Packing(data));
        }
        
        public async Task SaveAsync(string path, string data)
        {
            await Conversion.ByteArrayToFileAsync(path, Packing(data));
        }
        //-----------------------------------------------------------------------------------------
        private byte[] Packing(Var userData)
        { 
            using var varPacking = mMemoryPacking.CreateStorage(Var.GetBinarySize(userData));
            Var.SaveToMemoryBinary(varPacking, userData);

            var fullSize = Memory.GetSizeOfPackedByte() + //random
                Memory.GetSizeOfPackedInt() + //version      
                varPacking.Size + //data
                Memory.GetSizeOfPackedGuidEx(); //md5

            var result = new byte[fullSize];

            var rand = new System.Random();
            var random = Conversion.ToByte(rand.Next() % 256);

            var offset = 0;
            Memory.Packing(result, ref offset, random); //random
            Memory.Packing(result, ref offset, 1); //version
            Memory.Memcpy(result, ref offset, varPacking.Buffer, 0, varPacking.Size); //data
            Memory.Packing(result, ref offset, Conversion.ToMd5(varPacking.Buffer, 0, varPacking.Size)); //md5

            for (var i = Memory.GetSizeOfPackedByte(); i != result.Length; i++)
                result[i] = (byte)((result[i] + mPass[i % mPass.Length] + random) % 256);

            return result;
        }
        //-----------------------------------------------------------------------------------------
        private Var Unpacking(byte[] userData)
        {
            var minSize =
                Memory.GetSizeOfPackedByte() + //random
                Memory.GetSizeOfPackedInt() + //version
                //data
                Memory.GetSizeOfPackedGuidEx();  //md5
                
            if (userData.Length - minSize <= 0)
                throw new Exception("No data.");

            var offset = 0;
            var random = Memory.UnpackingByte(userData, ref offset); //random
 
            var userDataLength = userData.Length;
            var passLength = mPass.Length;

            for (var i = Memory.GetSizeOfPackedByte(); i != userDataLength; i++)
                userData[i] = (byte)((userData[i] + 512 - mPass[i % passLength] - random) % 256);

            var version = Memory.UnpackingInt(userData, ref offset);

            if (version != 1)
                throw new Exception("Version not supported.");

            var dataOffset = offset;
            var md5Data = Conversion.ToMd5(userData, ref offset, userData.Length - minSize);
            var md5Save = Memory.UnpackingGuidEx(userData, ref offset);

            if (md5Save != md5Data)
                throw new Exception("Data corrupted.");
             
            var memory = new MemoryUnpacking(userData, dataOffset);
            var result = Var.LoadFromMemoryBinary(memory);

            return result;
        }
        //-----------------------------------------------------------------------------------------
    }
    //*********************************************************************************************
}