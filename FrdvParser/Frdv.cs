using System.IO;
using System.Reflection;
using System.Text;

namespace FrdvParser
{
    public class Frdv
    {
        private struct Entry
        {
            public uint offset;
            public short[] indices;
            public Vector4[] values;
        } //struct

        private string name;
        private uint signature;
        private float versionNum;
        private uint numEntries;
        private Entry[] entries;
        private const int NUM_INDICES = 7;
        private const int NUM_ENTRIES = 7;

        public void Read(FileStream stream)
        {
            BinaryReader reader = new BinaryReader(stream);

            name = stream.Name;
            signature = reader.ReadUInt32();
            versionNum = reader.ReadSingle();
            numEntries = reader.ReadUInt32();

            entries = new Entry[numEntries];

            reader.BaseStream.Position += 4;

            for (int i = 0; i < numEntries; i++)
            {
                entries[i].indices = new short[NUM_INDICES];
                entries[i].values = new Vector4[NUM_ENTRIES];
                entries[i].offset = reader.ReadUInt32();
            } //for

            //align the stream.
            if (reader.BaseStream.Position % 0x10 != 0)
                reader.BaseStream.Position += 0x10 - reader.BaseStream.Position % 0x10;

            for (int i = 0; i < numEntries; i++)
            {
                for (int j = 0; j < NUM_INDICES; j++)
                    entries[i].indices[j] = reader.ReadInt16();

                reader.BaseStream.Position += 2;

                for (int j = 0; j < NUM_ENTRIES; j++)
                    for (int h = 0; h < 4; h++)
                        entries[i].values[j][h] = reader.ReadSingle();
            } //for
        } //Read

        public void WriteToTextFile(FileStream stream)
        {
            string[] bones;
            int bonesLength;
            StringBuilder stringBuilder = new StringBuilder();

            if (File.Exists($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\bones.txt"))
                bones = File.ReadAllLines($"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)}\\bones.txt");
            else
                bones = new string[0];

            bonesLength = bones.Length;

            stringBuilder.Append(Path.GetFileName(name));
            stringBuilder.Append($"\nVersion Number: {versionNum}");
            stringBuilder.Append($"\nNumber of Entries: {numEntries}");
            stringBuilder.Append("\n----------------------------------------------------------------");

            for (int i = 0; i < numEntries; i++)
            {
                stringBuilder.Append($"\n\nEntry: {i}");
                stringBuilder.Append("\n----------------------------------------------------------------");

                for (int j = 0; j < NUM_INDICES; j++)
                {
                    if (entries[i].indices[j] != -1)
                        if (bonesLength > 0)
                            stringBuilder.Append($"\nBone {j}: {bones[entries[i].indices[j]]}");
                        else
                            stringBuilder.Append($"\nBone Index {j}: {entries[i].indices[j]}");
                    else
                        stringBuilder.Append($"\nBone {j}: [Empty]");
                } //for

                for (int j = 0; j < NUM_ENTRIES; j++)
                    stringBuilder.Append($"\nValues {j}: [{entries[i].values[j].x}, {entries[i].values[j].y}, {entries[i].values[j].z}, {entries[i].values[j].w}]");
            } //for

            StreamWriter writer = new StreamWriter(stream);
            writer.AutoFlush = true;
            writer.Write(stringBuilder);
        } //WriteToTextFile
    } //class
} //namespace FrdvParser