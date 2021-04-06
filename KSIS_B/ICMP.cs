using System;
using System.Text;

namespace KSIS_B
{
    internal class ICMP : Program
    {
        public byte Type;
        public byte Code;
        public UInt16 Checksum;
        public int MessageSize;
        public byte[] Message = new byte[1024];

        public ICMP()
        {
            /*(-_-)*/
        }

        public ICMP(byte[] data, int size)
        {
            Type = data[20];
            Code = data[21];
            Checksum = BitConverter.ToUInt16(data, 22);
            MessageSize = size - 24;
            Buffer.BlockCopy(data, 24, Message, 0, MessageSize);
        }

        public ICMP PacketMade(int identificator,int sequence)
        {
            ICMP pack = new ICMP();
            byte[] data = new byte[1024];

            pack.Type = 0x08; //тип эхо-запрос
            pack.Code = 0x00; //код недостежимости сети
            pack.Checksum = 0;
            Buffer.BlockCopy(BitConverter.GetBytes(identificator), 0, pack.Message, 0, 2); //заполнение идентификатора
            Buffer.BlockCopy(BitConverter.GetBytes(sequence), 0, pack.Message, 2, 2); //заполнение порядкового номера
            data = Encoding.ASCII.GetBytes("test packet"); //подготовка к заполнению содержимого пакета
            Buffer.BlockCopy(data, 0, pack.Message, 4, data.Length); //заполнение подготвленных данных
            pack.MessageSize = data.Length + 4;

            UInt16 chcksum = pack.getChecksum();
            pack.Checksum = chcksum;

            return pack;

        }

        public byte[] getBytes()
        {
            byte[] data = new byte[MessageSize + 9];
            Buffer.BlockCopy(BitConverter.GetBytes(Type), 0, data, 0, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(Code), 0, data, 1, 1);
            Buffer.BlockCopy(BitConverter.GetBytes(Checksum), 0, data, 2, 2);
            Buffer.BlockCopy(Message, 0, data, 4, MessageSize);
            return data;
        }

        public UInt16 getChecksum()
        {
            UInt32 chcksm = 0;
            byte[] data = getBytes();
            int packetsize = MessageSize + 8;
            int index = 0;

            while (index < packetsize)
            {
                chcksm += Convert.ToUInt32(BitConverter.ToUInt16(data, index));
                index += 2;
            }
            chcksm = (chcksm >> 16) + (chcksm & 0xffff);
            chcksm += (chcksm >> 16);
            return (UInt16)(~chcksm);
        }
    }
}